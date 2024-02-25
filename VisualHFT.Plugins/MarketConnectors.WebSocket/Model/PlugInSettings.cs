using MarketConnectors.WebSocket.UserControls;
using MarketConnectors.WebSocket.ViewModel;
using Newtonsoft.Json;
using VisualHFT.Commons.WPF.ViewMapping;
using VisualHFT.UserSettings;

namespace MarketConnectors.WebSocket.Model
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
        public required string HostName { get; set; }
        public required int Port { get; set; }
        public int ProviderId { get; set; }
        public required string ProviderName { get; set; }
        public string Symbol { get; set; }
        public VisualHFT.Model.Provider Provider { get; set; }
        public AggregationLevel AggregationLevel { get; set; }
    }
}