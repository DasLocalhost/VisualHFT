using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using VisualHFT.Commons.NotificationManager;
using VisualHFT.Commons.NotificationManager.Notifications;
using VisualHFT.NotificationManager.Slack;
using VisualHFT.NotificationManager.Twitter;
using VisualHFT.PluginManager;
using VisualHFT.UserSettings;

namespace VisualHFT.NotificationManager.Toast
{
    /// <summary>
    /// Notifications logic for Windows toast notifications.
    /// </summary>
    [Settings(typeof(ToastNotificationSetting))]
    public class ToastNotificationBehaviour : BaseNotificationBehaviour
    {
        #region Fields

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType);

        #endregion

        public ToastNotificationBehaviour(ISettingsManager settingsManager) : base(settingsManager)
        {
            NotificationTargetName = "Windows 10 Toast Notifications";
            Version = "1.0.0.0";
        }

        #region BaseNotificationBehaviour implementation

        public override ToastNotificationSetting? Settings => _settings as ToastNotificationSetting;

        public override void Init(List<IPlugin> plugins)
        {
            base.Init(plugins);
            _settings?.InitPluginRelatedSettings(plugins);
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

            var pluginSetting = _settings.GetPluginSettings(notification.PluginId) as ToastPluginNotificationSetting;
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
            var settings = new ToastNotificationSetting(GetUniqueId(), NotificationTargetName)
            {
                Threshold = 5,
                UpdateTime = 100
            };

            SaveToUserSettings(settings);

            return settings;
        }

        #endregion

        private void ShowSimpleNotification(TextNotification notification, ToastPluginNotificationSetting pluginSetting)
        {
            // Check this setting if we want to prevent app from opening on toast click
            // ToastNotificationManagerCompat.OnActivated += ToastNotificationManagerCompat_OnActivated;
            var useTime = pluginSetting.IncludeTimeStamp;

            var builder = new ToastContentBuilder()
                .AddText(notification.Title)
                .AddText(notification.Text);

            if (useTime)
                builder.AddText($"Time: {DateTime.Now}");

            // TODO : test if this directive works correctly on previous windows versions
#if WINDOWS10_0_17763_0_OR_GREATER
            builder.Show();
#else
            log.Warn($"Notifications: [{NotificationTargetName}] is not available in current OS.");
#endif
        }
    }
}