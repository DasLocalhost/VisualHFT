using Newtonsoft.Json;
using VisualHFT.Commons.NotificationManager;
using VisualHFT.Commons.WPF.ViewMapping;
using VisualHFT.UserSettings;
using VisualHFT.View.Settings;
using VisualHFT.ViewModel.Settings;

namespace VisualHFT.Notifications.Toast
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

        // TODO : maybe moved from settings file to plugin? plugin will decide what is a standard Threshold and Rule
        protected override IPluginNotificationSettings GetDefaultPluginBasedSettings(string pluginId)
        {
            return new ToastPluginNotificationSetting(this)
            {
                IsEnabled = false,
                IncludeTimeStamp = false,
                PluginId = pluginId,
                Threshold = 1,
                ThresholdRule = ThresholdRule.Less
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
        #region IBaseSettings implementation

        [JsonIgnore]
        public string? SettingId { get; set; }

        [JsonIgnore]
        public SettingKey? SettingKey { get; set; }

        #endregion

        /// <summary>
        /// True if we need to include a time stamp to notification.
        /// </summary>
        public bool IncludeTimeStamp { get; set; }

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
