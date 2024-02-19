using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json.Linq;
using Prism.Mvvm;
using QuickFix;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing.Printing;
using System.Linq;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using VisualHFT.Commons.Helpers;
using VisualHFT.Commons.WPF.Helper;
using VisualHFT.Commons.WPF.ViewModel;
using VisualHFT.Helpers;
using VisualHFT.Model;
using VisualHFT.UserSettings;

namespace VisualHFT.ViewModel
{

    public class vmUserSettings : BindableBase
    {
        #region Fields

        private string _validationMessage = string.Empty;
        private bool _containsUnsaved = false;
        private List<BaseSettingsViewModel> _allViewModels = new List<BaseSettingsViewModel>();
        private readonly ISettingsManager _settingsManager;

        #endregion

        #region Properties

        /// <summary>
        /// Flag that shows if any changes are unsaved
        /// </summary>
        public bool ContainsUnsaved
        {
            get => _containsUnsaved;
            set
            {
                if (_containsUnsaved != value)
                {
                    _containsUnsaved = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Message to show saving error details.
        /// </summary>
        public string ValidationMessage
        {
            get => _validationMessage;
            set
            {
                if (_validationMessage != value)
                {
                    _validationMessage = value;
                    RaisePropertyChanged();
                }
            }
        }

        public ObservableCollection<TreeViewModel> Categories { get; set; } = new ObservableCollection<TreeViewModel>();

        public UserSettings.UserSettings? Settings => _settingsManager.UserSettings;

        public TreeViewModel? SelectedSettings { get; set; }

        #endregion

        public ICommand SaveCommand { get; set; }
        public ICommand CancelCommand { get; set; }

        public event EventHandler? OnClose;

        public vmUserSettings(ISettingsManager settingsManager)
        {
            _settingsManager = settingsManager;

            CancelCommand = new RelayCommand<object>(Cancel);
            SaveCommand = new RelayCommand<object>(SaveSettings, CanSave);

            ParseSettings();

            SelectedSettings = Categories.FirstOrDefault();
        }

        private void ParseSettings()
        {
            _allViewModels.Clear();
            Categories.Clear();

            // TODO : custom exception here
            if (_settingsManager.UserSettings == null)
                return;

            var settings = _settingsManager.UserSettings.ComponentSettings;
            var pluginNotificationSettings = _settingsManager.UserSettings.GetPluginsRelatedNotificationSettings();

            foreach (var group in settings)
            {
                var key = group.SettingKey;
                var containers = new List<TreeViewModel>();

                foreach (var container in group.SettingContainers)
                {
                    // TODO : logs here
                    if (container.Settings == null)
                        continue;

                    var id = container.Id;
                    var header = id;
                    var content = ParseAsAttribute(key, id, container.Settings) ?? BuildNoSettingsView();

                    if (content is UserControl view && view.DataContext is BaseSettingsViewModel baseViewModel)
                        header = baseViewModel.Header;

                    var tvmContainer = new TreeViewModel(header, container.Settings, content);

                    // TODO : to refact! unreadable!
                    if (pluginNotificationSettings != null && tvmContainer.Contents != null && content is not NoSettingViewModel)
                        foreach (var plugSetting in pluginNotificationSettings.Where(_ => _.Key == id))
                        {
                            var contentInner = ParseAsAttribute(SettingKey.NOTIFICATION, plugSetting.Key, plugSetting.Setting);
                            if (contentInner == null)
                                continue;

                            tvmContainer.Contents.Add(new TreeViewModel.SettingViewPair(plugSetting.Setting, contentInner));
                        }

                    containers.Add(tvmContainer);
                }

                var categoryName = AttributesHelper.GetEnumDescription(key) ?? key.ToString();
                var category = new TreeViewModel(categoryName, containers);

                Categories.Add(category);
            }

            // Reset vm's to no-changes state
            foreach (var vm in _allViewModels)
                vm.ResetStatus();
        }

        private object? ParseAsAttribute(SettingKey key, string id, IBaseSettings setting)
        {
            // Get settings UI from metadata
            var mappedUI = Commons.WPF.Helper.UIHelper.GetSettingsUI(setting);

            if (mappedUI == null || mappedUI?.view is not UserControl view || mappedUI?.vm is not BaseSettingsViewModel viewModel)
                return null;

            viewModel.SettingKey = key;
            viewModel.SettingId = id;

            viewModel.SettingChanged += SettingsChanged;

            _allViewModels.Add(viewModel);

            view.DataContext = viewModel;

            return view;
        }

        private object BuildNoSettingsView()
        {
            return new NoSettingViewModel();
        }

        private void SettingsChanged(object? sender, EventArgs e)
        {
            ContainsUnsaved = true;
            (SaveCommand as RelayCommand<object>)?.RaiseCanExecuteChanged();
        }

        private void SaveSettings(object obj)
        {
            try
            {
                var settings = Settings.ComponentSettings.FirstOrDefault(_ => _.SettingKey == SettingKey.NOTIFICATION)?.SettingContainers;

                var changedVm = _allViewModels.Where(_ => _.ContainsUnsavedChanges);
                foreach (var vm in changedVm)
                {
                    try
                    {
                        vm.ApplyChanges();
                    }
                    catch
                    {
                        // TODO : change the logic here
                    }
                }

                _settingsManager.Save();

                ContainsUnsaved = false;
                (SaveCommand as RelayCommand<object>)?.RaiseCanExecuteChanged();

                ValidationMessage = "Settings saved";
            }
            catch (Exception ex)
            {
                // TODO : add logs
                ValidationMessage = ex.Message;
            }
        }

        private bool CanSave(object obj)
        {
            return true; // ContainsUnsaved && !_allViewModels.Any(_ => !_.CheckIfValid());
        }

        private void Cancel(object obj)
        {
            OnClose?.Invoke(this, new EventArgs());
        }
    }

    public class TreeViewModel : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged implementation

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void RaisePropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        private BaseSettingsViewModel? _baseSettingViewModel;

        public string Header { get; set; }
        public bool IsContainer { get; set; } = false;
        public List<SettingViewPair>? Contents { get; set; } = null;
        public bool IsValid { get; set; } = true;

        public bool IsBlank => Contents?.Count(_ => _.View is not NoSettingViewModel) == 0;
        public event EventHandler<bool>? OnValidationError;

        public ObservableCollection<TreeViewModel> Children { get; set; } = new ObservableCollection<TreeViewModel>();

        public TreeViewModel(string header, IBaseSettings setting, object content)
            : this(header, new List<SettingViewPair> { new SettingViewPair(setting, content) }) { }

        public TreeViewModel(string header, List<SettingViewPair> contents)
        {
            Header = header;
            Contents = contents;

            // Cover the settings changed event to display if data is invalid or not
            foreach (var content in Contents)
            {
                if (content.View is UserControl view && view.DataContext is BaseSettingsViewModel baseSettingViewModel)
                {
                    _baseSettingViewModel = baseSettingViewModel;
                    baseSettingViewModel.SettingChanged += BaseSettingViewModel_SettingChanged;
                }
            }
        }

        public TreeViewModel(string name, IList<TreeViewModel> children)
        {
            Header = name;
            Contents = null;
            IsContainer = true;

            Children = new ObservableCollection<TreeViewModel>(children);

            foreach (var child in Children)
                child.OnValidationError += Child_OnValidationError;

            ValidateAll();
        }

        private void BaseSettingViewModel_SettingChanged(object? sender, EventArgs e)
        {
            if (sender is BaseSettingsViewModel baseSettingViewModel)
            {
                var vmStatus = baseSettingViewModel.CheckIfValid();

                if (vmStatus != IsValid)
                {
                    IsValid = vmStatus;
                    RaisePropertyChanged("IsValid");

                    OnValidationError?.Invoke(this, vmStatus);
                }
            }
        }

        private void ValidateAll()
        {
            foreach (var child in Children)
                child.ValidateAll();

            if (_baseSettingViewModel != null)
            {
                var vmStatus = _baseSettingViewModel.CheckIfValid();

                if (vmStatus != IsValid)
                {
                    IsValid = vmStatus;
                    RaisePropertyChanged("IsValid");

                    OnValidationError?.Invoke(this, vmStatus);
                }
            }
        }

        private void Child_OnValidationError(object? sender, bool e)
        {
            var isValid = !Children.Any(_ => !_.IsValid);

            if (isValid != IsValid)
            {
                IsValid = isValid;
                OnValidationError?.Invoke(this, e);
            }
        }

        public struct SettingViewPair
        {
            public IBaseSettings Setting { get; set; }
            public object View { get; set; }

            public SettingViewPair(IBaseSettings setting, object view)
            {
                Setting = setting;
                View = view;
            }
        }
    }

    /// <summary>
    /// Empty viewmodel representing missing settings.
    /// </summary>
    public class NoSettingViewModel
    {

    }
}