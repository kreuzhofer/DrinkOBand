using System;
using System.Collections.Generic;
using System.Linq;
using DrinkOBand.Core.Entities;
using Newtonsoft.Json;
using Plugin.Settings.Abstractions;

namespace DrinkOBand.Core.Infrastructure
{
    public class DrinkLogCache : IDrinkLogCache
    {
        private ISettings _settings;

        public DrinkLogCache(ISettings settings)
        {
            _settings = settings;
        }

        public List<DrinkLogItem> GetCache()
        {
            List<DrinkLogItem> cache = null;
            string cacheValue = _settings.GetValueOrDefault<string>(Globals.Setting_DrinkEventCache, null);
            
            if (cacheValue == null)
            {
                cache = new List<DrinkLogItem>();
                _settings.AddOrUpdateValue(Globals.Setting_DrinkEventCache, JsonConvert.SerializeObject(cache));
            }
            else
            {
                cache = JsonConvert.DeserializeObject<List<DrinkLogItem>>(_settings.GetValueOrDefault<string>(Globals.Setting_DrinkEventCache, null));
            }
            return cache;
        }

        public void AddToCache(DrinkLogItem item)
        {
            var cache = GetCache();
            cache.Add(item);
            SetCache(cache);
            Resolver.Resolve<ILogCache>().Event("Amount"+item.Amount);
        }

        public void SetCache(List<DrinkLogItem> cache)
        {
            _settings.AddOrUpdateValue(Globals.Setting_DrinkEventCache, JsonConvert.SerializeObject(cache));
        }

        public int GetTodaysAmount()
        {
            return GetCache().Where(d => d.Timestamp.Date == DateTime.Today).Sum(s => s.Amount);
        }

    }
}