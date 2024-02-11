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
    public class ToastSettingsViewModel : BaseNotificationSettingsViewModel
    {
        private const string _defaultHeader = "Toast Notifications Settings";

        #region IDataErrorInfo implementation

        public override string Error => null;

        public override string this[string columnName]
        {
            get
            {
                switch (columnName)
                {
                    case nameof(Threshold):

                        if (Threshold <= 0 || Threshold > 10)
                            return "Threshold should be between 0 and 10.";

                        break;
                    case nameof(UpdateTime):

                        if (UpdateTime < 50 || UpdateTime > 1000)
                            return "Update time (ms) should be between 50 and 100.";

                        break;
                    default:

                        return null;
                }

                return null;
            }
        }

        #endregion

        public ToastSettingsViewModel(ToastNotificationSetting setting) : base(setting, _defaultHeader)
        {

        }

        public override bool CheckIfValid()
        {
            return string.IsNullOrEmpty(this[nameof(Threshold)])
                && string.IsNullOrEmpty(this[nameof(UpdateTime)]);
        }

        public override void ApplyChanges()
        {
            // TODO : logs / Exceptions here
            if (_setting is not ToastNotificationSetting castedSetting)
                return;

            if (SettingKey == null || SettingId == null)
                return;

            castedSetting.Threshold = Threshold;
            castedSetting.UpdateTime = UpdateTime;

            SettingsManager.Instance.UserSettings?.RaiseSettingsChanged(castedSetting);
        }
    }
}