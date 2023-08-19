using Avro;
using AvroSerializer.Generators.Helpers;
using Microsoft.CodeAnalysis;
using System;
using System.Linq;
using System.Text;

namespace AvroSerializer.Generators.SerializationGenerators
{
    public static class RecordGenerator
    {
        public static void GenerateSerializationSourceForRecordField(Field field, StringBuilder serializationCode, PrivateFieldsCode privateFieldsCode, GeneratorExecutionContext context, ISymbol originTypeSymbol, string sourceAccesor)
        {
            var classSymbol = originTypeSymbol as INamedTypeSymbol;

            var property = classSymbol.GetMembers().FirstOrDefault(s => s.Kind is SymbolKind.Property && s.Name.Equals(field.Name, StringComparison.InvariantCultureIgnoreCase)) as IPropertySymbol;

            ITypeSymbol typeName;

            if (property.Type.AllInterfaces.Any(i => i.Name.Contains("IEnumerable")) && !property.Type.Name.Equals("string", StringComparison.InvariantCultureIgnoreCase))
            {
                if (property.Type is INamedTypeSymbol namedTypeSymbol)
                {
                    typeName = namedTypeSymbol.TypeArguments.First();
                }
                else if (property.Type is IArrayTypeSymbol arrayTypeSymbol)
                {
                    typeName = arrayTypeSymbol.ElementType;
                }
                else
                {
                    typeName = property.Type;
                }
            }
            else
            {
                typeName = property.Type;
            }

            SerializationGenerator.GenerateSerializatonSourceForSchema(field.Schema, serializationCode, privateFieldsCode, context, typeName, $"{sourceAccesor}.{property.Name}");
        }
    }
}
