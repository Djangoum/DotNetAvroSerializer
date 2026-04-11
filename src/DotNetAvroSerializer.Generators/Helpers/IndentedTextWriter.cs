using System;
using System.Text;

namespace DotNetAvroSerializer.Generators.Helpers;

internal sealed class IndentedTextWriter
{
    private const string DefaultIndentation = "    ";
    private const char DefaultNewLine = '\n';

    private readonly StringBuilder _builder;
    private int _currentIndentationLevel;
    private string _currentIndentation;
    private string[] _availableIndentations;

    public IndentedTextWriter()
    {
        _builder = new StringBuilder();
        _currentIndentationLevel = 0;
        _currentIndentation = "";
        _availableIndentations = new string[4];
        _availableIndentations[0] = "";

        for (int i = 1; i < _availableIndentations.Length; i++)
        {
            _availableIndentations[i] = _availableIndentations[i - 1] + DefaultIndentation;
        }
    }

    public void IncreaseIndent()
    {
        _currentIndentationLevel++;

        if (_currentIndentationLevel == _availableIndentations.Length)
        {
            Array.Resize(ref _availableIndentations, _availableIndentations.Length * 2);
        }

        _currentIndentation = _availableIndentations[_currentIndentationLevel]
            ??= _availableIndentations[_currentIndentationLevel - 1] + DefaultIndentation;
    }

    public void DecreaseIndent()
    {
        _currentIndentationLevel--;
        _currentIndentation = _availableIndentations[_currentIndentationLevel];
    }

    public Block WriteBlock()
    {
        WriteLine("{");
        IncreaseIndent();
        return new Block(this);
    }

    public void Write(string content)
    {
        WriteRawText(content);
    }

    public void Write(string content, bool isMultiline)
    {
        if (!isMultiline)
        {
            WriteRawText(content);
            return;
        }

        int start = 0;
        int length = content.Length;

        while (start < length)
        {
            int newLineIndex = content.IndexOf(DefaultNewLine, start);

            if (newLineIndex < 0)
            {
                if (start < length)
                {
                    WriteRawText(content.Substring(start));
                }

                break;
            }

            string line = content.Substring(start, newLineIndex - start);

            if (line.Length > 0)
            {
                WriteRawText(line);
            }

            _builder.Append(DefaultNewLine);
            start = newLineIndex + 1;
        }
    }

    public void WriteLine()
    {
        _builder.Append(DefaultNewLine);
    }

    public void WriteLine(string content)
    {
        WriteRawText(content);
        _builder.Append(DefaultNewLine);
    }

    public override string ToString()
    {
        return _builder.ToString();
    }

    private void WriteRawText(string content)
    {
        if (_builder.Length == 0 || _builder[_builder.Length - 1] == DefaultNewLine)
        {
            _builder.Append(_currentIndentation);
        }

        _builder.Append(content);
    }

    public struct Block : IDisposable
    {
        private readonly IndentedTextWriter _writer;
        private bool _disposed;

        internal Block(IndentedTextWriter writer)
        {
            _writer = writer;
            _disposed = false;
        }

        public void Dispose()
        {
            if (_disposed) return;

            _disposed = true;
            _writer.DecreaseIndent();
            _writer.WriteLine("}");
        }
    }
}
