using Microsoft.CodeAnalysis.Text;

namespace DotNetAvroSerializer.Generators.Models;

public record SmallLocation(string FilePath, TextSpan TextSpan, LinePositionSpan LineSpan);
