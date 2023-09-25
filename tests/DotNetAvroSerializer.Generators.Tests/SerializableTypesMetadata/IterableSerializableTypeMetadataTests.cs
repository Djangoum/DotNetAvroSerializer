using System.Collections.Immutable;
using DotNetAvroSerializer.Generators.Models;
using FluentAssertions;
using Microsoft.CodeAnalysis;
using Moq;
using Xunit;

namespace DotNetAvroSerializer.Generators.Tests.SerializableTypesMetadata;
public class IterableSerializableTypeMetadataTests
{
    [Fact]
    public void IsValidArrayType_MustReturnFalseForString()
    {
        var namedTypeSymbolMock = new Mock<INamedTypeSymbol>();
        namedTypeSymbolMock.Setup(_ => _.SpecialType).Returns(SpecialType.System_String);

        var result = IterableSerializableTypeMetadata.IsValidArrayType(namedTypeSymbolMock.Object);

        result.Should().BeFalse();
    }

    [Fact]
    public void IsValidArrayType_MustReturnTrueForAnythingIEnumerable()
    {
        var namedTypeSymbolMock = new Mock<INamedTypeSymbol>();
        var interfaceTypeSymbolMock = new Mock<INamedTypeSymbol>();

        namedTypeSymbolMock.Setup(_ => _.AllInterfaces)
            .Returns(ImmutableArray.Create(interfaceTypeSymbolMock.Object));

        interfaceTypeSymbolMock.Setup(_ => _.SpecialType).Returns(SpecialType.System_Collections_IEnumerable);

        var result = IterableSerializableTypeMetadata.IsValidArrayType(namedTypeSymbolMock.Object);

        result.Should().BeTrue();
    }

    [Fact]
    public void IsValidArrayType_MustReturnTrueForAnyArrayTypeSymbol()
    {
        var namedTypeSymbolMock = new Mock<IArrayTypeSymbol>();

        var result = IterableSerializableTypeMetadata.IsValidArrayType(namedTypeSymbolMock.Object);

        result.Should().BeTrue();
    }
}
