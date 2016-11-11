using System;
using Windows.UI.Notifications;
using DrinkOBand.Core.Helpers;
using DrinkOBand.Core.Infrastructure;

namespace DrinkOBand.Common.Infrastructure
{
    public class LiveTileUpdater : ILiveTileUpdater
    {
        private IResourceRepository _resourceRepository;
        private ISettingsStore _settingsStore;
        private IUnitHelper _unitHelper;
        private ILogCache _log;
        private IDrinkLogCache _drinkLogCache;

        public LiveTileUpdater(IResourceRepository resourceRepository, ISettingsStore settingsStore, IUnitHelper unitHelper, 
            ILogCache logCache, IDrinkLogCache drinkLogCache)
        {
            _resourceRepository = resourceRepository;
            _settingsStore = settingsStore;
            _unitHelper = unitHelper;
            _log = logCache;
            _drinkLogCache = drinkLogCache;
        }

        public void UpdateLiveTile()
        {
            try
            {
                var todaysAmount = _drinkLogCache.GetTodaysAmount();
                // set livetile
                var tileTemplate = NotificationsExtensions.TileContent.TileContentFactory.CreateTileSquare150x150Text01();
                tileTemplate.TextHeading.Text = _resourceRepository.GetString("msgTodaysProgress");
                tileTemplate.TextBody1.Text = string.Format(_resourceRepository.GetString("txtDailyGoal"), _unitHelper.GetAmount(_settingsStore.DailyGoal), _unitHelper.AmountText);
                tileTemplate.TextBody2.Text = string.Format(_resourceRepository.GetString("txtAchievedToday"), _unitHelper.GetAmount(todaysAmount), _unitHelper.AmountText);

                var tileTemplateWide = NotificationsExtensions.TileContent.TileContentFactory.CreateTileWide310x150Text01();
                tileTemplateWide.TextHeading.Text = _resourceRepository.GetString("msgTodaysProgress");
                tileTemplateWide.TextBody1.Text = string.Format(_resourceRepository.GetString("txtDailyGoal"), _unitHelper.GetAmount(_settingsStore.DailyGoal), _unitHelper.AmountText);
                tileTemplateWide.TextBody2.Text = string.Format(_resourceRepository.GetString("txtAchievedToday"), _unitHelper.GetAmount(todaysAmount), _unitHelper.AmountText);
                tileTemplateWide.Square150x150Content = tileTemplate;

                var updater = TileUpdateManager.CreateTileUpdaterForApplication();
                updater.EnableNotificationQueue(true);
                updater.Clear();
                updater.Update(new TileNotification(tileTemplateWide.GetXml()));
            }
            catch (Exception ex)
            {
                // suppress
                _log.Log(ex.Message, LogCacheLogLevel.Error);
            }
        }
    }
}