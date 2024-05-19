using log4net.Plugin;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using System.Windows.Data;
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
    /// Change notifications settings (both in settings and notifications managers)
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

        public ObservableCollection<PluginInfo> Plugins { get; private set; }
        public ICollectionView? PluginsView { get; private set; }

        public NotificationsViewModel(IPluginManager pluginManager, ISettingsManager settingsManager, INotificationManager notificationManager)
        {
            _pluginManager = pluginManager;
            _settingsManager = settingsManager;
            _notificationManager = notificationManager;

            OkCommand = new RelayCommand<object>(Save);
            CancelCommand = new RelayCommand<object>(Cancel);

            ResumeCommand = new RelayCommand<NotificationsInfo>(Resume);
            PauseCommand = new RelayCommand<NotificationsInfo>(Pause);

            Plugins = new ObservableCollection<PluginInfo>();

            UpdateValues();
        }

        private void Save(object obj)
        {
            foreach (var notification in Plugins.SelectMany(_ => _.Notifications))
            {
                if (notification.HasChanged())
                    notification.SaveChanges();
            }

            _settingsManager.Save();

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
            Plugins.Clear();

            if (_settingsManager.UserSettings == null)
                return;

            var pluginNotificationSettings = _settingsManager.UserSettings.GetPluginsRelatedNotificationSettings();

            if (pluginNotificationSettings == null)
                return;

            foreach (var container in pluginNotificationSettings.GroupBy(_ => _.Setting.PluginId))
            {
                var pluginName = _pluginManager.GetPluginName(container.Key) ?? container.Key ?? "NO_PLUGIN_NAME";
                var isStudy = _pluginManager.IsPluginStudy(container.Key);

                Plugins.Add(new PluginInfo(pluginName, isStudy, container.Select(_ => _.Setting).ToList()));
            }

            PluginsView = CollectionViewSource.GetDefaultView(Plugins);

            using (PluginsView.DeferRefresh())
            {
                PluginsView.SortDescriptions.Clear();

                PluginsView.SortDescriptions.Add(new SortDescription("IsStudy", ListSortDirection.Ascending));
                PluginsView.SortDescriptions.Add(new SortDescription("PluginName", ListSortDirection.Ascending));
            }
        }
    }

    public class PluginInfo : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged implementation

        public event PropertyChangedEventHandler? PropertyChanged;

        #endregion

        public string PluginName { get; private set; }
        public bool? IsStudy { get; private set; }

        public ObservableCollection<NotificationsInfo> Notifications { get; set; }

        public PluginInfo(string name, bool? isStudy, IList<IPluginNotificationSettings> notifications)
        {
            PluginName = name;
            IsStudy = isStudy;

            Notifications = new ObservableCollection<NotificationsInfo>(notifications.Select(_ => new NotificationsInfo(_)));
        }
    }

    public class NotificationsInfo : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged implementation

        public event PropertyChangedEventHandler? PropertyChanged;

        #endregion

        private bool isActive;

        public IPluginNotificationSettings Setting { get; private set; }

        public string? TargetType { get; set; }

        public double? AboveThreshold { get; set; }
        public bool? AboveThresholdEnabled { get; set; }

        public double? BelowThreshold { get; set; }
        public bool? BelowThresholdEnabled { get; set; }

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

            //PluginName = settings.PluginId;
            TargetType = settings.ParentSettings.ShortTargetName;
            
            AboveThreshold = settings.AboveThreshold;
            AboveThresholdEnabled = settings.AboveThresholdEnabled;

            BelowThreshold = settings.BelowThreshold;
            BelowThresholdEnabled = settings.BelowThresholdEnabled;

            IsActive = settings.IsEnabled;
        }

        public bool HasChanged()
        {
            if (IsActive != Setting.IsEnabled)
                return true;

            if (AboveThreshold != Setting.AboveThreshold)
                return true;

            if (AboveThresholdEnabled != Setting.AboveThresholdEnabled)
                return true;

            if (BelowThreshold != Setting.BelowThreshold)
                return true;

            if (BelowThresholdEnabled != Setting.BelowThresholdEnabled)
                return true;

            return false;
        }

        public void SaveChanges()
        {
            Setting.IsEnabled = isActive;

            Setting.AboveThreshold = AboveThreshold;
            Setting.AboveThresholdEnabled = AboveThresholdEnabled ?? false;
            Setting.BelowThreshold = BelowThreshold;
            Setting.BelowThresholdEnabled = BelowThresholdEnabled ?? false;

            Setting.UpdateInSource();
        }
    }
}
