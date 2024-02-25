using Newtonsoft.Json;
using VisualHFT.PluginManager;

namespace VisualHFT.UserSettings
{
    /// <summary>
    /// Base settings for notifications behaviour with technical properties as threshold and update time
    /// </summary>
    public abstract class BaseNotificationSettings : INotificationSettings
    {
        #region IBaseSettings implementation

        [JsonIgnore]
        public string? SettingId { get; set; }

        [JsonIgnore]
        public SettingKey? SettingKey { get; set; }

        #endregion

        /// <summary>
        /// Header to display on UI
        /// </summary>
        public string SettingsHeader { get; }

        /// <summary>
        /// Name of the notifications target system.
        /// </summary>
        public string? TargetName { get; }

        /// <summary>
        /// Id of related notifications behavior.
        /// </summary>
        [JsonIgnore]
        public string BehaviourId { get; set; }

        /// <summary>
        /// Count of notifications taking from Notifications pool on each tick
        /// </summary>
        public int Threshold { get; set; }
        /// <summary>
        /// Time in ms between ticks
        /// </summary>
        public int UpdateTime { get; set; }

        /// <summary>
        /// Plugins-related settings
        /// </summary>
        public Dictionary<string, IPluginNotificationSettings>? PluginSettings { get; set; }

        #region INotificationSettings implementation

        #endregion

        public BaseNotificationSettings(string header, string? targetName, string behaviourId)
        {
            SettingsHeader = header;
            TargetName = targetName;
            BehaviourId = behaviourId;
        }

        /// <summary>
        /// Update the list of plugins settings with missing settings based on list of active plugins
        /// </summary>
        /// <param name="plugins">List of active plugins</param>
        public void InitPluginRelatedSettings(List<IPlugin> plugins)
        {
            if (PluginSettings == null)
                PluginSettings = new Dictionary<string, IPluginNotificationSettings>();

            foreach (var plugin in plugins)
            {
                var id = plugin.GetPluginUniqueID();

                if (!PluginSettings.ContainsKey(id))
                    PluginSettings.Add(id, GetDefaultPluginBasedSettings(id));
            }
        }

        /// <summary>
        /// Update subsetting on parent by Unique id.
        /// </summary>
        /// <param name="pluginNotificationSettings">Sub setting</param>
        public void UpdateSubSetting(IPluginNotificationSettings pluginNotificationSettings)
        {
            if (PluginSettings == null || string.IsNullOrEmpty(pluginNotificationSettings.PluginId))
                return;

            PluginSettings[pluginNotificationSettings.PluginId] = pluginNotificationSettings;
        }

        /// <summary>
        /// Generate a default plugin-related setting
        /// </summary>
        /// <returns>Plugin-related setting</returns>
        protected abstract IPluginNotificationSettings GetDefaultPluginBasedSettings(string pluginId);

        /// <summary>
        /// Get plugin-related notifications settings for the specific plugin.
        /// </summary>
        /// <param name="pluginId">Unique Id of the specific plugin.</param>
        /// <returns>Notification settings for the specific plugin.</returns>
        public IPluginNotificationSettings? GetPluginSettings(string pluginId)
        {
            return PluginSettings?[pluginId];
        }
    }
}