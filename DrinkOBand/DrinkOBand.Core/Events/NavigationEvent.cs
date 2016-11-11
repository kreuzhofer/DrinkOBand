using System;
using Microsoft.Practices.Prism.PubSubEvents;

namespace DrinkOBand.Core.Events
{
    public class NavigationEvent : PubSubEvent<INavigationEventPayload>
    {
         
    }

    public interface INavigationEventPayload { }

    public class NavigationEventPayload : INavigationEventPayload
    {
        public Type PageType { get; set; }
        public object NavigationParameter { get; set; }
    }

    public class NavigateRootFrameEventPayload : INavigationEventPayload
    {
        public Type PageType { get; set; }
        public object NavigationParameter { get; set; }
    }

    public class NavigateBackEventPayload : INavigationEventPayload { }
}