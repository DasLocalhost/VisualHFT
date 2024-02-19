using QuickFix;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisualHFT.Commons.API.Slack;
using VisualHFT.Commons.NotificationManager;
using VisualHFT.Commons.NotificationManager.Notifications;
using VisualHFT.NotificationManager.Toast;
using VisualHFT.NotificationManager.Zapier;
using VisualHFT.PluginManager;
using VisualHFT.UserSettings;

namespace VisualHFT.NotificationManager.Slack
{
    /// <summary>
    /// Notifications logic for Slack notifications.
    /// </summary>
    [Settings(typeof(SlackNotificationSetting))]
    public class SlackNotificationBehaviour : BaseNotificationBehaviour
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType);

        public SlackNotificationBehaviour(ISettingsManager settingsManager) : base(settingsManager)
        {
            NotificationTargetName = "Slack Notifications";
            Version = "1.0.0.0";
        }

        #region BaseNotificationBehavior implementation
        
        public override SlackNotificationSetting? Settings => _settings as SlackNotificationSetting;

        public override void Init(List<IPlugin> plugins)
        {
            // Code to init a behaviour
            base.Init(plugins);
            _settings?.InitPluginRelatedSettings(plugins);

            SetUpConnection();

            log.Debug($"Notifications: [{NotificationTargetName}] behavior initialized successfully.");
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

            var pluginSetting = _settings.GetPluginSettings(notification.PluginId) as SlackPluginNotificationSetting;
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
            var settings = new SlackNotificationSetting(GetUniqueId(), NotificationTargetName);

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
            // TODO : fix exception here.
            if (_settings is not SlackNotificationSetting slackSettings)
                throw new Exception();
        }

        /// <summary>
        /// Send the simple text notification to the target system.
        /// </summary>
        /// <param name="textNotification">Notification to send</param>
        /// <param name="pluginSetting">Plugin-related notification system</param>
        private void ShowSimpleNotification(TextNotification textNotification, SlackPluginNotificationSetting pluginSetting)
        {
            if (pluginSetting.Token == null)
            {
                log.Warn($"Notifications: [{NotificationTargetName}] has an empty token for [{textNotification.PluginName}] plugin. Received notification will be skipped.");
                return;
            };
            if (pluginSetting.Channel == null)
            {
                log.Warn($"Notifications: [{NotificationTargetName}] has an empty channel name for [{textNotification.PluginName}] plugin. Received notification will be skipped.");
                return;
            };

            var client = new SlackClient(pluginSetting.Token);

            client.Send(new SlackMessage()
            {
                Channel = pluginSetting.Channel,
                Text = textNotification.Text
            });
        }
    }
}