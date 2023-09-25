using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace DotNetAvroSerializer.Generators.Write;

public partial class AvroSerializerSourceGenerator
{
    private SourceText GetGeneratedSerializationSource(string serializerNamespace, string serializerClassName, string serializableFullyQualifiedTypeName, string serializeCode, string privateMembersCode)
    {
        return CSharpSyntaxTree.ParseText(
            CompilationUnit()
                .WithUsings(
                    List<UsingDirectiveSyntax>(
                        new UsingDirectiveSyntax[]{
                            UsingDirective(
                                QualifiedName(
                                    IdentifierName("DotNetAvroSerializer"),
                                    IdentifierName("Primitives"))),
                            UsingDirective(
                                QualifiedName(
                                    IdentifierName("DotNetAvroSerializer"),
                                    IdentifierName("LogicalTypes"))),
                            UsingDirective(
                                QualifiedName(
                                    IdentifierName("DotNetAvroSerializer"),
                                    IdentifierName("Exceptions"))),
                            UsingDirective(
                                QualifiedName(
                                    IdentifierName("DotNetAvroSerializer"),
                                    IdentifierName("ComplexTypes"))),
                            UsingDirective(
                                QualifiedName(
                                    IdentifierName("System"),
                                    IdentifierName("Linq"))),
                            UsingDirective(
                                QualifiedName(
                                    IdentifierName("System"),
                                    IdentifierName("IO")))}))
                .WithMembers(
                    SingletonList<MemberDeclarationSyntax>(
                        NamespaceDeclaration(
                            IdentifierName(serializerNamespace))
                        .WithMembers(
                            SingletonList<MemberDeclarationSyntax>(
                                ClassDeclaration(serializerClassName)
                                .WithModifiers(
                                    TokenList(
                                        new[]{
                                            Token(SyntaxKind.PublicKeyword),
                                            Token(SyntaxKind.PartialKeyword)}))
                                .WithMembers(
                                    List(GetMemberDeclarations(serializableFullyQualifiedTypeName, serializeCode, privateMembersCode)))))))
                .NormalizeWhitespace()
                .ToFullString())
            .GetRoot().NormalizeWhitespace().GetText(Encoding.UTF8);
    }

    private IEnumerable<MemberDeclarationSyntax> GetMemberDeclarations(string serializableFullyQualifiedTypeName, string serializeCode, string privateMembers)
    {
        var memberDeclarations = new List<MemberDeclarationSyntax>
        {
            MethodDeclaration(
                ArrayType(
                    PredefinedType(
                        Token(SyntaxKind.ByteKeyword)))
                .WithRankSpecifiers(
                    SingletonList<ArrayRankSpecifierSyntax>(
                        ArrayRankSpecifier(
                            SingletonSeparatedList<ExpressionSyntax>(
                                OmittedArraySizeExpression())))),
                Identifier("Serialize"))
            .WithModifiers(
                TokenList(
                    new []
                    {
                        Token(SyntaxKind.PublicKeyword),
                        Token(SyntaxKind.OverrideKeyword)
                    })
                )
            .WithParameterList(
                ParameterList(
                    SingletonSeparatedList<ParameterSyntax>(
                        Parameter(
                            Identifier("source"))
                        .WithType(
                            IdentifierName(serializableFullyQualifiedTypeName)))))
            .WithBody(
                Block(
                    LocalDeclarationStatement(
                        VariableDeclaration(
                            IdentifierName(
                                Identifier(
                                    TriviaList(),
                                    SyntaxKind.VarKeyword,
                                    "var",
                                    "var",
                                    TriviaList())))
                        .WithVariables(
                            SingletonSeparatedList<VariableDeclaratorSyntax>(
                                VariableDeclarator(
                                    Identifier("outputStream"))
                                .WithInitializer(
                                    EqualsValueClause(
                                        ObjectCreationExpression(
                                            IdentifierName("MemoryStream"))
                                        .WithArgumentList(
                                            ArgumentList())))))),
                    ExpressionStatement(
                        InvocationExpression(
                            IdentifierName("SerializeToStream"))
                        .WithArgumentList(
                            ArgumentList(
                                SeparatedList<ArgumentSyntax>(
                                    new SyntaxNodeOrToken[]{
                                        Argument(
                                            IdentifierName("outputStream")),
                                        Token(SyntaxKind.CommaToken),
                                        Argument(
                                            IdentifierName("source"))})))),
                    ReturnStatement(
                        InvocationExpression(
                            MemberAccessExpression(
                                SyntaxKind.SimpleMemberAccessExpression,
                                IdentifierName("outputStream"),
                                IdentifierName("ToArray")))))),

            MethodDeclaration(
                PredefinedType(
                    Token(SyntaxKind.VoidKeyword)),
                Identifier("SerializeToStream"))
            .WithModifiers(
                TokenList(
                    new []
                    {
                        Token(SyntaxKind.PublicKeyword),
                        Token(SyntaxKind.OverrideKeyword)
                    })
                )
            .WithParameterList(
                ParameterList(
                    SeparatedList<ParameterSyntax>(
                        new SyntaxNodeOrToken[]{
                            Parameter(
                                Identifier("outputStream"))
                            .WithType(
                                IdentifierName("Stream")),
                            Token(SyntaxKind.CommaToken),
                            Parameter(
                                Identifier("source"))
                            .WithType(
                                IdentifierName(serializableFullyQualifiedTypeName))})))
            .WithBody(
                Block(ParseStatement(serializeCode)))
        };

        if (!string.IsNullOrWhiteSpace(privateMembers))
        {
            memberDeclarations.Add(ParseMemberDeclaration(privateMembers));
        }

        return memberDeclarations;
    }
}
