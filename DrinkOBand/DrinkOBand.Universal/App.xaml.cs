using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Background;
using Windows.ApplicationModel.Store;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.SpeechRecognition;
using Windows.Networking.PushNotifications;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using DrinkOBand.Common;
using DrinkOBand.Common.Infrastructure;
using DrinkOBand.Core;
using DrinkOBand.Core.Events;
using DrinkOBand.Core.Helpers;
using DrinkOBand.Core.Infrastructure;
using DrinkOBand.Universal.Infrastructure;
using DrinkOBand.ViewModels;
using DrinkOBand.Views;
using Microsoft.Practices.Prism.Mvvm;
using Microsoft.Practices.Prism.PubSubEvents;
using Microsoft.Practices.Unity;
using Microsoft.WindowsAzure.MobileServices;
using Plugin.Settings;
using Plugin.Settings.Abstractions;
using MobileServicesAuthentication = DrinkOBand.Universal.Infrastructure.MobileServicesAuthentication;
#if STORESIMULATOR
using IAPEngine = Windows.ApplicationModel.Store.CurrentAppSimulator;
#else
using IAPEngine = Windows.ApplicationModel.Store.CurrentApp;
#endif

// The Blank Application template is documented at http://go.microsoft.com/fwlink/?LinkId=391641

namespace DrinkOBand
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public sealed partial class App : Application
    {
        private TransitionCollection transitions;

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += this.OnSuspending;
            this.Resuming += OnResuming;

            Initialize();
        }

        private void Initialize()
        {
            // register types, platform specific
            Resolver.Container.RegisterInstance<ILogCache>(new LogCache());
            Resolver.Container.RegisterType<IToastHelper, ToastHelper>();
            Resolver.Container.RegisterType<ILiveTileUpdater, LiveTileUpdater>();
            Resolver.Container.RegisterType<IResourceRepository, ResourceRepository>(new ContainerControlledLifetimeManager());
            Resolver.Container.RegisterType<IMobileServicesAuthentication, MobileServicesAuthentication>();

            // register container extensions
            Resolver.Container.AddNewExtension<CoreUnityContainerExtension>();
            Resolver.Container.AddNewExtension<CommonUnityContainerExtension>();

            // register view mappings
            Resolver.Resolve<INavigationService>().RegisterView<DailyProgressPageViewModel, DailyProgressPage>();
            Resolver.Resolve<INavigationService>().RegisterView<SettingsPageViewModel, SettingsPage>();
            Resolver.Resolve<INavigationService>().RegisterView<HistoryPageViewModel, HistoryPage>();
            Resolver.Resolve<INavigationService>().RegisterView<InfoPageViewModel, InfoPage>();
            Resolver.Resolve<INavigationService>().RegisterView<SetupUnitSystemPageViewModel, SetupUnitSystemPage>();

            ViewModelLocationProvider.SetDefaultViewModelFactory(
                delegate(Type viewtype)
                {
                    var result = Resolver.Container.Resolve(viewtype);
                    return result;
                });
        }


        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used when the application is launched to open a specific file, to display
        /// search results, and so forth.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override async void OnLaunched(LaunchActivatedEventArgs e)
        {
#if DEBUG
            //if (System.Diagnostics.Debugger.IsAttached)
            //{
            //    this.DebugSettings.EnableFrameRateCounter = true;
            //}
#endif

#if WINDOWS_UWP
#pragma warning disable 4014
            InstallCortanaCommands(); // start installation in the background. this will take very long in the latest preview builds for whatever reason :-(
#pragma warning restore 4014
#endif
            Frame rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                // TODO: change this value to a cache size that is appropriate for your application
                rootFrame.CacheSize = 1;

                // Set the default language
                rootFrame.Language = Windows.Globalization.ApplicationLanguages.Languages[0];

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    // TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            if (rootFrame.Content == null)
            {
                // Removes the turnstile navigation for startup.
                if (rootFrame.ContentTransitions != null)
                {
                    this.transitions = new TransitionCollection();
                    foreach (var c in rootFrame.ContentTransitions)
                    {
                        this.transitions.Add(c);
                    }
                }

                rootFrame.ContentTransitions = null;
                rootFrame.Navigated += this.RootFrame_FirstNavigated;

                // When the navigation stack isn't restored navigate to the first page,
                // configuring the new page by passing required information as a navigation
                // parameter

                if (!rootFrame.Navigate(typeof(ExtendedSplash), e.Arguments))
                {
                    throw new Exception("Failed to create initial page");
                }
            }

            // Ensure the current window is active
            Window.Current.Activate();

            
        }

        private async Task InstallCortanaCommands()
        {
            try
            {
                // Register Cortana commands
                var storageFile =
                    await Package.Current.InstalledLocation.GetFileAsync(@"CortanaCommands.xml");
                await
                    Windows.ApplicationModel.VoiceCommands.VoiceCommandDefinitionManager
                        .InstallCommandDefinitionsFromStorageFileAsync(storageFile);
            }
            catch
            {
                // TODO notify user about this
            }
        }

        /// <summary>
        /// Restores the content transitions after the app has launched.
        /// </summary>
        /// <param name="sender">The object where the handler is attached.</param>
        /// <param name="e">Details about the navigation event.</param>
        private void RootFrame_FirstNavigated(object sender, NavigationEventArgs e)
        {
            var rootFrame = sender as Frame;
            rootFrame.ContentTransitions = this.transitions ?? new TransitionCollection() { new NavigationThemeTransition() };
            rootFrame.Navigated -= this.RootFrame_FirstNavigated;
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            Debug.WriteLine("OnSuspending");
            Resolver.Resolve<IEventAggregator>().GetEvent<AppStateChangeEvent>().Publish(AppStateEvent.OnSuspending);
            var deferral = e.SuspendingOperation.GetDeferral();

            // TODO: Save application state and stop any background activity
            deferral.Complete();
        }
        private void OnResuming(object sender, object o)
        {
            Debug.WriteLine("OnResuming");
            Resolver.Resolve<IEventAggregator>().GetEvent<AppStateChangeEvent>().Publish(AppStateEvent.OnResuming);
        }


        protected override void OnActivated(IActivatedEventArgs args)
        {
            // Windows Phone 8.1 requires you to handle the respose from the WebAuthenticationBroker.
#if WINDOWS_PHONE_APP
            if (args.Kind == ActivationKind.WebAuthenticationBrokerContinuation)
            {
                // Completes the sign-in process started by LoginAsync.
                // Change 'MobileService' to the name of your MobileServiceClient instance. 
                try
                {
                    App.MobileService.LoginComplete(args as WebAuthenticationBrokerContinuationEventArgs);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Error while authenticating: {0}", ex);
                }
            }
#endif
#if WINDOWS_UWP
            if (args.Kind == ActivationKind.VoiceCommand)
            {
                var commandArgs = args as Windows.ApplicationModel.Activation.VoiceCommandActivatedEventArgs;

                var speechRecognitionResult = commandArgs.Result;
            }
#endif

            base.OnActivated(args);
        }
    }
}