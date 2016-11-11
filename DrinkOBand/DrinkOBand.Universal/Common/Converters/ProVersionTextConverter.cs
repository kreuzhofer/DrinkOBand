using System;
using Windows.UI.Xaml.Data;

namespace DrinkOBand.Common.Converters
{
    public class ProVersionTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is bool)
            {
                var isPro = (bool)value;
                return isPro ? "Drink O'Band Pro" : "Drink O'Band";
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}