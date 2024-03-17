using VisualHFT.Notifications.Twitter;

namespace VisualHFT.ViewModel.Settings
{
    public class TwitterSettingsViewModel : BaseNotificationSettingsViewModel
    {
        private const string _defaultHeader = "Twitter Notifications Settings";

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

        public TwitterSettingsViewModel(TwitterNotificationSetting setting) : base(setting, _defaultHeader)
        {

        }

        public override bool CheckIfValid()
        {
            return string.IsNullOrEmpty(this[nameof(Threshold)])
                && string.IsNullOrEmpty(this[nameof(UpdateTime)]);
        }

        protected override bool CanExecuteOkCommand(object obj)
        {
            return string.IsNullOrEmpty(this[nameof(Threshold)])
                && string.IsNullOrEmpty(this[nameof(UpdateTime)]);
        }

        public override void ApplyChanges()
        {
            // TODO : logs / Exceptions here
            if (!(_setting is TwitterNotificationSetting castedSetting))
                return;

            if (SettingKey == null || SettingId == null)
                return;

            castedSetting.Threshold = Threshold;
            castedSetting.UpdateTime = UpdateTime;

            // TODO : event here
            // SettingsManager.Instance.UserSettings?.RaiseSettingsChanged(castedSetting);
        }
    }
}