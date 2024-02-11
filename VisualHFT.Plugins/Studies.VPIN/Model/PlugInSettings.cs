using System.Collections.Generic;
using VisualHFT.Model;
using VisualHFT.UserSettings;
using VisualHFT.Studies.VPIN.UserControls;
using VisualHFT.Studies.VPIN.ViewModel;
using VisualHFT.Commons.WPF.ViewModel;

namespace VisualHFT.Studies.VPIN.Model
{
    [DefaultSettingsView(typeof(PluginSettingsViewModel), typeof(PluginSettingsView))]
    [CompactSettingsView(typeof(PluginSettingsViewModel), typeof(PluginCompactSettingsView))]
    public class PlugInSettings : ISetting
    {
        public double BucketVolSize { get; set; }
        public string Symbol { get; set; }
        public Provider Provider { get; set; }
        public AggregationLevel AggregationLevel { get; set; }
    }
}