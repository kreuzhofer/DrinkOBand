using System;
using Windows.ApplicationModel.Background;

namespace DrinkOBand.BackgroundTasks.UWP.Infrastructure
{
    internal static class BackgroundTaskHelper
    {
        private const string drinkreminderbackgroundtask = "DrinkReminderBackgroundTask";
        public static async void RegisterBackgroundTaskAsync()
        {
            // register the background task
            var taskRegistered = false;

            foreach (var task in BackgroundTaskRegistration.AllTasks)
            {
                if (task.Value.Name == drinkreminderbackgroundtask)
                {
                    taskRegistered = true;
                    break;
                }
            }
            if (!taskRegistered)
            {
                var result = await BackgroundExecutionManager.RequestAccessAsync();
                if (result == BackgroundAccessStatus.AllowedMayUseActiveRealTimeConnectivity)
                {
                    var builder = new BackgroundTaskBuilder();
                    builder.Name = drinkreminderbackgroundtask;
                    builder.TaskEntryPoint = "DrinkOBand.BackgroundTasks.UWP.DrinkReminderBackgroundTask";
                    builder.SetTrigger(new TimeTrigger(15, false));
                    BackgroundTaskRegistration task = builder.Register();
                }
            }
        }

        public static void UnregisterBackgroundTask()
        {
            foreach (var task in BackgroundTaskRegistration.AllTasks)
            {
                if (task.Value.Name == drinkreminderbackgroundtask)
                {
                    task.Value.Unregister(true);
                }
            }
        }
    }
}