using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisualHFT.Commons.API.Zapier;
using VisualHFT.Commons.NotificationManager;
using VisualHFT.Commons.NotificationManager.Notifications;
using VisualHFT.NotificationManager.Slack;
using VisualHFT.NotificationManager.Toast;
using VisualHFT.NotificationManager.Twitter;
using VisualHFT.PluginManager;
using VisualHFT.UserSettings;

namespace VisualHFT.NotificationManager.Zapier
{
    /// <summary>
    /// Notifications logic for Zapier notifications.
    /// </summary>
    [Settings(typeof(ZapierNotificationSetting))]
    public class ZapierNotificationBehaviour : BaseNotificationBehaviour
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType);

        public ZapierNotificationBehaviour(ISettingsManager settingsManager) : base(settingsManager)
        {
            NotificationTargetName = "Zapier Notifications";
            Version = "1.0.0.0";
        }

        #region BaseNotificationBehavior implementation

        public override ZapierNotificationSetting? Settings => _settings as ZapierNotificationSetting;

        public override void Init(List<IPlugin> plugins)
        {
            // Code to init a behaviour
            base.Init(plugins);
            _settings?.InitPluginRelatedSettings(plugins);

            SetUpConnection();
        }

        public override void Send(INotification notification)
        {
            log.Debug($"Notifications: [{NotificationTargetName}] behavior received a new notification.");

            // If settings are not init yet - skip the notification
            if (_settings == null)
            {
                log.Warn($"Notifications: [{NotificationTargetName}] is not initialized properly. Received notification will be skipped.");
                return;
            }

            var pluginSetting = _settings.GetPluginSettings(notification.PluginId) as ZapierPluginNotificationSetting;
            if (pluginSetting == null)
            {
                log.Warn($"Notifications: Notification settings for [{notification.PluginName}] not found for [{NotificationTargetName}] behavior. Received notification will be skipped.");
                return;
            }

            // New types of notifications should be covered here
            switch (notification)
            {
                case TextNotification textNotification:

                    ShowSimpleNotification(textNotification, pluginSetting);
                    break;

                default:

                    throw new NotSupportedTypeException(this, notification);
            }
        }

        protected override BaseNotificationSettings InitializeDefaultSettings()
        {
            var settings = new ZapierNotificationSetting(GetUniqueId(), NotificationTargetName);

            SaveToUserSettings(settings);

            return settings;
        }

        #endregion

        /// <summary>
        /// Set up the initial connection to the target system. Any kinds of token-checking methods should be placed here.
        /// </summary>
        /// <exception cref="Exception"></exception>
        private void SetUpConnection()
        {
            if (!(_settings is ZapierNotificationSetting zapierSettings))
                throw new Exception();
        }

        private void ShowSimpleNotification(TextNotification textNotification, ZapierPluginNotificationSetting pluginSetting)
        {
            if (pluginSetting.FullWebHookUrl == null)
            {
                log.Warn($"Notifications: [{NotificationTargetName}] has an web hook url for [{textNotification.PluginName}] plugin. Received notification will be skipped.");
                return;
            };

            var client = new ZapierClient(pluginSetting.FullWebHookUrl);

            client?.Send(new ZapierMessage()
            {
                Header = textNotification.Title,
                Text = textNotification.Text
            });
        }
    }
}