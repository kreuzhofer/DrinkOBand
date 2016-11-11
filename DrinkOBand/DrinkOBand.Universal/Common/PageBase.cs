using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using DrinkOBand.Common;
using Microsoft.Practices.Prism.Mvvm;

namespace DrinkOBand.Universal.Common
{
    public class PageBase : Page, IView
    {
        public PageBase()
        {
        }
        
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.Loaded += OnLoaded;
            base.OnNavigatedTo(e);
            
            var navigationAware = this.DataContext as INavigationAware;
            if (navigationAware != null)
                navigationAware.OnNavigatedTo(e.Parameter);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.Loaded -= OnLoaded;
            base.OnNavigatedFrom(e);

            var navigationAware = this.DataContext as INavigationAware;
            if (navigationAware != null)
                navigationAware.OnNavigatedFrom();
        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            var navigationAware = this.DataContext as INavigationAware;
            if (navigationAware != null)
                navigationAware.Loaded();
        }


    }
}
