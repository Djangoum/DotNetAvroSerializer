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

    internal static Schema Parse(string schema)
    {
        using var document = JsonDocument.Parse(schema);
        var parserContext = new ParserContext();
        return ParseSchema(document.RootElement, parserContext, null);
    }

    private static Schema ParseSchema(JsonElement schemaElement, ParserContext parserContext, string enclosingNamespace)
    {
        return schemaElement.ValueKind switch
        {
            JsonValueKind.String => ParseStringSchema(schemaElement.GetString(), parserContext, enclosingNamespace),
            JsonValueKind.Array => new UnionSchema(schemaElement.EnumerateArray().Select(s => ParseSchema(s, parserContext, enclosingNamespace)).ToList()),
            JsonValueKind.Object => ParseObjectSchema(schemaElement, parserContext, enclosingNamespace),
            _ => throw new InvalidOperationException("Schema is not valid")
        };
    }

    private static Schema ParseStringSchema(string typeName, ParserContext parserContext, string enclosingNamespace)
    {
        if (string.IsNullOrWhiteSpace(typeName))
            throw new InvalidOperationException("Schema type name is not valid");

        if (PrimitiveTypes.Contains(typeName))
            return new PrimitiveSchema(typeName);

        return parserContext.ResolveNamedSchema(typeName, enclosingNamespace);
    }

    private static Schema ParseObjectSchema(JsonElement schemaObject, ParserContext parserContext, string enclosingNamespace)
    {
        if (!schemaObject.TryGetProperty("type", out var typeElement))
            throw new InvalidOperationException("Schema object has no type");

        Schema parsedSchema = typeElement.ValueKind switch
        {
            JsonValueKind.String => ParseTypeNameSchema(schemaObject, typeElement.GetString(), parserContext, enclosingNamespace),
            JsonValueKind.Object => ParseSchema(typeElement, parserContext, enclosingNamespace),
            JsonValueKind.Array => ParseSchema(typeElement, parserContext, enclosingNamespace),
            _ => throw new InvalidOperationException("Schema type is invalid")
        };

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
            throw new InvalidOperationException("Schema type name is not valid");

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
            throw new InvalidOperationException("Array schema has no items");

        return new ArraySchema(ParseSchema(itemSchemaElement, parserContext, enclosingNamespace));
    }

    private static Schema ParseMapSchema(JsonElement schemaObject, ParserContext parserContext, string enclosingNamespace)
    {
        if (!schemaObject.TryGetProperty("values", out var valueSchemaElement))
            throw new InvalidOperationException("Map schema has no values");

        return new MapSchema(ParseSchema(valueSchemaElement, parserContext, enclosingNamespace));
    }

    private static Schema ParseRecordSchema(JsonElement schemaObject, ParserContext parserContext, string enclosingNamespace)
    {
        if (!schemaObject.TryGetProperty("name", out var nameElement)
            || nameElement.ValueKind != JsonValueKind.String)
            throw new InvalidOperationException("Record schema has no name");

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
            throw new InvalidOperationException("Record schema has no fields");

        foreach (var field in fieldsElement.EnumerateArray())
        {
            if (!field.TryGetProperty("name", out var fieldNameElement)
                || fieldNameElement.ValueKind != JsonValueKind.String)
                throw new InvalidOperationException("Record field has no name");

            if (!field.TryGetProperty("type", out var fieldSchemaElement))
                throw new InvalidOperationException("Record field has no type");

            fields.Add(new Field(fieldNameElement.GetString(), ParseSchema(fieldSchemaElement, parserContext, schemaNamespace)));
        }

        return recordSchema;
    }

    private static Schema ParseEnumSchema(JsonElement schemaObject, ParserContext parserContext, string enclosingNamespace)
    {
        if (!schemaObject.TryGetProperty("name", out var nameElement)
            || nameElement.ValueKind != JsonValueKind.String)
            throw new InvalidOperationException("Enum schema has no name");

        if (!schemaObject.TryGetProperty("symbols", out var symbolsElement)
            || symbolsElement.ValueKind != JsonValueKind.Array)
            throw new InvalidOperationException("Enum schema has no symbols");

        var schemaNamespace = schemaObject.TryGetProperty("namespace", out var namespaceElement)
            && namespaceElement.ValueKind == JsonValueKind.String
            ? namespaceElement.GetString()
            : enclosingNamespace;

        var name = nameElement.GetString();
        var fullName = parserContext.GetFullName(name, schemaNamespace);
        var schema = new EnumSchema(GetSimpleName(name), symbolsElement.EnumerateArray().Select(s => s.GetString()).ToArray());
        parserContext.RegisterNamedSchema(fullName, schema);
        return schema;
    }

    private static Schema ParseFixedSchema(JsonElement schemaObject, ParserContext parserContext, string enclosingNamespace)
    {
        if (!schemaObject.TryGetProperty("name", out var nameElement)
            || nameElement.ValueKind != JsonValueKind.String)
            throw new InvalidOperationException("Fixed schema has no name");

        if (!schemaObject.TryGetProperty("size", out var sizeElement)
            || sizeElement.ValueKind != JsonValueKind.Number
            || !sizeElement.TryGetInt32(out var size))
            throw new InvalidOperationException("Fixed schema has invalid size");

        var schemaNamespace = schemaObject.TryGetProperty("namespace", out var namespaceElement)
            && namespaceElement.ValueKind == JsonValueKind.String
            ? namespaceElement.GetString()
            : enclosingNamespace;

        var name = nameElement.GetString();
        var fullName = parserContext.GetFullName(name, schemaNamespace);
        var schema = new FixedSchema(GetSimpleName(name), size);
        parserContext.RegisterNamedSchema(fullName, schema);
        return schema;
    }

    private static string GetSimpleName(string fullName) => fullName.Split('.').Last();

    private static Dictionary<string, string> ExtractRawProperties(JsonElement schemaObject)
    {
        var properties = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);

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

        public string GetFullName(string name, string schemaNamespace)
        {
            if (name.Contains(".", StringComparison.InvariantCulture))
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

            if (typeName.Contains(".", StringComparison.InvariantCulture))
                throw new InvalidOperationException($"Named schema {typeName} was not found");

            if (!string.IsNullOrWhiteSpace(enclosingNamespace))
            {
                var namespacedType = $"{enclosingNamespace}.{typeName}";
                if (namedSchemas.TryGetValue(namespacedType, out var namespacedSchema))
                    return namespacedSchema;
            }

            if (simpleNameToFullName.TryGetValue(typeName, out var fullName)
                && namedSchemas.TryGetValue(fullName, out var shortNameSchema))
                return shortNameSchema;

            throw new InvalidOperationException($"Named schema {typeName} was not found");
        }
    }
}
