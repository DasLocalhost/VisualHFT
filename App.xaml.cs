using Autofac;
using Autofac.Core;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using VisualHFT.Commons.NotificationManager;
using VisualHFT.Commons.PluginManager;
using VisualHFT.PluginManager;
using VisualHFT.UserSettings;
using VisualHFT.Notifications;
using VisualHFT.Notifications.Slack;
using VisualHFT.Notifications.Toast;
using VisualHFT.Notifications.Twitter;
using VisualHFT.Notifications.Zapier;

namespace VisualHFT
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static IContainer? _container;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            //Initialize logging
            log4net.Config.XmlConfigurator.Configure(new System.IO.FileInfo("log4net.config"));

            //Launch the GC cleanup thread
            Task.Run(async () => { await GCCleanupAsync(); });

            var builder = new ContainerBuilder();

            // Register setting manager
            builder.RegisterType<SettingsManager>()
                .As<ISettingsManager>()
                .SingleInstance()
                .AutoActivate();

            // Register plugin manager and load plugins
            builder.RegisterType<PluginManager.PluginManager>()
                .As<IPluginManager>()
                .SingleInstance()
                .OnActivating(_ => LoadPlugins(_.Instance))
                .AutoActivate();

            // Register notification behaviours
            builder.RegisterType<SlackNotificationBehaviour>()
                .As<INotificationBehaviour>()
                .OnActivating(_ => _.Instance.Initialize());

            builder.RegisterType<ToastNotificationBehaviour>()
                .As<INotificationBehaviour>()
                .OnActivating(_ => _.Instance.Initialize());

            builder.RegisterType<TwitterNotificationBehaviour>()
                .As<INotificationBehaviour>()
                .OnActivating(_ => _.Instance.Initialize());

            builder.RegisterType<ZapierNotificationBehaviour>()
                .As<INotificationBehaviour>()
                .OnActivating(_ => _.Instance.Initialize());

            // Register notification manager
            builder.RegisterType<NotificationManager>()
                .As<INotificationManager>()
                .SingleInstance()
                .OnActivated(_ => SetupNotifications(_.Instance))
                .AutoActivate();

            // Register the dashboard win
            builder.RegisterType<Dashboard>().AsSelf();

            // Build a DI container
            _container = builder.Build();

            // Start main window using DI container as a context
            using (var scope = _container.BeginLifetimeScope())
            {
                var mainwin = _container.Resolve<Dashboard>();
                mainwin.Show();
            }
        }

        private void LoadPlugins(IPluginManager pluginManager)
        {
            try
            {
                pluginManager.Initialize();
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

        private void SetupNotifications(INotificationManager notificationManager)
        {
            // TODO : add logs here
            notificationManager.Initialize();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            if (_container != null)
            {
                INotificationManager? notificationMgr;
                if (_container.TryResolve(out notificationMgr))
                {
                    // TODO : Stop notifications here
                    //notificationMgr
                }

                IPluginManager? pluginMgr;
                if (_container.TryResolve(out pluginMgr))
                {
                    pluginMgr.UnloadPlugins();
                }
            }

            base.OnExit(e);
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
