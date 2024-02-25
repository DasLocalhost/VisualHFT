using Newtonsoft.Json;
using VisualHFT.Commons.WPF.ViewMapping;
using VisualHFT.Model;
using VisualHFT.Studies.MarketResilience.UserControls;
using VisualHFT.Studies.MarketResilience.ViewModel;
using VisualHFT.UserSettings;

namespace VisualHFT.Studies.MarketResilience.Model
{
    [DefaultSettingsView(typeof(PluginSettingsViewModel), typeof(PluginSettingsView))]
    [CompactSettingsView(typeof(PluginSettingsViewModel), typeof(PluginCompactSettingsView))]
    public class PlugInSettings : ISetting
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

        [JsonIgnore]
        public string TitlePostfix { get; set; } = string.Empty;
    }
}