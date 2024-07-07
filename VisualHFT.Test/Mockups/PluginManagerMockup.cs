using QuickFix.Fields;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using VisualHFT.Commons.NotificationManager;
using VisualHFT.Commons.NotificationManager.Notifications;
using VisualHFT.Commons.PluginManager;
using VisualHFT.PluginManager;
using VisualHFT.UserSettings;

namespace VisualHFT.Test.Mockups
{
    public class PluginManagerMockup : IPluginManager
    {
        #region Fields

        private readonly ISettingsManager _settingsManager;

        #endregion

        public List<IPlugin> AllPlugins { get; set; }

        public bool AllPluginsReloaded { get; set; } = true;

        public PluginManagerMockup(ISettingsManager settingsManager)
        {
            _settingsManager = settingsManager;
        }

        public void Initialize()
        {
            AllPlugins = new List<IPlugin>
            {
                new PluginMockup()
            };
        }

        public void RaiseNotifications(int count = 1, string subject = "test subj", string text = "test text")
        {
            foreach (var plugin in AllPlugins.OfType<PluginMockup>())
            {
                plugin.RaiseNotifications(count, subject, text);
            }
        }

        public void SettingPlugin(IPlugin plugin)
        {
            throw new NotImplementedException();
        }

        public void StartPlugin(IPlugin plugin)
        {
            throw new NotImplementedException();
        }

        public void StopPlugin(IPlugin plugin)
        {
            throw new NotImplementedException();
        }

        public void UnloadPlugins()
        {
            throw new NotImplementedException();
        }
    }

    public class PluginMockup : IPlugin, INotificationSource
    {
        public string Name { get; set; }
        public string Version { get; set; }
        public string Description { get; set; }
        public string Author { get; set; }
        public ISetting Settings { get; set; }
        public ePluginStatus Status { get; set; }
        public Action CloseSettingWindow { get; set; }

        public event EventHandler<ErrorEventArgs> OnError;
        public event EventHandler<INotification> OnNotificationRaised;

        public PluginMockup()
        {
            Name = "Test Plugin";
            Version = "0.0.0.1";
            Description = "Mockedup plugin for tests purpose";
            Author = "Dev";
        }

        public void RaiseNotifications(int count = 1, string subject = "test subj", string text = "test text")
        {
            for (int i = 0; i < count; i++)
            {
                var notification = new TextNotification(subject, $"{text} [{i}]", i)
                    .FromPlugin("VPINStudy", GetPluginUniqueID())
                    .SetConcatenation(Concatenation.Simple);

                OnNotificationRaised?.Invoke(this, notification);
            }
        }

        public string GetPluginUniqueID()
        {
            // Get the fully qualified name of the assembly
            string assemblyName = GetType().Assembly.FullName;

            // Concatenate the attributes
            string combinedAttributes = $"{Name}{Author}{Version}{Description}{assemblyName}";

            // Compute the SHA256 hash
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(combinedAttributes));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}