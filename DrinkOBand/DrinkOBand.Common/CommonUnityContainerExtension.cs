using DrinkOBand.Common.Infrastructure;
using DrinkOBand.Core.Infrastructure;
using Microsoft.Practices.Unity;

namespace DrinkOBand.Common
{
    public class CommonUnityContainerExtension : UnityContainerExtension
    {
        protected override void Initialize()
        {
            this.Container.RegisterType<IPasswordVault, UwpPasswordVault>();
        }
    }
}