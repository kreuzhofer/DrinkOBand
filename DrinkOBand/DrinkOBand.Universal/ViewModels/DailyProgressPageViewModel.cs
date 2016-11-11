using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Acr.UserDialogs;
using DrinkOBand.Common;
using DrinkOBand.Core.Entities;
using DrinkOBand.Core.Events;
using DrinkOBand.Core.Helpers;
using DrinkOBand.Core.Infrastructure;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.PubSubEvents;

namespace DrinkOBand.ViewModels
{
    public class DailyProgressPageViewModel : BaseViewModel
    {
        private ILiveTileUpdater _liveTileUpdater;
        private ISettingsStore _settingsStore;
        private IDrinkLogRepository _drinkLogRepository;
        private ILogCache _logCache;
        private IResourceRepository _resourceRepository;
        private IUnitHelper _unitHelper;
        private IDrinkLogCache _drinkLogCache;
        private Timer _timer;

        public DailyProgressPageViewModel(ILiveTileUpdater liveTileUpdater, ISettingsStore settingsStore, 
            IDrinkLogRepository drinkLogRepository, ILogCache logCache, 
            IEventAggregator eventAggregator, IResourceRepository resourceRepository,
            IUnitHelper unitHelper, IDrinkLogCache drinkLogCache) : base(eventAggregator)
        {
            _liveTileUpdater = liveTileUpdater;
            _settingsStore = settingsStore;
            _drinkLogRepository = drinkLogRepository;
            _logCache = logCache;
            _resourceRepository = resourceRepository;
            _unitHelper = unitHelper;
            _drinkLogCache = drinkLogCache;

            if (_settingsStore.UnitSystem == 0)
            {
                Amount1Text = _resourceRepository.GetString("amount1Metric");
                Amount2Text = _resourceRepository.GetString("amount2Metric");
                Amount3Text = _resourceRepository.GetString("amount3Metric");
                Amount4Text = _resourceRepository.GetString("amount4Metric");
                Amount5Text = _resourceRepository.GetString("amount5Metric");
                Amount6Text = _resourceRepository.GetString("amount6Metric");
            }
            else
            {
                Amount1Text = _resourceRepository.GetString("amount1US");
                Amount2Text = _resourceRepository.GetString("amount2US");
                Amount3Text = _resourceRepository.GetString("amount3US");
                Amount4Text = _resourceRepository.GetString("amount4US");
                Amount5Text = _resourceRepository.GetString("amount5US");
                Amount6Text = _resourceRepository.GetString("amount6US");
            }
            Unit = _unitHelper.AmountText;
        }

        #region Events

        public override async void Loaded()
        {
            base.Loaded();

            RefreshDailyAmount();
        }


        public override void OnNavigatedTo(object parameter)
        {
            _timer = new Timer(TimerCallback, null, 5000, 5000);
            base.OnNavigatedTo(parameter);
        }

        private void TimerCallback(object state)
        {
            SynchronizationContext.Post(o =>
            {
                RefreshDailyAmount();
            }, null);
        }

        public override void OnNavigatedFrom()
        {
            _timer.Dispose();
            _timer = null;
        }

        #endregion

        #region Properties

        private double _dailyProgress;

        public double DailyProgress
        {
            get { return _dailyProgress; }
            set { SetProperty(ref _dailyProgress, value); }
        }

        private string _dailyGoal;
        public string DailyGoal
        {
            get { return _dailyGoal; }
            set
            {
                SetProperty(ref _dailyGoal, value);
            }
        }

        private string _amount1Text;

        public string Amount1Text
        {
            get { return _amount1Text; }
            set { SetProperty(ref _amount1Text, value); }
        }

        private string _amount2Text;

        public string Amount2Text
        {
            get { return _amount2Text; }
            set { SetProperty(ref _amount2Text, value); }
        }

        private string _amount3Text;

        public string Amount3Text
        {
            get { return _amount3Text; }
            set { SetProperty(ref _amount3Text, value); }
        }

        private string _amount4Text;

        public string Amount4Text
        {
            get { return _amount4Text; }
            set { SetProperty(ref _amount4Text, value); }
        }

        private string _amount5Text;

        public string Amount5Text
        {
            get { return _amount5Text; }
            set { SetProperty(ref _amount5Text, value); }
        }

        private string _amount6Text;
        public string Amount6Text
        {
            get { return _amount6Text; }
            set { SetProperty(ref _amount6Text, value); }
        }

        private string _unit;


        public string Unit
        {
            get { return _unit; }
            set { SetProperty(ref _unit, value); }
        }

        private bool _isCustomAmountOpen;

        public bool IsCustomAmountOpen
        {
            get { return _isCustomAmountOpen; }
            set { SetProperty(ref _isCustomAmountOpen, value); }
        }

        private string _customAmountText;

        public string CustomAmountText
        {
            get { return _customAmountText; }
            set { SetProperty(ref _customAmountText, value); }
        }

        private bool _customAmountIsFocused;


        public bool CustomAmountIsFocused
        {
            get { return _customAmountIsFocused; }
            set { SetProperty(ref _customAmountIsFocused, value); }
        }

        #endregion

        #region Commands

        public DelegateCommand<string> AmountSelectedCommand
        {
            get
            {
                return new DelegateCommand<string>(async selectedValue =>
                {
                    var amount = _unitHelper.GetMappedAmount(Int32.Parse(selectedValue));
                    await AddAmount(amount);
                });
            }
        }

        private async Task AddAmount(int amount)
        {
            _drinkLogCache.AddToCache(new DrinkLogItem {Amount = amount, Timestamp = DateTime.Now});
            RefreshDailyAmount();
            _liveTileUpdater.UpdateLiveTile();
            _settingsStore.PassedNotifications = -1; // add more time to wait until next notification
            _eventAggregator.GetEvent<SyncRequestedEvent>().Publish(new SyncRequestedEventParameters());
        }

        public DelegateCommand DeleteEntriesOfTodayCommand
        {
            get
            {
                return new DelegateCommand(async () =>
                {
                    await _drinkLogRepository.DeleteEntriesOfToday();
                });
            }
        }

        public DelegateCommand SyncCommand
        {
            get
            {
                return new DelegateCommand(() =>
                {
                    _eventAggregator.GetEvent<SyncRequestedEvent>().Publish(new SyncRequestedEventParameters());
                });
            }
        }

        public DelegateCommand CustomAmountSelectedCommand
        {
            get
            {
                return new DelegateCommand(() =>
                {
                    CustomAmountText = null;
                    IsCustomAmountOpen = true;
                    CustomAmountIsFocused = true;
                });
            }
        }

        public DelegateCommand SaveCustomAmountCommand
        {
            get
            {
                return new DelegateCommand(async () =>
                {
                    IsCustomAmountOpen = false;
                    int amount;
                    if (int.TryParse(CustomAmountText, out amount))
                    {
                        var finalAmount = _unitHelper.GetAmount(amount);
                        await AddAmount(finalAmount);
                    }
                    else
                    {
                        await UserDialogs.Instance.AlertAsync(_resourceRepository.GetString("msgInvalidAmount"), _resourceRepository.GetString("msgError"));
                    }
                });
            }
        }

        public DelegateCommand CancelCustomAmountCommand
        {
            get
            {
                return new DelegateCommand(() =>
                {
                    IsCustomAmountOpen = false;
                });
            }
        }

        #endregion

        private async void RefreshDailyAmount()
        {
            await _drinkLogRepository.SyncCacheEntriesAsync();
            DailyGoal = _unitHelper.GetAmount(_settingsStore.DailyGoal).ToString();
            var amount = _unitHelper.GetAmount(_drinkLogCache.GetTodaysAmount());
            if (amount != DailyProgress)
            {
                DailyProgress = amount;
            }
        }

    }
}