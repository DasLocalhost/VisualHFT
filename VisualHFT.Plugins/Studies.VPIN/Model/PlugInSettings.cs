using Newtonsoft.Json;
using VisualHFT.Commons.WPF.ViewMapping;
using VisualHFT.Model;
using VisualHFT.Studies.VPIN.UserControls;
using VisualHFT.Studies.VPIN.ViewModel;
using VisualHFT.UserSettings;

namespace VisualHFT.Studies.VPIN.Model
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

        public double BucketVolSize { get; set; }
        public string Symbol { get; set; }
        public Provider Provider { get; set; }
        public AggregationLevel AggregationLevel { get; set; }
    }
}