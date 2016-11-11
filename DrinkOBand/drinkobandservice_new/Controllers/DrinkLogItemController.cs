using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.OData;
using drinkobandserviceService.DataObjects;
using drinkobandserviceService.Models;
using Microsoft.Azure.Mobile.Server;
using Microsoft.Azure.Mobile.Server.Authentication;
using Microsoft.Azure.Mobile.Server.Config;
using Microsoft.Azure.NotificationHubs;

namespace drinkobandserviceService.Controllers
{
    [Authorize]
    [MobileAppController]
    public class DrinkLogItemController : TableController<DrinkLogItem>
    {
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            drinkobandserviceContext context = new drinkobandserviceContext();
            DomainManager = new EntityDomainManager<DrinkLogItem>(context, Request);
        }

        // GET tables/DrinkLogItem
        public async Task<IQueryable<DrinkLogItem>> GetAllDrinkLogItem()
        {
            var identity = await GetUserId();
            var providerIdentiy = await GetProviderUserId();
            return Query().Where(q => q.UserId == identity);
        }

        // GET tables/DrinkLogItem/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public SingleResult<DrinkLogItem> GetDrinkLogItem(string id)
        {
            return Lookup(id);
        }

        // PATCH tables/DrinkLogItem/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task<DrinkLogItem> PatchDrinkLogItem(string id, Delta<DrinkLogItem> patch)
        {
             return UpdateAsync(id, patch);
        }

        // POST tables/DrinkLogItem
        public async Task<IHttpActionResult> PostDrinkLogItem(DrinkLogItem item)
        {
            item.UserId = await GetUserId();
            item.ProviderUserId = await GetProviderUserId();
            DrinkLogItem current = await InsertAsync(item);

            await NotifyAboutNewData(item.UserId);

            return CreatedAtRoute("Tables", new { id = current.Id }, current);
        }

        // DELETE tables/DrinkLogItem/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task DeleteDrinkLogItem(string id)
        {
             return DeleteAsync(id);
        }

        private async Task<string> GetUserId()
        {
            var obj = await this.User.GetAppServiceIdentityAsync<MicrosoftAccountCredentials>(Request);
            var identity = this.User as ClaimsPrincipal;
            var identities = identity.Identities.ToList();
            var value = identity.Claims.Where(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier").FirstOrDefault().Value;
            return value;
        }

        private async Task<string> GetProviderUserId()
        {
            var obj = await this.User.GetAppServiceIdentityAsync<MicrosoftAccountCredentials>(Request);
            return obj.UserId;
        }

        private async Task NotifyAboutNewData(string user)
        {
            // Get the settings for the server project.
            HttpConfiguration config = this.Configuration;
            MobileAppSettingsDictionary settings =
                this.Configuration.GetMobileAppSettingsProvider().GetMobileAppSettings();

            // Get the Notification Hubs credentials for the Mobile App.
            string notificationHubName = settings.NotificationHubName;
            string notificationHubConnection = settings
                .Connections[MobileAppSettingsKeys.NotificationHubConnectionString].ConnectionString;

            // Create a new Notification Hub client.
            NotificationHubClient hub = NotificationHubClient
            .CreateClientFromConnectionString(notificationHubConnection, notificationHubName);

            string userTag = "_UserId:" + user;

            try
            {
                //// Sending the message so that all template registrations that contain "messageParam"
                //// will receive the notifications. This includes APNS, GCM, WNS, and MPNS template registrations.
                //Dictionary<string, string> templateParams = new Dictionary<string, string>();
                //templateParams["messageParam"] = user + " has added an item to the list.";

                //    // Send the push notification and log the results.
                //    var result1 = await hub.SendTemplateNotificationAsync(templateParams);

                    // Send the push notification and log the results.
                    var result = await hub.SendNotificationAsync(new WindowsNotification("update", new Dictionary<string, string> { { "X-WNS-Type", "wns/raw" } }), userTag); // { "Content-Type", "application/octet-stream"},

                // Write the success result to the logs.
                config.Services.GetTraceWriter().Info(result.State.ToString());
            }
            catch (System.Exception ex)
            {
                // Write the failure result to the logs.
                config.Services.GetTraceWriter()
                    .Error(ex.Message, null, "Push.SendAsync Error");
            }
        }

    }
}