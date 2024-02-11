using System.Collections.Generic;
using VisualHFT.Model;
using VisualHFT.UserSettings;
using VisualHFT.Studies.MarketResilience.UserControls;
using VisualHFT.Studies.MarketResilience.ViewModel;
using VisualHFT.Commons.WPF.ViewModel;
using Newtonsoft.Json;

namespace VisualHFT.Studies.MarketResilience.Model
{
    [DefaultSettingsView(typeof(PluginSettingsViewModel), typeof(PluginSettingsView))]
    [CompactSettingsView(typeof(PluginSettingsViewModel), typeof(PluginCompactSettingsView))]
    public class PlugInSettings : ISetting
    {
        public string Symbol { get; set; }
        public Provider Provider { get; set; }
        public AggregationLevel AggregationLevel { get; set; }

        [JsonIgnore]
        public string TitlePostfix { get; set; } = string.Empty;
    }
}