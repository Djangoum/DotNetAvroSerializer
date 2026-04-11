using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace DotNetAvroSerializer.Generators.Schemas;

internal static class AvroSchemaParser
{
    private static readonly HashSet<string> PrimitiveTypes = new(StringComparer.InvariantCultureIgnoreCase)
    {
        "null",
        "boolean",
        "int",
        "long",
        "float",
        "double",
        "bytes",
        "string"
    };

    internal static bool TryParse(string schema, out Schema parsedSchema, out string errorMessage)
    {
        try
        {
            using var document = JsonDocument.Parse(schema);
            var parserContext = new ParserContext();
            parsedSchema = ParseSchema(document.RootElement, parserContext, null);
            errorMessage = parserContext.ErrorMessage;
            return parsedSchema is not null && string.IsNullOrWhiteSpace(errorMessage);
        }
        catch (JsonException ex)
        {
            parsedSchema = null;
            errorMessage = ex.Message;
            return false;
        }
    }

    private static Schema ParseSchema(JsonElement schemaElement, ParserContext parserContext, string enclosingNamespace)
    {
        return schemaElement.ValueKind switch
        {
            JsonValueKind.String => ParseStringSchema(schemaElement.GetString(), parserContext, enclosingNamespace),
            JsonValueKind.Array => ParseUnionSchema(schemaElement, parserContext, enclosingNamespace),
            JsonValueKind.Object => ParseObjectSchema(schemaElement, parserContext, enclosingNamespace),
            _ => parserContext.Fail("Schema is not valid")
        };
    }

    private static Schema ParseUnionSchema(JsonElement schemaElement, ParserContext parserContext, string enclosingNamespace)
    {
        var schemas = new List<Schema>();

        foreach (var element in schemaElement.EnumerateArray())
        {
            var schema = ParseSchema(element, parserContext, enclosingNamespace);
            if (schema is null)
                return null;

            schemas.Add(schema);
        }

        return new UnionSchema(schemas);
    }

    private static Schema ParseStringSchema(string typeName, ParserContext parserContext, string enclosingNamespace)
    {
        if (string.IsNullOrWhiteSpace(typeName))
            return parserContext.Fail("Schema type name is not valid");

        if (PrimitiveTypes.Contains(typeName))
            return new PrimitiveSchema(typeName);

        return parserContext.ResolveNamedSchema(typeName, enclosingNamespace);
    }

    private static Schema ParseObjectSchema(JsonElement schemaObject, ParserContext parserContext, string enclosingNamespace)
    {
        if (!schemaObject.TryGetProperty("type", out var typeElement))
            return parserContext.Fail($"Schema object{GetSchemaNameSuffix(schemaObject)} has no type");

        Schema parsedSchema = typeElement.ValueKind switch
        {
            JsonValueKind.String => ParseTypeNameSchema(schemaObject, typeElement.GetString(), parserContext, enclosingNamespace),
            JsonValueKind.Object => ParseSchema(typeElement, parserContext, enclosingNamespace),
            JsonValueKind.Array => ParseSchema(typeElement, parserContext, enclosingNamespace),
            _ => parserContext.Fail($"Schema type is invalid{GetSchemaNameSuffix(schemaObject)}")
        };

        if (parsedSchema is null)
            return null;

        if (schemaObject.TryGetProperty("logicalType", out var logicalTypeElement)
            && logicalTypeElement.ValueKind == JsonValueKind.String)
        {
            return new LogicalSchema(logicalTypeElement.GetString(), parsedSchema, ExtractRawProperties(schemaObject));
        }

        return parsedSchema;
    }

    private static Schema ParseTypeNameSchema(JsonElement schemaObject, string typeName, ParserContext parserContext, string enclosingNamespace)
    {
        if (string.IsNullOrWhiteSpace(typeName))
            return parserContext.Fail("Schema type name is not valid");

        return typeName switch
        {
            "array" => ParseArraySchema(schemaObject, parserContext, enclosingNamespace),
            "map" => ParseMapSchema(schemaObject, parserContext, enclosingNamespace),
            "record" => ParseRecordSchema(schemaObject, parserContext, enclosingNamespace),
            "enum" => ParseEnumSchema(schemaObject, parserContext, enclosingNamespace),
            "fixed" => ParseFixedSchema(schemaObject, parserContext, enclosingNamespace),
            _ when PrimitiveTypes.Contains(typeName) => new PrimitiveSchema(typeName),
            _ => parserContext.ResolveNamedSchema(typeName, enclosingNamespace)
        };
    }

    private static Schema ParseArraySchema(JsonElement schemaObject, ParserContext parserContext, string enclosingNamespace)
    {
        if (!schemaObject.TryGetProperty("items", out var itemSchemaElement))
            return parserContext.Fail($"Array schema{GetSchemaNameSuffix(schemaObject)} has no items");

        var itemSchema = ParseSchema(itemSchemaElement, parserContext, enclosingNamespace);
        return itemSchema is null ? null : new ArraySchema(itemSchema);
    }

    private static Schema ParseMapSchema(JsonElement schemaObject, ParserContext parserContext, string enclosingNamespace)
    {
        if (!schemaObject.TryGetProperty("values", out var valueSchemaElement))
            return parserContext.Fail($"Map schema{GetSchemaNameSuffix(schemaObject)} has no values");

        var valueSchema = ParseSchema(valueSchemaElement, parserContext, enclosingNamespace);
        return valueSchema is null ? null : new MapSchema(valueSchema);
    }

    private static Schema ParseRecordSchema(JsonElement schemaObject, ParserContext parserContext, string enclosingNamespace)
    {
        if (!schemaObject.TryGetProperty("name", out var nameElement)
            || nameElement.ValueKind != JsonValueKind.String)
            return parserContext.Fail("Record schema has no name");

        var schemaNamespace = schemaObject.TryGetProperty("namespace", out var namespaceElement)
            && namespaceElement.ValueKind == JsonValueKind.String
            ? namespaceElement.GetString()
            : enclosingNamespace;

        var name = nameElement.GetString();
        var fullName = parserContext.GetFullName(name, schemaNamespace);

        var fields = new List<Field>();
        var recordSchema = new RecordSchema(GetSimpleName(name), fields);
        parserContext.RegisterNamedSchema(fullName, recordSchema);

        if (!schemaObject.TryGetProperty("fields", out var fieldsElement)
            || fieldsElement.ValueKind != JsonValueKind.Array)
            return parserContext.Fail($"Record schema {fullName} has no fields");

        foreach (var field in fieldsElement.EnumerateArray())
        {
            if (!field.TryGetProperty("name", out var fieldNameElement)
                || fieldNameElement.ValueKind != JsonValueKind.String)
                return parserContext.Fail($"Record schema {fullName} has a field with no name");

            if (!field.TryGetProperty("type", out var fieldSchemaElement))
                return parserContext.Fail($"Record field {fieldNameElement.GetString()} in schema {fullName} has no type");

            var fieldSchema = ParseSchema(fieldSchemaElement, parserContext, schemaNamespace);
            if (fieldSchema is null)
                return null;

            fields.Add(new Field(fieldNameElement.GetString(), fieldSchema));
        }

        return recordSchema;
    }

    private static Schema ParseEnumSchema(JsonElement schemaObject, ParserContext parserContext, string enclosingNamespace)
    {
        if (!schemaObject.TryGetProperty("name", out var nameElement)
            || nameElement.ValueKind != JsonValueKind.String)
            return parserContext.Fail("Enum schema has no name");

        var schemaNamespace = schemaObject.TryGetProperty("namespace", out var namespaceElement)
            && namespaceElement.ValueKind == JsonValueKind.String
            ? namespaceElement.GetString()
            : enclosingNamespace;

        var name = nameElement.GetString();
        var fullName = parserContext.GetFullName(name, schemaNamespace);

        if (!schemaObject.TryGetProperty("symbols", out var symbolsElement)
            || symbolsElement.ValueKind != JsonValueKind.Array)
            return parserContext.Fail($"Enum schema {fullName} has no symbols");

        var schema = new EnumSchema(GetSimpleName(name), symbolsElement.EnumerateArray().Select(s => s.GetString()).ToArray());
        parserContext.RegisterNamedSchema(fullName, schema);
        return schema;
    }

    private static Schema ParseFixedSchema(JsonElement schemaObject, ParserContext parserContext, string enclosingNamespace)
    {
        if (!schemaObject.TryGetProperty("name", out var nameElement)
            || nameElement.ValueKind != JsonValueKind.String)
            return parserContext.Fail("Fixed schema has no name");

        var schemaNamespace = schemaObject.TryGetProperty("namespace", out var namespaceElement)
            && namespaceElement.ValueKind == JsonValueKind.String
            ? namespaceElement.GetString()
            : enclosingNamespace;

        var name = nameElement.GetString();
        var fullName = parserContext.GetFullName(name, schemaNamespace);

        if (!schemaObject.TryGetProperty("size", out var sizeElement)
            || sizeElement.ValueKind != JsonValueKind.Number
            || !sizeElement.TryGetInt32(out var size))
            return parserContext.Fail($"Fixed schema {fullName} has invalid size");

        var schema = new FixedSchema(GetSimpleName(name), size);
        parserContext.RegisterNamedSchema(fullName, schema);
        return schema;
    }

    private static string GetSimpleName(string fullName) => fullName.Split('.').Last();

    private static string GetSchemaNameSuffix(JsonElement schemaObject)
    {
        if (!schemaObject.TryGetProperty("name", out var nameElement)
            || nameElement.ValueKind != JsonValueKind.String)
            return string.Empty;

        return $" {nameElement.GetString()}";
    }

    private static Dictionary<string, string> ExtractRawProperties(JsonElement schemaObject)
    {
        var properties = new Dictionary<string, string>(StringComparer.InvariantCulture);

        foreach (var property in schemaObject.EnumerateObject())
        {
            properties[property.Name] = property.Value.GetRawText();
        }

        return properties;
    }

    private sealed class ParserContext
    {
        private readonly Dictionary<string, Schema> namedSchemas = new(StringComparer.InvariantCulture);
        private readonly Dictionary<string, string> simpleNameToFullName = new(StringComparer.InvariantCulture);

        public string ErrorMessage { get; private set; }

        public Schema Fail(string message)
        {
            ErrorMessage ??= message;
            return null;
        }

        public string GetFullName(string name, string schemaNamespace)
        {
            if (name.Contains(".", StringComparison.Ordinal))
                return name;

            return string.IsNullOrWhiteSpace(schemaNamespace) ? name : $"{schemaNamespace}.{name}";
        }

        public void RegisterNamedSchema(string fullName, Schema schema)
        {
            namedSchemas[fullName] = schema;
            var simpleName = fullName.Split('.').Last();
            if (!simpleNameToFullName.ContainsKey(simpleName))
            {
                simpleNameToFullName[simpleName] = fullName;
            }
        }

        public Schema ResolveNamedSchema(string typeName, string enclosingNamespace)
        {
            if (namedSchemas.TryGetValue(typeName, out var directSchema))
                return directSchema;

            if (typeName.Contains(".", StringComparison.Ordinal))
                return Fail($"Named schema {typeName} was not found");

            if (!string.IsNullOrWhiteSpace(enclosingNamespace))
            {
                var namespacedType = $"{enclosingNamespace}.{typeName}";
                if (namedSchemas.TryGetValue(namespacedType, out var namespacedSchema))
                    return namespacedSchema;
            }

            if (simpleNameToFullName.TryGetValue(typeName, out var fullName)
                && namedSchemas.TryGetValue(fullName, out var shortNameSchema))
                return shortNameSchema;

            return Fail($"Named schema {typeName} was not found");
        }
    }
}
