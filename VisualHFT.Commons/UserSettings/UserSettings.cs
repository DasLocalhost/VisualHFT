using log4net;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace VisualHFT.UserSettings
{
    [Serializable]
    [JsonConverter(typeof(UserSettingsSerializer))]
    public class UserSettings
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public HashSet<SettingsGroup> ComponentSettings { get; set; }

        public event EventHandler<IBaseSettings>? SettingsChanged;

        public event EventHandler? OnSave;

        public UserSettings()
        {
            ComponentSettings = new HashSet<SettingsGroup>();
        }

        /// <summary>
        /// Get Setting by group key and settings id.
        /// </summary>
        /// <param name="key">Key of settings group</param>
        /// <param name="id">Settings id</param>
        /// <returns>Settings instance</returns>
        public IBaseSettings? GetSetting(SettingKey key, string id)
        {
            var group = ComponentSettings.FirstOrDefault(_ => _.SettingKey == key);

            if (group == null)
            {
                log.Warn($"Settings: Setting group with [{key}] key is not found in User Settings.");
                return null;
            }

            var container = group.SettingContainers.FirstOrDefault(_ => _.Id == id);

            if (container == null)
            {
                log.Warn($"Settings: Setting with [{id}] id is not found in User Settings, [{key}] group.");
                return null;
            }

            return container.Settings;
        }

        /// <summary>
        /// Get Setting by settings id.
        /// </summary>
        /// <param name="id">Settings id</param>
        /// <returns>Settings instance</returns>
        public IBaseSettings? GetSetting(string id)
        {
            var container = ComponentSettings.SelectMany(_ => _.SettingContainers).FirstOrDefault(_ => _.Id == id);

            if (container == null)
            {
                log.Warn($"Settings: Settings with [{id}] id is not found in User Settings.");
                return null;
            }

            return container.Settings;
        }

        /// <summary>
        /// Set or update the settings instance in the User settings.
        /// </summary>
        /// <param name="key">Settings group to save in</param>
        /// <param name="id">Settings id to save</param>
        /// <param name="setting">Settings instance</param>
        public void SetSetting(SettingKey key, string id, IBaseSettings setting)
        {
            var settingGroup = new SettingsGroup(key);

            if (ComponentSettings.Contains(settingGroup))
                settingGroup = ComponentSettings.First(_ => _.SettingKey == key);
            else
                ComponentSettings.Add(settingGroup);

            settingGroup.AddOrUpdate(id, setting);

            SaveSetting(setting);
        }

        /// <summary>
        /// Save settings in file and raise a Settings Changed event to let all subsystem know about changes.
        /// </summary>
        /// <param name="setting">Settings instance</param>
        public void SaveSetting(IBaseSettings setting)
        {
            RaiseSettingsChanged(setting);
            OnSave?.Invoke(this, EventArgs.Empty);
            //_settingsManager.Save();
        }

        /// <summary>
        /// Raise a Settings Changed event to let all subsystem know about changes.
        /// </summary>
        /// <param name="setting">Settings instance</param>
        public void RaiseSettingsChanged(IBaseSettings setting)
        {
            SettingsChanged?.Invoke(this, setting);
        }

        /// <summary>
        /// Get the list of Plugin-related settings for all notification behaviors.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<(string Key, IPluginNotificationSettings Setting)>? GetPluginsRelatedNotificationSettings()
        {
            var group = ComponentSettings.FirstOrDefault(_ => _.SettingKey == SettingKey.NOTIFICATION);

            if (group == null)
            {
                log.Warn($"Settings: NOTIFICATION settings group not found.");
                yield break;
            }

            foreach (var container in group.SettingContainers)
            {
                if (!(container.Settings is BaseNotificationSettings baseSettings))
                    continue;

                if (baseSettings.PluginSettings == null)
                    continue;

                foreach (var pluginSettings in baseSettings.PluginSettings)
                    yield return (pluginSettings.Key, pluginSettings.Value);
            }
        }
    }

    public class SettingsGroup
    {

        [JsonConverter(typeof(StringEnumConverter))]
        public SettingKey SettingKey { get; set; }

        public HashSet<SettingContainer> SettingContainers { get; set; }

        public SettingsGroup(SettingKey key)
        {
            SettingKey = key;
            SettingContainers = new HashSet<SettingContainer>();
        }

        public void AddOrUpdate(string id, IBaseSettings settings)
        {
            var setting = SettingContainers.FirstOrDefault(_ => _.Id == id);

            if (setting == null)
            {
                setting = new SettingContainer(id, settings);
                SettingContainers.Add(setting);
            }

            setting.Settings = settings;
        }

        public override int GetHashCode()
        {
            return SettingKey.GetHashCode();
        }

        public override bool Equals(object? obj)
        {
            if (obj is SettingsGroup group)
                return this.GetHashCode() == group.GetHashCode();

            return base.Equals(obj);
        }
    }

    public class SettingContainer
    {
        public string Id { get; set; }
        public IBaseSettings Settings { get; set; }

        public SettingContainer(string id, IBaseSettings settings)
        {
            Id = id;
            Settings = settings;
        }

        public override int GetHashCode()
        {
            return Id?.GetHashCode() ?? 0;
        }

        public override bool Equals(object? obj)
        {
            if (obj is SettingContainer container)
                return this.GetHashCode() == container.GetHashCode();

            return base.Equals(obj);
        }
    }

    /// <summary>
    /// Custom serializer to keep settings saved in dictionary-like way.
    /// </summary>
    public class UserSettingsSerializer : JsonConverter
    {
        private const string _settingsKey = "Settings";
        private const string _componentsSettingsKey = "ComponentSettings";

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            var userSetting = value as UserSettings;
            if (userSetting == null)
                throw new NotImplementedException();

            var settingsDict = UserSettingsDictionary.From(userSetting);

            serializer.Serialize(writer, settingsDict);
        }

        public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            try
            {
                var settingsDict = serializer.Deserialize<UserSettingsDictionary>(reader);

                if (settingsDict == null || settingsDict.ComponentSettings.Count == 0 || settingsDict.Settings.Count == 0)
                    return null;

                return UserSettingsDictionary.To(settingsDict);
            }
            catch
            {
                // TODO : add logs here
                return null;
            }
        }

        public override bool CanConvert(Type objectType)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Proxy class between json file and settings container class used to save settings properly.
        /// </summary>
        private class UserSettingsDictionary
        {
            private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

            public Dictionary<SettingKey, object> Settings { get; set; } = new Dictionary<SettingKey, object>();
            public Dictionary<SettingKey, Dictionary<string, object>> ComponentSettings { get; set; } = new Dictionary<SettingKey, Dictionary<string, object>>();

            /// <summary>
            /// Convert UserSettings to Dictionary ready for serialization.
            /// </summary>
            /// <param name="userSettings">Settings to save</param>
            /// <returns>Settings as dictionary</returns>
            public static UserSettingsDictionary? From(UserSettings? userSettings)
            {
                if (userSettings == null)
                    return null;

                var settings = new Dictionary<SettingKey, object>();
                var componentSettings = new Dictionary<SettingKey, Dictionary<string, object>>();

                foreach (var settingGroup in userSettings.ComponentSettings)
                {
                    var key = settingGroup.SettingKey;

                    if (componentSettings.ContainsKey(key))
                    {
                        log.Warn($"Settings: Duplicated component key [{key}].");
                        continue;
                    }

                    var components = new Dictionary<string, object>();

                    foreach (var container in settingGroup.SettingContainers)
                    {
                        var id = container.Id;

                        if (components.ContainsKey(id))
                        {
                            log.Warn($"Settings: Duplicated settings id [{id}].");
                            continue;
                        }

                        components.Add(id, container.Settings);
                    }

                    componentSettings.Add(key, components);
                }

                return new UserSettingsDictionary()
                {
                    ComponentSettings = componentSettings,
                    Settings = settings
                };
            }

            /// <summary>
            /// Convert Dictionary to User Settings.
            /// </summary>
            /// <param name="userSettingsDictionary">Settings dictionary, deserialized from settings json file.</param>
            /// <returns>Ready-to-use settings container.</returns>
            public static UserSettings? To(UserSettingsDictionary? userSettingsDictionary)
            {
                if (userSettingsDictionary == null)
                    return null;

                var userSettings = new UserSettings();

                foreach (var comp in userSettingsDictionary.ComponentSettings)
                {
                    var key = comp.Key;
                    var containers = new HashSet<SettingContainer>();

                    foreach (var container in comp.Value)
                    {
                        if (container.Value is not IBaseSettings setting)
                            continue;

                        containers.Add(new SettingContainer(container.Key, setting));

                        if (container.Value is IBaseSettings settings)
                        {
                            settings.SettingKey = key;
                            settings.SettingId = container.Key;
                        }

                        if (container.Value is BaseNotificationSettings notificationSetting)
                        {
                            if (notificationSetting.PluginSettings != null)
                                foreach (var subSetting in notificationSetting.PluginSettings)
                                {
                                    subSetting.Value.PluginId = subSetting.Key;
                                    subSetting.Value.ParentSettings = notificationSetting;
                                }

                            notificationSetting.BehaviourId = container.Key;
                        }
                    }

                    userSettings.ComponentSettings.Add(new SettingsGroup(key) { SettingContainers = containers });
                }

                return userSettings;
            }
        }
    }
}