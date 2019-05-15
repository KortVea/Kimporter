using System;
using System.Collections.Generic;
using System.Text;

namespace DataProcessor.Models
{
    public class DownloadedPropertyData
    {
        public Guid Id { get; set; }
        public string PropertyKey { get; set; }
        public string PropertyValue { get; set; }
        public Guid TraceDataId { get; set; }
    }
}
