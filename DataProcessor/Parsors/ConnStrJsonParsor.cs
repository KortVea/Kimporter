using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace DataProcessor
{
    public static class ConnStrJsonParsor
    {
        public static Dictionary<string, string> Parse(string str)
        {
            return JsonConvert.DeserializeObject<Dictionary<string, string>>(str);
        }
    }
}
