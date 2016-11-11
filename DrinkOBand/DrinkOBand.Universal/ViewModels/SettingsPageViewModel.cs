using System;
using System.Collections.ObjectModel;
using System.Linq;
using DrinkOBand.Common;
using DrinkOBand.Core;
using DrinkOBand.Core.Entities;
using DrinkOBand.Core.Events;
using DrinkOBand.Core.Helpers;
using DrinkOBand.Core.Infrastructure;
using DrinkOBand.Core.ViewModels;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.PubSubEvents;

namespace DrinkOBand.ViewModels
{
    public class SettingsPageViewModel : BaseViewModel
    {
        private ISettingsStore _settingsStore;
        private IResourceRepository _resourceRepository;
        private INavigationService _navigationService;
        private IUnitHelper _unitHelper;
        private bool init = false;

        public SettingsPageViewModel(ISettingsStore settingsStore, IResourceRepository resourceRepository, 
            IEventAggregator eventAggregator, INavigationService navigationService, IUnitHelper unitHelper) : base(eventAggregator)
        {
            _settingsStore = settingsStore;
            _resourceRepository = resourceRepository;
            _navigationService = navigationService;
            _unitHelper = unitHelper;
        }

        public override void Loaded()
        {
            base.Loaded();

            //settings
            init = true;
            EnableCloudBackup = _settingsStore.CloudEnabled;
            init = false;
            DailyGoal = _unitHelper.GetAmount(_settingsStore.DailyGoal).ToString();
            QuietHoursEnabled = _settingsStore.QuietHoursEnabled;
            QuietHoursStart = _settingsStore.QuietHoursStart;
            QuietHoursEnd = _settingsStore.QuietHoursEnd;

            // Intervals
            Intervals.Add(new IntervalItem() { Name = _resourceRepository.GetString("remind15Min"), Value = 1 });
            Intervals.Add(new IntervalItem() { Name = _resourceRepository.GetString("remind30Min"), Value = 2 });
            Intervals.Add(new IntervalItem() { Name = _resourceRepository.GetString("remind45Min"), Value = 3 });
            Intervals.Add(new IntervalItem() { Name = _resourceRepository.GetString("remind60Min"), Value = 4 });
            Intervals.Add(new IntervalItem() { Name = _resourceRepository.GetString("remind120Min"), Value = 8 });
            SelectedInterval = Intervals.FirstOrDefault(i => i.Value == _settingsStore.NotificationInterval);

            // Unit Systems
            UnitSystems.Add(new UnitSystemItem() {Id = 0, Name = _resourceRepository.GetString("unitMetric")});
            UnitSystems.Add(new UnitSystemItem() { Id = 1, Name = _resourceRepository.GetString("unitUSImperial") });
            SelectedUnitSystem = UnitSystems.FirstOrDefault(i => i.Id == _settingsStore.UnitSystem);
            DailyWaterGoalHeader = String.Format(_resourceRepository.GetString("txtDailyWaterGoal"),
                _unitHelper.AmountText);
        }

        #region Commands

        #endregion

        #region Properties

        private bool _enableCloudBackup;
        public bool EnableCloudBackup
        {
            get { return _enableCloudBackup; }
            set
            {
                SetProperty(ref _enableCloudBackup, value);
                _settingsStore.CloudEnabled = value;
                if (!value)
                {
                    // disable cloud backup and remove credentials
                    Resolver.Resolve<IMobileServicesAuthentication>().RemoveCredentials();
                }
                else if(!init)
                {
                    Resolver.Resolve<IEventAggregator>().GetEvent<SyncRequestedEvent>().Publish(new SyncRequestedEventParameters());
                }
            }
        }

        private string _dailyGoal;

        public string DailyGoal
        {
            get { return _dailyGoal; }
            set
            {
                int number;
                if (!int.TryParse(value, out number))
                {
                    return;
                }
                SetProperty(ref _dailyGoal, value);
                if (_settingsStore.UnitSystem == 0)
                {
                    _settingsStore.DailyGoal = number;
                }
                else
                {
                    _settingsStore.DailyGoal = _unitHelper.OZToMl(number);
                }
            }
        }

        private IntervalItem _selectedInterval;
        public IntervalItem SelectedInterval
        {
            get { return _selectedInterval; }
            set
            {
                SetProperty(ref _selectedInterval, value);
                _settingsStore.NotificationInterval = value.Value;
            }
        }

        private ObservableCollection<IntervalItem> _intervals = new ObservableCollection<IntervalItem>();
        public ObservableCollection<IntervalItem> Intervals
        {
            get { return _intervals; }
            set { SetProperty(ref _intervals, value); }
        }

        private bool _quietHoursEnabled;
        public bool QuietHoursEnabled
        {
            get { return _quietHoursEnabled; }
            set
            {
                SetProperty(ref _quietHoursEnabled, value);
                _settingsStore.QuietHoursEnabled = value;
            }
        }

        private TimeSpan _quietHoursStart;
        public TimeSpan QuietHoursStart
        {
            get { return _quietHoursStart; }
            set
            {
                SetProperty(ref _quietHoursStart, value);
                _settingsStore.QuietHoursStart = value;
            }
        }

        private TimeSpan _quietHoursEnd;

        public TimeSpan QuietHoursEnd
        {
            get { return _quietHoursEnd; }
            set
            {
                SetProperty(ref _quietHoursEnd, value);
                _settingsStore.QuietHoursEnd = value;
            }
        }

        private UnitSystemItem _selectedUnitSystem;

        public UnitSystemItem SelectedUnitSystem
        {
            get { return _selectedUnitSystem; }
            set
            {
                SetProperty(ref _selectedUnitSystem, value);
                _settingsStore.UnitSystem = value.Id;
                DailyWaterGoalHeader = String.Format(_resourceRepository.GetString("txtDailyWaterGoal"),
                    _unitHelper.AmountText);
                DailyGoal = _unitHelper.GetAmount(_settingsStore.DailyGoal).ToString();
            }
        }

        private string _dailyWaterGoalHeader;

        public string DailyWaterGoalHeader
        {
            get { return _dailyWaterGoalHeader; }
            set { SetProperty(ref _dailyWaterGoalHeader, value); }
        }

        private ObservableCollection<UnitSystemItem> _unitSystems = new ObservableCollection<UnitSystemItem>();
        public ObservableCollection<UnitSystemItem> UnitSystems
        {
            get { return _unitSystems; }
            set { SetProperty(ref _unitSystems, value); }
        }

        #endregion
    }
}