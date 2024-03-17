using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisualHFT.Commons.PluginManager;
using VisualHFT.PluginManager;
using VisualHFT.UserSettings;

namespace VisualHFT.Test.Mockups
{
    public class PluginManagerMockup : IPluginManager
    {
        #region Fields

        private readonly ISettingsManager _settingsManager;

        #endregion

        public List<IPlugin> AllPlugins => throw new NotImplementedException();

        public bool AllPluginsReloaded => throw new NotImplementedException();

        public PluginManagerMockup(ISettingsManager settingsManager)
        {
            _settingsManager = settingsManager;
        }

        public void Initialize()
        {

        }

        public void SettingPlugin(IPlugin plugin)
        {

        }

        public void StartPlugin(IPlugin plugin)
        {

        }

        public void StopPlugin(IPlugin plugin)
        {

        }

        public void UnloadPlugins()
        {

        }
    }
}