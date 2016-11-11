namespace DrinkOBand.Core.Infrastructure
{
    public interface ILogCache
    {
        void Log(string message, LogCacheLogLevel logLevel);
        void Metric(string name, double value);
        void Event(string name);
    }
}