using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using DrinkOBand.Common;
using DrinkOBand.Core.Helpers;
using DrinkOBand.Core.Infrastructure;
using Microsoft.Practices.Prism.PubSubEvents;
using WinRTXamlToolkit.Controls.DataVisualization.Charting;

namespace DrinkOBand.ViewModels
{
    public class HistoryPageViewModel : BaseViewModel
    {
        private IDrinkLogRepository _drinkLogRepository;
        private IUnitHelper _unitHelper;
        private IResourceRepository _resourceRepository;

        public HistoryPageViewModel(IDrinkLogRepository drinkLogRepository, IEventAggregator eventAggregator,
            IUnitHelper unitHelper, IResourceRepository resourceRepository) : base(eventAggregator)
        {
            _drinkLogRepository = drinkLogRepository;
            _unitHelper = unitHelper;
            _resourceRepository = resourceRepository;
        }

        public override void Loaded()
        {
            base.Loaded();

            Task.Factory.StartNew(async () =>
            {
                var thisWeeksHistory = await _drinkLogRepository.GetThisWeeksAmounts();
                var lastWeeksHistory = await _drinkLogRepository.GetLastWeeksAmounts();
                foreach (var item in thisWeeksHistory)
                {
                    SynchronizationContext.Post(state => ThisWeeksAmounts.Add(new NameValueItem() { Name = item.Key, Value = _unitHelper.GetAmount(item.Value) }), null);
                }
                foreach (var item in lastWeeksHistory)
                {
                    SynchronizationContext.Post(state => LastWeeksAmounts.Add(new NameValueItem() { Name = item.Key, Value = _unitHelper.GetAmount(item.Value) }), null);
                }
            });

        }

        private ObservableCollection<NameValueItem> _lastWeeksAmounts = new ObservableCollection<NameValueItem>();

        public ObservableCollection<NameValueItem> LastWeeksAmounts
        {
            get { return _lastWeeksAmounts; }
            set { SetProperty(ref _lastWeeksAmounts, value); }
        }

        private ObservableCollection<NameValueItem> _thisWeeksAmounts = new ObservableCollection<NameValueItem>();


        public ObservableCollection<NameValueItem> ThisWeeksAmounts
        {
            get { return _thisWeeksAmounts; }
            set { SetProperty(ref _thisWeeksAmounts, value); }
        }

    }
    public class NameValueItem
    {
        public string Name { get; set; }
        public int Value { get; set; }
    }
}