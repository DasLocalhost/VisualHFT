using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisualHFT.UserSettings
{
    /// <summary>
    /// Base interface for all types of settings
    /// </summary>
    public interface IBaseSettings 
    {
        ///// <summary>
        ///// Event raised on settings changed to inform services.
        ///// </summary>
        //event EventHandler? OnSettingsChanged;
    }

    // TODO : check if this attribute still needed
    /// <summary>
    /// Attribute to map setting with the related service.
    /// </summary>
    public class SettingsAttribute : Attribute
    {
        private readonly Type _settingsType;
        public Type SettingsType => _settingsType;

        public SettingsAttribute(Type settingsType)
        {
            this._settingsType = settingsType;
        }
    }
}
