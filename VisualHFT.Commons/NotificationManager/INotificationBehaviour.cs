using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisualHFT.Commons.API;
using VisualHFT.PluginManager;
using VisualHFT.UserSettings;

namespace VisualHFT.Commons.NotificationManager
{
    /// <summary>
    /// Representing different notification behaviour, such as notifications using Windows Toast, Social networks, Messangers etc.
    /// </summary>
    public interface INotificationBehaviour
    {
        string UniqueId { get; }
        string? NotificationTargetName { get; }

        BaseNotificationSettings? Settings { get; }

        /// <summary>
        /// Init behaviour using default settings.
        /// </summary>
        void Init(List<IPlugin> plugins);

        /// <summary>
        /// Render the notification in the target system.
        /// </summary>
        /// <param name="notification">Data to send as notification.</param>
        void Send(INotification notification);

        /// <summary>
        /// Update existing notification.
        /// </summary>
        void Update(INotification notification);
    }

    /// <summary>
    /// Represents errors that occur when notification type is not supported by selected behaviour.
    /// </summary>
    public class NotSupportedTypeException : Exception 
    { 
        public NotSupportedTypeException(INotificationBehaviour behaviour, INotification notification) 
            : base($"{behaviour.GetType()} doesn't support {notification.GetType()} type of notifications.") { }
    }
}
