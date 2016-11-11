using Windows.UI.Notifications;
using DrinkOBand.Core.Infrastructure;

namespace DrinkOBand.Common.Infrastructure
{
    public class ToastHelper : IToastHelper
    {
        public void ShowToastNotification(string message)
        {
            var toastContent = NotificationsExtensions.ToastContent.ToastContentFactory.CreateToastText01();
            toastContent.TextBodyWrap.Text = message;
            var toast = new ToastNotification(toastContent.GetXml());
            ToastNotificationManager.CreateToastNotifier().Show(toast);
        }
    }
}