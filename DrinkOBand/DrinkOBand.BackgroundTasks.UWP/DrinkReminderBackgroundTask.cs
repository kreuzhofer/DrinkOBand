using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Foundation.Metadata;
using Windows.Storage;
using Windows.System;
using Windows.UI.Notifications;
using DrinkOBand.Common;
using DrinkOBand.Common.Infrastructure;
using DrinkOBand.Core;
using DrinkOBand.Core.Entities;
using DrinkOBand.Core.Helpers;
using DrinkOBand.Core.Infrastructure;
using Microsoft.Practices.Unity;
using Newtonsoft.Json;
using NotificationsExtensions.ToastContent;
using Plugin.Settings;
using Plugin.Settings.Abstractions;

namespace DrinkOBand.BackgroundTasks.UWP
{
    public sealed class DrinkReminderBackgroundTask : IBackgroundTask
    {
        private Stopwatch _watch;

        private IResourceRepository _resourceRepository;
        private ILiveTileUpdater _liveTileUpdater;
        private ISettingsStore _settingsStore;
        private ILogCache _logCache;
        private IUnitHelper _unitHelper;
        private IToastHelper _toastHelper;
        private IDrinkLogCache _drinkLogCache;

        public DrinkReminderBackgroundTask()
        {
            // init Resolver
            Resolver.Container.RegisterInstance<ILogCache>(new LogCache());
            Resolver.Container.RegisterInstance<ISettings>(CrossSettings.Current);
            Resolver.Container.RegisterType<ISettingsStore, SettingsStore>();
            Resolver.Container.RegisterType<IResourceRepository, ResourceRepository>();
            Resolver.Container.RegisterType<ILiveTileUpdater, LiveTileUpdater>();
            Resolver.Container.RegisterType<IUnitHelper, UnitHelper>();
            Resolver.Container.RegisterType<IToastHelper, ToastHelper>();
            Resolver.Container.RegisterType<IDrinkLogCache, DrinkLogCache>();

            _resourceRepository = Resolver.Resolve<IResourceRepository>();
            _liveTileUpdater = Resolver.Resolve<ILiveTileUpdater>();
            _settingsStore = Resolver.Resolve<ISettingsStore>();
            _logCache = Resolver.Resolve<ILogCache>();
            _unitHelper = Resolver.Resolve<IUnitHelper>();
            _toastHelper = Resolver.Resolve<IToastHelper>();
            _drinkLogCache = Resolver.Resolve<IDrinkLogCache>();
        }

        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            if (_settingsStore.FirstStart)
            {
                return;
            }
            _settingsStore.PassedNotifications++;
            if (_settingsStore.PassedNotifications < _settingsStore.NotificationInterval)
            {
                return;
            }
            // check for quiet hours interval
            if (_settingsStore.QuietHoursEnabled)
            {
                var now = DateTime.Now;
                var timeOfDay = now.TimeOfDay;
                if (_settingsStore.QuietHoursStart <= _settingsStore.QuietHoursEnd &&
                    _settingsStore.QuietHoursStart <= timeOfDay && _settingsStore.QuietHoursEnd >= timeOfDay) // easy
                {
                    return;
                }
                else if ((timeOfDay < TimeSpan.FromHours(24) && timeOfDay >= _settingsStore.QuietHoursStart) ||
                    timeOfDay >= TimeSpan.Zero && timeOfDay <= _settingsStore.QuietHoursEnd)
                {
                    return;
                }
            }
            _settingsStore.PassedNotifications = 0;

            BackgroundTaskDeferral _deferral = taskInstance.GetDeferral();
            _logCache.Log("DrinkReminderBackgroundTask|Starting", LogCacheLogLevel.Verbose);
            _watch = Stopwatch.StartNew();
            ExecuteDrinkReminder(_watch);
            _logCache.Log(String.Format("DrinkReminderBackgroundTask|Ended normally after {0}", _watch.Elapsed),
                LogCacheLogLevel.Verbose);

            _deferral.Complete();
        }

        private void ExecuteDrinkReminder(Stopwatch watch)
        {
            _logCache.Log("ExecuteDrinkReminderAsync|Begin", LogCacheLogLevel.Verbose);
            try
            {
				_toastHelper.ShowToastNotification(_resourceRepository.GetString("msgTimeToDrink"));
            }
            catch (Exception ex)
            {
                _logCache.Log(ex.Message, LogCacheLogLevel.Critical);
            }
        }
    }
}