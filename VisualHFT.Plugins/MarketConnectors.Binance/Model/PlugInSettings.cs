using MarketConnectors.Binance.UserControls;
using MarketConnectors.Binance.ViewModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using VisualHFT.Commons.WPF.ViewMapping;
using VisualHFT.UserSettings;

namespace MarketConnectors.Binance.Model
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

        public string ApiKey { get; set; }
        public string ApiSecret { get; set; }
        public List<string> Symbols { get; set; }
        public int DepthLevels { get; set; }
        public int UpdateIntervalMs { get; set; }

        public string Symbol { get; set; }
        public VisualHFT.Model.Provider Provider { get; set; }
        public AggregationLevel AggregationLevel { get; set; }

        public event EventHandler? OnSettingsChanged;
    }
}