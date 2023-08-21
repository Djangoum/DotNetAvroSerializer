using System.Text.RegularExpressions;

namespace DotNetAvroSerializer.Generators.Helpers
{
    public static class VariableNamesHelpers
    {
        public static string RemoveSpecialCharacters(string str)
        {
            return Regex.Replace(str, "[^a-zA-Z0-9]_+", "", RegexOptions.Compiled);
        }
    }
}
