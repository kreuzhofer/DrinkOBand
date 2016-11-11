using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(drinkobandserviceService.Startup))]

namespace drinkobandserviceService
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureMobileApp(app);
        }
    }
}