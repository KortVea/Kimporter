using System.Text.RegularExpressions;

namespace DataProcessor.Helpers
{
    internal static class InnerTextExtension
    {
        internal static string TrimInnerText(this string input)
        {
            input = input.Trim();
            const string ptn = @"\r\n\s+";
            return Regex.Replace(input, ptn, " ");
        }
    }
}
