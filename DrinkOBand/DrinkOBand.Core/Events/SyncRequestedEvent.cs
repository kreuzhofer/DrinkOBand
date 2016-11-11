using Microsoft.Practices.Prism.PubSubEvents;

namespace DrinkOBand.Core.Events
{
    public class SyncRequestedEvent : PubSubEvent<SyncRequestedEventParameters>
    {
         
    }

    public class SyncRequestedEventParameters
    {
    }
}