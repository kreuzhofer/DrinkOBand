using System;

namespace DrinkOBand.Core.Helpers
{
    public interface ISettingsStore
    {
        bool FirstStart { get; set; }
        int DailyGoal { get; set; }
        bool CloudEnabled { get; set; }
        int NotificationInterval { get; set; }
        int PassedNotifications { get; set; }
        int LastWeeksAverage { get; set; }
        int LastWeeksMax { get; set; }
        bool QuietHoursEnabled { get; set; }
        TimeSpan QuietHoursStart { get; set; }
        TimeSpan QuietHoursEnd { get; set; }
        int ThisWeeksAverage { get; set; }
        int ThisWeeksMax { get; set; }
        int UnitSystem { get; set; }
        string VersionInfoLastShown { get; set; }
    }
}