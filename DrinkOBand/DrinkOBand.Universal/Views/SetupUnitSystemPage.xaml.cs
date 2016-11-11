using System;
using System.Collections.Generic;
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
using DrinkOBand.Common;
using DrinkOBand.Core;
using DrinkOBand.Core.Helpers;
using DrinkOBand.Universal.Common;
using DrinkOBand.ViewModels;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace DrinkOBand.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SetupUnitSystemPage : PageBase
    {
        private ISettingsStore _settingsStore;

        public SetupUnitSystemPage()
        {
            this.InitializeComponent();
            _settingsStore = Resolver.Resolve<ISettingsStore>();
        }

        private void ButtonContinue_Click(object sender, RoutedEventArgs e)
        {
            if (((SetupUnitSystemPageViewModel) DataContext).IsDesktop)
            {
                Frame.Navigate(typeof(RootPage), "ClearBackStack");
                _settingsStore.FirstStart = false;
            }
            else
            {
                Frame.Navigate(typeof(RootPage));
            }

        }
    }
}
