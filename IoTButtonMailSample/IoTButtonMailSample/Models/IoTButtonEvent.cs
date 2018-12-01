using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace IoTButtonMailSample.Models
{
    public class IoTButtonEvent
    {
        [JsonProperty("serialNumber")]
        public string SerialNumber { get; set; }

        [JsonProperty("batteryVoltage")]
        public string BatteryVoltage { get; set; }

        [JsonProperty("clickType")]
        [JsonConverter(typeof(StringEnumConverter))]
        public ClickType ClickType { get; set; }
    }
}
