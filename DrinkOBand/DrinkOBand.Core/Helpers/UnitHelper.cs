using System;
using System.Collections.Generic;
using DrinkOBand.Core.Infrastructure;
using DrinkOBand.Core.ViewModels;

namespace DrinkOBand.Core.Helpers
{
    public class UnitHelper : IUnitHelper
    {

        private ISettingsStore _settingsStore;
        public UnitHelper(ISettingsStore settingsStore, IResourceRepository resourceRepository)
        {
            _settingsStore = settingsStore;
            _resourceRepository = resourceRepository;
        }

        public int OZToMl(int value)
        {
            return value*30;
        }

        public static readonly Dictionary<int, int> AmountMappingsMetric = new Dictionary<int, int>
        {
            {1, 150},
            {2, 300},
            {3, 500},
            {4, 750},
            {5, 1000},
            {6, 1500}
        };

        public static readonly Dictionary<int, int> AmountMappingsUS = new Dictionary<int, int>
        {
            {1, 120},
            {2, 300},
            {3, 480},
            {4, 750},
            {5, 960},
            {6, 1920}
        };

        private IResourceRepository _resourceRepository;


        public int GetMappedAmount(int key)
        {
            if (_settingsStore.UnitSystem == 0)
                return AmountMappingsMetric[key];
            else
                return AmountMappingsUS[key];
        }

        public int GetAmount(int value)
        {
            if (_settingsStore.UnitSystem == 0)
                return value;
            return value/30;
        }

        public string AmountText
        {
            get
            {
                if (_settingsStore.UnitSystem == 0)
                {
                    return _resourceRepository.GetString("unitTextMetric");
                }
                return _resourceRepository.GetString("unitTextUS");
            }
        }
    }
}