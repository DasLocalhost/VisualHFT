using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VisualHFT.Commons.NotificationManager;
using VisualHFT.UserSettings;

namespace VisualHFT.NotificationManager
{
    /// <summary>
    /// Balancer that is controlling how often notifications are raised to target system (like Twitter, Win Toast etc)
    /// </summary>
    public class NotificationBalancer : IDisposable
    {
        #region Fields

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly string _behaviourId;
        private readonly BaseNotificationSettings _settings;
        private readonly ISettingsManager _settingsManager;
        private ConcurrentQueue<INotification> _notificationsQueue = new ConcurrentQueue<INotification>();

        private Thread _processingThread;
        private CancellationTokenSource _tokenSource;
        private CancellationToken _token;

        private ManualResetEvent _pauseToken;

        private ProcessingStatus _status;

        private int _dequeueChunkSize = 1;
        private int _dequeueWaitInterwal = 100;

        private readonly int _retryOnError = 5;

        #endregion

        /// <summary>
        /// Current status of the processing unit.
        /// </summary>
        public ProcessingStatus Status => _status;

        /// <summary>
        /// Unique id of the notification behaviour related to this processor.
        /// </summary>
        public string BehaviourId => _behaviourId;

        /// <summary>
        /// Event rised when notifications should be sent to the target system.
        /// </summary>
        public event EventHandler<IList<INotification>>? OnDequeue;

        public NotificationBalancer(BaseNotificationSettings settings, ISettingsManager settingsManager)
        {
            _status = ProcessingStatus.NotInit;

            _pauseToken = new ManualResetEvent(true);
            _behaviourId = settings.BehaviourId;

            _settings = settings;
            _settingsManager = settingsManager;

            _dequeueChunkSize = _settings.Threshold;
            _dequeueWaitInterwal = _settings.UpdateTime;

            _settingsManager.SettingsChanged += SettingsChanged;
        }

        /// <summary>
        /// Update values after settings changed.
        /// </summary>
        private void SettingsChanged(object? sender, IBaseSettings setting)
        {
            if (setting != _settings)
                return;

            _dequeueChunkSize = _settings.Threshold;
            _dequeueWaitInterwal = _settings.UpdateTime;
        }

        /// <summary>
        /// Enqueue notification to be processed.
        /// </summary>
        /// <param name="notification">Standard notification</param>
        public void Enqueue(INotification notification)
        {
            _notificationsQueue.Enqueue(notification);
        }

        #region Control methods

        /// <summary>
        /// Start processing.
        /// </summary>
        /// <exception cref="ProcessingWrongStatusException">Raise if operation cannot be done because of current processing status</exception>
        public void Start()
        {
            if (_status != ProcessingStatus.NotInit)
                throw new ProcessingWrongStatusException(_status, ProcessingStatus.NotInit);

            try
            {
                _processingThread = new Thread(ProcessNotifications);
                _tokenSource = new CancellationTokenSource();
                _token = _tokenSource.Token;

                _processingThread.SetApartmentState(ApartmentState.STA);
                _processingThread.Start();

                _status = ProcessingStatus.Running;
            }
            catch (Exception ex)
            {
                _status = ProcessingStatus.NotInit;
                throw new BalancerStatusChangedFailed(_settings.TargetName, ProcessingStatus.Running, ex);
            }
        }

        /// <summary>
        /// Pause processing.
        /// </summary>
        /// <exception cref="ProcessingWrongStatusException">Raise if operation cannot be done because of current processing status</exception>
        public void Pause()
        {
            if (_status != ProcessingStatus.Running)
                throw new ProcessingWrongStatusException(_status, ProcessingStatus.Running);

            try
            {
                _pauseToken?.Reset();

                _status = ProcessingStatus.Paused;
            }
            catch (Exception ex)
            {
                _status = ProcessingStatus.Running;
                throw new BalancerStatusChangedFailed(_settings.TargetName, ProcessingStatus.Paused, ex);
            }
        }

        /// <summary>
        /// Resume processing.
        /// </summary>
        /// <exception cref="ProcessingWrongStatusException">Raise if operation cannot be done because of current processing status</exception>
        public void Resume()
        {
            if (_status != ProcessingStatus.Paused)
                throw new ProcessingWrongStatusException(_status, ProcessingStatus.Paused);

            try
            {
                _pauseToken?.Set();

                _status = ProcessingStatus.Running;
            }
            catch (Exception ex)
            {
                _status = ProcessingStatus.Paused;
                throw new BalancerStatusChangedFailed(_settings.TargetName, ProcessingStatus.Running, ex);
            }
        }

        /// <summary>
        /// Stop processing and clean all resources.
        /// </summary>
        public void Dispose()
        {
            // Unsubscribe from event
            _settingsManager.SettingsChanged -= SettingsChanged;

            // Stop prcessing thread if not null
            if (_processingThread != null)
                _tokenSource.Cancel();

            _status = ProcessingStatus.Disposed;
        }

        #endregion

        /// <summary>
        /// Main processing method.
        /// </summary>
        /// <param name="obj"></param>
        private void ProcessNotifications(object? obj)
        {
            try
            {
                int _errorsCount = 0;

                while (true)
                {
                    try
                    {
                        // Check for pause or cancelation
                        _pauseToken.WaitOne();
                        if (_token.IsCancellationRequested)
                            break;

                        // If any notifications are ready to be processed - dequeu and send them back to notifications manager
                        var notifications = _notificationsQueue.TryDequeueChunk(_dequeueChunkSize).ToList();

                        if (notifications != null && notifications.Count > 0)
                            OnDequeue?.Invoke(this, notifications);

                        // Wait some time before processing the next chunk of notification
                        Thread.Sleep(_dequeueWaitInterwal);

                        // Drop attempts count
                        _errorsCount = 0;
                    }
                    catch (OperationCanceledException)
                    {
                        throw;
                    }
                    catch (Exception ex)
                    {
                        _errorsCount++;
                        log.Warn($"Notifications: [{_settings.TargetName}] balancer failed to process notifications chunk (attempt {_errorsCount} from {_retryOnError}).", ex);

                        // If any unhandled exceptions - try again 5 times and than stop processing.
                        if (_errorsCount >= _retryOnError)
                            throw;
                    }
                };

                // This code is unreacheble in standard case 
                log.Warn($"Notifications: [{_settings.TargetName}] balancer suddenly stopped.");
                _status = ProcessingStatus.Paused;
            }
            catch (OperationCanceledException)
            {
                log.Warn($"Notifications: [{_settings.TargetName}] balancer stopped.");
            }
            catch (Exception ex)
            {
                log.Error($"Notifications: [{_settings.TargetName}] balancer stopped with error.", ex);
                _status = ProcessingStatus.Paused;
            }
        }
    }

    /// <summary>
    /// The exception that is thrown when trying to run a method with incorrect processing status.
    /// </summary>
    public class ProcessingWrongStatusException : Exception
    {
        public ProcessingWrongStatusException(ProcessingStatus status, ProcessingStatus expectingStatus, [CallerMemberName] string methodName = "")
            : base($"{status} is an incorrect processing status for {methodName} method, expecting {expectingStatus} status.") { }
    }

    /// <summary>
    /// The exception that is thrown when switch to another balancer status failed.
    /// </summary>
    public class BalancerStatusChangedFailed : Exception
    {
        public BalancerStatusChangedFailed(string? targetName, ProcessingStatus status, Exception? innerException = null)
            : base($"Failed to switch balancer to {status} status for [{targetName}] behavior.", innerException) { }
    }

    /// <summary>
    /// Status of the processing unit.
    /// </summary>
    public enum ProcessingStatus
    {
        /// <summary>
        /// Processing unit is not initialized yet.
        /// </summary>
        NotInit,
        /// <summary>
        /// Processing is running in normal way.
        /// </summary>
        Running,
        /// <summary>
        /// Processing is paused, but could be resumed.
        /// </summary>
        Paused,
        /// <summary>
        /// Processing unit is disposed, processing could not be resumed.
        /// </summary>
        Disposed
    }
}