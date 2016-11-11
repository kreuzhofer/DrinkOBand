using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Background;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Networking.PushNotifications;
using Windows.Phone.UI.Input;
using Windows.Security.Credentials;
using Windows.Storage;
using Windows.System;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using DrinkOBand.Common;
using DrinkOBand.Core;
using DrinkOBand.Core.Entities;
using DrinkOBand.Core.Events;
using DrinkOBand.Core.Helpers;
using DrinkOBand.Core.Infrastructure;
using DrinkOBand.Universal.Common;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.PubSubEvents;
using Microsoft.WindowsAzure.MobileServices;
using WinRTXamlToolkit.IO.Extensions;

namespace DrinkOBand.ViewModels
{
    public class RootPageViewModel : BaseViewModel
    {
        private INavigationService _navigationService;
        private IResourceRepository _resourceRepository;
        private ISettingsStore _settingsStore;
        private IEventAggregator _eventAggregator;
        private IDrinkLogRepository _drinkLogRepository;
        private DataTransferManager dataTransferManager;
        private ILogCache _logCache;
        PushNotificationChannel channel = null;
        private volatile bool _syncInProgress = false;
        private MobileServiceClientManager _clientManager;

        public RootPageViewModel(INavigationService navigationService, IResourceRepository resourceRepository, ISettingsStore settingsStore, 
            IEventAggregator eventAggregator, IDrinkLogRepository drinkLogRepository, ILogCache logCache, MobileServiceClientManager clientManager) : base(eventAggregator)
        {
            _navigationService = navigationService;
            _resourceRepository = resourceRepository;
            _settingsStore = settingsStore;
            _eventAggregator = eventAggregator;
            _drinkLogRepository = drinkLogRepository;
            _logCache = logCache;
            _clientManager = clientManager;
        }

        // When share is invoked (by the user or programatically) the event handler we registered will be called to populate the datapackage with the
        // data to be shared.
        private async void OnDataRequested(DataTransferManager sender, DataRequestedEventArgs e)
        {
            // Call the scenario specific function to populate the datapackage with the data to be shared.
            try
            {
                var deferral = e.Request.GetDeferral();

                var tempFolder = ApplicationData.Current.TemporaryFolder;
                var file =
                    await tempFolder.CreateFileAsync("DrinkOBand_Export.csv",
                        CreationCollisionOption.ReplaceExisting);
                var items = await _drinkLogRepository.GetAllEntriesAsync();
                var line = "Date;Amount;";
                using (var streamWriter = new StreamWriter(await file.OpenStreamForWriteAsync()))
                {
                    await streamWriter.WriteLineAsync(line);

                    foreach (var drinkLogItem in items)
                    {
                        line = String.Format("{0};{1};", drinkLogItem.Timestamp.ToString("yyyy-MM-dd hh:mm:ss"), drinkLogItem.Amount);
                        await streamWriter.WriteLineAsync(line);
                    }
                }

                e.Request.Data.SetStorageItems(new[] {file});
                e.Request.Data.SetText("Data export from Drink O'Band");
                e.Request.Data.Properties.Title = file.Name;
                deferral.Complete();
            }
            catch(Exception ex)
            {
                _logCache.Log(ex.Message, LogCacheLogLevel.Error);
                e.Request.FailWithDisplayText(_resourceRepository.GetString("msgSharingExportFailed"));
            }
        }

        public override void OnNavigatedTo(object navigationParameter)
        {
            base.OnNavigatedTo(navigationParameter);

            _eventAggregator.GetEvent<AppStateChangeEvent>().Subscribe(AppStateChanged);
            _eventAggregator.GetEvent<SyncRequestedEvent>().Subscribe(SyncRequested);

            // Register the current page as a share source.
            this.dataTransferManager = DataTransferManager.GetForCurrentView();
            this.dataTransferManager.DataRequested += new TypedEventHandler<DataTransferManager, DataRequestedEventArgs>(this.OnDataRequested);

            // check if version info has been shown for this version already
            var packageVersion = VersionInfo.GetAppVersion();
#if DEBUG
            _settingsStore.VersionInfoLastShown = null;
#endif
            if (this._settingsStore.VersionInfoLastShown == null || _settingsStore.VersionInfoLastShown != packageVersion)
            {
                var lang = CultureInfo.CurrentUICulture.ToString();
                if (lang.ToLower().StartsWith("en"))
                {
                    VersionInfoUrl = "ms-appx-web:///html/VERSION.en.html";
                }
                else if(lang.ToLower().StartsWith("de"))
                {
                    VersionInfoUrl = "ms-appx-web:///html/VERSION.de.html";
                }
                VersionInfoVisible = true;
                _settingsStore.VersionInfoLastShown = packageVersion;
            }

        }

        public override void OnNavigatedFrom()
        {
            base.OnNavigatedFrom();

            _eventAggregator.GetEvent<AppStateChangeEvent>().Unsubscribe(AppStateChanged);
            _eventAggregator.GetEvent<SyncRequestedEvent>().Unsubscribe(SyncRequested);

            // Unregister the current page as a share source.
            this.dataTransferManager.DataRequested -= new TypedEventHandler<DataTransferManager, DataRequestedEventArgs>(this.OnDataRequested);
        }

        private void SyncRequested(SyncRequestedEventParameters syncRequestedEventParameters)
        {
            SynchronizationContext.Post(async state =>
            {
                await StartCloudSync();
            }, null);

        }

        public override async void Loaded()
        {
            base.Loaded();

            _navigationService.Navigate<DailyProgressPageViewModel>();

            await StartCloudSync();
        }

        private async Task StartCloudSync()
        {
            if (_settingsStore.CloudEnabled && !_syncInProgress)
            {
                _syncInProgress = true;
#if WINDOWS_UWP
                if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
                {
#endif
                    var statusBar = Windows.UI.ViewManagement.StatusBar.GetForCurrentView();
                    await statusBar.ShowAsync();
                    await statusBar.ProgressIndicator.ShowAsync();
                    statusBar.ProgressIndicator.Text = "Syncing...";
#if WINDOWS_UWP
                }
                else
                {
                    IsBusy = true;
                }
#endif
                bool authResult = false;
                try
                {
                    authResult = await Resolver.Resolve<IMobileServicesAuthentication>().AuthenticateAsync();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
                try
                {
                    if (authResult)
                    {
                        // open push channel
                        if (channel == null)
                        {
                            try
                            {
                                channel =
                                    await
                                        PushNotificationChannelManager.CreatePushNotificationChannelForApplicationAsync();
                                await _clientManager.MobileService.GetPush().RegisterAsync(channel.Uri);

                                channel.PushNotificationReceived += OnPushNotification;

                            }
                            catch (Exception ex)
                            {
                                // ... 
                            }
                        }

                        PasswordVault vault = new PasswordVault();
                        var credential = vault.FindAllByResource(MobileServiceAuthenticationProvider.MicrosoftAccount.ToString()).FirstOrDefault();
                        await Resolver.Resolve<IDrinkLogRepository>().SyncAsync(credential.UserName);
                    }
                    else
                    {
                        //_settingsStore.CloudEnabled = false; //TODO send message to settings page to disable cloud backup
                        //await new MessageDialog(_resourceRepository.GetString("toastServiceAuthenticationFailed")).ShowAsync();
                    }
                }
                finally
                {
#if WINDOWS_UWP
                    if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
                    {
                        var statusBar = Windows.UI.ViewManagement.StatusBar.GetForCurrentView();
#endif

                        await statusBar.ProgressIndicator.HideAsync();
#if WINDOWS_UWP
                    }
                    else
                    {
                        IsBusy = false;
                    }
#endif
                    _syncInProgress = false;
                }
            }
        }

        private void AppStateChanged(AppStateEvent newState)
        {
            if (newState == AppStateEvent.OnResuming)
            {
                SynchronizationContext.Post(async state =>
                {
                    await StartCloudSync();
                }, null);
            }
        }


#region Properties

        private int _popup = 0;
        public int Popup
        {
            get { return _popup; }
            set { SetProperty(ref _popup, value); }
        }

        private bool _isBusy;

        public bool IsBusy
        {
            get { return _isBusy; }
            set { SetProperty(ref _isBusy, value); }
        }

        private bool _versionInfoVisible;

        public bool VersionInfoVisible
        {
            get { return _versionInfoVisible; }
            set { SetProperty(ref _versionInfoVisible, value); }
        }

        private string _versionInfoUrl;

        public string VersionInfoUrl
        {
            get { return _versionInfoUrl; }
            set { SetProperty(ref _versionInfoUrl, value); }
        }

#endregion

#region Commands

        public DelegateCommand MenuCommand {
            get
            {
                return new DelegateCommand(() =>
                {
                    Popup = Popup == 1 ? 0 : 1;
                });
            }
        }

        public DelegateCommand TestNotificationCommand
        {
            get
            {
                return new DelegateCommand(() =>
                {

                });
            }
        }

        public DelegateCommand ClosePopupCommand
        {
            get
            {
                return new DelegateCommand(() =>
                {
                    Popup = 0;
                });
            }
        }

        public DelegateCommand SettingsCommand
        {
            get
            {
                return new DelegateCommand(() =>
                {
                    Popup = 0;
                    _navigationService.Navigate<SettingsPageViewModel>();
                });
            }
        }

        public DelegateCommand HomeCommand
        {
            get
            {
                return new DelegateCommand(() =>
                {
                    Popup = 0;
                    _navigationService.Navigate<DailyProgressPageViewModel>();
                });
            }
        }

        public DelegateCommand HistoryCommand
        {
            get
            {
                return new DelegateCommand(() =>
                {
                    Popup = 0;
                    _navigationService.Navigate<HistoryPageViewModel>();
                });
            }
        }

        public DelegateCommand InfoCommand
        {
            get
            {
                return new DelegateCommand(() =>
                {
                    Popup = 0;
                    _navigationService.Navigate<InfoPageViewModel>();
                });
            }
        }

        public DelegateCommand ExportCommand
        {
            get
            {
                return new DelegateCommand(() =>
                {
                    Popup = 0;
                    DataTransferManager.ShowShareUI();
                });
            }
        }

        public DelegateCommand VersionInfoPopupCloseCommand
        {
            get
            {
                return new DelegateCommand(() =>
                {
                    VersionInfoVisible = false;
                });
            }
        }

        #endregion


        private async void OnPushNotification(PushNotificationChannel sender, PushNotificationReceivedEventArgs e)
        {
            String notificationContent = String.Empty;

            switch (e.NotificationType)
            {
                case PushNotificationType.Badge:
                    notificationContent = e.BadgeNotification.Content.GetXml();
                    break;

                case PushNotificationType.Tile:
                    notificationContent = e.TileNotification.Content.GetXml();
                    break;

                case PushNotificationType.Toast:
                    notificationContent = e.ToastNotification.Content.GetXml();
                    break;

                case PushNotificationType.Raw:
                    notificationContent = e.RawNotification.Content;
                    if (notificationContent == "update")
                    {
                        Resolver.Resolve<IEventAggregator>().GetEvent<SyncRequestedEvent>().Publish(new SyncRequestedEventParameters());
                        e.Cancel = true;
                    }
                    break;
            }

            e.Cancel = true;
        }


    }
}