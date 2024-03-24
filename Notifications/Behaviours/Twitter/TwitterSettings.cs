using Newtonsoft.Json;
using VisualHFT.Commons.NotificationManager;
using VisualHFT.Commons.WPF.ViewMapping;
using VisualHFT.UserSettings;
using VisualHFT.View.Settings;
using VisualHFT.ViewModel.Settings;

namespace VisualHFT.Notifications.Twitter
{
    /// <summary>
    /// Global settings for Twitter notifications. 
    /// </summary>
    [DefaultSettingsView(typeof(TwitterSettingsViewModel), typeof(TwitterSettingsView))]
    public class TwitterNotificationSetting : BaseNotificationSettings
    {
        /// <summary>
        /// Default header to be shown on UI.
        /// </summary>
        private const string _defaultHeader = "Twitter Notifications";

        public TwitterNotificationSetting(string behaviourId, string? targetName) : base(_defaultHeader, targetName, behaviourId) { }

        #region BaseNotificationSettings implementation

        protected override IPluginNotificationSettings GetDefaultPluginBasedSettings(string pluginId)
        {
            return new TwitterPluginNotificationSetting(this)
            {
                IsEnabled = false,
                PluginId = pluginId,
                Threshold = 1,
                ThresholdRule = ThresholdRule.Less
            };
        }

        #endregion
    }

    /// <summary>
    /// Plugin-related settings for Twitter notifications. 
    /// </summary>
    [DefaultSettingsView(typeof(TwitterPluginSettingsViewModel), typeof(TwitterPluginSettingsView))]
    public class TwitterPluginNotificationSetting : IPluginNotificationSettings
    {
        #region IBaseSettings implementation

        [JsonIgnore]
        public string? SettingId { get; set; }

        [JsonIgnore]
        public SettingKey? SettingKey { get; set; }

        #endregion

        #region Properties

        public string? ApiToken { get; set; }
        public string? ApiSecret { get; set; }
        public string? OAuthToken { get; set; }
        public string? OAuthSecret { get; set; }
        public string? AuthVerifier { get; set; }
        public string? AccessToken { get; set; }
        public string? AccessSecret { get; set;}

        #endregion

        #region IPluginNotificationSettings implementation

        public bool IsEnabled { get; set; }
        public double? Threshold { get; set; }
        public ThresholdRule ThresholdRule { get; set; }

        [JsonIgnore]
        public string? PluginId { get; set; }
        [JsonIgnore]
        public BaseNotificationSettings ParentSettings { get; set; }

        public void UpdateInSource()
        {
            ParentSettings.UpdateSubSetting(this);
        }

        #endregion

        public TwitterPluginNotificationSetting(TwitterNotificationSetting parentSettings)
        {
            ParentSettings = parentSettings;
        }

        public override string ToString()
        {
            return $"{PluginId?.Substring(0, 5) ?? "NO ID"} - Status: {IsEnabled}";
        }
    }
}