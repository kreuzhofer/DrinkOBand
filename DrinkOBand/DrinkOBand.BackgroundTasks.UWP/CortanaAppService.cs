using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.ApplicationModel.AppService;
using Windows.ApplicationModel.Background;
using Windows.ApplicationModel.VoiceCommands;
using Windows.Storage;
using DrinkOBand.Common;
using DrinkOBand.Common.Infrastructure;
using DrinkOBand.Core;
using DrinkOBand.Core.Entities;
using DrinkOBand.Core.Helpers;
using DrinkOBand.Core.Infrastructure;
using Microsoft.Practices.Unity;
using Plugin.Settings;
using Plugin.Settings.Abstractions;

namespace DrinkOBand.BackgroundTasks.UWP
{
    public sealed class CortanaAppService: IBackgroundTask
    {
        private BackgroundTaskDeferral serviceDeferral;
        VoiceCommandServiceConnection voiceServiceConnection;

        private IUnitHelper _unitHelper;
        private IDrinkLogCache _drinkLogCache;
        private ISettingsStore _settingsStore;
        private ILiveTileUpdater _liveTileUpdater;

        public CortanaAppService()
        {
            // init Resolver
            Resolver.Container.RegisterInstance<ILogCache>(new LogCache());
            Resolver.Container.RegisterInstance<ISettings>(CrossSettings.Current);
            Resolver.Container.RegisterType<ISettingsStore, SettingsStore>();
            Resolver.Container.RegisterType<IResourceRepository, ResourceRepository>();
            Resolver.Container.RegisterType<ILiveTileUpdater, LiveTileUpdater>();
            Resolver.Container.RegisterType<IUnitHelper, UnitHelper>();
            Resolver.Container.RegisterType<IToastHelper, ToastHelper>();
            Resolver.Container.RegisterType<IDrinkLogCache, DrinkLogCache>();

            _unitHelper = Resolver.Resolve<IUnitHelper>();
            _drinkLogCache = Resolver.Resolve<IDrinkLogCache>();
            _settingsStore = Resolver.Resolve<ISettingsStore>();
            _liveTileUpdater = Resolver.Resolve<ILiveTileUpdater>();
        }


        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            this.serviceDeferral = taskInstance.GetDeferral();

            taskInstance.Canceled += OnTaskCanceled;

            var triggerDetails =
              taskInstance.TriggerDetails as AppServiceTriggerDetails;

            if (triggerDetails != null &&
              triggerDetails.Name == "CortanaAppService")
            {
                try
                {
                    voiceServiceConnection =
                      VoiceCommandServiceConnection.FromAppServiceTriggerDetails(
                        triggerDetails);

                    voiceServiceConnection.VoiceCommandCompleted +=
                      VoiceCommandCompleted;

                    VoiceCommand voiceCommand = await voiceServiceConnection.GetVoiceCommandAsync();

                    switch (voiceCommand.CommandName)
                    {
                        case "drinkAGlass":
                        {
                            var subject = voiceCommand.Properties["subject"][0];
                            var amount = voiceCommand.Properties["amount"][0];
                            await SendCompletionMessageForFixedAmount(amount, subject);
                            break;
                        }
                        case "drinkAmount":
                            {
                                var amount = voiceCommand.SpeechRecognitionResult.SemanticInterpretation.Properties["amount"][0];
                                var beverage = voiceCommand.Properties["beverage"][0];
                                await SendCompletionMessageForAmount(amount, beverage);
                                break;
                            }

                        // As a last resort launch the app in the foreground
                        default:
                            LaunchAppInForeground();
                            break;
                    }
                }
                finally
                {
                    //if (this.serviceDeferral != null)
                    //{
                    //    //Complete the service deferral
                    //    this.serviceDeferral.Complete();
                    //}
                }
            }
        }

        private async Task SendCompletionMessageForFixedAmount(string amount, string subject)
        {
            var userMessage = new VoiceCommandUserMessage();
            int amountnumber;
            if (int.TryParse(amount, out amountnumber))
            {
                userMessage.DisplayMessage = String.Format("Das habe ich gespeichert.", amount, subject);
                userMessage.SpokenMessage = String.Format("Ich habe {0} {1} gespeichert.", amount, subject);

                var contentTiles = new List<VoiceCommandContentTile>();
                contentTiles.Add(new VoiceCommandContentTile()
                {
                    ContentTileType = VoiceCommandContentTileType.TitleOnly,
                    Title = String.Format("{0} {1} gespeichert.", amount, subject)
                });

                var response = VoiceCommandResponse.CreateResponse(userMessage, contentTiles);
                await voiceServiceConnection.ReportSuccessAsync(response);
            }
            else
            {
                
            }
            await Task.Delay(2000);
        }

        private async Task SendCompletionMessageForAmount(string amount, string beverage)
        {
            var userMessage = new VoiceCommandUserMessage();

            userMessage.DisplayMessage = String.Format("Das habe ich gespeichert.");
            userMessage.SpokenMessage = String.Format("Ich habe {0}ml {1} gespeichert.", amount, beverage);

            var contentTiles = new List<VoiceCommandContentTile>();
            contentTiles.Add(new VoiceCommandContentTile()
            {
                ContentTileType = VoiceCommandContentTileType.TitleOnly,
                Title = String.Format("{0}ml {1}", amount, beverage)
            });

            var response = VoiceCommandResponse.CreateResponse(userMessage, contentTiles);
            await voiceServiceConnection.ReportSuccessAsync(response);
            await Task.Delay(2000);
        }



        /// <summary>
        /// Provide a simple response that launches the app. Expected to be used in the
        /// case where the voice command could not be recognized (eg, a VCD/code mismatch.)
        /// </summary>
        private async void LaunchAppInForeground()
        {
            var userMessage = new VoiceCommandUserMessage();
            userMessage.SpokenMessage = "Starte Drink O'Band";

            var response = VoiceCommandResponse.CreateResponse(userMessage);

            response.AppLaunchArgument = "";

            await voiceServiceConnection.RequestAppLaunchAsync(response);
        }

        private void VoiceCommandCompleted(
          VoiceCommandServiceConnection sender,
          VoiceCommandCompletedEventArgs args)
        {
            if (this.serviceDeferral != null)
            {
                // Insert your code here
                //Complete the service deferral
                this.serviceDeferral.Complete();
            }
        }

        /// <summary>
        /// When the background task is cancelled, clean up/cancel any ongoing long-running operations.
        /// This cancellation notice may not be due to Cortana directly. The voice command connection will
        /// typically already be destroyed by this point and should not be expected to be active.
        /// </summary>
        /// <param name="sender">This background task instance</param>
        /// <param name="reason">Contains an enumeration with the reason for task cancellation</param>
        private void OnTaskCanceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            System.Diagnostics.Debug.WriteLine("Task cancelled, clean up");
            if (this.serviceDeferral != null)
            {
                //Complete the service deferral
                this.serviceDeferral.Complete();
            }
        }

    }
}