using System;

namespace DrinkOBand.Core.Entities
{
    public class DrinkLogItem
    {
        public string Id { get; set; }
        public string UserId { get; set; }

        public DateTime Timestamp { get; set; }
        public int Amount { get; set; }
    }
}