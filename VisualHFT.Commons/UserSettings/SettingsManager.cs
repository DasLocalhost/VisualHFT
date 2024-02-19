using Newtonsoft.Json;

namespace VisualHFT.UserSettings
{
    /*
     USAGE:
            // To set a setting
            SettingsManager.Instance.SetSetting(SettingKey.Theme, "Dark");

            // To get a setting
            object theme = SettingsManager.Instance.GetSetting(SettingKey.Theme);

            // To save settings
            SettingsManager.Instance.SaveSettings();     
     */
    public class SettingsManager : ISettingsManager
    {
        #region Fields

        private string appDataPath;
        private string settingsFilePath;
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly JsonSerializerSettings _serializerSettings = new JsonSerializerSettings()
        {
            TypeNameHandling = TypeNameHandling.Auto,
        };

        #endregion

        public event EventHandler<IBaseSettings>? SettingsChanged;

        //public static SettingsManager Instance => lazy.Value;

        public UserSettings? UserSettings { get; set; }

        public string GetAllSettings()
        {
            if (File.Exists(settingsFilePath))
            {
                string json = File.ReadAllText(settingsFilePath);
                return json;
            }
            else
                return null;
        }

        private SettingsManager()
        {
            appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            settingsFilePath = Path.Combine(appDataPath, "VisualHFT", "settings.json");
            // Load settings from file or create new
            LoadSettings();
        }

        private void LoadSettings()
        {
            // Deserialize from JSON file
            if (File.Exists(settingsFilePath))
            {
                string json = File.ReadAllText(settingsFilePath);
                UserSettings = JsonConvert.DeserializeObject<UserSettings>(json, _serializerSettings);
            }
            else
            {
                UserSettings = new UserSettings();
            }

            if (UserSettings != null)
            {
                UserSettings.SettingsChanged += (_, __) => SettingsChanged?.Invoke(this, __);
                UserSettings.OnSave += (_, __) => Save();
            }
        }

        private void SaveSettings()
        {
            try
            {
                // Ensure the directory exists
                string directoryPath = Path.GetDirectoryName(settingsFilePath);
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                // Serialize to JSON file
                string json = JsonConvert.SerializeObject(UserSettings, _serializerSettings);

                // Write to file
                File.WriteAllText(settingsFilePath, json);
            }
            catch (Exception ex)
            {
                // Log or handle the exception as needed
                log.Error($"An error occurred while saving settings: {ex.ToString()}");
            }
        }

        public T? GetSetting<T>(SettingKey key, string id) where T : class
        {
            var value = UserSettings.GetSetting(key, id);

            if (value == null)
                return null;

            if (value is T castedValue)
                return castedValue;

            try
            {
                var strValue = value.ToString();
                if (strValue == null)
                    return null;

                var typedValue = JsonConvert.DeserializeObject<T>(strValue);
                if (typedValue != null)
                    return typedValue;
            }
            catch (JsonException)
            {
                return null;
            }

            return null;
        }

        public void SetSetting(SettingKey key, string id, IBaseSettings setting)
        {
            UserSettings?.SetSetting(key, id, setting);
            SettingsChanged?.Invoke(this, setting);

            SaveSettings();
        }

        public void Save()
        {
            SaveSettings();
        }
    }
}