using Microsoft.Practices.Unity;

namespace DrinkOBand.Core
{
    public static class Resolver
    {
        static Resolver()
        {
            _container = new UnityContainer();
        }

        private static UnityContainer _container;

        public static T Resolve<T>()
        {
            return _container.Resolve<T>();
        }

        public static UnityContainer Container
        {
            get {  return _container; }
        }

    }
}