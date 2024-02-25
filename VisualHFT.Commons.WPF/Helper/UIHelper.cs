using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using VisualHFT.Commons.Helpers;
using VisualHFT.Commons.WPF.View;
using VisualHFT.Commons.WPF.ViewMapping;
using VisualHFT.Commons.WPF.ViewModel;
using VisualHFT.PluginManager;

namespace VisualHFT.Commons.WPF.Helper
{
    // TODO : to refact UIHelper is a duplicated class. Need to specify name
    public static class UIHelper
    {
        /// <summary>
        /// Open the compact settings window for the plugin.
        /// </summary>
        /// <param name="plugin">Plugin, a source of the CompactSettingsViewAttribute attribute</param>
        public static void ShowCompactSettings(IPlugin plugin)
        {
            var settings = GetCompactSettingsUI(plugin.Settings);

            if (settings == null || !settings.IsValid<IModularViewModel>())
            {
                throw new Exception($"Plugins: Can't find Settings UI for [{plugin.Name}] plugin.");
            }

            var view = settings.View as UserControl;
            var viewModel = settings.ViewModel as IModularViewModel;

            ShowDialog(view, viewModel);
        }

        /// <summary>
        /// Get view and viewmodel described in the DefaultSettingsView Attribute.
        /// </summary>
        /// <param name="source">Source of the attribute</param>
        /// <returns>View and viewmodel pair or a null-value, that should be covered in calling class</returns>
        public static Map? GetSettingsUI(object source)
        {
            return GetViewMapping<DefaultSettingsViewAttribute>(source);
        }

        /// <summary>
        /// Get view and viewmodel described in the CompactSettingsViewAttribute Attribute.
        /// </summary>
        /// <param name="source">Source of the attribute</param>
        /// <returns>View and viewmodel pair or a null-value, that should be covered in calling class</returns>
        public static Map? GetCompactSettingsUI(object source)
        {
            return GetViewMapping<CompactSettingsViewAttribute>(source);
        }

        /// <summary>
        /// Get view and viewmodel described in the PluginTileViewAttribute Attribute.
        /// </summary>
        /// <param name="source">Source of the attribute</param>
        /// <returns>View and viewmodel pair or a null-value, that should be covered in calling class</returns>
        public static Map? GetPluginTile(object source)
        {
            return GetViewMapping<PluginTileViewAttribute>(source);
        }

        /// <summary>
        /// Get view mapping based on view mapping attribute.
        /// </summary>
        /// <typeparam name="T">Type of view mapping attribute to extract mapping</typeparam>
        /// <param name="source">The source of the attribute</param>
        /// <returns>View and viewmodel pair</returns>
        private static Map? GetViewMapping<AttrT>(object source) where AttrT : Attribute, IViewMapping
        {
            // Get the view / viewmodel types from the attribute
            var vmType = AttributesHelper.GetAttributeValue<AttrT, Type>(source, _ => _.ViewModelType);
            var viewType = AttributesHelper.GetAttributeValue<AttrT, Type>(source, _ => _.ViewType);

            // Return null if mapping is missing
            if (vmType == null || viewType == null)
                return null;

            // Create instances of view and viewmodel
            var view = Activator.CreateInstance(viewType) as UserControl;
            var viewModel = Activator.CreateInstance(vmType, source);

            if (view == null || viewModel == null)
                return null;

            return new Map(view, viewModel);
        }

        /// <summary>
        /// Open a modal window with UC as a content and VM binded to it.
        /// </summary>
        /// <param name="view">Content of the modal window</param>
        /// <param name="viewmodel">Viewmodel that should be binded to the content</param>
        public static void ShowDialog(UserControl view, IModularViewModel viewmodel)
        {
            var dialogWindow = MakeModalWindow();

            view.DataContext = viewmodel;
            dialogWindow.Content = view;

            viewmodel.OnClose += (_, __) => dialogWindow.Close();

            dialogWindow.Show();
        }

        /// <summary>
        /// Create a standard modal window.
        /// </summary>
        /// <returns>Modal window</returns>
        private static Window MakeModalWindow()
        {
            var modalWindow = new ModalWindow();

            modalWindow.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;
            modalWindow.Topmost = true;
            modalWindow.ShowInTaskbar = false;

            return modalWindow;
        }
    }
}
