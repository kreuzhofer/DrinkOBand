using Microsoft.WindowsAzure.MobileServices;

namespace DrinkOBand.Core.Infrastructure
{
    public class MobileServiceClientManager
    {
        MobileServiceClient client;

        public MobileServiceClientManager()
        {
            this.client = new MobileServiceClient(Globals.ApplicationURL);
        }

        public MobileServiceClient MobileService
        {
            get { return client; }
        }

    }
}