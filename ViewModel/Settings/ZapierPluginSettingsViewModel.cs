using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisualHFT.Commons.WPF.ViewModel;
using VisualHFT.NotificationManager.Slack;
using VisualHFT.NotificationManager.Toast;
using VisualHFT.NotificationManager.Zapier;
using VisualHFT.UserSettings;
using static log4net.Appender.RollingFileAppender;

namespace VisualHFT.ViewModel.Settings
{
    public class ZapierPluginSettingsViewModel : BaseSettingsViewModel
    {
        private const string _defaultHeader = "Zapier Notifications";

        #region Fields

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private string? _webHookUrl;
        private bool _isEnabled;

        #endregion

        #region IDataErrorInfo implementation

        public override string Error => null;

        public override string this[string columnName]
        {
            get
            {
                //switch (columnName)
                //{
                //    case nameof(Threshold):

                //        if (Threshold <= 0 || Threshold > 10)
                //            return "Threshold should be between 1 and 10.";

                //        break;
                //    case nameof(UpdateTime):

                //        if (UpdateTime < 50 || UpdateTime > 1000)
                //            return "Update time (ms) should be between 50 and 100.";

                //        break;
                //    default:

                //        return null;
                //}

                return null;
            }
        }

        #endregion

        public string? WebHookUrl
        {
            get => _webHookUrl;
            set
            {
                if (_webHookUrl != value)
                {
                    _webHookUrl = value;
                    RaisePropertyChanged();
                }
            }
        }

        public string WebHookPrefix { get; private set; }

        public bool IsEnabled
        {
            get => _isEnabled;
            set
            {
                if (_isEnabled != value)
                {
                    _isEnabled = value;
                    RaisePropertyChanged();
                }
            }
        }

        public ZapierPluginSettingsViewModel(IPluginNotificationSettings setting) : base(setting, _defaultHeader)
        {
            // TODO : custom exception here
            if (setting is not ZapierPluginNotificationSetting pluginSettings)
                throw new Exception();

            WebHookUrl = pluginSettings.WebHookUrl;
            WebHookPrefix = pluginSettings.WebHookPrefix;

            IsEnabled = pluginSettings.IsEnabled;
        }

        public override bool CheckIfValid()
        {
            return true;
        }

        protected override bool CanExecuteOkCommand(object obj)
        {
            return true;
        }

        public override void ApplyChanges()
        {
            if (_setting is not ZapierPluginNotificationSetting castedSetting)
            {
                log.Error("Notifications: Can't cast Plugin-related settings to Zapier settings.");
                return;
            }

            if (SettingKey == null || SettingId == null)
            {
                log.Error($"Notifications: Can't save Zapier Plugin-related settings. Setting Key - [{SettingKey}]. Setting Id - [{SettingId}]");
                return;
            }

            castedSetting.WebHookUrl = WebHookUrl;
            castedSetting.IsEnabled = IsEnabled;

            castedSetting.UpdateInSource();
        }
    }
}
