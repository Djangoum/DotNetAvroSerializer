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
        writer.WriteLine($"public partial class {serializerClassName}");

        using (writer.WriteBlock())
        {
            if (serializerApiKind is SerializerApiKind.Sync)
            {
                writer.WriteLine("/// <summary>");
                writer.WriteLine("/// Serializes <paramref name=\"source\"/> to Avro binary format and returns the resulting payload.");
                writer.WriteLine("/// </summary>");
                writer.WriteLine("/// <param name=\"source\">The value to serialize.</param>");
                writer.WriteLine("/// <returns>A byte array that contains the Avro-encoded payload.</returns>");
                writer.WriteLine("/// <exception cref=\"AvroSerializationException\">Thrown when serialization fails because the source value does not match the configured Avro schema.</exception>");
                writer.WriteLine($"public override byte[] Serialize({serializableFullyQualifiedTypeName} source)");
                using (writer.WriteBlock())
                {
                    writer.WriteLine("var outputStream = new MemoryStream();");
                    writer.WriteLine("SerializeToStream(outputStream, source);");
                    writer.WriteLine("return outputStream.ToArray();");
                }

                writer.WriteLine();
                writer.WriteLine("/// <summary>");
                writer.WriteLine("/// Serializes <paramref name=\"source\"/> to Avro binary format and writes it to <paramref name=\"outputStream\"/>.");
                writer.WriteLine("/// </summary>");
                writer.WriteLine("/// <param name=\"outputStream\">The destination stream that receives the Avro payload.</param>");
                writer.WriteLine("/// <param name=\"source\">The value to serialize.</param>");
                writer.WriteLine("/// <exception cref=\"System.ArgumentNullException\">Thrown when <paramref name=\"outputStream\"/> is <see langword=\"null\"/>.</exception>");
                writer.WriteLine("/// <exception cref=\"AvroSerializationException\">Thrown when serialization fails because the source value does not match the configured Avro schema.</exception>");
                writer.WriteLine($"public override void SerializeToStream(Stream outputStream, {serializableFullyQualifiedTypeName} source)");
                using (writer.WriteBlock())
                {
                    writer.Write(serializeCode, isMultiline: true);
                }

                writer.WriteLine();
                writer.WriteLine("/// <summary>");
                writer.WriteLine("/// Gets the number of items in <paramref name=\"source\"/> without using LINQ extensions.");
                writer.WriteLine("/// </summary>");
                writer.WriteLine("/// <typeparam name=\"T\">The collection item type.</typeparam>");
                writer.WriteLine("/// <param name=\"source\">The sequence whose item count should be determined.</param>");
                writer.WriteLine("/// <returns>The number of items in <paramref name=\"source\"/>.</returns>");
                writer.WriteLine("/// <exception cref=\"System.ArgumentNullException\">Thrown when <paramref name=\"source\"/> is <see langword=\"null\"/>.</exception>");
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
                writer.WriteLine("/// <summary>");
                writer.WriteLine("/// Serializes <paramref name=\"source\"/> to Avro binary format asynchronously and returns the resulting payload.");
                writer.WriteLine("/// </summary>");
                writer.WriteLine("/// <param name=\"source\">The value to serialize.</param>");
                writer.WriteLine("/// <param name=\"cancellationToken\">A token that can be used to cancel asynchronous serialization.</param>");
                writer.WriteLine("/// <returns>A task that completes with a byte array containing the Avro-encoded payload.</returns>");
                writer.WriteLine("/// <exception cref=\"AvroSerializationException\">Thrown when serialization fails because the source value does not match the configured Avro schema, or when async collection block size is invalid.</exception>");
                writer.WriteLine($"public override async Task<byte[]> SerializeAsync({serializableFullyQualifiedTypeName} source, CancellationToken cancellationToken = default)");
                using (writer.WriteBlock())
                {
                    writer.WriteLine("var outputStream = new MemoryStream();");
                    writer.WriteLine("await SerializeToStreamAsync(outputStream, source, cancellationToken);");
                    writer.WriteLine("return outputStream.ToArray();");
                }

                writer.WriteLine();
                writer.WriteLine("/// <summary>");
                writer.WriteLine("/// Serializes <paramref name=\"source\"/> to Avro binary format asynchronously and writes it to <paramref name=\"outputStream\"/>.");
                writer.WriteLine("/// </summary>");
                writer.WriteLine("/// <remarks>");
                writer.WriteLine("/// For array and map schemas, serialization is streamed in Avro blocks.");
                writer.WriteLine("/// The serializer buffers up to <see cref=\"AsyncArrayItemCountBlockSize\"/> items, writes a block count, then writes each item in that block.");
                writer.WriteLine("/// A final zero-sized block is written to mark the end of the collection.");
                writer.WriteLine("/// </remarks>");
                writer.WriteLine("/// <param name=\"outputStream\">The destination stream that receives the Avro payload.</param>");
                writer.WriteLine("/// <param name=\"source\">The value to serialize.</param>");
                writer.WriteLine("/// <param name=\"cancellationToken\">A token that can be used to cancel asynchronous serialization.</param>");
                writer.WriteLine("/// <exception cref=\"System.ArgumentNullException\">Thrown when <paramref name=\"outputStream\"/> is <see langword=\"null\"/>.</exception>");
                writer.WriteLine("/// <exception cref=\"AvroSerializationException\">Thrown when serialization fails because the source value does not match the configured Avro schema, or when async collection block size is invalid.</exception>");
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
