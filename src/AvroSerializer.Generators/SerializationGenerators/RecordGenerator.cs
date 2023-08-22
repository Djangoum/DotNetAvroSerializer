using Avro;
using DotNetAvroSerializer.Generators.Exceptions;
using DotNetAvroSerializer.Generators.Helpers;
using Microsoft.CodeAnalysis;
using System;
using System.Linq;
using System.Text;

namespace DotNetAvroSerializer.Generators.SerializationGenerators
{
    public static class RecordGenerator
    {
        public static void GenerateSerializationSourceForRecordField(Field field, StringBuilder serializationCode, PrivateFieldsCode privateFieldsCode, SourceProductionContext context, ISymbol originTypeSymbol, string sourceAccesor)
        {
            var classSymbol = originTypeSymbol as INamedTypeSymbol;

            var property = classSymbol.GetMembers().FirstOrDefault(s => s.Kind is SymbolKind.Property && s.Name.Equals(field.Name, StringComparison.InvariantCultureIgnoreCase)) as IPropertySymbol;

            if (property is null)
                throw new AvroGeneratorException($"Property {field.Name} not found in {originTypeSymbol.Name}");

            ITypeSymbol typeName = property.Type;

            SerializationGenerator.GenerateSerializatonSourceForSchema(field.Schema, serializationCode, privateFieldsCode, context, typeName, $"{sourceAccesor}.{property.Name}");
        }
    }
}
