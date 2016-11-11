using Windows.ApplicationModel;
using Windows.UI.Xaml;
using DrinkOBand.Common;
using Microsoft.Practices.Prism.PubSubEvents;

namespace DrinkOBand.ViewModels
{
    public class InfoPageViewModel : BaseViewModel
    {
        public InfoPageViewModel(IEventAggregator eventAggregator) : base(eventAggregator)
        {
            VersionNumber = GetPackageVersion();
        }

        private string _versionNumber;
        public string VersionNumber
        {
            get { return _versionNumber; }
            set { SetProperty(ref _versionNumber, value); }
        }

        private string GetPackageVersion()
        {
            return Package.Current.Id.Version.Major + "."
                                    + Package.Current.Id.Version.Minor + "."
                                    + Package.Current.Id.Version.Build + "."
                                    + Package.Current.Id.Version.Revision;
        }
    }
}