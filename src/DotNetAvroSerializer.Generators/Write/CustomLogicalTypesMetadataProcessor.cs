using System;
using System.Collections.Generic;
using System.Linq;
using DotNetAvroSerializer.Generators.Diagnostics;
using DotNetAvroSerializer.Generators.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;

namespace DotNetAvroSerializer.Generators.Write;

public static class CustomLogicalTypesMetadataProcessor
{
    public static (IEnumerable<CustomLogicalTypeMetadata> fullyQualifiedLogicalTypes, IEnumerable<Diagnostic> diagnostics) GetCustomLogicalTypesMetadata(ExpressionSyntax customLogicalTypesDeclarationExpression, ClassDeclarationSyntax serializerSymbol, GeneratorSyntaxContext ctx)
    {
        if (customLogicalTypesDeclarationExpression is null)
            return (Array.Empty<CustomLogicalTypeMetadata>(), Array.Empty<Diagnostic>());

        var customLogicalTypesArray = ExtractCustomLogicalTypesArray(customLogicalTypesDeclarationExpression, ctx);

        var fullyQualifiedLogicalTypes = new List<CustomLogicalTypeMetadata>(customLogicalTypesArray.Count());
        var diagnosticsProduced = new List<Diagnostic>();

        foreach (var customLogicalTypeSymbol in customLogicalTypesArray)
        {
            if (!customLogicalTypeSymbol.IsStatic)
            {
                diagnosticsProduced.Add(Diagnostic.Create(DiagnosticsDescriptors.LogicalTypeIsNotStaticClass, customLogicalTypeSymbol.Locations.First(), customLogicalTypeSymbol.Name));
            }
            else if (!DoesCustomLogicalTypeHaveLogicalTypeNameAttribute(customLogicalTypeSymbol))
            {
                diagnosticsProduced.Add(Diagnostic.Create(DiagnosticsDescriptors.LogicalTypeDoesNotHaveLogicalTypeNameAttribute, customLogicalTypeSymbol.Locations.First(), customLogicalTypeSymbol.Name));
            }
            else if (!DoesCustomLogicalTypeHaveCanSerializeMethod(customLogicalTypeSymbol))
            {
                diagnosticsProduced.Add(Diagnostic.Create(DiagnosticsDescriptors.LogicalTypeDoesNotHaveCanSerializeMethod, customLogicalTypeSymbol.Locations.First(), customLogicalTypeSymbol.Name));
            }
            else if (!DoesCustomLogicalTypeHaveConvertToBaseSchemaTypeMethod(customLogicalTypeSymbol))
            {
                diagnosticsProduced.Add(Diagnostic.Create(DiagnosticsDescriptors.LogicalTypeDoesNotHaveConvertToBaseTypeMethod, customLogicalTypeSymbol.Locations.First(), customLogicalTypeSymbol.Name));
            }
            else
            {
                var convertToBaseTypeMethodParameters = GetConvertToBaseTypeMethodParameters(customLogicalTypeSymbol, "ConvertToBaseSchemaType");
                var canSerializeMethodParameters = GetConvertToBaseTypeMethodParameters(customLogicalTypeSymbol, "CanSerialize");

                var logicalTypeName = ExtractLogicalTypeNameFromAttribute(customLogicalTypeSymbol);

                fullyQualifiedLogicalTypes.Add(new CustomLogicalTypeMetadata(
                    logicalTypeName,
                    customLogicalTypeSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
                    convertToBaseTypeMethodParameters,
                    canSerializeMethodParameters
                ));
            };
        }

        return (fullyQualifiedLogicalTypes, diagnosticsProduced);
    }

    private static bool DoesCustomLogicalTypeHaveLogicalTypeNameAttribute(INamedTypeSymbol customLogicalTypeSymbol) =>
        customLogicalTypeSymbol.GetAttributes().Any(a =>
            a.AttributeClass.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat).Equals("global::DotNetAvroSerializer.LogicalTypeNameAttribute",
                StringComparison.InvariantCultureIgnoreCase));

    private static IEnumerable<IParameterSymbol> GetConvertToBaseTypeMethodParameters(INamedTypeSymbol customLogicalTypeSymbol, string methodName) =>
        customLogicalTypeSymbol.GetMembers()
            .Where(m => m.Kind == SymbolKind.Method)
            .Cast<IMethodSymbol>()
            .First(m => m.Name.Equals(methodName))
            .Parameters
            .Skip(1);

    private static string ExtractLogicalTypeNameFromAttribute(INamedTypeSymbol customLogicalTypeSymbol) =>
        customLogicalTypeSymbol.GetAttributes().First(a => a.AttributeClass
            .ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat).Equals(
                "global::DotNetAvroSerializer.LogicalTypeNameAttribute",
                StringComparison.InvariantCultureIgnoreCase)).ConstructorArguments.First().Value.ToString();

    private static bool DoesCustomLogicalTypeHaveConvertToBaseSchemaTypeMethod(INamedTypeSymbol customLogicalTypeSymbol) =>
        customLogicalTypeSymbol
            .GetMembers()
            .Where(m => m.Kind == SymbolKind.Method)
            .Cast<IMethodSymbol>()
            .Any(m => m.Name.Equals("ConvertToBaseSchemaType", StringComparison.InvariantCultureIgnoreCase)
                      && m.Parameters.Any());

    private static bool DoesCustomLogicalTypeHaveCanSerializeMethod(INamedTypeSymbol customLogicalTypeSymbol) =>
        customLogicalTypeSymbol
            .GetMembers()
            .Where(m => m.Kind == SymbolKind.Method)
            .Cast<IMethodSymbol>()
            .Any(m => m.Name.Equals("CanSerialize", StringComparison.InvariantCultureIgnoreCase)
                      && m.ReturnType.SpecialType is SpecialType.System_Boolean
                      && m.Parameters.Any()
                      && m.Parameters.First().Type.SpecialType is SpecialType.System_Object);

    private static IEnumerable<INamedTypeSymbol> ExtractCustomLogicalTypesArray(ExpressionSyntax customLogicalTypesDeclarationExpression, GeneratorSyntaxContext ctx) =>
        ctx
            .SemanticModel
            .GetOperation(customLogicalTypesDeclarationExpression)!
            .ChildOperations
            .FirstOrDefault(o => o.Kind == OperationKind.ArrayInitializer)
            ?.ChildOperations
            .Where(o => o.Kind == OperationKind.TypeOf)
            .Select(o => (o as ITypeOfOperation)!.TypeOperand as INamedTypeSymbol);
}