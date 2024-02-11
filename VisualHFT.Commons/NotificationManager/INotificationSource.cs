using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace VisualHFT.Commons.NotificationManager
{
    /// <summary>
    /// Object that can send notifications to the Notification System.
    /// </summary>
    public interface INotificationSource
    {
        /// <summary>
        /// Raise data to the Notification Manager.
        /// </summary>
        event EventHandler<INotification> OnNotificationRaised;
    }
}