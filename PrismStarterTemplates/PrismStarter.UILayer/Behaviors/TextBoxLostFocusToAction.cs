using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace PrismStarter.UILayer.Behaviors
{
    public static class TextBoxLostFocusToAction
    {
        public static DependencyProperty ActionProperty =
            DependencyProperty.RegisterAttached("Action", typeof(Action<object>), typeof(TextBoxLostFocusToAction), new PropertyMetadata(null, OnActionChanged));

        public static void SetAction(DependencyObject sender, Action<object> action) {
            if (sender == null) {
                return;
            }

            sender.SetValue(ActionProperty, action);
        }

        public static Action<object> GetAction(DependencyObject sender) {
            if (sender == null) {
                return null;
            }

            return (Action<object>)sender.GetValue(ActionProperty);
        }

        private static void OnActionChanged(DependencyObject d, DependencyPropertyChangedEventArgs args) {
            TextBox textBox = (TextBox)d;

            if (textBox != null) {
                textBox.LostFocus += textBox_LostFocus;
            }
        }

        static void textBox_LostFocus(object sender, RoutedEventArgs e) {
            var textBox = (TextBox)sender;
            Action<object> action = (Action<object>)textBox.GetValue(ActionProperty);
            action(e.OriginalSource);
        }
    }
}