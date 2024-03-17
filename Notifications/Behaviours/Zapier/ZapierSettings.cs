using Newtonsoft.Json;
using VisualHFT.Commons.WPF.ViewMapping;
using VisualHFT.UserSettings;
using VisualHFT.View.Settings;
using VisualHFT.ViewModel.Settings;

namespace VisualHFT.Notifications.Zapier
{
    /// <summary>
    /// Global settings for Zapier notifications. 
    /// </summary>
    [DefaultSettingsView(typeof(ZapierSettingsViewModel), typeof(ZapierSettingsView))]
    public class ZapierNotificationSetting : BaseNotificationSettings
    {
        /// <summary>
        /// Default header to be shown on UI.
        /// </summary>
        private const string _defaultHeader = "Zapier Notifications";

        public ZapierNotificationSetting(string behaviourId, string? targetName) : base(_defaultHeader, targetName, behaviourId) { }

        #region BaseNotificationSettings implementation

        protected override IPluginNotificationSettings GetDefaultPluginBasedSettings(string pluginId)
        {
            return new ZapierPluginNotificationSetting(this)
            {
                IsEnabled = false,
                WebHookUrl = string.Empty,
                PluginId = pluginId
            };
        }

        #endregion
    }

    /// <summary>
    /// Plugin-related settings for Zapier notifications. 
    /// </summary>
    [DefaultSettingsView(typeof(ZapierPluginSettingsViewModel), typeof(ZapierPluginSettingsView))]
    public class ZapierPluginNotificationSetting : IPluginNotificationSettings
    {
        #region IBaseSettings implementation

        [JsonIgnore]
        public string? SettingId { get; set; }

        [JsonIgnore]
        public SettingKey? SettingKey { get; set; }

        #endregion

        /// <summary>
        /// Default prefix for Zapier web hook url
        /// </summary>
        private const string _webHookPrefix = "https://hooks.zapier.com/hooks/catch/";

        #region Properties

        /// <summary>
        /// Url to webhook used to sent notifications.
        /// </summary>
        public string? WebHookUrl { get; set; }

        /// <summary>
        /// Prefix of webhook Url to show on UI properly.
        /// </summary>
        [JsonIgnore]
        public string WebHookPrefix => _webHookPrefix;
        /// <summary>
        /// Fill web hook Url used in API calls.
        /// </summary>
        [JsonIgnore]
        public string FullWebHookUrl => _webHookPrefix + WebHookUrl;

        #endregion

        #region IPluginNotificationSettings implementation

        public bool IsEnabled { get; set; }

        [JsonIgnore]
        public string? PluginId { get; set; }
        [JsonIgnore]
        public BaseNotificationSettings ParentSettings { get; set; }

        public void UpdateInSource()
        {
            ParentSettings.UpdateSubSetting(this);
        }

        #endregion

        public ZapierPluginNotificationSetting(ZapierNotificationSetting parentSettings)
        {
            ParentSettings = parentSettings;
        }

        public override string ToString()
        {
            return $"{PluginId?.Substring(0, 5) ?? "NO ID"} - Status: {IsEnabled}";
        }
    }
}
