using DataProcessor.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace DataProcessor.Models
{
    public class FeatureDescriptionBindingModel
    {
        [JsonProperty(PropertyName = "type")]
        public int Type { get; set; }

        [JsonProperty(PropertyName = "Unit")]
        public string EquipmentId { get; set; }

        [JsonProperty(PropertyName = "Date")]
        [JsonConverter(typeof(DMYDateTimeConverter))]
        public DateTime Time { get; set; }

        [JsonProperty(PropertyName = "Location")]
        public string Location { get; set; }

        [JsonProperty(PropertyName = "Speed(mph)")]
        public float Speed { get; set; }

        [JsonProperty(PropertyName = "Mileage(mi)")]
        public float Mileage { get; set; }

        [JsonProperty(PropertyName = "Orientation")]
        public string Orientation { get; set; }

        [JsonProperty(PropertyName = "Total capacity")]
        public float TotalCapacity { get; set; }

        [JsonProperty(PropertyName = "Exceeded fuel level difference limit")]
        public float ExceededFLDLimit { get; set; }

        [JsonProperty(PropertyName = "Fuel level percentage")]
        public float FuelLevelPercentage { get; set; }

        [JsonProperty(PropertyName = "Difference in fuel level")]
        public float DiffFuelLevel { get; set; }

        public Dictionary<string, string> DownloadedPropertyData { get; set; }
    }
}
