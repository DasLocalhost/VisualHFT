using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisualHFT.UserSettings;
using VisualHFT.ViewModel;

namespace VisualHFT.Helpers
{
    public static class UIHelper
    {
        public static void ShowMainSettings(ISettingsManager settingsManager)
        {
            var vm = new vmUserSettings(settingsManager);
            var form = new View.UserSettings();

            form.DataContext = vm;
            vm.OnClose += (_, __) => form.Close();
            form.ShowDialog();
        }
    }
}
