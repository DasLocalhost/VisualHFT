using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace VisualHFT.Commons.NotificationManager
{
    // TODO : factory to create different types of notifications?

    /// <summary>
    /// A minimal unit of the Notification System, contains data related to the single notification.
    /// </summary>
    public interface INotification
    {
        /// <summary>
        /// Name of the plugin that is a source of the notification.
        /// </summary>
        public string PluginName { get; set; }

        /// <summary>
        /// Unique Id of the plugin that is a source of the notification.
        /// </summary>
        public string PluginId { get; set; }

        /// <summary>
        /// Value related to the current notification. Could represent any value raising from the Plugin.
        /// </summary>
        public double Value { get; set; }

        /// <summary>
        /// Level of priority for notification.
        /// </summary>
        public NotificationLevel Level { get; set; }

        /// <summary>
        /// Describing the concatenation login in the context of the message.
        /// </summary>
        public Concatenation Concatenation { get; set; }

        #region Wrappers to simplify a constructor

        // TODO : use a Plugin name from the assemply instead of setting it as a string
        /// <summary>
        /// Bind notification to the Plugin.
        /// </summary>
        /// <param name="pluginName">Name of the source of notification</param>
        /// <param name="pluginId">Unique id of the source of notification</param>
        /// <returns>Preinit notification</returns>
        public INotification FromPlugin(string pluginName, string pluginId);
        /// <summary>
        /// Set the level to mark an importance of notification.
        /// </summary>
        /// <param name="level">Level of importance</param>
        /// <returns>Preinit notification</returns>
        public INotification SetLevel(NotificationLevel level);
        /// <summary>
        /// Set the concatenation logic for notification.
        /// </summary>
        /// <param name="concatenation">Concatenation logic</param>
        /// <returns>Preinit notification</returns>
        public INotification SetConcatenation(Concatenation concatenation);

        #endregion
    }

    /// <summary>
    /// Enum describing different levels of notifications.
    /// </summary>
    public enum NotificationLevel
    {
        /// <summary>
        /// Low priority information.
        /// </summary>
        Low,
        /// <summary>
        /// High priority information that should be visibly highlighted.
        /// </summary>
        High,
        /// <summary>
        /// Critical information that should be displayed to user as soon as possible.
        /// </summary>
        Critical
    }

    /// <summary>
    /// Enum describing different approaches to concat notifications.
    /// </summary>
    public enum Concatenation
    {
        /// <summary>
        /// Each Simple type notification is an independed object that shoudln't be concatenated.
        /// </summary>
        Simple,
        /// <summary>
        /// Cumulative notifications are using the same tag to override each other.
        /// </summary>
        Cumulative
    }
}
