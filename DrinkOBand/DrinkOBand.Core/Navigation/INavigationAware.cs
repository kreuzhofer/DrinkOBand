namespace DrinkOBand.Common
{
    public interface INavigationAware
    {
        void OnNavigatedTo(object parameter);
        void OnNavigatedFrom();
        void Loaded();
    }
}