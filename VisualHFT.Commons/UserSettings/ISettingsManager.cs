using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisualHFT.UserSettings
{
    public interface ISettingsManager
    {
        UserSettings? UserSettings { get; set; }

        event EventHandler<IBaseSettings>? SettingsChanged;

        T? GetSetting<T>(SettingKey key, string id) where T : class;

        public void SetSetting(SettingKey key, string id, IBaseSettings setting);

        public void Save();
    }
}