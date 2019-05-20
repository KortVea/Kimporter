using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataProcessor.Helpers
{
    public class DMYDateTimeConverter : IsoDateTimeConverter
    {
        public DMYDateTimeConverter()
        {
            Culture = new System.Globalization.CultureInfo("en-AU");
        }
    }
}
