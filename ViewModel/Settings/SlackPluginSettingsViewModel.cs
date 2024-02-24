using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisualHFT.Commons.WPF.ViewModel;
using VisualHFT.NotificationManager.Slack;
using VisualHFT.UserSettings;
using static log4net.Appender.RollingFileAppender;

namespace VisualHFT.ViewModel.Settings
{
    public class SlackPluginSettingsViewModel : BaseSettingsViewModel
    {
        private const string _defaultHeader = "Slack Notifications";

        #region Fields

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private bool _isEnabled;
        private string? _channel;
        private string? _token;

        #endregion

        #region IDataErrorInfo implementation

        public override string Error => null;

        public override string this[string columnName]
        {
            get
            {
                switch (columnName)
                {
                    case nameof(Channel):

                        if (string.IsNullOrEmpty(Channel))
                            return "Channel should not be empty.";

                        break;
                    case nameof(Token):

                        if (string.IsNullOrEmpty(Token))
                            return "Token should not be empty.";

                        break;
                    default:

                        return null;
                }

                return null;
            }
        }

        #endregion

        #region Properties

        public string? Channel 
        { 
            get => _channel; 
            set
            {
                if (_channel != value)
                {
                    _channel = value;
                    RaisePropertyChanged();
                }
            }
        }

        public string? Token
        {
            get => _token; 
            set
            {
                if (_token != value)
                {
                    _token = value;
                    RaisePropertyChanged();
                }
            }
        }

        #endregion

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

        public SlackPluginSettingsViewModel(IPluginNotificationSettings setting) : base(setting, _defaultHeader)
        {
            // TODO : custom exception here
            if (!(setting is SlackPluginNotificationSetting pluginSettings))
                throw new Exception();

            Token = pluginSettings.Token;
            Channel = pluginSettings.Channel;
            
            IsEnabled = pluginSettings.IsEnabled;
        }

        public override bool CheckIfValid()
        {
            return true;
        }

        public override void ApplyChanges()
        {
            if (_setting is not SlackPluginNotificationSetting castedSetting)
            {
                log.Error("Notifications: Can't cast Plugin-related settings to Slack settings.");
                return;
            }

            if (SettingKey == null || SettingId == null)
            {
                log.Error($"Notifications: Can't save Slack Plugin-related settings. Setting Key - [{SettingKey}]. Setting Id - [{SettingId}]");
                return;
            }

            castedSetting.Token = Token;
            castedSetting.Channel = Channel;

            castedSetting.IsEnabled = IsEnabled;

            castedSetting.UpdateInSource();
        }
    }
}