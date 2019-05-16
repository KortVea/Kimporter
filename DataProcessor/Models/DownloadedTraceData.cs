using System;
using System.Collections.Generic;
using System.Text;

namespace DataProcessor.Models
{
    public class DownloadedTraceData
    {
        public Guid Id { get; set; }
        public int Type { get; set; }
        public DateTime Time { get; set; }
        public string Source { get; set; }
        public Double Latitude { get; set; }
        public Double Longitude { get; set; }
        public Double Milage { get; set; }
        public bool MilageSpecified => Milage == 0.0 ? false : true;
        public Double Heading { get; set; }
        public bool HeadingSpecified => Heading == 0.0 ? false : true;
        public int Speed { get; set; }
        public bool SpeedSpecified => Speed == 0 ? false : true;
        public bool WasProcessed { get; set; }
        public int NumberOfProperties { get; set; }
        public Int64 Hash { get; set; }
        public bool OutOfSync { get; set; }
    }
}
