using Avro;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;

namespace DotNetAvroSerializer.Generators.Models
{
    internal record SerializerMetadata(string SerializerClassName, 
        string SerializerNamespace, 
        Schema AvroSchema, 
        SerializableTypeMetadata SerializableTypeMetadata, 
        IEnumerable<CustomLogicalTypeMetadata> CustomLogicalTypesMetadata)
    {
        private SmallLocation location;
        
        public void ExtractLocation(Location serializerLocation)
        {
            this.location = new SmallLocation(serializerLocation.SourceTree?.FilePath, serializerLocation.SourceSpan, serializerLocation.GetLineSpan().Span);
        }

        public Location GetSerializerLocation() => Location.Create(location.FilePath, location.TextSpan, location.LineSpan);
    }
}