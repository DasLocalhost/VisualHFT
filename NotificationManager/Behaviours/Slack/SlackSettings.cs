using Newtonsoft.Json;
using VisualHFT.Commons.WPF.ViewMapping;
using VisualHFT.UserSettings;
using VisualHFT.View.Settings;
using VisualHFT.ViewModel.Settings;

namespace VisualHFT.NotificationManager.Slack
{
    /// <summary>
    /// Global settings for Slack notifications. 
    /// </summary>
    [DefaultSettingsView(typeof(SlackSettingsViewModel), typeof(SlackSettingsView))]
    public class SlackNotificationSetting : BaseNotificationSettings
    {
        /// <summary>
        /// Default header to be shown on UI.
        /// </summary>
        private const string _defaultHeader = "Slack Notifications";

        public SlackNotificationSetting(string behaviourId, string? targetName) : base(_defaultHeader, targetName, behaviourId) { }

        #region BaseNotificationSettings implementation

        protected override IPluginNotificationSettings GetDefaultPluginBasedSettings(string pluginId)
        {
            return new SlackPluginNotificationSetting(this)
            {
                IsEnabled = false,
                Channel = string.Empty,
                Token = string.Empty,
                PluginId = pluginId
            };
        }

        #endregion
    }

    /// <summary>
    /// Plugin-related settings for Slack notifications. 
    /// </summary>
    [DefaultSettingsView(typeof(SlackPluginSettingsViewModel), typeof(SlackPluginSettingsView))]
    public class SlackPluginNotificationSetting : IPluginNotificationSettings
    {
        #region IBaseSettings implementation

        [JsonIgnore]
        public string? SettingId { get; set; }

        [JsonIgnore]
        public SettingKey? SettingKey { get; set; }

        #endregion

        /// <summary>
        /// Id of Slack channel to send notifications.
        /// </summary>
        public string? Channel { get; set; }
        /// <summary>
        /// App-level token of Slack bot app.
        /// </summary>
        public string? Token { get; set; }

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

        public SlackPluginNotificationSetting(SlackNotificationSetting parentSettings)
        {
            ParentSettings = parentSettings;
        }

        public override string ToString()
        {
            return $"{PluginId?.Substring(0, 5) ?? "NO ID"} - Status: {IsEnabled}";
        }
    }
}
