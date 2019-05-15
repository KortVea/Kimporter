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
        public bool MilageSpecified { get; set; }
        public Double Heading { get; set; }
        public bool HeadingSpecified { get; set; }
        public int Speed { get; set; }
        public bool SpeedSpecified { get; set; }
        public bool WasProcessed { get; set; }
        public int NumberOfProperties { get; set; }
        public Int64 Hash { get; set; }
        public bool OutOfSync { get; set; }
    }
}
