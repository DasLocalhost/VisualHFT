using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using VisualHFT.Commons.API.Twitter;
using VisualHFT.Commons.Studies;
using VisualHFT.Commons.WPF.ViewModel;
using VisualHFT.Helpers;
using VisualHFT.NotificationManager.Slack;
using VisualHFT.NotificationManager.Toast;
using VisualHFT.NotificationManager.Twitter;
using VisualHFT.UserSettings;
using static log4net.Appender.RollingFileAppender;

namespace VisualHFT.ViewModel.Settings
{
    public class TwitterPluginSettingsViewModel : BaseSettingsViewModel
    {
        private const string _defaultHeader = "Twitter Notifications";

        #region Fields

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private bool _isEnabled;

        private string? _apiToken;
        private string? _apiSecret;
        private string? _oAuthToken;
        private string? _oAuthSecret;
        private string? _authVerifier;
        private string? _accessToken;
        private string? _accessSecret;
        private bool _isBusy = false;

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

        #region Commands

        public ICommand Auth1Command { get; private set; }
        public ICommand Auth2Command { get; private set; }

        #endregion

        #region Properties

        public string? ApiToken
        {
            get => _apiToken;
            set
            {
                if (_apiToken != value)
                {
                    _apiToken = value;
                    (Auth1Command as RelayCommand<object>)?.RaiseCanExecuteChanged();
                    RaisePropertyChanged();
                }
            }
        }
        public string? ApiSecret
        {
            get => _apiSecret;
            set
            {
                if (_apiSecret != value)
                {
                    _apiSecret = value;
                    (Auth1Command as RelayCommand<object>)?.RaiseCanExecuteChanged();
                    RaisePropertyChanged();
                }
            }
        }
        public string? OAuthToken
        {
            get => _oAuthToken;
            set
            {
                if (_oAuthToken != value)
                {
                    _oAuthToken = value;
                    (Auth2Command as RelayCommand<object>)?.RaiseCanExecuteChanged();
                    RaisePropertyChanged();
                }
            }
        }
        public string? OAuthSecret
        {
            get => _oAuthSecret;
            set
            {
                if (_oAuthSecret != value)
                {
                    _oAuthSecret = value;
                    (Auth2Command as RelayCommand<object>)?.RaiseCanExecuteChanged();
                    RaisePropertyChanged();
                }
            }
        }
        public string? AuthVerifier
        {
            get => _authVerifier;
            set
            {
                if (_authVerifier != value)
                {
                    _authVerifier = value;
                    (Auth2Command as RelayCommand<object>)?.RaiseCanExecuteChanged();
                    RaisePropertyChanged();
                }
            }
        }
        public string? AccessToken
        {
            get => _accessToken;
            set
            {
                if (_accessToken != value)
                {
                    _accessToken = value;
                    RaisePropertyChanged();
                }
            }
        }
        public string? AccessSecret
        {
            get => _accessSecret;
            set
            {
                if (_accessSecret != value)
                {
                    _accessSecret = value;
                    RaisePropertyChanged();
                }
            }
        }

        #endregion

        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                if (_isBusy != value)
                {
                    _isBusy = value;
                    (Auth1Command as RelayCommand<object>)?.RaiseCanExecuteChanged();
                    (Auth2Command as RelayCommand<object>)?.RaiseCanExecuteChanged();
                    RaisePropertyChanged();
                }
            }
        }

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

        public TwitterPluginSettingsViewModel(IPluginNotificationSettings setting) : base(setting, _defaultHeader)
        {
            // TODO : custom exception here
            if (setting is not TwitterPluginNotificationSetting pluginSettings)
                throw new Exception();

            ApiToken = pluginSettings.ApiToken;
            ApiSecret = pluginSettings.ApiSecret;

            AccessToken = pluginSettings.AccessToken;
            AccessSecret = pluginSettings.AccessSecret;

            Auth1Command = new RelayCommand<object>(AuthStep1, CanExecuteAuthStep1);
            Auth2Command = new RelayCommand<object>(AuthStep2, CanExecuteAuthStep2);

            //TestPluginProperty = pluginSettings.TestPluginProperty;
            IsEnabled = pluginSettings.IsEnabled;
        }

        private bool CanExecuteAuthStep1(object obj)
        {
            if (IsBusy)
                return false;

            return !string.IsNullOrEmpty(ApiToken) && !string.IsNullOrEmpty(ApiSecret);
        }

        private bool CanExecuteAuthStep2(object obj)
        {
            if (IsBusy)
                return false;

            return !string.IsNullOrEmpty(OAuthToken) && !string.IsNullOrEmpty(OAuthSecret) && !string.IsNullOrEmpty(AuthVerifier);
        }

        private async void AuthStep1(object obj)
        {
            if (string.IsNullOrEmpty(ApiToken) || string.IsNullOrEmpty(ApiSecret))
                return;

            IsBusy = true;

            string? oAuthToken = null;
            string? oAuthSecret = null;

            var oauthResponse = await Task.Run(() => TwitterClient.GetOAuthToken(ApiToken, ApiSecret));

            if (oauthResponse == null || oauthResponse?.token == null || oauthResponse?.secret == null)
                return;

            OAuthToken = oauthResponse?.token;
            OAuthSecret = oauthResponse?.secret;

            await Task.Run(() => TwitterClient.OpenAuthUrl(OAuthToken, OAuthSecret));

            IsBusy = false;
        }

        private async void AuthStep2(object obj)
        {
            if (string.IsNullOrEmpty(ApiToken) || string.IsNullOrEmpty(AuthVerifier))
                return;

            IsBusy = true;

            var accessResponse = await Task.Run(() => TwitterClient.GetAccessToken(OAuthToken, AuthVerifier));

            IsBusy = false;

            AccessToken = accessResponse?.token;
            AccessSecret = accessResponse?.secret;
        }

        public override bool CheckIfValid()
        {
            // TODO : check validations for all plugin-related notification settings
            return true;
        }

        protected override bool CanExecuteOkCommand(object obj)
        {
            return true;
        }

        public override void ApplyChanges()
        {
            if (_setting is not TwitterPluginNotificationSetting castedSetting)
            {
                log.Error("Notifications: Can't cast Plugin-related settings to Twitter settings.");
                return;
            }

            if (SettingKey == null || SettingId == null)
            {
                log.Error($"Notifications: Can't save Twitter Plugin-related settings. Setting Key - [{SettingKey}]. Setting Id - [{SettingId}]");
                return;
            }

            castedSetting.ApiToken = ApiToken;
            castedSetting.ApiSecret = ApiSecret;

            castedSetting.AccessToken = AccessToken;
            castedSetting.AccessSecret = AccessSecret;

            castedSetting.IsEnabled = IsEnabled;

            castedSetting.UpdateInSource();
        }
    }
}
