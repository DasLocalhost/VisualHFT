using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using VisualHFT.Commons.PluginManager;
using VisualHFT.PluginManager;

namespace VisualHFT.ViewModel
{
    public class vmPluginManager : BindableBase, IDisposable
    {
        private ObservableCollection<IPlugin> _plugins;
        private readonly IPluginManager _pluginManager;

        public ObservableCollection<IPlugin> Plugins
        {
            get { return _plugins; }
            set { SetProperty(ref _plugins, value); }
        }

        public ICommand StartPluginCommand { get; private set; }
        public ICommand StopPluginCommand { get; private set; }
        public ICommand ConfigurePluginCommand { get; private set; }

        public vmPluginManager(IPluginManager pluginManager)
        {
            _pluginManager = pluginManager;

            _plugins = new ObservableCollection<IPlugin>(_pluginManager.AllPlugins);
            RaisePropertyChanged(nameof(Plugins));

            StartPluginCommand = new DelegateCommand<IPlugin>(StartPlugin, CanStartPlugin);
            StopPluginCommand = new DelegateCommand<IPlugin>(StopPlugin, CanStopPlugin);
            ConfigurePluginCommand = new DelegateCommand<IPlugin>(ConfigurePlugin);
        }
        private bool CanStartPlugin(IPlugin plugin)
        {
            return plugin.Status != ePluginStatus.STARTED;
        }
        private bool CanStopPlugin(IPlugin plugin)
        {
            return plugin.Status != ePluginStatus.STOPPED;
        }
        private void StartPlugin(IPlugin plugin)
        {
            _pluginManager.StartPlugin(plugin);

            _plugins = new ObservableCollection<IPlugin>(_pluginManager.AllPlugins);
            // Notify of any property changes if needed
            RaisePropertyChanged(nameof(Plugins));
            // Refresh the CanExecute status
            (StartPluginCommand as DelegateCommand<IPlugin>).RaiseCanExecuteChanged();
            (StopPluginCommand as DelegateCommand<IPlugin>).RaiseCanExecuteChanged();
        }

        private void StopPlugin(IPlugin plugin)
        {
            _pluginManager.StopPlugin(plugin);
            _plugins = new ObservableCollection<IPlugin>(_pluginManager.AllPlugins);
            // Notify of any property changes if needed
            RaisePropertyChanged(nameof(Plugins));
            // Refresh the CanExecute status
            (StartPluginCommand as DelegateCommand<IPlugin>).RaiseCanExecuteChanged();
            (StopPluginCommand as DelegateCommand<IPlugin>).RaiseCanExecuteChanged();
        }

        private void ConfigurePlugin(IPlugin plugin)
        {
            _pluginManager.SettingPlugin(plugin);
        }

        public void Dispose()
        {
            // Any cleanup logic if needed
        }
    }

}
