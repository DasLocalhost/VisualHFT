using System.Collections.Generic;
using VisualHFT.UserSettings;
using MarketConnectors.WebSocket.ViewModel;
using MarketConnectors.WebSocket.UserControls;
using VisualHFT.Commons.WPF.ViewModel;

namespace MarketConnectors.WebSocket.Model
{
    [DefaultSettingsView(typeof(PluginSettingsViewModel), typeof(PluginSettingsView))]
    [CompactSettingsView(typeof(PluginSettingsViewModel), typeof(PluginCompactSettingsView))]
    public class PlugInSettings : ISetting
    {
        public required string HostName { get; set; }
        public required int Port { get; set; }
        public int ProviderId { get; set; }
        public required string ProviderName { get; set; }
        public string Symbol { get; set; }
        public VisualHFT.Model.Provider Provider { get; set; }
        public AggregationLevel AggregationLevel { get; set; }
    }
}