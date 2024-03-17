using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisualHFT.UserSettings;

namespace VisualHFT.Test.Mockups
{
    public class SettingsManagerMockup : ISettingsManager
    {
        public UserSettings.UserSettings? UserSettings { get; set; }

        public event EventHandler<IBaseSettings>? SettingsChanged;

        public SettingsManagerMockup()
        {
            LoadSettings();
        }

        private void LoadSettings()
        {

        }

        public T? GetSetting<T>(SettingKey key, string id) where T : class
        {


            return null;
        }

        public void SetSetting(SettingKey key, string id, IBaseSettings setting)
        {

        }

        public void Save()
        {

        }
    }
}
