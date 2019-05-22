using System;
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
        public static IEnumerable<DownloadedTraceData> Parse(Stream stream)
        {
            try
            {
                var parser = new Parser();
                parser.Parse(stream);

                var root = (Kml)parser.Root;
                var doc = (Document)root.Feature;
                var pointsFolder = (Folder)doc.Features.FirstOrDefault(f => f.Name == "Trace points");
                var trace = pointsFolder?.Features.OfType<Placemark>().Select(i => TransformPlacemarkIntoTrace(i));
                return trace;
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
                Milage = description.Milage,//todo: unit transform
                //MilageSpecified = des,
                Heading = 0,
                //HeadingSpecified = ,
                Speed = Convert.ToInt32(description.Speed),
                //SpeedSpecified = ,
                WasProcessed = false,
                NumberOfProperties = propData.Count(),
                DownloadedPropertyData = propData,
                OutOfSync = true
            };
            
            result.Hash = Hashing.CreateHash(result);//not here but 
            return result;
        }
    }
}
