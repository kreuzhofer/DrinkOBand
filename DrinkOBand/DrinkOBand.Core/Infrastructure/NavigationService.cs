using System;
using System.Collections.Generic;
using DrinkOBand.Core.Events;
using Microsoft.Practices.Prism.PubSubEvents;

namespace DrinkOBand.Core.Infrastructure
{
    public class NavigationService : INavigationService
    {
        private Dictionary<Type, Type> _viewMappings = new Dictionary<Type, Type>();
        private IEventAggregator _eventAggregator;

        public NavigationService(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
        }

        public void Navigate(Type viewModelType, object navigationParameter = null)
        {
            _eventAggregator.GetEvent<NavigationEvent>().Publish(new NavigationEventPayload {PageType = _viewMappings[viewModelType], NavigationParameter = navigationParameter});
        }

        public void Navigate<TViewModel>(object navigationParameter = null)
        {
            Navigate(typeof(TViewModel), navigationParameter);
        }

        public void NavigateRoot(Type viewModelType, object navigationParameter = null)
        {
            _eventAggregator.GetEvent<NavigationEvent>().Publish(new NavigateRootFrameEventPayload { PageType = _viewMappings[viewModelType], NavigationParameter = navigationParameter });
        }

        public void NavigateRoot<TViewModel>(object navigationParameter = null)
        {
            Navigate(typeof(TViewModel), navigationParameter);
        }

        public void NavigateBack()
        {
            _eventAggregator.GetEvent<NavigationEvent>().Publish(new NavigateBackEventPayload());
        }

        public void RegisterView<TViewModel, TView>()
        {
            _viewMappings.Add(typeof(TViewModel), typeof(TView));
        }
    }
}