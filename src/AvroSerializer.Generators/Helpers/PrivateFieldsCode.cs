using System.Collections.Generic;
using System.Text;

namespace AvroSerializer.Generators.Helpers
{
    public class PrivateFieldsCode
    {
        private readonly StringBuilder privateFieldsCode = new StringBuilder();
        private List<string> symbols = new List<string>();

        public void AppendLine(string symbolName, string code)
        {
            if (symbols.Contains(symbolName)) return;

            privateFieldsCode.AppendLine(code);
        }

        public override string ToString()
        {
            return privateFieldsCode.ToString();
        }
    }
}
