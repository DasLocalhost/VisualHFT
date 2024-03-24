using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using VisualHFT.Commons.NotificationManager;
using VisualHFT.Commons.PluginManager;
using VisualHFT.Commons.WPF.ViewModel;
using VisualHFT.Helpers;
using VisualHFT.PluginManager;
using VisualHFT.UserSettings;

namespace VisualHFT.ViewModel.Notification
{
    /// <summary>
    /// Change notifications settings (both in settings and notifications managers
    /// </summary>
    public class NotificationsViewModel : IModularViewModel
    {
        private readonly IPluginManager _pluginManager;
        private readonly ISettingsManager _settingsManager;
        private readonly INotificationManager _notificationManager;

        #region IModularViewModel implementation

        public ICommand OkCommand { get; private set; }
        public ICommand CancelCommand { get; private set; }
        public ICommand ResumeCommand { get; private set; }
        public ICommand PauseCommand { get; private set; }

        public event EventHandler? OnClose;

        #endregion

        public ObservableCollection<NotificationsInfo> Notifications { get; private set; }

        public NotificationsViewModel(IPluginManager pluginManager, ISettingsManager settingsManager, INotificationManager notificationManager)
        {
            _pluginManager = pluginManager;
            _settingsManager = settingsManager;
            _notificationManager = notificationManager;

            OkCommand = new RelayCommand<object>(Save);
            CancelCommand = new RelayCommand<object>(Cancel);

            ResumeCommand = new RelayCommand<NotificationsInfo>(Resume);
            PauseCommand = new RelayCommand<NotificationsInfo>(Pause);

            Notifications = new ObservableCollection<NotificationsInfo>();

            UpdateValues();
        }

        private void Save(object obj)
        {
            foreach (var notification in Notifications)
            {
                if (notification.HasChanged())
                    notification.SaveChanges();
            }

            OnClose?.Invoke(this, EventArgs.Empty);
        }

        private void Cancel(object obj)
        {
            OnClose?.Invoke(this, EventArgs.Empty);
        }

        private void Resume(NotificationsInfo info)
        {
            info.IsActive = true;
        }

        private void Pause(NotificationsInfo info)
        {
            info.IsActive = false;
        }

        private void UpdateValues()
        {
            Notifications.Clear();

            if (_settingsManager.UserSettings == null)
                return;

            var pluginNotificationSettings = _settingsManager.UserSettings.GetPluginsRelatedNotificationSettings();

            if (pluginNotificationSettings == null)
                return;

            foreach (var plugSettingContainer in pluginNotificationSettings)
            {
                var plugSetting = plugSettingContainer.Setting;

                Notifications.Add(new NotificationsInfo(plugSetting)
                {
                    PluginName = _pluginManager.GetPluginName(plugSetting.PluginId),
                    TargetType = plugSetting.ParentSettings.ShortTargetName
                });
            }
        }
    }

    public class NotificationsInfo : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged implementation

        public event PropertyChangedEventHandler? PropertyChanged;

        #endregion

        private bool isActive;

        public IPluginNotificationSettings Setting { get; private set; }

        public string? PluginName { get; set; }
        public string? TargetType { get; set; }

        public double? Threshold { get; set; }
        public ThresholdRule ThresholdRule { get; set; }

        public bool IsActive
        {
            get => isActive;
            set
            {
                if (isActive != value)
                {
                    isActive = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsActive)));
                }
            }
        }

        public NotificationsInfo(IPluginNotificationSettings settings)
        {
            Setting = settings;

            PluginName = settings.PluginId;
            TargetType = settings.SettingId;
            Threshold = settings.Threshold ?? 0;
            ThresholdRule = settings.ThresholdRule;
            IsActive = settings.IsEnabled;
        }

        public bool HasChanged()
        {
            if (IsActive != Setting.IsEnabled)
                return true;

            if (Threshold != Setting.Threshold)
                return true;

            if (ThresholdRule != Setting.ThresholdRule)
                return true;

            return false;
        }

        public void SaveChanges()
        {
            Setting.UpdateInSource();
        }
    }
}
