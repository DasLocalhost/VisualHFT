using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using VisualHFT.Commons.WPF.ViewModel;
using VisualHFT.DataRetriever;
using VisualHFT.Helpers;
using MarketConnectors.WebSocket.Model;
using VisualHFT.UserSettings;

namespace MarketConnectors.WebSocket.ViewModel
{
    public class PluginSettingsViewModel : BaseSettingsViewModel
    {
        private const string _defaultHeader = "WebSocket PlugIn Settings";

        #region IDataErrorInfo implementation

        public override string Error => null;

        public override string this[string columnName]
        {
            get
            {
                switch (columnName)
                {
                    /*case nameof(ApiKey):
                        if (string.IsNullOrWhiteSpace(ApiKey))
                            return "API Key cannot be empty.";
                        break;

                    case nameof(ApiSecret):
                        if (string.IsNullOrWhiteSpace(ApiSecret))
                            return "API Secret cannot be empty.";
                        break;*/

                    case nameof(ProviderId):
                        if (ProviderId <= 0)
                            return "Provider ID should be a positive integer.";
                        break;

                    case nameof(ProviderName):
                        if (string.IsNullOrWhiteSpace(ProviderName))
                            return "Provider name cannot be empty.";
                        break;

                    default:
                        return null;
                }
                return null;
            }
        }


        #endregion

        #region Fields

        private string _hostName;
        private int _port;
        private int _providerId;
        private string _providerName;
        private string _validationMessage = string.Empty;
        private string _successMessage = string.Empty;
        private Action _actionCloseWindow;

        #endregion

        #region Properties

        public string HostName
        {
            get => _hostName;
            set { _hostName = value; RaisePropertyChanged(); }
        }
        public int Port
        {
            get => _port;
            set { _port = value; RaisePropertyChanged(); }
        }
        public int ProviderId
        {
            get => _providerId;
            set
            {
                _providerId = value;
                RaisePropertyChanged();
                RaiseCanExecuteChanged();
            }
        }
        public string ProviderName
        {
            get => _providerName;
            set
            {
                _providerName = value;
                RaisePropertyChanged();
                RaiseCanExecuteChanged();
            }
        }
        public string ValidationMessage
        {
            get { return _validationMessage; }
            set { _validationMessage = value; RaisePropertyChanged(raiseSettingsChanged: false); }
        }

        public string SuccessMessage
        {
            get { return _successMessage; }
            set { _successMessage = value; RaisePropertyChanged(raiseSettingsChanged: false); }
        }

        #endregion

        public ICommand OkCommand { get; private set; }
        public ICommand CancelCommand { get; private set; }
        public Action UpdateSettingsFromUI{ get; set; }

        public PluginSettingsViewModel(PlugInSettings settings) : base(settings, _defaultHeader)
        {
            //OkCommand = new RelayCommand<object>(ExecuteOkCommand, CanExecuteOkCommand);
            //CancelCommand = new RelayCommand<object>(ExecuteCancelCommand);
            //_actionCloseWindow = actionCloseWindow;

            _hostName = settings.HostName;
            _port = settings.Port;
            _providerId = settings.ProviderId;
            _providerName = settings.ProviderName;
        }

        public override bool CheckIfValid()
        {
            // This checks if any validation message exists for any of the properties
            return string.IsNullOrWhiteSpace(this[nameof(ProviderId)]) &&
                   string.IsNullOrWhiteSpace(this[nameof(ProviderName)]);
        }

        private void ExecuteOkCommand(object obj)
        {            
            SuccessMessage = "Settings saved successfully!";
            UpdateSettingsFromUI?.Invoke();
            _actionCloseWindow?.Invoke();
        }
        private void ExecuteCancelCommand(object obj)
        {
            _actionCloseWindow?.Invoke();
        }
        private bool CanExecuteOkCommand(object obj)
        {
            // This checks if any validation message exists for any of the properties
            return string.IsNullOrWhiteSpace(this[nameof(HostName)]) &&
                   string.IsNullOrWhiteSpace(this[nameof(Port)]) &&
                   string.IsNullOrWhiteSpace(this[nameof(ProviderId)]) &&
                   string.IsNullOrWhiteSpace(this[nameof(ProviderName)]);
        }
        private void RaiseCanExecuteChanged()
        {
            (OkCommand as RelayCommand<object>)?.RaiseCanExecuteChanged();
        }

        public override void ApplyChanges()
        {
            // TODO : logs / Exceptions here
            if (_setting is not PlugInSettings castedSetting)
                return;

            if (SettingKey == null || SettingId == null)
                return;

            castedSetting.HostName = HostName;
            castedSetting.Port = Port;
            castedSetting.Provider.ProviderID = ProviderId;

            SettingsManager.Instance.UserSettings?.RaiseSettingsChanged(castedSetting);
        }
    }
}
