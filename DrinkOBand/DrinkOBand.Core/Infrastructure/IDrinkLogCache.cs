using System.Collections.Generic;
using DrinkOBand.Core.Entities;

namespace DrinkOBand.Core.Infrastructure
{
    public interface IDrinkLogCache
    {
        List<DrinkLogItem> GetCache();
        void AddToCache(DrinkLogItem item);
        void SetCache(List<DrinkLogItem> cache);
        int GetTodaysAmount();
    }
}