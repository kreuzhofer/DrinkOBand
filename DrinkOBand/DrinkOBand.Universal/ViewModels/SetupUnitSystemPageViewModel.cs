using DrinkOBand.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DrinkOBand.Core.Helpers;
using DrinkOBand.Core.Infrastructure;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.PubSubEvents;

namespace DrinkOBand.ViewModels
{
    public class SetupUnitSystemPageViewModel : BaseViewModel
    {
        private INavigationService _navigationService;

        public SetupUnitSystemPageViewModel(IEventAggregator eventAggregator, INavigationService navigationService,
            ISettingsStore settingsStore) : base(eventAggregator)
        {
            _navigationService = navigationService;
            _settingsStore = settingsStore;
        }

        private int _selectedUnitSystem;
        private ISettingsStore _settingsStore;

        public int SelectedUnitSystem
        {
            get { return _selectedUnitSystem; }
            set
            {
                SetProperty(ref _selectedUnitSystem, value);
                _settingsStore.UnitSystem = value;
            }
        }

        public override void OnNavigatedTo(object parameter)
        {
            base.OnNavigatedTo(parameter);

            SelectedUnitSystem = _settingsStore.UnitSystem;
        }
    }
}
