using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisualHFT.Commons.NotificationManager;
using VisualHFT.Notifications.Slack;
using VisualHFT.UserSettings;

namespace VisualHFT.Test.Mockups
{
    public class NotificationBehaviorMockup : INotificationBehaviour
    {
        public string UniqueId => throw new NotImplementedException();

        public string? TargetName { get; set; } = "Mockup";

        public BaseNotificationSettings? Settings => throw new NotImplementedException();

        public void Initialize()
        {
            throw new NotImplementedException();
        }

        public void Send(INotification notification)
        {
            throw new NotImplementedException();
        }

        public void Update(INotification notification)
        {
            throw new NotImplementedException();
        }
    }

    public class NotificationSettingsMockup : BaseNotificationSettings
    {
        private const string _defaultHeader = "Test";

        public NotificationSettingsMockup(string behaviourId, string? targetName) : base(_defaultHeader, targetName, behaviourId) { }

        protected override IPluginNotificationSettings GetDefaultPluginBasedSettings(string pluginId)
        {
            return new NotificationPluginSettingsMockup(this)
            {
                PluginId = pluginId
            };
        }
    }

    public class NotificationPluginSettingsMockup : IPluginNotificationSettings
    {
        public bool IsEnabled => true;

        public string? PluginId { get; set; }
        public BaseNotificationSettings ParentSettings { get; set; }
        public string? SettingId { get; set; }
        public SettingKey? SettingKey { get; set; }

        public NotificationPluginSettingsMockup(NotificationSettingsMockup parentSettings)
        {
            ParentSettings = parentSettings;
        }

        public void UpdateInSource()
        {
            throw new NotImplementedException();
        }
    }
}
