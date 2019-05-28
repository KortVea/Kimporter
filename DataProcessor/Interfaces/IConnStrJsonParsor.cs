using System.Collections.Generic;

namespace DataProcessor.Interfaces
{
    public interface IConnStrJsonParsor
    {
        Dictionary<string, string> Parse(string str);
    }
}