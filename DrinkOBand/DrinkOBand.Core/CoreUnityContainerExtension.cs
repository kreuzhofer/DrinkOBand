using DrinkOBand.Core.Helpers;
using DrinkOBand.Core.Infrastructure;
using Microsoft.Practices.Prism.PubSubEvents;
using Microsoft.Practices.Unity;
using Plugin.Settings;
using Plugin.Settings.Abstractions;

namespace DrinkOBand.Core
{
    public class CoreUnityContainerExtension : UnityContainerExtension
    {
        protected override void Initialize()
        {
            this.Container.RegisterInstance<ISettings>(CrossSettings.Current);
            this.Container.RegisterType<ISettingsStore, SettingsStore>();
            this.Container.RegisterType<IDrinkLogRepository, DrinkLogRepository>();
            this.Container.RegisterType<MobileServiceClientManager>(new ContainerControlledLifetimeManager());
            this.Container.RegisterType<INavigationService, NavigationService>(new ContainerControlledLifetimeManager());
            this.Container.RegisterInstance<IEventAggregator>(new EventAggregator());
            this.Container.RegisterType<IUnitHelper, UnitHelper>();
            this.Container.RegisterType<IDrinkLogCache, DrinkLogCache>();
        }
    }
}