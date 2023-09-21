using Avro;
using DotNetAvroSerializer.Generators.Diagnostics;
using DotNetAvroSerializer.Generators.Exceptions;
using DotNetAvroSerializer.Generators.Helpers;
using DotNetAvroSerializer.Generators.Models;
using DotNetAvroSerializer.Generators.SerializationGenerators;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using Avro.Util;
using Microsoft.CodeAnalysis.Operations;

namespace DotNetAvroSerializer.Generators.Write
{
    [Generator]
    public partial class AvroSerializerSourceGenerator : IIncrementalGenerator
    {
        private static bool IsSyntaxGenerationCandidate(SyntaxNode node) => node is ClassDeclarationSyntax c && (c.BaseList?.Types.First().Type.ToString().Contains("AvroSerializer") ?? false);

        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            IncrementalValuesProvider<(SerializerMetadata serializerMetadata, EquatableArray<Diagnostic> errors)> serializersMetadataWithErrors = context.SyntaxProvider
                .CreateSyntaxProvider(
                    predicate: static (s, _) => IsSyntaxGenerationCandidate(s),
                    transform: static (ctx, ct) => GetTargetDataForGeneration(ctx, ct));

            IncrementalValuesProvider<Diagnostic> errors =
                serializersMetadataWithErrors.SelectMany(static (values, _) => values.errors);
            
            IncrementalValuesProvider<SerializerMetadata> validSerializers = serializersMetadataWithErrors
                .Where(static (item) => item.errors.IsEmpty)
                .Select(static (s, ct) => s.serializerMetadata);
            
            context.RegisterSourceOutput(errors, (ctx, error) =>
            {
                ctx.ReportDiagnostic(error);
            });

            context.RegisterSourceOutput(validSerializers, (context, serializerData) =>
            {
                var (serializationCode, privateFieldsCode, diagnostic) = SerializationCodeGeneratorLoop(serializerData, serializerData.AvroSchema);

                if (diagnostic is not null)
                {
                    context.ReportDiagnostic(diagnostic);
                }
                else
                {
                    context.AddSource($"{serializerData.SerializerClassName}.write.g.cs", 
                        GetGeneratedSerializationSource(
                            serializerData.SerializerNamespace, 
                            serializerData.SerializerClassName,
                            serializerData.SerializableTypeMetadata.FullNameDisplay,
                            serializationCode,
                            privateFieldsCode));
                }
            });
        }

        private static (SerializerMetadata serializerMetadata, EquatableArray<Diagnostic> errors) GetTargetDataForGeneration(GeneratorSyntaxContext ctx, CancellationToken ct)
        {
            if (!Debugger.IsAttached) Debugger.Launch();

            var diagnostics = ImmutableArray<Diagnostic>.Empty;
            var serializerSyntax = ctx.Node as ClassDeclarationSyntax;

            ct.ThrowIfCancellationRequested();

            if (!(serializerSyntax.Modifiers.Any(m => m.IsKind(SyntaxKind.PartialKeyword))
                && serializerSyntax.Modifiers.Any(m => m.IsKind(SyntaxKind.PublicKeyword))))
            {
                diagnostics = diagnostics.Add(Diagnostic.Create(DiagnosticsDescriptors.SerializersMustBePartialClassesDescriptor, serializerSyntax.GetLocation(), serializerSyntax.Identifier.ToString()));
                return (null, diagnostics);
            }

            var attribute = serializerSyntax
                .AttributeLists
                .SelectMany(x => x.Attributes)
                .FirstOrDefault(attr => attr.Name.ToString() == "AvroSchema");

            if (attribute is null)
            {
                diagnostics = diagnostics.Add(Diagnostic.Create(DiagnosticsDescriptors.MissingAvroSchemaDescriptor, serializerSyntax.GetLocation(), serializerSyntax.Identifier.ToString()));
                return (null, diagnostics);
            }

            var attributeSchemaText = attribute.ArgumentList?.Arguments.First()?.Expression;
            var customLogicalTypes = attribute.ArgumentList?.Arguments.Count > 1
                ? attribute.ArgumentList.Arguments.ElementAt(1).Expression
                : null;

            var (customLogicalTypeNames, logicalTypesDiagnostics) = GetCustomLogicalTypesMetadata(customLogicalTypes, serializerSyntax, ctx);

            if (logicalTypesDiagnostics.Any())
            {
                diagnostics = diagnostics.AddRange(logicalTypesDiagnostics);
                return (null, diagnostics);
            }

            var schemaString = ctx
                .SemanticModel
                .GetConstantValue(attributeSchemaText);

            ct.ThrowIfCancellationRequested();

            if (!schemaString.HasValue)
            {
                diagnostics = diagnostics.Add(Diagnostic.Create(DiagnosticsDescriptors.MissingSchemaInAvroSchemaAttributeDescriptor, serializerSyntax.GetLocation(), serializerSyntax.Identifier.ToString()));
                return (null, diagnostics);
            }

            Schema schema = default;

            try
            {
                schema = Schema.Parse(schemaString.ToString());
            }
            catch (Exception ex)
            {
                diagnostics = diagnostics.Add(Diagnostic.Create(DiagnosticsDescriptors.AvroSchemaIsNotValidDescriptor, serializerSyntax.GetLocation(), serializerSyntax.Identifier.ToString(), ex.Message));
                return (null, diagnostics);
            }
            if (!Debugger.IsAttached) Debugger.Launch();

            ct.ThrowIfCancellationRequested();

            var serializableType = GetSerializableTypeSymbol(serializerSyntax, ctx);

            var serializableTypeMetadata = SerializableTypeMetadata.From(serializableType);

            if (serializableTypeMetadata is null)
            {
                diagnostics = diagnostics.Add(Diagnostic.Create(DiagnosticsDescriptors.SerializableTypeIsNotAllowedDescriptor, serializerSyntax.GetLocation(), serializerSyntax.Identifier.ToString()));
            }

            ct.ThrowIfCancellationRequested();

            return (
                new SerializerMetadata(serializerSyntax.Identifier.ToString(),
                    Namespaces.GetNamespace(serializerSyntax), schema, serializableTypeMetadata, customLogicalTypeNames), diagnostics);
        }
        
        private static (IEnumerable<CustomLogicalTypeMetadata> fullyQualifiedLogicalTypes, IEnumerable<Diagnostic> diagnostics) GetCustomLogicalTypesMetadata(ExpressionSyntax customLogicalTypesDeclarationExpression, ClassDeclarationSyntax serializerSymbol, GeneratorSyntaxContext ctx)
        {
            var customLogicalTypesArray = ctx
                .SemanticModel
                .GetOperation(customLogicalTypesDeclarationExpression)!
                .ChildOperations
                .FirstOrDefault(o => o.Kind == OperationKind.ArrayInitializer)
                ?.ChildOperations
                .Where(o => o.Kind == OperationKind.TypeOf)
                .Select(o => (o as ITypeOfOperation)!.TypeOperand as INamedTypeSymbol);

            var fullyQualifiedLogicalTypes = new List<CustomLogicalTypeMetadata>(customLogicalTypesArray.Count());
            var diagnosticsProduced = new List<Diagnostic>();
            
            foreach (var customLogicalTypeSymbol in customLogicalTypesArray)
            {
                if (!customLogicalTypeSymbol.IsStatic)
                {
                    diagnosticsProduced.Add(Diagnostic.Create(DiagnosticsDescriptors.LogicalTypeIsNotStaticClass, customLogicalTypeSymbol.Locations.First(), customLogicalTypeSymbol.Name));
                }
                else if (!customLogicalTypeSymbol.GetAttributes().Any(a =>
                        a.AttributeClass.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat).Equals("global::DotNetAvroSerializer.LogicalTypeNameAttribute",
                            StringComparison.InvariantCultureIgnoreCase)))
                {
                    diagnosticsProduced.Add(Diagnostic.Create(DiagnosticsDescriptors.LogicalTypeDoesNotHaveLogicalTypeNameAttribute, customLogicalTypeSymbol.Locations.First(), customLogicalTypeSymbol.Name));
                }
                else if (!customLogicalTypeSymbol
                             .GetMembers()
                             .Where(m => m.Kind == SymbolKind.Method)
                             .Cast<IMethodSymbol>()
                             .Any(m => m.Name.Equals("CanSerialize", StringComparison.InvariantCultureIgnoreCase) 
                                       && m.ReturnType.Name.Equals("Boolean", StringComparison.InvariantCultureIgnoreCase) 
                                       && m.Parameters.Any() 
                                       && m.Parameters.First().Type.Name.Equals("object", StringComparison.InvariantCultureIgnoreCase)))
                {
                    diagnosticsProduced.Add(Diagnostic.Create(DiagnosticsDescriptors.LogicalTypeDoesNotHaveCanSerializeMethod, customLogicalTypeSymbol.Locations.First(), customLogicalTypeSymbol.Name));
                }
                else if (!customLogicalTypeSymbol
                             .GetMembers()
                             .Where(m => m.Kind == SymbolKind.Method)
                             .Cast<IMethodSymbol>()
                             .Any(m => m.Name.Equals("ConvertToBaseSchemaType", StringComparison.InvariantCultureIgnoreCase)
                                       && m.Parameters.Any()))
                {
                    diagnosticsProduced.Add(Diagnostic.Create(DiagnosticsDescriptors.LogicalTypeDoesNotHaveConvertToBaseTypeMethod, customLogicalTypeSymbol.Locations.First(), customLogicalTypeSymbol.Name));
                }
                else
                {
                    var convertToBaseTypeMethodParameters = customLogicalTypeSymbol.GetMembers()
                        .Where(m => m.Kind == SymbolKind.Method)
                        .Cast<IMethodSymbol>()
                        .First(m => m.Name.Equals("ConvertToBaseSchemaType"))
                        .Parameters
                        .Skip(1);

                    var logicalTypeName = customLogicalTypeSymbol.GetAttributes().First(a => a.AttributeClass
                        .ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat).Equals(
                            "global::DotNetAvroSerializer.LogicalTypeNameAttribute",
                            StringComparison.InvariantCultureIgnoreCase)).ConstructorArguments.First().Value.ToString();

                    fullyQualifiedLogicalTypes.Add(new CustomLogicalTypeMetadata()
                    {
                        Name = logicalTypeName,
                        LogicalTypeFullyQualifiedName = customLogicalTypeSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
                        OrderedSchemaProperties = convertToBaseTypeMethodParameters.Select(p =>
                        { 
                            var overridenName = p
                                .GetAttributes()
                                .FirstOrDefault(a => a
                                    .AttributeClass.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)
                                    .Equals("global::DotNetAvroSerializer.LogicalTypePropertyNameAttribute"))
                                .ConstructorArguments.First()
                                .Value;

                            return overridenName is not null ? overridenName.ToString() : p.Name;
                        })
                    });

                    LogicalTypeFactory.Instance.Register(new CustomLogicalType(logicalTypeName));
                }
            }
            
            return (fullyQualifiedLogicalTypes, diagnosticsProduced);
        }
        
        private static ITypeSymbol GetSerializableTypeSymbol(ClassDeclarationSyntax serializerSyntax, GeneratorSyntaxContext ctx)
        {
            var serializerGenericArgument = ((GenericNameSyntax)serializerSyntax.BaseList.Types.First().Type).TypeArgumentList.Arguments.First();

            var symbol = ctx.SemanticModel.GetSymbolInfo(serializerGenericArgument).Symbol as ITypeSymbol;

            if (serializerGenericArgument.Kind() == SyntaxKind.NullableType)
            {
                symbol = symbol.WithNullableAnnotation(NullableAnnotation.Annotated);
            }

            return symbol;
        }

        private (string serializationCode, string privateFieldsCode, Diagnostic diagnostic) SerializationCodeGeneratorLoop(SerializerMetadata serializerMetadata, Schema schema, string sourceAccessor = "source")
        {
            try
            {
                var serializationCode = new StringBuilder();
                var privateFieldsCode = new PrivateFieldsCode();

                SerializationGenerator.GenerateSerializatonSourceForSchema(schema, serializationCode, privateFieldsCode, serializerMetadata.SerializableTypeMetadata, serializerMetadata.CustomLogicalTypesMetadata, sourceAccessor);

                return (serializationCode.ToString(), privateFieldsCode.ToString(), null);
            }
            catch (AvroGeneratorException ex)
            {
                return (string.Empty, string.Empty, Diagnostic.Create(DiagnosticsDescriptors.SerializableTypeMissMatchDescriptor, null, ex.Message));
            }
        }
    }
}
