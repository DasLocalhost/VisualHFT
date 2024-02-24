using log4net.Plugin;
using NetMQ.Sockets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using VisualHFT.Commons.NotificationManager;
using VisualHFT.Commons.PluginManager;
using VisualHFT.Commons.Studies;
using VisualHFT.Commons.WPF.ViewModel;
using VisualHFT.DataRetriever;
using VisualHFT.UserSettings;
using VisualHFT.ViewModel;
using VisualHFT.Commons.WPF.Helper;

namespace VisualHFT.PluginManager
{
    public class PluginManager : IPluginManager
    {
        #region Fields

        private List<IPlugin> ALL_PLUGINS = new List<IPlugin>();
        private object _locker = new object();

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly ISettingsManager _settingsManager;

        #endregion

        #region Properties

        public List<IPlugin> AllPlugins { get { lock (_locker) return ALL_PLUGINS; } }
        public bool AllPluginsReloaded { get; internal set; }

        #endregion

        //public static PluginManager? Instance { get; private set; } = null;

        public PluginManager(ISettingsManager settingsManager)
        {
            _settingsManager = settingsManager;
        }

        public void Initialize()
        {
            try
            {
                AllPluginsReloaded = false;
                LoadPlugins();
                StartPlugins();
                AllPluginsReloaded = true;
            }
            catch (Exception ex)
            {
                log.Error("Plugins: Initialization failed.", ex);
            }
        }

        private void LoadPlugins()
        {
            // 1. By default load all dll's in current Folder. 
            var pluginsDirectory = AppDomain.CurrentDomain.BaseDirectory; // This gets the directory where your WPF app is running
            lock (_locker)
                LoadPluginsByDirectory(pluginsDirectory);

            // 3. Load Other Plugins in different folders

            // 4. If is Started, then Start

            // 5. If empty or Stopped. Do nothing.

        }

        private void StartPlugins()
        {
            lock (_locker)
            {
                if (ALL_PLUGINS.Count == 0) { return; }
                foreach (var plugin in ALL_PLUGINS)
                {
                    try
                    {
                        StartPlugin(plugin);
                    }
                    catch (Exception ex)
                    {
                        string msg = $"Plugin {plugin.Name} cannot be loaded: ";
                        log.Error(msg, ex);
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            // Display popup message
                            MessageBox.Show(msg + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        });
                    }
                }
            }
        }

        public void StartPlugin(IPlugin plugin)
        {
            try
            {
                if (plugin != null)
                {
                    if (plugin is IDataRetriever dataRetriever)
                    {
                        //DATA RETRIEVER
                        var processor = new VisualHFT.DataRetriever.DataProcessor(dataRetriever);
                        dataRetriever.StartAsync();
                    }
                    else if (plugin is IStudy study)
                    {
                        study.StartAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public void StopPlugin(IPlugin plugin)
        {
            try
            {
                if (plugin != null)
                {
                    if (plugin is IDataRetriever dataRetriever)
                    {
                        dataRetriever.StopAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public void SettingPlugin(IPlugin plugin)
        {
            if (plugin == null)
                return;

            try
            {
                var settings = UIHelper.GetSettingsUI(plugin);

                if (settings == null || settings?.view is not UserControl view || settings?.vm is not IModularViewModel viewModel)
                {
                    log.Warn($"Plugins: Can't find Settings UI for [{plugin.Name}] plugin.");
                    return;
                }

                view.DataContext = viewModel;
            }
            catch (Exception ex)
            {

            }

            // TODO : replace this part with one above
            UserControl _ucSettings = null;
            try
            {

                if (plugin != null)
                {
                    var formSettings = new View.PluginSettings();
                    plugin.CloseSettingWindow = () =>
                    {
                        formSettings.Close();
                    };

                    _ucSettings = plugin.GetUISettings() as UserControl;
                    if (_ucSettings == null)
                    {
                        plugin.CloseSettingWindow = null;
                        formSettings = null;
                        return;
                    }
                    formSettings.MainGrid.Children.Add(_ucSettings);
                    formSettings.Title = $"{plugin.Name} Settings";
                    formSettings.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;
                    formSettings.Topmost = true;
                    formSettings.ShowInTaskbar = false;
                    formSettings.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public void UnloadPlugins()
        {
            lock (_locker)
            {
                if (ALL_PLUGINS.Count == 0) { return; }
                foreach (var plugin in ALL_PLUGINS.OfType<IDisposable>())
                {
                    plugin.Dispose();
                }
            }
        }

        private void LoadPluginsByDirectory(string pluginsDirectory)
        {
            foreach (var file in Directory.GetFiles(pluginsDirectory, "*.dll"))
            {
                try
                {
                    var assembly = Assembly.LoadFrom(file);
                    foreach (var type in assembly.GetExportedTypes())
                    {
                        if (!type.IsAbstract && type.GetInterfaces().Contains(typeof(IPlugin)))
                        {
                            var plugin = Activator.CreateInstance(type, _settingsManager) as IPlugin;
                            if (string.IsNullOrEmpty(plugin.Name))
                                continue;

                            ALL_PLUGINS.Add(plugin);
                            plugin.OnError += Plugin_OnError;
                            log.Info("Plugins: " + plugin.Name + " loaded OK.");
                        }

                    }
                }
                catch (Exception ex)
                {
                    log.Error(ex);
                    throw new Exception($"Plugin {file} has failed to load. Error: " + ex.Message);
                }
            }
        }

        private void Plugin_OnError(object? sender, ErrorEventArgs e)
        {
            if (e.IsCritical)
            {
                log.Error(e.PluginName, e.Exception);
                Helpers.HelperCommon.GLOBAL_DIALOGS["error"](e.Exception.Message, e.PluginName);
            }
            else
            {
                //LOG error
                log.Error(e.PluginName, e.Exception);
            }
        }
    }
}