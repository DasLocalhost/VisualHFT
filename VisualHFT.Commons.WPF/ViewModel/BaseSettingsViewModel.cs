using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using VisualHFT.Helpers;
using VisualHFT.UserSettings;

namespace VisualHFT.Commons.WPF.ViewModel
{
    public abstract class BaseSettingsViewModel : INotifyPropertyChanged, IDataErrorInfo, IModularViewModel
    {
        // TODO : refact - check properties from child classes, some of them should be moved here

        #region INotifyPropertyChanged implementation

        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Wrapper around standard property changed including the possibility to raise a setting changed event.
        /// </summary>
        /// <param name="raiseSettingsChanged">True if property is a real setting, not a technical field, and setting should be marked as changed.</param>
        /// <param name="propertyName">Name of property changed.</param>
        protected virtual void RaisePropertyChanged([CallerMemberName] string? propertyName = null, bool raiseSettingsChanged = true)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            if (raiseSettingsChanged)
                SettingsChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #region IDataErrorInfo implementation

        public abstract string Error { get; }
        public abstract string this[string columnName] { get; }

        #endregion

        #region IModularViewModel implementation

        public ICommand OkCommand { get; }
        public ICommand CancelCommand { get; }

        public event EventHandler OnClose;

        #endregion

        protected IBaseSettings _setting;
        public IBaseSettings Setting { get { return _setting; } }

        public string? SettingId { get; set; }
        public SettingKey? SettingKey { get; set; }

        public string SuccessMessage { get; set; }

        public string Header { get; protected set; } = string.Empty;

        public bool ContainsUnsavedChanges { get; protected set; }

        /// <summary>
        /// Raise the event on any changes on UI. By default including to the INotifyPropertyChanged implementation.
        /// </summary>
        public event EventHandler? SettingsChanged;

        /// <summary>
        /// Raise the event to save changed settings in settings manager.
        /// </summary>
        public event EventHandler<IBaseSettings>? SettingsSaved;

        public BaseSettingsViewModel(IBaseSettings setting, string header)
        {
            _setting = setting;
            Header = header;

            SettingId = setting.SettingId;
            SettingKey = setting.SettingKey;

            OkCommand = new RelayCommand<object>(ExecuteOkCommand, CanExecuteOkCommand);
            CancelCommand = new RelayCommand<object>(ExecuteCancelCommand);

            this.SettingsChanged += (_, __) => ContainsUnsavedChanges = true;
        }


        protected void ExecuteOkCommand(object obj)
        {
            SuccessMessage = "Settings saved successfully!";
            ApplyChanges();
            Close();
        }

        protected void ExecuteCancelCommand(object obj)
        {
            Close();
        }

        protected void RaiseCanExecuteChanged()
        {
            (OkCommand as RelayCommand<object>)?.RaiseCanExecuteChanged();
        }

        protected abstract bool CanExecuteOkCommand(object obj);

        /// <summary>
        /// Close the modular window contains current viewmodel as a content
        /// </summary>
        protected void Close()
        {
            OnClose?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Set the status of the vm to not changed
        /// </summary>
        public void ResetStatus()
        {
            ContainsUnsavedChanges = false;
        }

        /// <summary>
        /// Check if no validation erros
        /// </summary>
        /// <returns>True if settings are valid</returns>
        public abstract bool CheckIfValid();

        /// <summary>
        /// Apply changes to the settings.
        /// </summary>
        public abstract void ApplyChanges();

        /// <summary>
        /// Method to raise the Settings Saved event from the implementations.
        /// </summary>
        /// <param name="settings"></param>
        protected void RaiseSettingsSaved(IBaseSettings settings)
        {
            SettingsSaved?.Invoke(this, settings);
        }

        public void Cancel()
        {

        }
    }
}