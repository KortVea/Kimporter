﻿using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace DataProcessor.Models
{
    public class FeatureDescriptionBindingModel
    {
        [JsonProperty(PropertyName = "type")]
        public int Type { get; set; }

        [JsonProperty(PropertyName = "Unit")]
        public string EquipmentId { get; set; }

        [JsonProperty(PropertyName = "Date")]
        public DateTime Date { get; set; }

        [JsonProperty(PropertyName = "Location")]
        public string Location { get; set; }

        [JsonProperty(PropertyName = "Speed(mph)")]
        public float Speed { get; set; }

        [JsonProperty(PropertyName = "Mileage(mi)")]
        public float Milage { get; set; }

        [JsonProperty(PropertyName = "Orientation")]
        public string Orientation { get; set; }

        [JsonProperty(PropertyName = "Total capacity")]
        public float TotalCapacity { get; set; }

        [JsonProperty(PropertyName = "Exceeded fuel level difference limit")]
        public float ExceededFLDLimit { get; set; }

        [JsonProperty(PropertyName = "Time")]
        public DateTime Time { get; set; }

        [JsonProperty(PropertyName = "Fuel level percentage")]
        public float FuelLevelPercentage { get; set; }

        [JsonProperty(PropertyName = "Difference in fuel level")]
        public float DiffFuelLevel { get; set; }
    }
}
