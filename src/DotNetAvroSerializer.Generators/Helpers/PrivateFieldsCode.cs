using System.Collections.Generic;

namespace DotNetAvroSerializer.Generators.Helpers;

internal sealed class PrivateFieldsCode
{
    private readonly IndentedTextWriter _writer = new IndentedTextWriter();
    private readonly List<string> _symbols = new List<string>();

    public void AppendLine(string symbolName, string code)
    {
        if (_symbols.Contains(symbolName)) return;

        _symbols.Add(symbolName);
        _writer.WriteLine(code);
    }

    public override string ToString()
    {
        return _writer.ToString();
    }
}
