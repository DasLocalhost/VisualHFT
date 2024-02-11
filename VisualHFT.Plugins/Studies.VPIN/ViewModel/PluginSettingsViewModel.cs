using Newtonsoft.Json.Linq;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration.Provider;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using VisualHFT.Commons.WPF.ViewModel;
using VisualHFT.Helpers;
using VisualHFT.Studies.VPIN.Model;
using VisualHFT.UserSettings;
using VisualHFT.ViewModel.Model;
using static log4net.Appender.RollingFileAppender;

namespace VisualHFT.Studies.VPIN.ViewModel
{
    public class PluginSettingsViewModel : BaseSettingsViewModel
    {
        private const string _defaultHeader = "VPIN PlugIn Settings";

        #region IDataErrorInfo implementation

        public override string Error => null;
        public override string this[string columnName]
        {
            get
            {
                switch (columnName)
                {
                    case nameof(BucketVolumeSize):
                        if (BucketVolumeSize <= 0)
                            return "Bucket Volume Size should be a positive number.";
                        break;

                    case nameof(SelectedProvider):
                        if (SelectedProvider == null)
                            return "Select the Provider.";
                        break;
                    case nameof(SelectedSymbol):
                        if (string.IsNullOrWhiteSpace(SelectedSymbol))
                            return "Select the Symbol.";
                        break;

                    default:
                        return null;
                }
                return null;
            }
        }

        #endregion

        #region Fields

        private double _bucketVolSize;
        private ObservableCollection<Provider> _providers;
        private ObservableCollection<string> _symbols;
        private VisualHFT.ViewModel.Model.Provider _selectedProvider;
        private int? _selectedProviderID;
        private string _selectedSymbol;
        private AggregationLevel _aggregationLevelSelection;

        private string _validationMessage = string.Empty;
        private string _successMessage = string.Empty;
        private Action _actionCloseWindow;

        #endregion

        #region Properties

        public ObservableCollection<VisualHFT.ViewModel.Model.Provider> Providers { get => _providers; set => _providers = value; }
        public ObservableCollection<string> Symbols { get => _symbols; set => _symbols = value; }

        public int? SelectedProviderID
        {
            get { return _selectedProviderID; }
            set
            {
                _selectedProviderID = value;
                RaisePropertyChanged();
                RaiseCanExecuteChanged();
                LoadSelectedProviderID();
            }
        }
        public double BucketVolumeSize
        {
            get => _bucketVolSize;
            set
            {
                _bucketVolSize = value;
                RaisePropertyChanged();
                RaiseCanExecuteChanged();
            }
        }
        public VisualHFT.ViewModel.Model.Provider SelectedProvider
        {
            get => _selectedProvider;
            set
            {
                _selectedProvider = value;
                RaisePropertyChanged();
                RaiseCanExecuteChanged();
                LoadSelectedProviderID();
            }
        }
        public string SelectedSymbol
        {
            get => _selectedSymbol;
            set
            {
                _selectedSymbol = value;
                RaisePropertyChanged();
                RaiseCanExecuteChanged();
            }
        }

        public AggregationLevel AggregationLevelSelection
        {
            get => _aggregationLevelSelection;
            set
            {
                _aggregationLevelSelection = value;
                RaisePropertyChanged();
            }
        }
        public ObservableCollection<Tuple<string, AggregationLevel>> AggregationLevels { get; set; }


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
        public Action UpdateSettingsFromUI { get; set; }

        public PluginSettingsViewModel(PlugInSettings settings) : base(settings, _defaultHeader)
        {
            //OkCommand = new RelayCommand<object>(ExecuteOkCommand, CanExecuteOkCommand);
            //CancelCommand = new RelayCommand<object>(ExecuteCancelCommand);
            //_actionCloseWindow = actionCloseWindow;

            _symbols = new ObservableCollection<string>(HelperSymbol.Instance);
            _providers = Provider.CreateObservableCollection();
            RaisePropertyChanged(nameof(Providers));
            RaisePropertyChanged(nameof(Symbols));

            // TODO : check this part. Should be mapped using provider id?
            //SelectedProvider = settings.Provider;

            BucketVolumeSize = settings.BucketVolSize;
            SelectedSymbol = settings.Symbol;
            SelectedProviderID = settings.Provider.ProviderID;
            AggregationLevelSelection = settings.AggregationLevel;

            HelperProvider.Instance.OnDataReceived += PROVIDERS_OnDataReceived;
            HelperSymbol.Instance.OnCollectionChanged += ALLSYMBOLS_CollectionChanged;

            AggregationLevels = new ObservableCollection<Tuple<string, AggregationLevel>>();
            foreach (AggregationLevel level in Enum.GetValues(typeof(AggregationLevel)))
            {
                AggregationLevels.Add(new Tuple<string, AggregationLevel>(HelperCommon.GetEnumDescription(level), level));
            }
            //AggregationLevelSelection = AggregationLevel.Automatic;

            LoadSelectedProviderID();
        }

        public override bool CheckIfValid()
        {
            // This checks if any validation message exists for any of the properties
            return // string.IsNullOrWhiteSpace(this[nameof(BucketVolumeSize)]) &&
                   string.IsNullOrWhiteSpace(this[nameof(SelectedProvider)]) &&
                   string.IsNullOrWhiteSpace(this[nameof(SelectedSymbol)]);
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
            return // string.IsNullOrWhiteSpace(this[nameof(BucketVolumeSize)]) &&
                   string.IsNullOrWhiteSpace(this[nameof(SelectedProvider)]) &&
                   string.IsNullOrWhiteSpace(this[nameof(SelectedSymbol)]);

        }

        private void RaiseCanExecuteChanged()
        {
            (OkCommand as RelayCommand<object>)?.RaiseCanExecuteChanged();
        }

        private void LoadSelectedProviderID()
        {
            if (_selectedProvider != null)
            {
                _selectedProviderID = _selectedProvider.ProviderID;
                RaisePropertyChanged(nameof(SelectedSymbol));
            }
            else if (_selectedProviderID.HasValue && _providers.Any())
            {
                _selectedProvider = _providers.FirstOrDefault(x => x.ProviderID == _selectedProviderID.Value);
                RaisePropertyChanged(nameof(SelectedProvider));
            }
        }

        private void ALLSYMBOLS_CollectionChanged(object? sender, EventArgs e)
        {
            _symbols = new ObservableCollection<string>(HelperSymbol.Instance);
            RaisePropertyChanged(nameof(Symbols));
        }

        private void PROVIDERS_OnDataReceived(object? sender, VisualHFT.Model.Provider e)
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Render, new Action(() =>
            {
                var item = new VisualHFT.ViewModel.Model.Provider(e);
                if (!_providers.Any(x => x.ProviderCode == e.ProviderCode))
                    _providers.Add(item);
                if (_selectedProvider == null && e.Status == eSESSIONSTATUS.BOTH_CONNECTED) //default provider must be the first who's Active
                    SelectedProvider = item;
            }));
        }

        public override void ApplyChanges()
        {
            // TODO : logs / Exceptions here
            if (_setting is not PlugInSettings castedSetting)
                return;

            if (SettingKey == null || SettingId == null)
                return;

            castedSetting.BucketVolSize = BucketVolumeSize;
            castedSetting.Symbol = SelectedSymbol;
            castedSetting.Provider.ProviderID = SelectedProviderID ?? 0;
            castedSetting.AggregationLevel = AggregationLevelSelection;

            SettingsManager.Instance.UserSettings?.RaiseSettingsChanged(castedSetting);
        }
    }
}