using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace DataProcessor.Helpers
{
    internal static class InnerTextExtention
    {
        internal static string TrimInnerText(this string input)
        {
            input = input.Trim();
            var ptn = @"\r\n\s+";
            return Regex.Replace(input, ptn, " ");
        }
    }
}
