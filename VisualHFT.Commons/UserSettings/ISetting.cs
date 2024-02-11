using VisualHFT.Model;

namespace VisualHFT.UserSettings
{
    /// <summary>
    /// Settings container for plugins
    /// </summary>
    public interface ISetting : IBaseSettings
    {
        string Symbol { get; set; }
        Provider Provider { get; set; }
        AggregationLevel AggregationLevel { get; set; }
    }

}
