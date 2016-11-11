using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Phone.UI.Input;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using DrinkOBand.Common;
using DrinkOBand.Core;
using DrinkOBand.Core.Entities;
using DrinkOBand.Core.Events;
using DrinkOBand.Core.Helpers;
using DrinkOBand.Core.Infrastructure;
using DrinkOBand.Universal.Common;
using DrinkOBand.ViewModels;
using Microsoft.Practices.Prism.Mvvm;
using Microsoft.Practices.Prism.PubSubEvents;
using Microsoft.Practices.Prism.StoreApps;
using Newtonsoft.Json;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace DrinkOBand.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class RootPage : PageBase
    {
        private bool _isHardwareButtonsAPIPresent;

        public RootPage()
        {
            this.InitializeComponent();
            _isHardwareButtonsAPIPresent = Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.Phone.UI.Input.HardwareButtons");
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            var parameter = e.Parameter as string;
            if (parameter != null && parameter == "ClearBackStack")
            {
                Frame.BackStack.Clear();
            }

            Resolver.Resolve<IEventAggregator>().GetEvent<NavigationEvent>().Subscribe(ExecuteNavigationEvent);

            if (_isHardwareButtonsAPIPresent)
            {
                Windows.Phone.UI.Input.HardwareButtons.BackPressed += HardwareButtons_BackPressed;
            }

            SystemNavigationManager.GetForCurrentView().BackRequested += SystemNavigationManager_BackRequested;

            innerFrame.Navigated += InnerFrame_Navigated;
        }

        private void InnerFrame_Navigated(object sender, NavigationEventArgs e)
        {
            // Update the Back button depending on whether we can go Back.
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
                innerFrame.CanGoBack ?
                AppViewBackButtonVisibility.Visible :
                AppViewBackButtonVisibility.Collapsed;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            Resolver.Resolve<IEventAggregator>().GetEvent<NavigationEvent>().Unsubscribe(ExecuteNavigationEvent);
            if (_isHardwareButtonsAPIPresent)
            {
                Windows.Phone.UI.Input.HardwareButtons.BackPressed -= HardwareButtons_BackPressed;
            }
            SystemNavigationManager.GetForCurrentView().BackRequested -= SystemNavigationManager_BackRequested;
        }

        private void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        {
            var vm = ((RootPageViewModel) DataContext);
            if (vm.Popup == 1)
            {
                vm.Popup = 0;
                e.Handled = true;
                return;
            }
            if (innerFrame.CanGoBack)
            {
                innerFrame.GoBack();
                e.Handled = true;
                return;
            }
        }

        private void SystemNavigationManager_BackRequested(object sender, BackRequestedEventArgs e)
        {
            var vm = ((RootPageViewModel)DataContext);
            if (vm.Popup == 1)
            {
                vm.Popup = 0;
                e.Handled = true;
                return;
            }
            if (innerFrame.CanGoBack)
            {
                innerFrame.GoBack();
                e.Handled = true;
                return;
            }
        }

        private void ExecuteNavigationEvent(INavigationEventPayload payload)
        {
            if (payload is NavigationEventPayload)
            {
                var p = payload as NavigationEventPayload;
                innerFrame.Navigate(p.PageType, p.NavigationParameter);

            }
            else if (payload is NavigateBackEventPayload)
            {
                if (innerFrame.CanGoBack)
                {
                    innerFrame.GoBack();
                }
            }
            else if (payload is NavigateRootFrameEventPayload)
            {
                var p = payload as NavigateRootFrameEventPayload;
                Frame.Navigate(p.PageType, p.NavigationParameter);
            }
        }
    }
}
