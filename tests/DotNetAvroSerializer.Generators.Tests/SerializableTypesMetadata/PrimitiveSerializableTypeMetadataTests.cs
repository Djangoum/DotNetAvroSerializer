using Microsoft.CodeAnalysis;
using Moq;

namespace DotNetAvroSerializer.Generators.Tests.SerializableTypesMetadata;

internal class PrimitiveSerializableTypeMetadataTests
{
    private readonly Mock<ITypeSymbol> typeSymbolMock;

    public PrimitiveSerializableTypeMetadataTests()
    {
        typeSymbolMock = new Mock<ITypeSymbol>();
    }
}
