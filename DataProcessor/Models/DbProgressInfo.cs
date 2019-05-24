using System;
using System.Collections.Generic;
using System.Text;

namespace DataProcessor.Models
{
    public class DbProgressInfo
    {
        public ProgressType ProgressType { get; set; }
        public int Number1 { get; set; }
        public int Number2 { get; set; }
        public string Message { get; set; }
    }

    public enum ProgressType
    {
        Default, Writing, Reading, Exception
    }
}
