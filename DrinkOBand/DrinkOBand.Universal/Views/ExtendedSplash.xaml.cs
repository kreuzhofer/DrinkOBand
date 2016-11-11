using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Acr.UserDialogs;
using DrinkOBand.Core;
using DrinkOBand.Core.Helpers;
using DrinkOBand.Core.Infrastructure;
using DrinkOBand.Universal.Common;
using DrinkOBand.Universal.Infrastructure;
using DrinkOBand.ViewModels;
using Microsoft.Practices.Prism.Mvvm;
using Microsoft.Practices.Prism.PubSubEvents;
using Microsoft.Practices.Unity;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace DrinkOBand.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ExtendedSplash : Page
    {
        public ExtendedSplash()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Initialize();
        }

        private async void Initialize()
        {
            BackgroundTaskHelper.RegisterBackgroundTaskAsync();

            Resolver.Resolve<ILiveTileUpdater>().UpdateLiveTile();

            // initialize azure mobile service offline cache
            await Resolver.Resolve<IDrinkLogRepository>().InitLocalStoreAsync();

            // write old entries to db otherwise the settings property will get too large
            var synctask = Resolver.Resolve<IDrinkLogRepository>().SyncCacheEntriesAsync();

            // calculate last weeks max and average
            var task = Resolver.Resolve<IDrinkLogRepository>().GetLastWeeksAverageAndMax();
            var task2 = Resolver.Resolve<IDrinkLogRepository>().GetThisWeeksAverageAndMax();

            // new installation -> runs first setup
            Type firstPage = null;
            if (Resolver.Resolve<ISettingsStore>().FirstStart)
            {
                firstPage = typeof(SetupUnitSystemPage);
            }
            else
            {
                firstPage = typeof (RootPage);
            }



            Frame.Navigate(firstPage, "ClearBackStack");
        }
    }
}
