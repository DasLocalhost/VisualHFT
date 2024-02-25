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
using MarketConnectors.Binance.Model;
using static log4net.Appender.RollingFileAppender;
using VisualHFT.UserSettings;

namespace MarketConnectors.Binance.ViewModel
{
    public class PluginSettingsViewModel : BaseSettingsViewModel
    {
        private const string _defaultHeader = "Binance PlugIn Settings";

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

                    case nameof(SymbolsText):
                        if (Symbols == null || !Symbols.Any() || Symbols.Any(s => string.IsNullOrWhiteSpace(s)))
                            return "Symbols cannot be empty and should be comma-separated.";
                        break;

                    case nameof(DepthLevels):
                        if (DepthLevels <= 0)
                            return "Depth levels should be a positive integer.";
                        break;

                    case nameof(UpdateIntervalMs):
                        if (UpdateIntervalMs <= 0)
                            return "Update interval should be a positive integer.";
                        break;

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

        private string? _apiKey;
        private string? _apiSecret;
        private int? _depthLevels;
        private int? _updateIntervalMs;
        private int? _providerId;
        private string? _providerName;
        private List<string>? _symbols;
        private string? _symbolsText;
        private string? _validationMessage;

        #endregion

        #region Properties

        public string? ApiKey
        {
            get => _apiKey;
            set
            {
                _apiKey = value;
                RaisePropertyChanged();
            }
        }
        public string? ApiSecret
        {
            get => _apiSecret;
            set
            {
                _apiSecret = value;
                RaisePropertyChanged();
            }
        }
        public string? SymbolsText
        {
            get => string.Join(",", Symbols ?? new List<string>());
            set
            {
                Symbols = value?.Split(',').Select(s => s.Trim()).ToList();
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(Symbols));
                RaiseCanExecuteChanged();
            }
        }
        public List<string>? Symbols
        {
            get => _symbols;
            set
            {
                _symbols = value;
                RaisePropertyChanged();
                RaiseCanExecuteChanged();
            }
        }
        public int? DepthLevels
        {
            get => _depthLevels;
            set
            {
                _depthLevels = value;
                RaisePropertyChanged();
                RaiseCanExecuteChanged();
            }
        }
        public int? UpdateIntervalMs
        {
            get => _updateIntervalMs;
            set
            {
                _updateIntervalMs = value;
                RaisePropertyChanged();
                RaiseCanExecuteChanged();
            }
        }
        public int? ProviderId
        {
            get => _providerId;
            set
            {
                _providerId = value;
                RaisePropertyChanged();
                RaiseCanExecuteChanged();
            }
        }
        public string? ProviderName
        {
            get => _providerName;
            set
            {
                _providerName = value;
                RaisePropertyChanged();
                RaiseCanExecuteChanged();
            }
        }

        public string? ValidationMessage
        {
            get { return _validationMessage; }
            set
            {
                _validationMessage = value;
                RaisePropertyChanged(raiseSettingsChanged: false);
            }
        }

        #endregion

        public PluginSettingsViewModel(PlugInSettings settings) : base(settings, _defaultHeader)
        {
            _apiKey = settings.ApiKey;
            _apiSecret = settings.ApiSecret;
            _symbolsText = settings.Symbol;
            _depthLevels = settings.DepthLevels;
            _updateIntervalMs = settings.UpdateIntervalMs;
            _providerId = settings.Provider.ProviderID;
            _providerName = settings.Provider.ProviderName;
        }

        public override bool CheckIfValid()
        {
            // This checks if any validation message exists for any of the properties
            return string.IsNullOrWhiteSpace(this[nameof(SymbolsText)]) &&
                   string.IsNullOrWhiteSpace(this[nameof(DepthLevels)]) &&
                   string.IsNullOrWhiteSpace(this[nameof(UpdateIntervalMs)]) &&
                   string.IsNullOrWhiteSpace(this[nameof(ProviderId)]) &&
                   string.IsNullOrWhiteSpace(this[nameof(ProviderName)]);
        }

        protected override bool CanExecuteOkCommand(object obj)
        {
            // This checks if any validation message exists for any of the properties
            return string.IsNullOrWhiteSpace(this[nameof(SymbolsText)]) &&
                   string.IsNullOrWhiteSpace(this[nameof(DepthLevels)]) &&
                   string.IsNullOrWhiteSpace(this[nameof(UpdateIntervalMs)]) &&
                   string.IsNullOrWhiteSpace(this[nameof(ProviderId)]) &&
                   string.IsNullOrWhiteSpace(this[nameof(ProviderName)]);
        }

        public override void ApplyChanges()
        {
            // TODO : logs / Exceptions here
            if (_setting is not PlugInSettings castedSetting)
                return;

            if (SettingKey == null || SettingId == null)
                return;

            castedSetting.ApiSecret = ApiSecret ?? "";
            castedSetting.ApiKey = ApiKey ?? "";
            castedSetting.Symbols = Symbols ?? new List<string>();
            castedSetting.DepthLevels = DepthLevels ?? 0;
            castedSetting.UpdateIntervalMs = UpdateIntervalMs ?? 0;
            castedSetting.Provider.ProviderID = ProviderId ?? 0;

            RaiseSettingsSaved(castedSetting);
            //_settingsManager.UserSettings?.RaiseSettingsChanged(castedSetting);
        }
    }
}