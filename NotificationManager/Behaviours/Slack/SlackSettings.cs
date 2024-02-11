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
