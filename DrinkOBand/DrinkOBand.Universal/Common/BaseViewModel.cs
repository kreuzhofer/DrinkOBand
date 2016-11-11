using System.Threading;
using DrinkOBand.Core.Events;
using Microsoft.Practices.Prism.Mvvm;
using Microsoft.Practices.Prism.PubSubEvents;

namespace DrinkOBand.Common
{
    public class BaseViewModel : ViewModel, INavigationAware
    {
        protected IEventAggregator _eventAggregator;

        public BaseViewModel(IEventAggregator eventAggregator)
        {
#if WINDOWS_UWP
            IsDesktop = Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily == "Windows.Desktop";
#endif
            SynchronizationContext = SynchronizationContext.Current;
            _eventAggregator = eventAggregator;
#if DEBUG
            IsDebug = true;
#endif
        }

        public SynchronizationContext SynchronizationContext { get; set; }

        private string _pageTitle;

        public string PageTitle
        {
            get { return _pageTitle; }
            set { SetProperty(ref _pageTitle, value); }
        }

        public virtual void OnNavigatedTo(object parameter)
        {
        }

        public virtual void OnNavigatedFrom()
        {
        }

        public virtual void Loaded()
        {
        }

        private bool _isDesktop;

        public bool IsDesktop
        {
            get { return _isDesktop; }
            set { SetProperty(ref _isDesktop, value); }
        }

        private bool _isDebug;

        public bool IsDebug
        {
            get { return _isDebug; }
            set { SetProperty(ref _isDebug, value); }
        }
    }
}