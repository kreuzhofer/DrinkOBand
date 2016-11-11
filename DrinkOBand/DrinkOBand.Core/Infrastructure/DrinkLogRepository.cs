using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Acr.UserDialogs;
using DrinkOBand.Core.Entities;
using DrinkOBand.Core.Helpers;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.SQLiteStore;
using Microsoft.WindowsAzure.MobileServices.Sync;

namespace DrinkOBand.Core.Infrastructure
{
    public class DrinkLogRepository : IDrinkLogRepository
    {
        private IMobileServiceSyncTable<DrinkLogItem> drinkLogTable;
        private ISettingsStore _settingsStore;
        private IDrinkLogCache _drinkLogCache;
        private MobileServiceClientManager _clientManager;

        public DrinkLogRepository(MobileServiceClientManager clientManager, ISettingsStore settingsStore, IDrinkLogCache drinkLogCache)
        {
            _settingsStore = settingsStore;
            _drinkLogCache = drinkLogCache;
            _clientManager = clientManager;

            drinkLogTable = _clientManager.MobileService.GetSyncTable<DrinkLogItem>(); // offline sync
        }

        public async Task SyncCacheEntriesAsync()
        {
            var cache = _drinkLogCache.GetCache();
            var oldItems = cache;
            foreach (var item in oldItems)
            {
                if ((await drinkLogTable.Where(i => i.Timestamp == item.Timestamp).ToListAsync()).Any())
                {
                    continue;
                }
                if (item.Id == null)
                {
                    await drinkLogTable.InsertAsync(item);
                }
            }

            // get all items from db again, just in case there was an app reinstall or something was added on the server side
            cache = (await drinkLogTable.Where(i => i.Timestamp >= DateTime.Today).ToListAsync()).Where( 
                    i => i.Timestamp.Date == DateTime.Today.Date).ToList();

            _drinkLogCache.SetCache(cache);
        }

        public async Task<List<DrinkLogItem>> GetAllEntriesAsync()
        {
            return await drinkLogTable.ToListAsync();
        }

        public async Task<int[]> GetLastWeeksAverageAndMax()
        {
            var MondayThisWeek = DateTime.Today.AddDays(1 - (int)DateTime.Today.DayOfWeek);
            if (MondayThisWeek > DateTime.Today)
            {
                MondayThisWeek = MondayThisWeek.AddDays(-7);
            }
            var MondayLastWeek = DateTime.Today.AddDays(-6 - (int)DateTime.Today.DayOfWeek);
            if (MondayLastWeek >= MondayThisWeek)
            {
                MondayLastWeek = MondayLastWeek.AddDays(-7);
            }
            var entriesSinceLastWeek = await drinkLogTable.Where(i => i.Timestamp >= MondayLastWeek).ToListAsync();
            var lastWeeksSums = new List<int>();
            var currentDay = MondayLastWeek;
            for (int i = 0; i < 7; i++)
            {
                var sumOfDay = entriesSinceLastWeek.Where(d => d.Timestamp.Date == currentDay.Date).Sum(v => v.Amount);
                if (sumOfDay > 0)
                {
                    lastWeeksSums.Add(sumOfDay);
                }
                currentDay = currentDay.AddDays(1);
            }
            var average = lastWeeksSums.Count > 0 ? lastWeeksSums.Average() : 0;
            var max = lastWeeksSums.Count > 0 ? lastWeeksSums.Max() : 0;

            _settingsStore.LastWeeksAverage = (int)average;
            _settingsStore.LastWeeksMax = max;

            return new[] {(int)average, (int)max};
        }

        public async Task<int[]> GetThisWeeksAverageAndMax()
        {
            var MondayThisWeek = DateTime.Today.AddDays(1 - (int)DateTime.Today.DayOfWeek);
            if (MondayThisWeek > DateTime.Today)
            {
                MondayThisWeek = MondayThisWeek.AddDays(-7);
            }
            var entriesSinceLastWeek = await drinkLogTable.Where(i => i.Timestamp >= MondayThisWeek).ToListAsync();
            var lastWeeksSums = new List<int>();
            var currentDay = MondayThisWeek;
            for (int i = 0; i < 7; i++)
            {
                var sumOfDay = entriesSinceLastWeek.Where(d => d.Timestamp.Date == currentDay.Date).Sum(v => v.Amount);
                if (sumOfDay > 0)
                {
                    lastWeeksSums.Add(sumOfDay);
                }
                currentDay = currentDay.AddDays(1);
            }
            var average = lastWeeksSums.Count > 0 ? lastWeeksSums.Average() : 0;
            var max = lastWeeksSums.Count > 0 ? lastWeeksSums.Max() : 0;

            _settingsStore.ThisWeeksAverage = (int)average;
            _settingsStore.ThisWeeksMax = max;

            return new[] { (int)average, (int)max };
        }

        public async Task DeleteEntriesOfToday()
        {
            var allEntries = await drinkLogTable.ToListAsync();
            var entriesOfToday = allEntries.Where(e => e.Timestamp.Date == DateTime.Today);
            foreach (var drinkLogItem in entriesOfToday)
            {
                await drinkLogTable.DeleteAsync(drinkLogItem);
            }
            await _clientManager.MobileService.SyncContext.PushAsync();
        }

        public async Task<Dictionary<string, int>> GetThisWeeksAmounts()
        {
            var MondayThisWeek = DateTime.Today.AddDays(1-(int)DateTime.Today.DayOfWeek);
            if (MondayThisWeek > DateTime.Today)
            {
                MondayThisWeek = MondayThisWeek.AddDays(-7);
            }
            var entriesSinceMonday = await drinkLogTable.Where(i => i.Timestamp >= MondayThisWeek).ToListAsync();
            var lastWeeksSums = new Dictionary<string, int>(); ;
            var currentDay = MondayThisWeek;
            for (int i = 0; i < 7; i++)
            {
                var sumOfDay = entriesSinceMonday.Where(d => d.Timestamp.Date == currentDay.Date).Sum(v => v.Amount);
                lastWeeksSums.Add(currentDay.ToString("dddd"), sumOfDay);
                currentDay = currentDay.AddDays(1);
            }
            return lastWeeksSums;
        }

        public async Task<Dictionary<string,int>> GetLastWeeksAmounts()
        {
            var MondayThisWeek = DateTime.Today.AddDays(1 - (int)DateTime.Today.DayOfWeek);
            if (MondayThisWeek > DateTime.Today)
            {
                MondayThisWeek = MondayThisWeek.AddDays(-7);
            }
            var MondayLastWeek = DateTime.Today.AddDays(-6 - (int)DateTime.Today.DayOfWeek);
            if (MondayLastWeek >= MondayThisWeek)
            {
                MondayLastWeek = MondayLastWeek.AddDays(-7);
            }
            var entriesForLastWeek = await drinkLogTable.Where(i => i.Timestamp >= MondayLastWeek).ToListAsync();
            var lastWeeksSums = new Dictionary<string, int>();;
            var currentDay = MondayLastWeek;
            for (int i = 0; i < 7; i++)
            {
                var sumOfDay = entriesForLastWeek.Where(d => d.Timestamp.Date == currentDay.Date).Sum(v => v.Amount);
                lastWeeksSums.Add(currentDay.ToString("dddd"), sumOfDay);
                currentDay = currentDay.AddDays(1);
            }
            return lastWeeksSums;
        }

        public async Task InitLocalStoreAsync()
        {
            if (!_clientManager.MobileService.SyncContext.IsInitialized)
            {
                var store = new MobileServiceSQLiteStore("localstore.db");
                store.DefineTable<DrinkLogItem>();
                await _clientManager.MobileService.SyncContext.InitializeAsync(store);
            }
        }

        public async Task SyncAsync(string user)
        {
            String errorString = null;

            try
            {
                if ((await drinkLogTable.Where(i => i.UserId.StartsWith("Microsoft")).ToListAsync()).Count > 0)
                {
                    var items = await GetAllEntriesAsync();
                    foreach (var drinkLogItem in items)
                    {
                        drinkLogItem.UserId = user;
                        await drinkLogTable.UpdateAsync(drinkLogItem);
                    }
                }
                await _clientManager.MobileService.SyncContext.PushAsync();
            }
            catch (MobileServicePushFailedException ex)
            {
                foreach (var mobileServiceTableOperationError in ex.PushResult.Errors)
                {
                    if (mobileServiceTableOperationError.Status != null && mobileServiceTableOperationError.Status == HttpStatusCode.Conflict)
                    {
                        await mobileServiceTableOperationError.CancelAndUpdateItemAsync(mobileServiceTableOperationError.Item);
                    }
                }
                if (ex.PushResult.Errors.Any(e => !e.Handled))
                {
                    errorString = "Push failed because of sync errors: " +
                      ex.PushResult.Errors.Count + " errors, message: " + ex.Message;
                }
            }
            if (errorString != null)
            {
                await UserDialogs.Instance.AlertAsync(errorString);
            }

            try
            {
                errorString = null;
                await drinkLogTable.PullAsync("drinkLogItems", drinkLogTable.CreateQuery());
#if DEBUG
                var entries = await drinkLogTable.ToListAsync();
                foreach (var drinkLogItem in entries)
                {
                    Debug.WriteLine(drinkLogItem.Timestamp.ToString() + ":" + drinkLogItem.Amount);
                }
#endif
            }
            catch (Exception ex)
            {
                errorString = "Pull failed: " + ex.Message +
                  "\n\nIf you are still in an offline scenario, " +
                  "you can try your Pull again when connected with your Mobile Serice.";
            }
            if (errorString != null)
            {
                await UserDialogs.Instance.AlertAsync(errorString);
            }

        }
    }
}