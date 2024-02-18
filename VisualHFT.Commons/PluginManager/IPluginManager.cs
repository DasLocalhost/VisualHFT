using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisualHFT.PluginManager;

namespace VisualHFT.Commons.PluginManager
{
    public interface IPluginManager
    {
        List<IPlugin> AllPlugins { get; }
        bool AllPluginsReloaded { get; }

        void Initialize();

        void StartPlugin(IPlugin plugin);
        void StopPlugin(IPlugin plugin);
        void SettingPlugin(IPlugin plugin);
        void UnloadPlugins();
    }
}