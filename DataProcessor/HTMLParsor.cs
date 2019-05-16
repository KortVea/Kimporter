using System;
using System.Collections.Generic;
using System.Text;
using HtmlAgilityPack;
using DataProcessor.Models;
using System.IO;
using Newtonsoft.Json;
using DataProcessor.Helpers;

namespace DataProcessor
{
    internal static class HTMLStringParsor
    {
        internal static FeatureDescriptionBindingModel Parse(string html)
        {
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);

            var ths = htmlDoc.DocumentNode.SelectNodes("//th");
            var tds = htmlDoc.DocumentNode.SelectNodes("//td");
            string json;
            //var dic = new Directory<string, object>();
            using (var memStream = new MemoryStream())
            using (var sw = new StreamWriter(memStream))
            using (var sr = new StreamReader(memStream))
            {
                sw.Write('{');
                for (int i = 0; i < ths.Count; i++)
                {
                    sw.Write($"'{ths[i].InnerText.TrimInnerText()}':'{tds[i].InnerText.TrimInnerText()}'");
                    if (i < ths.Count - 1)
                    {
                        sw.Write(',');
                    }
                }
                sw.Write('}');
                sw.Flush();
                memStream.Seek(0, SeekOrigin.Begin);
                json = sr.ReadToEnd();
            }
            return JsonConvert.DeserializeObject<FeatureDescriptionBindingModel>(json);
        }
    }
}
