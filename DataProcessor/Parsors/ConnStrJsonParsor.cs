using DataProcessor.Interfaces;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace DataProcessor
{
    public class ConnStrJsonParsor : IConnStrJsonParsor
    {
        public Dictionary<string, string> Parse(string str)
        {
            return JsonConvert.DeserializeObject<Dictionary<string, string>>(str);
        }
    }
}
