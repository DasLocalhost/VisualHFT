using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using VisualHFT.PluginManager;

namespace VisualHFT
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            //Initialize logging
            log4net.Config.XmlConfigurator.Configure(new System.IO.FileInfo("log4net.config"));

            //Launch the GC cleanup thread
            Task.Run(async () => { await GCCleanupAsync(); });

            // Load Plugins
            Task.Run(LoadPlugins)
                .ContinueWith(_ => { SetupNotifications(); });
        }

        protected override void OnExit(ExitEventArgs e)
        {
            PluginManager.PluginManager.Instance?.UnloadPlugins();
            base.OnExit(e);
        }

        private async Task LoadPlugins()
        {
            try
            {
                PluginManager.PluginManager.Init();
            }
            catch (Exception ex)
            {
                // Handle the exception
                Application.Current.Dispatcher.Invoke(() =>
                {
                    MessageBox.Show("ERROR LOADING Plugins: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                });
            }
        }

        private void SetupNotifications()
        {
            // TODO : add logs here
            if (PluginManager.PluginManager.Instance == null || PluginManager.PluginManager.Instance?.AllPlugins == null)
                return;

            NotificationManager.NotificationManager.Init(PluginManager.PluginManager.Instance.AllPlugins);
        }

        private async Task GCCleanupAsync()
        {
            //due to the high volume of data do this periodically.(this will get fired every 5 secs)

            while (true)
            {
                await Task.Delay(5000);
                GC.Collect(); //force garbage collection
            };

        }
    }
}
