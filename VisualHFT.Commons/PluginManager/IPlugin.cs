using VisualHFT.UserSettings;

namespace VisualHFT.PluginManager
{
    public interface IPlugin
    {
        string Name { get; set; }
        string Version { get; set; }
        string Description { get; set; }
        string Author { get; set; }
        ISetting Settings { get; set; }
        ePluginStatus Status { get; set; }
        Action CloseSettingWindow { get; set; }

        event EventHandler<ErrorEventArgs> OnError;

        string GetPluginUniqueID();
    }
    public class ErrorEventArgs : EventArgs
    {
        public Exception Exception { get; set; }
        public string PluginName { get; set; }
        public bool IsCritical { get; set; }
    }

}
