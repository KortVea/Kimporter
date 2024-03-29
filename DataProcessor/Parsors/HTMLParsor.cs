﻿using DataProcessor.Helpers;
using DataProcessor.Models;
using HtmlAgilityPack;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("NUnitTest")]
namespace DataProcessor
{
    
    internal static class HTMLStringParsor
    {
        private static readonly string[] TraceKeys = typeof(FeatureDescriptionBindingModel).GetProperties()
                                            .Select(i => i.GetCustomAttribute<JsonPropertyAttribute>()?.PropertyName)
                                            .ToArray();

        internal static FeatureDescriptionBindingModel Parse(string html)
        {
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);

            var ths = htmlDoc.DocumentNode.SelectNodes("//th");
            var tds = htmlDoc.DocumentNode.SelectNodes("//td");

            string json;
            var propertyData = new Dictionary<string, string>();

            using (var memStream = new MemoryStream())
            using (var sw = new StreamWriter(memStream))
            using (var sr = new StreamReader(memStream))
            {
                var safeBoundary = Math.Min(ths.Count, tds.Count);
                sw.Write('{');
                for (int i = 0; i < safeBoundary; i++)
                {
                    var key = ths[i].InnerText.TrimInnerText();
                    var value = tds[i].InnerText.TrimInnerText();
                    if (TraceKeys.Contains(key))
                    {
                        sw.Write($"'{key}':'{value}'");
                        if (i < TraceKeys.Count() - 1)
                        {
                            sw.Write(',');
                        }
                    }
                    else
                    {
                        propertyData.Add(key, value);
                    }
                }
                sw.Write('}');
                sw.Flush();
                memStream.Seek(0, SeekOrigin.Begin);
                json = sr.ReadToEnd();
            }
            var traceData = JsonConvert.DeserializeObject<FeatureDescriptionBindingModel>(json);
            traceData.DownloadedPropertyData = propertyData;
            return traceData;
        }
    }
}
