﻿using System;
using SharpKml.Dom;
using DataProcessor.Models;
using SharpKml.Base;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using DataProcessor.Helpers;

namespace DataProcessor
{
    public static class KMLParsor
    {
        public static IEnumerable<DownloadedTraceData> Parse(string path)
        {
            try
            {
                using (var fs = File.OpenRead(path))
                {
                    var parser = new Parser();
                    parser.Parse(fs);

                    var root = (Kml)parser.Root;
                    var doc = (Document)root.Feature;
                    var pointsFolder = (Folder)doc.Features.FirstOrDefault(f => f.Name == "Trace points");
                    var trace = pointsFolder.Features.OfType<Placemark>().Select(i => TransformPlacemarkIntoTrace(i));
                    return trace;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private static DownloadedTraceData TransformPlacemarkIntoTrace(Placemark pm)
        {
            var description = HTMLStringParsor.Parse(pm.Description.Text);
            List<DownloadedPropertyData> propData = new List<DownloadedPropertyData>();
            var traceDataId = Guid.NewGuid();
            if (description.DownloadedPropertyData.Count > 0)
            {
                foreach (var kv in description.DownloadedPropertyData)
                {
                    propData.Add(new DownloadedPropertyData
                    {
                        Id = Guid.NewGuid(),
                        PropertyKey = kv.Key,
                        PropertyValue = kv.Value,
                        TraceDataId = traceDataId
                    });
                }
            }
            var result = new DownloadedTraceData
            {
                Id = traceDataId,
                Type = description.Type,
                Time = description.Time,
                Source = "N/A",
                Latitude = (pm.Geometry as Point).Coordinate.Latitude,
                Longitude = (pm.Geometry as Point).Coordinate.Longitude,
                Milage = description.Milage,
                Heading = 0,
                Speed = Convert.ToInt32(description.Speed),
                WasProcessed = false,
                NumberOfProperties = 0,
                DownloadedPropertyData = propData
                //OutOfSync = true
            };
            
            result.Hash = Hashing.CreateHash(result);//not here but 
            return result;
        }
    }
}
