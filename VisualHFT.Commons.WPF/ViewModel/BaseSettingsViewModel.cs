using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using VisualHFT.UserSettings;

namespace VisualHFT.Commons.WPF.ViewModel
{
    public abstract class BaseSettingsViewModel : INotifyPropertyChanged, IDataErrorInfo
    {
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
                SettingChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #region IDataErrorInfo implementation

        public abstract string Error { get; }
        public abstract string this[string columnName] { get; }

        #endregion

        protected IBaseSettings _setting;
        public IBaseSettings Setting { get { return _setting; } }

        public string? SettingId { get; set; }
        public SettingKey? SettingKey { get; set; }

        public string Header { get; protected set; } = string.Empty;

        public bool ContainsUnsavedChanges { get; protected set; }

        /// <summary>
        /// Raise the event on any changes on UI. By default including to the INotifyPropertyChanged implementation.
        /// </summary>
        public event EventHandler? SettingChanged;

        public BaseSettingsViewModel(IBaseSettings setting, string header)
        {
            _setting = setting;
            Header = header;

            this.SettingChanged += (_, __) => ContainsUnsavedChanges = true;
        }

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

        public void Cancel()
        {

        }
    }

    /// <summary>
    /// Attribute to make a simple mapping between a setting model and default view/vm to keep UI-related things out of business layer
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class DefaultSettingsViewAttribute : Attribute
    {
        private readonly Type _viewModelType;
        private readonly Type _viewType;

        public Type ViewModelType => _viewModelType;
        public Type ViewType => _viewType;

        public DefaultSettingsViewAttribute(Type viewModelType, Type viewType)
        {
            _viewModelType = viewModelType;
            _viewType = viewType;
        }
    }

    /// <summary>
    /// Attribute to make a simple mapping between a setting model and compact view/vm to display in dialog
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class CompactSettingsViewAttribute : Attribute
    {
        private readonly Type _viewModelType;
        private readonly Type _viewType;

        public Type ViewModelType => _viewModelType;
        public Type ViewType => _viewType;

        public CompactSettingsViewAttribute(Type viewModelType, Type viewType)
        {
            _viewModelType = viewModelType;
            _viewType = viewType;
        }
    }
}