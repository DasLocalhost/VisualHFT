using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisualHFT.Commons.NotificationManager;
using VisualHFT.Commons.PluginManager;
using VisualHFT.UserSettings;
using VisualHFT.Model;
using VisualHFT.Notifications;
using VisualHFT.Test.Mockups;

namespace VisualHFT.Test
{
    public class NotificationsTest
    {
        private IContainer? _container;

        [SetUp]
        public void Setup()
        {
            var builder = new ContainerBuilder();

            // Register setting manager
            builder.RegisterType<SettingsManagerMockup>()
                .As<ISettingsManager>()
                .SingleInstance()
                .AutoActivate();

            // Register plugin manager and load plugins
            builder.RegisterType<PluginManagerMockup>()
                .As<IPluginManager>()
                .SingleInstance()
                .OnActivated(_ => _.Instance.Initialize())
                .AutoActivate();

            // Register notification manager
            builder.RegisterType<NotificationManager>()
                .As<INotificationManager>()
                .SingleInstance()
                .OnActivated(_ => _.Instance.Initialize())
                .AutoActivate();

            // Build a DI container
            _container = builder.Build();
        }

    }
}