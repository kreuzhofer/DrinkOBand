using System;
using Windows.UI.Xaml.Data;

namespace DrinkOBand.Common.Converters
{
    public class SelectedUnitSystemConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return value.ToString() == parameter.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if ((bool) value)
            {
                return parameter;
            }
            else
            {
                return null;
            }
        }
    }
}