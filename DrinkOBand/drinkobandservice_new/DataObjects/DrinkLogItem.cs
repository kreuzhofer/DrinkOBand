using System;
using Microsoft.Azure.Mobile.Server;

namespace drinkobandserviceService.DataObjects
{
    public class DrinkLogItem : EntityData
    {
        public string UserId { get; set; }
        public string ProviderUserId { get; set; }

        public DateTime Timestamp { get; set; }
        public int Amount { get; set; }
    }
}