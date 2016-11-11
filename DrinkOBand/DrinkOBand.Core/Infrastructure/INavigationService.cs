using System;

namespace DrinkOBand.Core.Infrastructure
{
    public interface INavigationService
    {
        void RegisterView<TViewModel, TPage>();
        void Navigate(Type viewModelType, object navigationParameter = null);
        void Navigate<TViewModel>(object navigationParameter = null);
        void NavigateBack();
        void NavigateRoot(Type viewModelType, object navigationParameter = null);
        void NavigateRoot<TViewModel>(object navigationParameter = null);
    }
}