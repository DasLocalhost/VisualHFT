using System.Runtime;
using System.Security.Cryptography;
using System.Text;
using VisualHFT.Commons.PluginManager;
using VisualHFT.PluginManager;
using VisualHFT.UserSettings;

namespace VisualHFT.Commons.NotificationManager
{
    // TODO : both for notifications and plugins, settings could be moved to SettingContainer abstract class - no need to write same code for each of them
    public abstract class BaseNotificationBehaviour : INotificationBehaviour
    {
        #region Fields

        protected BaseNotificationSettings? _settings = null;
        protected readonly ISettingsManager _settingsManager;
        protected readonly IPluginManager _pluginManager;

        #endregion

        #region Properties

        public string? NotificationTargetName { get; protected set; }
        public string? Version { get; protected set; }

        #endregion

        #region INotificationBehaviour

        public string UniqueId => GetUniqueId();
        public virtual BaseNotificationSettings? Settings => _settings;
        public abstract void Send(INotification notification);

        #endregion

        public BaseNotificationBehaviour(ISettingsManager settingsManager, IPluginManager pluginManager)
        {
            _settingsManager = settingsManager;
            _pluginManager = pluginManager;
        }

        public virtual void Initialize()
        {
            LoadSettings();
        }

        protected void LoadSettings()
        {
            _settings = LoadFromUserSettings<BaseNotificationSettings>();

            if (_settings == null)
            {
                _settings = InitializeDefaultSettings();
            }
        }

        protected abstract BaseNotificationSettings InitializeDefaultSettings();

        // TODO : move to settings provider?
        protected T? LoadFromUserSettings<T>() where T : class
        {
            var settingFromFile = _settingsManager.GetSetting<T>(SettingKey.NOTIFICATION, GetUniqueId());

            return settingFromFile;
        }

        protected void SaveToUserSettings(IBaseSettings settings)
        {
            string header = GetUniqueId();
            _settingsManager.SetSetting(SettingKey.NOTIFICATION, header, settings);
        }

        /// <summary>
        /// TODO : move unique id to the field, don't show it on UI, replace with the readable name on UI
        /// </summary>
        /// <returns></returns>
        protected virtual string GetUniqueId()
        {
            // Get the fully qualified name of the assembly
            string? assemblyName = GetType().Assembly.FullName;

            // Concatenate the attributes
            string combinedAttributes = $"{NotificationTargetName}{Version}{assemblyName}";

            // Compute the SHA256 hash
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(combinedAttributes));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        public void Update(INotification notification)
        {
            throw new NotImplementedException();
        }
    }
}