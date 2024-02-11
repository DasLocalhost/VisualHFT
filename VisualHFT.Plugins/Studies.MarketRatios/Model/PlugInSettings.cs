using System.Collections.Generic;
using VisualHFT.Model;
using VisualHFT.UserSettings;
using VisualHFT.Studies.MarketRatios.ViewModel;
using VisualHFT.Studies.MarketRatios.UserControls;
using VisualHFT.Commons.WPF.ViewModel;

namespace VisualHFT.Studies.MarketRatios.Model
{
    [DefaultSettingsView(typeof(PluginSettingsViewModel), typeof(PluginSettingsView))]
    [CompactSettingsView(typeof(PluginSettingsViewModel), typeof(PluginCompactSettingsView))]
    public class PlugInSettings : ISetting
    {
        public string Symbol { get; set; }
        public Provider Provider { get; set; }
        public AggregationLevel AggregationLevel { get; set; }
    }
}
