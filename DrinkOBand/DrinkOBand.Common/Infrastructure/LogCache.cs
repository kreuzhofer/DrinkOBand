using System;
using System.Diagnostics;
using DrinkOBand.Core.Entities;
using DrinkOBand.Core.Infrastructure;

namespace DrinkOBand.Common
{
    public class LogCache : ILogCache
    {
        public async void Log(string message, LogCacheLogLevel logLevel)
        {
            var entry = new LogEntry()
            {
                Id = Guid.NewGuid(),
                EntryType = LogEntryType.Trace,
                Message = message,
                LogLevel = logLevel.ToString(),
                Timestamp = DateTime.Now
            };

            Debug.WriteLine(String.Format("LOG|{0}|{1}|{2}", entry.Timestamp, entry.LogLevel, entry.Message));
        }

        public async void Metric(string name, double value)
        {
            var entry = new LogEntry()
            {
                Id = Guid.NewGuid(),
                EntryType = LogEntryType.Metric,
                MetricName = name,
                MetricValue = value,
                Timestamp = DateTime.Now
            };
            Debug.WriteLine(String.Format("METRIC|{0}|{1}|{2}", entry.Timestamp, entry.MetricName, entry.MetricValue));
        }

        public async void Event(string name)
        {
            Debug.WriteLine("EVENT|{0}", name);
        }
    }
}
