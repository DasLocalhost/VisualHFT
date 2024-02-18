using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using VisualHFT.Commons.Helpers;
using VisualHFT.Commons.WPF.ViewModel;

namespace VisualHFT.Commons.WPF.Helper
{
    public static class UIHelper
    {
        /// <summary>
        /// Get view and viewmodel described in DefaultSettingsView Attribute.
        /// </summary>
        /// <param name="source">Source of attribute</param>
        /// <returns>View and viewmodel</returns>
        public static (object view, object vm)? GetSettingsUI(object source)
        {
            // Get the view / viewmodel types from the attribute
            var vmType = AttributesHelper.GetAttributeValue<DefaultSettingsViewAttribute, Type>(source, _ => _.ViewModelType);
            var viewType = AttributesHelper.GetAttributeValue<DefaultSettingsViewAttribute, Type>(source, _ => _.ViewType);

            // Return null if mapping is missing
            if (vmType == null || viewType == null)
                return null;

            // Create instances of view and viewmodel
            var view = Activator.CreateInstance(viewType) as UserControl;
            var viewModel = Activator.CreateInstance(vmType, source);

            if (view == null || viewModel == null) 
                return null;

            return (view, viewModel);
        }

        public static (object view, object vm)? GetCompactSettingsUI(object source)
        {
            // Get the view / viewmodel types from the attribute
            var vmType = AttributesHelper.GetAttributeValue<CompactSettingsViewAttribute, Type>(source, _ => _.ViewModelType);
            var viewType = AttributesHelper.GetAttributeValue<CompactSettingsViewAttribute, Type>(source, _ => _.ViewType);

            // Return null if mapping is missing
            if (vmType == null || viewType == null)
                return null;

            // Create instances of view and viewmodel
            var view = Activator.CreateInstance(viewType) as UserControl;
            var viewModel = Activator.CreateInstance(vmType, source);

            if (view == null || viewModel == null)
                return null;

            return (view, viewModel);
        }

        public static void ShowDialog(UserControl view, IModularViewModel viewmodel)
        {
            var dialogWindow = new Window();

            view.DataContext = viewmodel;
            dialogWindow.Content = view;

            viewmodel.OnClose += (_, __) => dialogWindow.Close();

            dialogWindow.Show();
        }
    }
}
