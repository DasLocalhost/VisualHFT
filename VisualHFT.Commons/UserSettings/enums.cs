using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisualHFT.UserSettings
{
    public enum SettingKey
    {
        [Description("Application Theme Mode")]
        APPLICATION_THEME_MODE,
        [Description("Application Initialization Size Width")]
        APPLICATION_INITIALIZATION_SIZE_WIDTH,
        [Description("Application Initialization Size Height")]
        APPLICATION_INITIALIZATION_SIZE_HEIGHT,
        [Description("Tile Study")]
        TILE_STUDY,
        [Description("Plugin")]
        PLUGIN,
        [Description("Notification")]
        NOTIFICATION,
        [Description("Test")]
        TEST
        // Add more settings here
    }

}
namespace VisualHFT.PluginManager
{ 
    public enum ePluginStatus
    {
        LOADED,
        STARTED,
        STOPPED,
        MALFUNCTIONING
    }
}
