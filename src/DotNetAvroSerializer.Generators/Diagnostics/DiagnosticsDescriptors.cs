using DotNetAvroSerializer.Generators.Write;
using Microsoft.CodeAnalysis;

namespace DotNetAvroSerializer.Generators.Diagnostics;

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
        messageFormat: $"AvroSchema provided for {{0}} is not valid errors text : {{1}}",
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

    public static DiagnosticDescriptor LogicalTypeIsNotStaticClass =>
        new DiagnosticDescriptor(
            id: "AVRO00007",
            title: "Logical type is not a static class",
            messageFormat: $"Logical type {{0}} is not a static type",
            category: typeof(AvroSerializerSourceGenerator).FullName,
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: $"Logical types must be static classes."
        );

    public static DiagnosticDescriptor LogicalTypeDoesNotHaveLogicalTypeNameAttribute =>
        new DiagnosticDescriptor(
            id: "AVRO00008",
            title: "LogicalTypeName attribute not found",
            messageFormat: $"No LogicalTypeName attribute provided in {{0}}",
            category: typeof(AvroSerializerSourceGenerator).FullName,
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: $"LogicalTypeName attribute not found."
        );

    public static DiagnosticDescriptor LogicalTypeDoesNotHaveCanSerializeMethod =>
        new DiagnosticDescriptor(
            id: "AVRO00009",
            title: "Logical type must have CanSerialize method",
            messageFormat: $"Logical type {{0}} has no CanSerialize method or that accepts object? and returns bool",
            category: typeof(AvroSerializerSourceGenerator).FullName,
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: $"Logical types must have CanSerialize method that accepts object? and returns bool."
        );

    public static DiagnosticDescriptor LogicalTypeDoesNotHaveConvertToBaseTypeMethod =>
        new DiagnosticDescriptor(
            id: "AVRO00010",
            title: "Logical type must have ConvertToBaseType method",
            messageFormat: $"Logical type {{0}} has no CanSerialize method",
            category: typeof(AvroSerializerSourceGenerator).FullName,
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: $"Logical types must have CanSerialize method."
        );

    public static DiagnosticDescriptor SerializerMustInheritFromAvroSerializerDescriptor =>
        new DiagnosticDescriptor(
            id: "AVRO00011",
            title: "Serializer must inherit from AvroSerializer<T> or AsyncAvroSerializer<T>",
            messageFormat: $"{{0}} must inherit from DotNetAvroSerializer.AvroSerializer<T> or DotNetAvroSerializer.AsyncAvroSerializer<T>",
            category: typeof(AvroSerializerSourceGenerator).FullName,
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: $"Avro serializers must directly inherit from DotNetAvroSerializer.AvroSerializer<T> or DotNetAvroSerializer.AsyncAvroSerializer<T>."
        );

    public static DiagnosticDescriptor AmbiguousFieldBindingDescriptor =>
        new DiagnosticDescriptor(
            id: "AVRO00012",
            title: "Ambiguous field binding",
            messageFormat: "{0}",
            category: typeof(AvroSerializerSourceGenerator).FullName,
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: "Multiple properties could map to the same Avro field."
        );

    public static DiagnosticDescriptor DuplicateAvroFieldAliasDescriptor =>
        new DiagnosticDescriptor(
            id: "AVRO00013",
            title: "Duplicate AvroField alias",
            messageFormat: "{0}",
            category: typeof(AvroSerializerSourceGenerator).FullName,
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: "The same AvroField alias is used on multiple properties in a record type."
        );

    public static DiagnosticDescriptor UnionSchemaOrderMismatchDescriptor =>
        new DiagnosticDescriptor(
            id: "AVRO00014",
            title: "Union/schema order mismatch",
            messageFormat: "{0}",
            category: typeof(AvroSerializerSourceGenerator).FullName,
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: "Union type arguments must align with Avro union schema ordering."
        );

    public static DiagnosticDescriptor UnsupportedNullablePatternDescriptor =>
        new DiagnosticDescriptor(
            id: "AVRO00015",
            title: "Unsupported nullable pattern",
            messageFormat: "{0}",
            category: typeof(AvroSerializerSourceGenerator).FullName,
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: "The nullable .NET type pattern is not supported by the Avro schema shape."
        );

    public static DiagnosticDescriptor UnsupportedMapKeyTypeDescriptor =>
        new DiagnosticDescriptor(
            id: "AVRO00016",
            title: "Unsupported map key type",
            messageFormat: "{0}",
            category: typeof(AvroSerializerSourceGenerator).FullName,
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: "Map schemas only support string dictionary keys."
        );

    public static DiagnosticDescriptor MultipleAvroSchemaAttributesDescriptor =>
        new DiagnosticDescriptor(
            id: "AVRO00017",
            title: "Multiple AvroSchema attributes found",
            messageFormat: "{0}",
            category: typeof(AvroSerializerSourceGenerator).FullName,
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: "Only the first AvroSchema attribute is used when multiple are applied."
        );
}
