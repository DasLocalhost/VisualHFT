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

namespace VisualHFT.NotificationManager.Toast
{
    /// <summary>
    /// Global settings for Win Toast notifications. 
    /// </summary>
    [DefaultSettingsView(typeof(ToastSettingsViewModel), typeof(ToastSettingsView))]
    public class ToastNotificationSetting : BaseNotificationSettings
    {
        /// <summary>
        /// Default header to be shown on UI.
        /// </summary>
        private const string _defaultHeader = "Toast Notifications";

        public ToastNotificationSetting(string behaviourId, string? targetName) : base(_defaultHeader, targetName, behaviourId) { }

        #region BaseNotificationSettings implementation

        protected override IPluginNotificationSettings GetDefaultPluginBasedSettings(string pluginId)
        {
            return new ToastPluginNotificationSetting(this)
            {
                IsEnabled = false,
                IncludeTimeStamp = false,
                PluginId = pluginId,
            };
        }

        #endregion
    }

    /// <summary>
    /// Plugin-related settings for Win Toast notifications. 
    /// </summary>
    [DefaultSettingsView(typeof(ToastPluginSettingsViewModel), typeof(ToastPluginSettingsView))]
    public class ToastPluginNotificationSetting : IPluginNotificationSettings
    {
        /// <summary>
        /// True if we need to include a time stamp to notification.
        /// </summary>
        public bool IncludeTimeStamp { get; set; }

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

        public ToastPluginNotificationSetting(ToastNotificationSetting parentSettings)
        {
            ParentSettings = parentSettings;
        }

        public override string ToString()
        {
            return $"{PluginId?.Substring(0, 5) ?? "NO ID"} - Status: {IsEnabled}";
        }
    }
}
