using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisualHFT.PluginManager;
using VisualHFT.ViewModel;

namespace VisualHFT.UserSettings
{
    public static class SettingsManagerExtension
    {
        public static void ShowMainSettings(this SettingsManager settingsManager)
        {
            var vm = new vmUserSettings();
            var form = new View.UserSettings();

            form.DataContext = vm;
            vm.OnClose += (_, __) => form.Close();
            form.ShowDialog();
        }
    }
}