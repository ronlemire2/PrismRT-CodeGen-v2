using System;
using Windows.UI.Xaml.Data;

namespace BigBrother.UILayer.Converters
{
    public class BooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return value.ToString();
        }
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return Boolean.Parse(value as string);
        }
    }
}
