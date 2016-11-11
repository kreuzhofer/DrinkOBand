using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using DrinkOBand.Common;
using DrinkOBand.Core.Entities;
using DrinkOBand.Core.Helpers;
using DrinkOBand.Core.Infrastructure;
using DrinkOBand.ViewModels;
using DrinkOBand.Universal.Common;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace DrinkOBand.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class DailyProgressPage : PageBase
    {
        public DailyProgressPage()
        {
            this.InitializeComponent();
        }

        private void CustomAmountPopup_OnOpened(object sender, object e)
        {
            CustomAmountTextBox.Focus(FocusState.Programmatic);
        }

        private void CustomAmountTextBox_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
            {
                var vm = this.DataContext as DailyProgressPageViewModel;
                vm.SaveCustomAmountCommand.Execute();
            }
            else if (e.Key == VirtualKey.Escape)
            {
                var vm = this.DataContext as DailyProgressPageViewModel;
                vm.CancelCustomAmountCommand.Execute();
            }
        }
    }
}
