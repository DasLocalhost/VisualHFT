using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace VisualHFT.Commons.NotificationManager.Notifications
{
    /// <summary>
    /// Simplest notification with just a title and text message.
    /// </summary>
    public class TextNotification : INotification
    {
        #region INotification implementation

        public string PluginName { get; set; }
        public NotificationLevel Level { get; set; }
        public Concatenation Concatenation { get; set; }
        public string PluginId { get; set; }

        #endregion

        /// <summary>
        /// Short summary of the notification.
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// Detailed information.
        /// </summary>
        public string Text { get; set; }

        public TextNotification(string title,
                                string text,
                                string pluginName = "",
                                string pluginId = "",
                                NotificationLevel level = NotificationLevel.Low,
                                Concatenation concatenation = Concatenation.Simple)
        {
            Title = title;
            Text = text;
            PluginName = pluginName;
            PluginId = pluginId;
            Level = level;
            Concatenation = concatenation;
        }

        public INotification FromPlugin(string pluginName, string pluginId)
        {
            PluginName = pluginName;
            PluginId = pluginId;
            return this;
        }

        public INotification SetLevel(NotificationLevel level)
        {
            Level = level;
            return this;
        }

        public INotification SetConcatenation(Concatenation concatenation)
        {
            Concatenation = concatenation;
            return this;
        }
    }
}
