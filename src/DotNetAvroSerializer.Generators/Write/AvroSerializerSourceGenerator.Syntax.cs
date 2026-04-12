using System.Text;
using DotNetAvroSerializer.Generators.Helpers;
using DotNetAvroSerializer.Generators.Models;
using Microsoft.CodeAnalysis.Text;

namespace DotNetAvroSerializer.Generators.Write;

public partial class AvroSerializerSourceGenerator
{
    private static SourceText GetGeneratedSerializationSource(string serializerNamespace, string serializerClassName, string serializableFullyQualifiedTypeName, SerializerApiKind serializerApiKind, string serializeCode, string serializeCodeAsync, string privateMembersCode)
    {
        var writer = new IndentedTextWriter();

        writer.WriteLine("using DotNetAvroSerializer.Primitives;");
        writer.WriteLine("using DotNetAvroSerializer.LogicalTypes;");
        writer.WriteLine("using DotNetAvroSerializer.Exceptions;");
        writer.WriteLine("using DotNetAvroSerializer.ComplexTypes;");
        writer.WriteLine("using System.Collections.Generic;");
        writer.WriteLine("using System.IO;");
        writer.WriteLine("using System.Threading.Tasks;");
        writer.WriteLine("using System.Threading;");
        writer.WriteLine();
        writer.WriteLine($"namespace {serializerNamespace};");
        writer.WriteLine();
        writer.WriteLine("[global::System.CodeDom.Compiler.GeneratedCodeAttribute(\"DotNetAvroSerializer\", \"1.0.0.0\")]");
        writer.WriteLine($"public partial class {serializerClassName}");

        using (writer.WriteBlock())
        {
            if (serializerApiKind is SerializerApiKind.Sync)
            {
                writer.WriteLine($"public override byte[] Serialize({serializableFullyQualifiedTypeName} source)");
                using (writer.WriteBlock())
                {
                    writer.WriteLine("var outputStream = new MemoryStream();");
                    writer.WriteLine("SerializeToStream(outputStream, source);");
                    writer.WriteLine("return outputStream.ToArray();");
                }

                writer.WriteLine();
                writer.WriteLine($"public override void SerializeToStream(Stream outputStream, {serializableFullyQualifiedTypeName} source)");
                using (writer.WriteBlock())
                {
                    writer.Write(serializeCode, isMultiline: true);
                }

                writer.WriteLine();
                writer.WriteLine("private static long GetCollectionCount<T>(IEnumerable<T> source)");
                using (writer.WriteBlock())
                {
                    writer.WriteLine("if (source is null)");
                    using (writer.WriteBlock())
                    {
                        writer.WriteLine("throw new global::System.ArgumentNullException(nameof(source));");
                    }

                    writer.WriteLine();
                    writer.WriteLine("if (source is ICollection<T> collection)");
                    using (writer.WriteBlock())
                    {
                        writer.WriteLine("return collection.Count;");
                    }

                    writer.WriteLine();
                    writer.WriteLine("if (source is IReadOnlyCollection<T> readOnlyCollection)");
                    using (writer.WriteBlock())
                    {
                        writer.WriteLine("return readOnlyCollection.Count;");
                    }

                    writer.WriteLine();
                    writer.WriteLine("long count = 0;");
                    writer.WriteLine();
                    writer.WriteLine("foreach (var _ in source)");
                    using (writer.WriteBlock())
                    {
                        writer.WriteLine("count++;");
                    }

                    writer.WriteLine();
                    writer.WriteLine("return count;");
                }
            }
            else
            {
                writer.WriteLine($"public override async Task<byte[]> SerializeAsync({serializableFullyQualifiedTypeName} source, CancellationToken cancellationToken = default)");
                using (writer.WriteBlock())
                {
                    writer.WriteLine("var outputStream = new MemoryStream();");
                    writer.WriteLine("await SerializeToStreamAsync(outputStream, source, cancellationToken);");
                    writer.WriteLine("return outputStream.ToArray();");
                }

                writer.WriteLine();
                writer.WriteLine($"public override async Task SerializeToStreamAsync(Stream outputStream, {serializableFullyQualifiedTypeName} source, CancellationToken cancellationToken = default)");
                using (writer.WriteBlock())
                {
                    writer.Write(serializeCodeAsync, isMultiline: true);
                }
            }

            if (!string.IsNullOrWhiteSpace(privateMembersCode))
            {
                writer.WriteLine();
                writer.Write(privateMembersCode, isMultiline: true);
            }
        }

        return SourceText.From(writer.ToString().Trim(), Encoding.UTF8);
    }
}
