using Newtonsoft.Json;
using VisualHFT.Commons.WPF.ViewMapping;
using VisualHFT.Model;
using VisualHFT.Studies.LOBImbalance.UserControls;
using VisualHFT.Studies.LOBImbalance.ViewModel;
using VisualHFT.UserSettings;

namespace VisualHFT.Studies.LOBImbalance.Model
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
    }
}
