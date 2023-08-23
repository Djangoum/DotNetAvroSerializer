using DotNetAvroSerializer.Generators.Write;
using Microsoft.CodeAnalysis;

namespace DotNetAvroSerializer.Generators.Diagnostics
{
    public static class DiagnosticsDescriptors
    {
        public static DiagnosticDescriptor MissingAvroSchemaDescriptor => new DiagnosticDescriptor(
            id: "AVRO00001",
            title: "Missing AvroSchema attribute",
            messageFormat: $"Missing AvroSchema attribute for {{0}}",
            category: typeof(AvroSerializerSourceGenerator).FullName,
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: $"AvroSerializer must have its corresponding AvroSchema attribute with a valid AvroSchema");

        public static DiagnosticDescriptor MissingSchemaInAvroSchemaAttributeDescriptor => new DiagnosticDescriptor(
            id: "AVRO00002",
            title: "Missing avro schema in AvroSchema attribute",
            messageFormat: $"Missing AvroSchema for {{0}}'s AvroSchema attribute",
            category: typeof(AvroSerializerSourceGenerator).FullName,
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: $"AvroSchema can't be provided with a null or empty avro schema");

        public static DiagnosticDescriptor AvroSchemaIsNotValidDescriptor => new DiagnosticDescriptor(
            id: "AVRO00003",
            title: "AvroSchema is not valid",
            messageFormat: $"AvroSchema provided for {{0}} is not valid errors text : {{0}}",
            category: typeof(AvroSerializerSourceGenerator).FullName,
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: $"AvroSchema is not valid");

        public static DiagnosticDescriptor SerializersMustBePartialClassesDescriptor => new DiagnosticDescriptor(
            id: "AVRO00004",
            title: "Serializer must be public partial classes",
            messageFormat: $"{{0}} is not a public partial class",
            category: typeof(AvroSerializerSourceGenerator).FullName,
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: $"AvroSerializers must be public partial classes");

        public static DiagnosticDescriptor SerializableTypeMissMatchDescriptor => new DiagnosticDescriptor(
            id: "AVRO00005",
            title: "Serializable type has some conflict with schema",
            messageFormat: $"{{0}}",
            category: typeof(AvroSerializerSourceGenerator).FullName,
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: $"Serializable type has some conflict with schema");

        public static DiagnosticDescriptor SerializableTypeIsNotAllowedDescriptor => new DiagnosticDescriptor(
            id: "AVRO00006",
            title: "Serializable type is not allowed",
            messageFormat: $"{{0}}",
            category: typeof(AvroSerializerSourceGenerator).FullName,
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: $"Serializable type is not allowed");
    }
}
