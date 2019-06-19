using System.Collections.Generic;

namespace DataProcessor.Interfaces
{
    public interface IConnStrJsonParser
    {
        Dictionary<string, string> Parse(string str);
    }
}