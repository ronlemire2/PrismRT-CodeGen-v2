// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved


using Microsoft.Practices.Prism.StoreApps;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.UI.Xaml;

namespace BigBrother.UILayer.Behaviors
{
    // Documentation on validating user input is at http://go.microsoft.com/fwlink/?LinkID=288817&clcid=0x409

    public static class HighlightFormFieldOnErrors
    {
        public static DependencyProperty PropertyErrorsProperty =
            DependencyProperty.RegisterAttached("PropertyErrors", typeof(ReadOnlyCollection<string>), typeof(HighlightFormFieldOnErrors), new PropertyMetadata(BindableValidator.EmptyErrorsCollection, OnPropertyErrorsChanged));

        public static ReadOnlyCollection<string> GetPropertyErrors(DependencyObject sender)
        {
            if (sender == null)
            {
                return null;
            }

            return (ReadOnlyCollection<string>)sender.GetValue(PropertyErrorsProperty);
        }

        public static void SetPropertyErrors(DependencyObject sender, ReadOnlyCollection<string> value)
        {
            if (sender == null)
            {
                return;
            }

            sender.SetValue(PropertyErrorsProperty, value);
        }

        private static void OnPropertyErrorsChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            if (args == null || args.NewValue == null)
            {
                return;
            }

            var control = (FrameworkElement)d;
            var propertyErrors = (ReadOnlyCollection<string>)args.NewValue;

            Style style = (propertyErrors.Any()) ? (Style)Application.Current.Resources["HighlightFormFieldStyle"] : (Style)Application.Current.Resources["FormFieldStyle"];
            control.Style = style;
        }
    }
}
