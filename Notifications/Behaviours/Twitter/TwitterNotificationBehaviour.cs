using System;
using VisualHFT.Commons.API.Twitter;
using VisualHFT.Commons.NotificationManager;
using VisualHFT.Commons.NotificationManager.Notifications;
using VisualHFT.Commons.PluginManager;
using VisualHFT.UserSettings;

namespace VisualHFT.Notifications.Twitter
{
    /// <summary>
    /// Notifications logic for Twitter notifications.
    /// </summary>
    [Settings(typeof(TwitterNotificationSetting))]
    public class TwitterNotificationBehaviour : BaseNotificationBehaviour
    {
        #region Fields

        private TwitterClient? _client;
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType);

        #endregion

        #region BaseNotificationBehavior implementation

        public override TwitterNotificationSetting? Settings => _settings as TwitterNotificationSetting;

        public override void Initialize()
        {
            // Code to init a behaviour
            base.Initialize();

            var plugins = _pluginManager.AllPlugins;
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

            var pluginSetting = _settings.GetPluginSettings(notification.PluginId) as TwitterPluginNotificationSetting;
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
            var settings = new TwitterNotificationSetting(GetUniqueId(), NotificationTargetName);

            SaveToUserSettings(settings);

            return settings;
        }

        #endregion

        public TwitterNotificationBehaviour(ISettingsManager settingsManager, IPluginManager pluginManager) : base(settingsManager, pluginManager)
        {
            NotificationTargetName = "Twitter Notifications";
            Version = "1.0.0.0";
        }

        /// <summary>
        /// Set up the initial connection to the target system. Any kinds of token-checking methods should be placed here.
        /// </summary>
        /// <exception cref="Exception"></exception>
        private void SetUpConnection()
        {

        }

        /// <summary>
        /// Send the simple text notification to the target system.
        /// </summary>
        /// <param name="textNotification">Notification to send</param>
        /// <param name="pluginSetting">Plugin-related notification system</param>
        private void ShowSimpleNotification(TextNotification textNotification, TwitterPluginNotificationSetting pluginSetting)
        {
            if (pluginSetting.AccessToken == null)
            {
                log.Warn($"Notifications: [{NotificationTargetName}] has an empty access token for [{textNotification.PluginName}] plugin. Received notification will be skipped.");
                return;
            };
            if (pluginSetting.AccessSecret == null)
            {
                log.Warn($"Notifications: [{NotificationTargetName}] has an empty access secret name for [{textNotification.PluginName}] plugin. Received notification will be skipped.");
                return;
            };
            if (pluginSetting.ApiToken == null)
            {
                log.Warn($"Notifications: [{NotificationTargetName}] has an empty api token name for [{textNotification.PluginName}] plugin. Received notification will be skipped.");
                return;
            };
            if (pluginSetting.ApiSecret == null)
            {
                log.Warn($"Notifications: [{NotificationTargetName}] has an empty api secret name for [{textNotification.PluginName}] plugin. Received notification will be skipped.");
                return;
            };

            var client = new TwitterClient(pluginSetting.ApiToken, pluginSetting.ApiSecret, pluginSetting.AccessToken, pluginSetting.AccessSecret);

            client.Send(new TwitterMessage() { Text = textNotification.Text });
        }
    }
}
