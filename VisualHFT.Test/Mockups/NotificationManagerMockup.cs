using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisualHFT.Commons.NotificationManager;
using VisualHFT.Commons.PluginManager;
using VisualHFT.Notifications;
using VisualHFT.PluginManager;
using VisualHFT.UserSettings;
using VisualHFT.ViewModel;

namespace VisualHFT.Test.Mockups
{
    /// <summary>
    /// INotificationManager implementation for tests
    /// </summary>
    public class NotificationManagerMockup : INotificationManager
    {
        #region Fields

        private readonly IPluginManager _pluginManager;
        private readonly ISettingsManager _settingsManager;

        private List<NotificationBalancer>? _balancers;
        private IEnumerable<INotificationBehaviour> _behaviorsList;
        private Dictionary<string, INotificationBehaviour>? _behaviours;

        #endregion

        public Dictionary<string, INotificationBehaviour>? ActiveBehaviours => throw new NotImplementedException();

        public NotificationManagerMockup(IEnumerable<INotificationBehaviour> behaviors, IPluginManager pluginManager, ISettingsManager settingsManager)
        {
            _pluginManager = pluginManager;
            _settingsManager = settingsManager;
            _behaviorsList = behaviors;
        }

        public void Initialize()
        {

        }
    }

    /// <summary>
    /// INotificationBehaviour implementation for tests
    /// </summary>
    public class NotificationBehaviourMockup : INotificationBehaviour
    {
        public string UniqueId => "NotificationBehaviourMockup";

        public string? NotificationTargetName => "NotificationTargetName";

        public BaseNotificationSettings? Settings => throw new NotImplementedException();

        public void Initialize(List<IPlugin> plugins)
        {
            throw new NotImplementedException();
        }

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
}
