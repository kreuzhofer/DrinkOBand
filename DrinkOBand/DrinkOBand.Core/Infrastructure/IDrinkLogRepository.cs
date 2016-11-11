using System.Collections.Generic;
using System.Threading.Tasks;
using DrinkOBand.Core.Entities;

namespace DrinkOBand.Core.Infrastructure
{
    public interface IDrinkLogRepository
    {
        Task SyncCacheEntriesAsync();

        Task<List<DrinkLogItem>> GetAllEntriesAsync();
        Task InitLocalStoreAsync();
        Task SyncAsync(string user);
        Task<int[]> GetLastWeeksAverageAndMax();
        Task<Dictionary<string,int>> GetLastWeeksAmounts();
        Task<Dictionary<string, int>> GetThisWeeksAmounts();
        Task<int[]> GetThisWeeksAverageAndMax();
        Task DeleteEntriesOfToday();
    }
}