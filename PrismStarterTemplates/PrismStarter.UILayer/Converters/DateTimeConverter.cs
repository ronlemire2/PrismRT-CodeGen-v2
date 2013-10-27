using System;
using Windows.UI.Xaml.Data;

namespace PrismStarter.UILayer.Converters
{
    public class DateTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language) {
            return value.ToString();
        }
        public object ConvertBack(object value, Type targetType, object parameter, string language) {
            return DateTime.Parse(value as string);
        }
    }
}
