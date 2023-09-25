using DotNetAvroSerializer.Generators.Models;
using FluentAssertions;
using Microsoft.CodeAnalysis;
using Moq;
using Xunit;

namespace DotNetAvroSerializer.Generators.Tests.SerializableTypesMetadata;
public class LogicalTypeSerializableTypeMetadataTests
{
    private readonly Mock<ITypeSymbol> typeSymbolMock;

    public LogicalTypeSerializableTypeMetadataTests()
    {
        typeSymbolMock = new Mock<ITypeSymbol>();
    }

    [Theory]
    [InlineData(typeof(DateTime), true)]
    [InlineData(typeof(DateOnly), true)]
    [InlineData(typeof(TimeOnly), true)]
    [InlineData(typeof(Guid), true)]
    [InlineData(typeof(bool), false)]
    public void IsAllowedPrimitiveType_MustAllowOnlyPermittedPrimitiveTypes(Type logicalType, bool success)
    {
        typeSymbolMock.Setup(_ => _.ToDisplayString(It.IsAny<SymbolDisplayFormat>())).Returns($"global::{logicalType.FullName}");

        var result = LogicalTypeSerializableTypeMetadata.IsValidLogicalType(typeSymbolMock.Object);

        result.Should().Be(success);
    }
}
