using Newtonsoft.Json;
using VisualHFT.Model;

namespace VisualHFT.UserSettings
{
    public class TileSettings : ISetting
    {
        #region IBaseSettings implementation

        [JsonIgnore]
        public string? SettingId { get; set; }

        [JsonIgnore]
        public SettingKey? SettingKey { get; set; }

        #endregion

        public string Symbol { get; set; }
        public Provider Provider { get; set; }
        public AggregationLevel AggregationLevel { get; set; }
    }
}
