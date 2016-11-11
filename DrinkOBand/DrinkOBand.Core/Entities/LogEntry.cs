using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrinkOBand.Core.Entities
{
    public class LogEntry
    {
        public Guid Id { get; set; }
        public LogEntryType EntryType { get; set; }
        public string MetricName { get; set; }
        public double MetricValue { get; set; }
        public string EventName { get; set; }
        public string Message { get; set; }
        public string LogLevel { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
