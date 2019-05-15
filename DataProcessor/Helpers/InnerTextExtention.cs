using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace DataProcessor.Helpers
{
    public static class InnerTextExtention
    {
        public static string TrimInnerText(this string input)
        {
            input = input.Trim();
            var ptn = @"\r\n\s+";
            return Regex.Replace(input, ptn, " ");
        }
    }
}
