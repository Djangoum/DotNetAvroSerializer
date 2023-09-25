using DotNetAvroSerializer.Generators.Models;
using FluentAssertions;
using Microsoft.CodeAnalysis;
using Moq;
using Xunit;

namespace DotNetAvroSerializer.Generators.Tests.SerializableTypesMetadata;

public class PrimitiveSerializableTypeMetadataTests
{
    private readonly Mock<ITypeSymbol> typeSymbolMock;

    public PrimitiveSerializableTypeMetadataTests()
    {
        typeSymbolMock = new Mock<ITypeSymbol>();
    }

    [Theory]
    [InlineData(SpecialType.System_String, true)]
    [InlineData(SpecialType.System_Int32, true)]
    [InlineData(SpecialType.System_Int64, true)]
    [InlineData(SpecialType.System_Boolean, true)]
    [InlineData(SpecialType.System_Double, true)]
    [InlineData(SpecialType.System_Single, true)]
    [InlineData(SpecialType.System_Byte, true)]
    [InlineData(SpecialType.System_Void, false)]
    [InlineData(SpecialType.None, false)]
    public void IsAllowedPrimitiveType_MustAllowOnlyPermittedPrimitiveTypes(SpecialType specialType, bool success)
    {
        typeSymbolMock.Setup(_ => _.SpecialType).Returns(specialType);

        var result = PrimitiveSerializableTypeMetadata.IsAllowedPrimitiveType(typeSymbolMock.Object);

        result.Should().Be(success);
    }

}
