// Helpers/Settings.cs

using System;
using Plugin.Settings.Abstractions;

namespace DrinkOBand.Core.Helpers
{
    /// <summary>
    /// This is the Settings static class that can be used in your Core solution or in any
    /// of your client applications. All settings are laid out the same exact way with getters
    /// and setters. 
    /// </summary>
    public class SettingsStore : ISettingsStore
    {
        public SettingsStore(ISettings settings)
        {
            _settings = settings;
        }

        #region Setting Constants

        private const string FirstStartKey = "first_start";
        private static readonly bool FirstStartDefault = true;
        private const string DailyGoalKey = "daily_goal";
        private static readonly int DailyGoalDefault = 3000;
        private const string CloudEnabledKey = "cloud_enabled";
        private static readonly bool CloudEnabledDefault = false;
        private const string NotificationIntervalKey = "interval";
        private static int NotificationIntervalDefault = 2;
        private const string PassedNotificationsKey = "passed_notifications";
        private static int PassedNotificationsDefault = 0;
        private const string LastWeeksAverageKey = "last_weeks_average";
        private static int LastWeeksAverageDefault = 0;
        private const string LastWeeksMaxKey = "last_weeks_max";
        private static int LastWeeksMaxDefault = 0;
        private const string QuietHoursEnabledKey = "quiet_hours_enabled";
        private static bool QuietHoursEnabledDefault = true;
        private const string QuietHoursStartKey = "quiet_hours_start";
        private static TimeSpan QuietHoursStartDefault = TimeSpan.FromHours(22);
        private const string QuietHoursEndKey = "quiet_hours_end";
        private static TimeSpan QuietHoursEndDefault = TimeSpan.FromHours(7);
        private ISettings _settings;
        private const string ThisWeeksAverageKey = "this_weeks_average";
        private static int ThisWeeksAverageDefault = 0;
        private const string ThisWeeksMaxKey = "this_weeks_max";
        private static int ThisWeeksMaxDefault = 0;
        private const string UnitSystemKey = "unit_system";
        private static int UnitSystemDefault = 0; // 0 = Metric, 1 = U.S., 2 = Imperial
        private const string VersionInfoLastShownKey = "version_info_last_shown";
        private static string VersionInfoLastShownDefault = null;

        #endregion

        public bool FirstStart
        {
            get { return _settings.GetValueOrDefault(FirstStartKey, FirstStartDefault); }
            set { _settings.AddOrUpdateValue(FirstStartKey, value); }
        }

        public int DailyGoal
        {
            get { return _settings.GetValueOrDefault(DailyGoalKey, DailyGoalDefault); }
            set { _settings.AddOrUpdateValue(DailyGoalKey, value); }
        }

        public bool CloudEnabled
        {
            get { return _settings.GetValueOrDefault(CloudEnabledKey, CloudEnabledDefault); }
            set { _settings.AddOrUpdateValue(CloudEnabledKey, value); }
        }

        public int NotificationInterval
        {
            get { return _settings.GetValueOrDefault(NotificationIntervalKey, NotificationIntervalDefault); }
            set { _settings.AddOrUpdateValue(NotificationIntervalKey, value); }
        }

        public int PassedNotifications
        {
            get { return _settings.GetValueOrDefault(PassedNotificationsKey, PassedNotificationsDefault); }
            set { _settings.AddOrUpdateValue(PassedNotificationsKey, value); }
        }

        public int LastWeeksAverage
        {
            get { return _settings.GetValueOrDefault(LastWeeksAverageKey, LastWeeksAverageDefault); }
            set { _settings.AddOrUpdateValue(LastWeeksAverageKey, value); }
        }

        public int LastWeeksMax
        {
            get { return _settings.GetValueOrDefault(LastWeeksMaxKey, LastWeeksMaxDefault); }
            set { _settings.AddOrUpdateValue(LastWeeksMaxKey, value); }
        }

        public bool QuietHoursEnabled
        {
            get { return _settings.GetValueOrDefault(QuietHoursEnabledKey, QuietHoursEnabledDefault); }
            set { _settings.AddOrUpdateValue(QuietHoursEnabledKey, value); }
        }

        public TimeSpan QuietHoursStart
        {
            get { return _settings.GetValueOrDefault(QuietHoursStartKey, QuietHoursStartDefault); }
            set { _settings.AddOrUpdateValue(QuietHoursStartKey, value); }
        }

        public TimeSpan QuietHoursEnd
        {
            get { return _settings.GetValueOrDefault(QuietHoursEndKey, QuietHoursEndDefault); }
            set { _settings.AddOrUpdateValue(QuietHoursEndKey, value); }
        }

        public int ThisWeeksAverage
        {
            get { return _settings.GetValueOrDefault(ThisWeeksAverageKey, ThisWeeksAverageDefault); }
            set { _settings.AddOrUpdateValue(ThisWeeksAverageKey, value); }
        }

        public int ThisWeeksMax
        {
            get { return _settings.GetValueOrDefault(ThisWeeksMaxKey, ThisWeeksMaxDefault); }
            set { _settings.AddOrUpdateValue(ThisWeeksMaxKey, value); }
        }

        public int UnitSystem
        {
            get { return _settings.GetValueOrDefault(UnitSystemKey, UnitSystemDefault); }
            set { _settings.AddOrUpdateValue(UnitSystemKey, value); }
        }

        public string VersionInfoLastShown
        {
            get { return _settings.GetValueOrDefault(VersionInfoLastShownKey, VersionInfoLastShownDefault); }
            set { _settings.AddOrUpdateValue(VersionInfoLastShownKey, value); }
        }
    }
}