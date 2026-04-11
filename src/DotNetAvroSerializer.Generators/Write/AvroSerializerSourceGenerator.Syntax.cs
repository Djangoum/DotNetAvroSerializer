using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;

namespace DotNetAvroSerializer.Generators.Write;

public partial class AvroSerializerSourceGenerator
{
    private SourceText GetGeneratedSerializationSource(string serializerNamespace, string serializerClassName, string serializableFullyQualifiedTypeName, string serializeCode, string serializeCodeAsync, string privateMembersCode)
    {
        var privateMembers = string.IsNullOrWhiteSpace(privateMembersCode) ? string.Empty : $"\n{privateMembersCode}\n";
        var source = $$"""
                       using DotNetAvroSerializer.Primitives;
                       using DotNetAvroSerializer.LogicalTypes;
                       using DotNetAvroSerializer.Exceptions;
                       using DotNetAvroSerializer.ComplexTypes;
                       using System.Collections.Generic;
                       using System.IO;
                       using System.Threading.Tasks;
                       using System.Threading;

                       namespace {{serializerNamespace}};

                       public partial class {{serializerClassName}}
                       {
                           public override byte[] Serialize({{serializableFullyQualifiedTypeName}} source)
                           {
                               var outputStream = new MemoryStream();
                               SerializeToStream(outputStream, source);
                               return outputStream.ToArray();
                           }

                           public override void SerializeToStream(Stream outputStream, {{serializableFullyQualifiedTypeName}} source)
                           {
                               {{serializeCode}}
                           }

                           public override async Task<byte[]> SerializeAsync({{serializableFullyQualifiedTypeName}} source, CancellationToken cancellationToken = default)
                           {
                               var outputStream = new MemoryStream();
                               await SerializeToStreamAsync(outputStream, source, cancellationToken);
                               return outputStream.ToArray();
                           }

                           public override async Task SerializeToStreamAsync(Stream outputStream, {{serializableFullyQualifiedTypeName}} source, CancellationToken cancellationToken = default)
                           {
                               {{serializeCodeAsync}}
                           }

                           private static long GetCollectionCount<T>(IEnumerable<T> source)
                           {
                               if (source is null)
                               {
                                   throw new global::System.ArgumentNullException(nameof(source));
                               }

                               if (source is ICollection<T> collection)
                               {
                                   return collection.Count;
                               }

                               if (source is IReadOnlyCollection<T> readOnlyCollection)
                               {
                                   return readOnlyCollection.Count;
                               }

                               long count = 0;

                               foreach (var _ in source)
                               {
                                   count++;
                               }

                               return count;
                           }{{privateMembers}}
                       }
                       """;

        return CSharpSyntaxTree.ParseText(source)
            .GetCompilationUnitRoot()
            .NormalizeWhitespace()
            .GetText(Encoding.UTF8);
    }
}
