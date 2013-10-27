using System;
using Windows.UI.Xaml.Data;

namespace PrismStarter.UILayer.Converters
{
    public class DecmialConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language) {
            return value.ToString();
        }
        public object ConvertBack(object value, Type targetType, object parameter, string language) {
            return decimal.Parse(value as string);
        }
    }
}