using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace VisualHFT.Commons.WPF.ViewMapping
{
    public interface IViewMapping
    {
        Type ViewModelType { get; }
        Type ViewType { get; }
    }

    /// <summary>
    /// Attribute to make a simple mapping between a setting model and default view/vm to keep UI-related things out of business layer
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class DefaultSettingsViewAttribute : Attribute, IViewMapping
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
    public class CompactSettingsViewAttribute : Attribute, IViewMapping
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

    /// <summary>
    /// Attribute to make a simple mapping between a plugin and custom tile control to display on dashboard
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class PluginTileViewAttribute : Attribute, IViewMapping
    {
        private readonly Type _viewModelType;
        private readonly Type _viewType;

        public Type ViewModelType => _viewModelType;
        public Type ViewType => _viewType;

        public PluginTileViewAttribute(Type viewModelType, Type viewType)
        {
            _viewModelType = viewModelType;
            _viewType = viewType;
        }
    }

    /// <summary>
    /// Container for view/viewmodel mappings.
    /// </summary>
    public class Map
    {
        public object? View { get; private set; }

        public object? ViewModel { get; private set; }

        public Map(object? view, object? vm)
        {
            View = view;
            ViewModel = vm;
        }

        /// <summary>
        /// Check if mapping is valid to proceed with. View should always be UserControl and ViewModel should be of specified type T.
        /// </summary>
        /// <typeparam name="T">Type of view model</typeparam>
        /// <returns>True if contained view/vm are valid, false if not.</returns>
        public bool IsValid<T>() where T : class
        {
            if (View == null || View is not UserControl)
                return false;

            if (ViewModel == null || ViewModel is not T)
                return false;

            return true;
        }
    }
}
