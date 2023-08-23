using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Text;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace DotNetAvroSerializer.Generators
{
    public partial class AvroSerializerSourceGenerator
    {
        private CompilationUnitSyntax GetGeneratedSerializationSource()
        {
            return CompilationUnit()
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
                            IdentifierName("namespaceName"))
                        .WithMembers(
                            SingletonList<MemberDeclarationSyntax>(
                                ClassDeclaration("className")
                                .WithModifiers(
                                    TokenList(
                                        new[]{
                                            Token(SyntaxKind.PublicKeyword),
                                            Token(SyntaxKind.PartialKeyword)}))
                                .WithMembers(
                                    SingletonList<MemberDeclarationSyntax>(
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
                                                Token(SyntaxKind.PublicKeyword)))
                                        .WithParameterList(
                                            ParameterList(
                                                SingletonSeparatedList<ParameterSyntax>(
                                                    Parameter(
                                                        Identifier("source"))
                                                    .WithType(
                                                        IdentifierName("serializableTypeName")))))
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
                                                            IdentifierName("ToArray"))))))))))))
                                            .NormalizeWhitespace();
        }
    }
}
