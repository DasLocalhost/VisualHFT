using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisualHFT.Commons.WPF.ViewModel;
using VisualHFT.UserSettings;
using VisualHFT.View.Settings;
using VisualHFT.ViewModel.Settings;

namespace VisualHFT.NotificationManager.Twitter
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
                PluginId = pluginId
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