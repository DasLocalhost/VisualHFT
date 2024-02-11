using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisualHFT.UserSettings;

namespace VisualHFT.UserSettings
{
    public interface INotificationSettings : IBaseSettings
    {
        string? SettingsHeader { get; }
        string BehaviourId { get; }

        public Dictionary<string, IPluginNotificationSettings>? PluginSettings { get; set; }
    }

    public interface IPluginNotificationSettings : IBaseSettings
    {
        bool IsEnabled { get; }
        string? PluginId { get; set; }

        public BaseNotificationSettings ParentSettings { get; set; }

        public void UpdateInSource();
    }
}