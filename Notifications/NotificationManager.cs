using System;
using System.Collections.Generic;
using System.Linq;
using VisualHFT.Commons.NotificationManager;
using VisualHFT.Commons.PluginManager;
using VisualHFT.Notifications.Slack;
using VisualHFT.Notifications.Toast;
using VisualHFT.Notifications.Twitter;
using VisualHFT.Notifications.Zapier;
using VisualHFT.UserSettings;

namespace VisualHFT.Notifications
{
    /// <summary>
    /// Manager to route notifications from plugins to different behaviours.
    /// </summary>
    public class NotificationManager : INotificationManager
    {
        private readonly IPluginManager _pluginManager;
        private readonly ISettingsManager _settingsManager;
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #region Fields

        private List<NotificationBalancer>? _balancers;
        private IEnumerable<INotificationBehaviour> _behaviorsList;
        private Dictionary<string, INotificationBehaviour>? _behaviours;
        private List<PluginRouting>? _routingMap;

        #endregion

        #region Properties

        /// <summary>
        /// Notifications behaviour that are active to be used in app.
        /// </summary>
        public Dictionary<string, INotificationBehaviour>? ActiveBehaviours => _behaviours;

        #endregion

        public NotificationManager(IEnumerable<INotificationBehaviour> behaviors, IPluginManager pluginManager, ISettingsManager settingsManager)
        {
            _pluginManager = pluginManager;
            _settingsManager = settingsManager;
            _behaviorsList = behaviors;
        }

        /// <summary>
        /// Try to init the notifications subsystem.
        /// Inner initialization order - Init Behaviours -> Init processors (balancers) -> Start processors -> Map routing -> Start listening
        /// </summary>
        /// <param name="plugins"></param>
        public void Initialize()
        {
            try
            {
                log.Info("Notifications: Start initialization.");

                log.Debug("Notifications: Init behaviors.");
                InitBehaviours();
                log.Debug("Notifications: Init balancers.");
                InitBalancers();

                log.Debug("Notifications: Start processing.");
                StartProcessing();

                // Make routing using plugin settings
                log.Debug("Notifications: Map routing.");
                MapRouting(_pluginManager.AllPlugins);

                // Subscribe to notification raised event only after init is done
                foreach (var plugin in _pluginManager.AllPlugins)
                {
                    if (plugin is INotificationSource source)
                    {
                        // TODO : make a global switch 'Notifications enabled / disabled'
                        source.OnNotificationRaised += OnNotificationRaised;
                        log.Info($"Notifications: subscription for [{plugin.Name}] plugin is done.");
                    }
                }

                log.Info("Notifications: Initialization completed.");
            }
            catch (Exception ex)
            {
                log.Error("Notifications: Initialization failed.", ex);
            }
        }

        #region Init methods

        /// <summary>
        /// Wrap the initiation of every available behaviour.
        /// </summary>
        private void InitBehaviours()
        {
            if (_pluginManager == null)
                throw new Exception("Can't initialized behaviors - PluginManager instance is null. Check the initialization order.");

            _behaviours = _behaviorsList.ToDictionary(_ => _.UniqueId);
        }

        /// <summary>
        /// Init notification processors for each behaviour.
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        private void InitBalancers()
        {
            if (_behaviours == null || !_behaviours.Any())
                throw new Exception("Can't initialized balancers - behaviors list is null or empty. Check the initialization order.");

            _balancers = new List<NotificationBalancer>();

            foreach (var behaviour in _behaviours.Values)
            {
                if (behaviour == null || behaviour.Settings == null)
                {
                    log.Warn($"Notifications: can't make a balancer for a null-value behavior.");
                    continue;
                }

                var balancer = new NotificationBalancer(behaviour.Settings, _settingsManager);
                balancer.OnDequeue += Route;

                _balancers.Add(balancer);
            }
        }

        /// <summary>
        /// Start processing of notifications from the queue. Important - processing always should be running in STA thread. 
        /// MTA thread will cause an unhandled 'Attempted to read or write protected memory' exception with Windows toast notifications.
        /// </summary>
        private void StartProcessing()
        {
            if (_balancers == null || !_balancers.Any())
                throw new Exception("Can't start balancers - balancers list is null or empty. Check the initialization order.");

            _balancers.ForEach(_ => _.Start());
        }

        // TODO : implement logic to update routing if settings changed
        /// <summary>
        /// Prepare a mapping for fast routing notifications from plugins to enabled notifications behaviours.
        /// </summary>
        /// <param name="plugins"></param>
        private void MapRouting(IList<PluginManager.IPlugin> plugins)
        {
            if (_behaviours == null || !_behaviours.Any())
                throw new Exception("Can't prepare routing - behaviours list is null or empty. Check the initialization order.");

            _routingMap = new List<PluginRouting>();

            foreach (var plugin in plugins)
            {
                var pluginId = plugin.GetPluginUniqueID();
                var pluginSettings = new Dictionary<string, IPluginNotificationSettings>();

                // Scan all behaviours and get plugin-related settings related to current plugin
                foreach (var behav in _behaviours.Values)
                {
                    if (behav.Settings is null)
                    {
                        log.Warn($"Notifications: can't make a routing for [{behav.TargetName}] behavior - behavior settings are null.");
                        continue;
                    }

                    var pluginSetting = behav.Settings.GetPluginSettings(pluginId);
                    if (pluginSetting is null)
                        continue;

                    pluginSettings.Add(behav.UniqueId, pluginSetting);
                }

                if (pluginSettings.Count == 0)
                {
                    log.Warn($"Notifications: can't make a routing for [{plugin.Name}] plugin - no routes found.");
                    continue;
                }

                _routingMap.Add(new PluginRouting(pluginId) { Settings = pluginSettings });
            }
        }

        #endregion

        /// <summary>
        /// Route the notification from the source to processor.
        /// </summary>
        /// <param name="source">Notification source</param>
        /// <param name="notification">Notification</param>
        private void OnNotificationRaised(object? source, INotification notification)
        {
            try
            {
                // In the case of extending - use another interface instead of IPlugin. Unique Id feature should be implemented for this interface.
                if (source is not PluginManager.IPlugin plugin)
                    return;

                // In case of (for some reason) receiving notifications by not initialized Notification Manager
                if (_balancers == null || _routingMap == null)
                    return;

                log.Info($"Notifications: New notification raised from [{notification.PluginName}].");

                var pluginId = plugin.GetPluginUniqueID();
                var routing = _routingMap.FirstOrDefault(_ => _.PluginId == pluginId);

                // Skip if no routing for the notification source found
                if (routing == null)
                {
                    log.Warn($"Notifications: No routes found for [{notification.PluginName}]. Notification will be skipped.");
                    return;
                }

                var enabledNotifications = routing.GetRouting();
                foreach (var processor in _balancers.Where(_ => enabledNotifications.Contains(_.BehaviourId)))
                {
                    processor.Enqueue(notification);
                }
            }
            catch (Exception ex)
            {
                log.Error("Notifications: Failed to process raised notification.", ex);
            }
        }


        /// <summary>
        /// Route the notification from Balancer to the Notification Behaviour.
        /// </summary>
        /// <param name="sender">Notification balancer</param>
        /// <param name="notifications">List of notifications ready to be sent</param>
        private void Route(object? sender, IList<INotification> notifications)
        {
            try
            {
                if (sender is not NotificationBalancer balancer)
                    return;

                var behaviour = _behaviours?[balancer.BehaviourId];

                if (behaviour == null)
                    throw new Exception($"Failed to route notifications: unknown behavior found [{balancer.BehaviourId}]");

                log.Debug($"Notifications: New notifications chunk ({notifications.Count}) routed for [{behaviour.TargetName}] target.");

                foreach (var notification in notifications)
                    behaviour?.Send(notification);
            }
            catch (Exception ex)
            {
                log.Error("Notifications: Failed to process raised notification.", ex);
            }
        }

        #region Inner routing class

        /// <summary>
        /// Class representing routing between Plugins and Notification Behaviours
        /// </summary>
        private class PluginRouting
        {
            #region Fields

            private readonly string _pluginId;
            private Dictionary<string, IPluginNotificationSettings> _settings = new Dictionary<string, IPluginNotificationSettings>();

            #endregion

            /// <summary>
            /// Notification-Plugin related settings
            /// </summary>
            public Dictionary<string, IPluginNotificationSettings> Settings
            {
                get => _settings;
                set
                {
                    if (_settings != value)
                    {
                        _settings = value;
                    }
                }
            }

            /// <summary>
            /// Id of the plugin related to this routing.
            /// </summary>
            public string PluginId => _pluginId;

            public PluginRouting(string pluginId)
            {
                _pluginId = pluginId;
            }

            /// <summary>
            /// Returns a list of notifications sources ids enabled for current Plugin.
            /// </summary>
            /// <returns>List of ids</returns>
            public IEnumerable<string> GetRouting()
            {
                return _settings.Where(_ => _.Value.IsEnabled).Select(_ => _.Key);
            }

            public override string ToString()
            {
                return $"{_pluginId.Substring(0, 5)} - Enable Routes: {GetRouting().Count()}";
            }
        }

        #endregion
    }

    /// <summary>
    /// Represents errors that occur when a target system for notification is not init in the app.
    /// </summary>
    public class UnknownBehaviourException : Exception
    {
        public UnknownBehaviourException(string pluginName, string targetSystem)
            : base($"Unknown target system {targetSystem} in notification sent by {pluginName} Plugin.") { }
    }

    /// <summary>
    /// Represents errors that occur when a target system for notification is not init in the app.
    /// </summary>
    public class NotSupportedConcatTypeException : Exception
    {
        public NotSupportedConcatTypeException(Concatenation concatenation)
            : base($"Concatenation type {concatenation} is not supported.") { }
    }
}
