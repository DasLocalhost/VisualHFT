using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisualHFT.Commons.WPF.ViewModel;
using VisualHFT.NotificationManager.Toast;
using VisualHFT.UserSettings;

namespace VisualHFT.ViewModel.Settings
{
    /// <summary>
    /// Base viewmodel for notifications settings. Contins technical fields such as threshold / update time
    /// </summary>
    public abstract class BaseNotificationSettingsViewModel : BaseSettingsViewModel
    {
        #region Fields

        protected int _threshold;
        protected int _updateTime;

        #endregion

        public int Threshold
        {
            get => _threshold;
            set
            {
                if (_threshold != value)
                {
                    _threshold = value;
                    RaisePropertyChanged();
                }
            }
        }

        public int UpdateTime
        {
            get => _updateTime;
            set
            {
                if (_updateTime != value)
                {
                    _updateTime = value;
                    RaisePropertyChanged();
                }
            }
        }

        public BaseNotificationSettingsViewModel(BaseNotificationSettings setting, string header) : base(setting, header)
        {
            _threshold = setting.Threshold;
            _updateTime = setting.UpdateTime;
        }
    }
}
