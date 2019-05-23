using Newtonsoft.Json.Converters;

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
