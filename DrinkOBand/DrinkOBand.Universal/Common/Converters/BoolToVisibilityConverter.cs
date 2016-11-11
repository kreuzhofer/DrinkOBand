using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace DrinkOBand.Common.Converters
{
    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (!(value is bool))
            {
                return Visibility.Collapsed;
            }
            var visible = (bool)value;
            if (parameter != null && parameter.ToString() == "conv")
            {
                visible = !visible;
            }
            return visible ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }
    }
}