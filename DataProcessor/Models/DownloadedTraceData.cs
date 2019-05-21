using System;
using System.Collections.Generic;

namespace DataProcessor.Models
{
    public partial class DownloadedTraceData: EntityBase
    {
        public DownloadedTraceData()
        {
            DownloadedPropertyData = new HashSet<DownloadedPropertyData>();
        }

        
        public int Type { get; set; }
        public DateTime Time { get; set; }
        public string Source { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public double Milage { get; set; }
        public bool MilageSpecified { get; set; }
        public double Heading { get; set; }
        public bool HeadingSpecified { get; set; }
        public int Speed { get; set; }
        public bool SpeedSpecified { get; set; }
        public bool WasProcessed { get; set; }
        public int NumberOfProperties { get; set; }
        public long Hash { get; set; }
        public bool? OutOfSync { get; set; }

        public virtual ICollection<DownloadedPropertyData> DownloadedPropertyData { get; set; }
    }
}
