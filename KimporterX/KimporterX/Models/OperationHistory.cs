using System;
using System.Collections.Generic;
using System.Text;

namespace KimporterX.Models
{
    public class OperationHistory
    {
        public DateTime Time { get; set; }
        public string FileName { get; set; }
        public string ConnName { get; set; }
        public int Type { get; set; }
        public string Output { get; set; }
    }
}
