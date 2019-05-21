﻿using System;
using System.Collections.Generic;

namespace DataProcessor.Models
{
    public partial class DownloadedPropertyData: EntityBase
    {
        public string PropertyKey { get; set; }
        public string PropertyValue { get; set; }
        public Guid TraceDataId { get; set; }

        //public virtual DownloadedTraceData TraceData { get; set; }
    }
}
