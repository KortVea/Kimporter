using Newtonsoft.Json;
using System.Collections.Generic;

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
