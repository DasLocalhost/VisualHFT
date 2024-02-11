using System.Collections.Generic;
using VisualHFT.Model;
using VisualHFT.UserSettings;
using VisualHFT.Studies.LOBImbalance.ViewModel;
using VisualHFT.Studies.LOBImbalance.UserControls;
using VisualHFT.Commons.WPF.ViewModel;

namespace VisualHFT.Studies.LOBImbalance.Model
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
