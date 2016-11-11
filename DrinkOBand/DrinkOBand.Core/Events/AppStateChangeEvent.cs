using Microsoft.Practices.Prism.PubSubEvents;

namespace DrinkOBand.Core.Events
{
    public class AppStateChangeEvent : PubSubEvent<AppStateEvent>
    {
         
    }

    public enum AppStateEvent
    {
        OnSuspending,
        OnResuming
    }
}