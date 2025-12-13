using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BarrocIntens.Data
{
    internal class BkrNumber
    {
        [JsonPropertyName("BKR-number")]
        public string BkrNumberValue { get; set; } = "";

        [JsonPropertyName("status")]
        public string Status { get; set; } = "";

    }
}
