using DataProcessor.Helpers;
using DataProcessor.Interfaces;
using DataProcessor.Models;
using SharpKml.Base;
using SharpKml.Dom;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DataProcessor
{
    public class KMLParser : IKMLParser
    {
        public async Task<IEnumerable<DownloadedTraceData>> Parse(Stream stream)
        {
            return await Task.Run(() =>
            {
                var parser = new Parser();
                parser.Parse(stream);

                var root = (Kml)parser.Root;
                var doc = (Document)root.Feature;
                var pointsFolder = (Folder)doc.Features.FirstOrDefault(f => f.Name == "Trace points");
                var trace = pointsFolder?.Features.OfType<Placemark>().Select(TransformPlacemarkIntoTrace);
                return trace ?? new List<DownloadedTraceData>();
            });
        }

        private DownloadedTraceData TransformPlacemarkIntoTrace(Placemark pm)
        {
            var description = HTMLStringParsor.Parse(pm.Description.Text);
            var propData = new List<DownloadedPropertyData>();
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
                Source = $"{description.EquipmentId},???,UKN",
                Latitude = (pm.Geometry as Point)?.Coordinate.Latitude,
                Longitude = (pm.Geometry as Point)?.Coordinate.Longitude,
                Milage = description.Mileage,
                MilageSpecified = true,
                Heading = Utilities.OrientationToHeading(description.Orientation),
                HeadingSpecified = true,
                Speed = Convert.ToInt32(description.Speed),
                SpeedSpecified = true,
                WasProcessed = false,
                NumberOfProperties = propData.Count(),
                DownloadedPropertyData = propData,
                OutOfSync = true
            };

            result.Hash = Hashing.CreateHash(result);
            return result;
        }
    }
}
