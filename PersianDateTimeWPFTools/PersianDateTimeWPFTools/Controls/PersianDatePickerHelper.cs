using PersianDateTimeWPFTools.Controls;
using PersianDateTimeWPFTools.Tools;
using PersianDateTimeWPFTools.Windows.Controls.Primitives;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
//using System.Windows.Controls.Primitives;
using System.Windows.Data;
#if NET8_0_OR_GREATER
#nullable disable
#endif
namespace PersianDateTimeWPFTools.Controls
{
    public static class PersianDatePickerHelper
    {
        private static readonly PersianDatePickerHelper.FirstNotNullOrEmptyConverter _watermarkConverter = new PersianDatePickerHelper.FirstNotNullOrEmptyConverter();
        public static readonly DependencyProperty IsEnabledProperty = DependencyProperty.RegisterAttached("IsEnabled", typeof(bool), typeof(PersianDatePickerHelper), new PropertyMetadata(new PropertyChangedCallback(PersianDatePickerHelper.OnIsEnabledChanged)));

        public static bool GetIsEnabled(PersianDatePicker datePicker)
        {
            return (bool)datePicker.GetValue(PersianDatePickerHelper.IsEnabledProperty);
        }

        public static void SetIsEnabled(PersianDatePicker datePicker, bool value)
        {
            datePicker.SetValue(PersianDatePickerHelper.IsEnabledProperty, (object)value);
        }

        private static void OnIsEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PersianDatePicker persianDatePicker = (PersianDatePicker)d;
            if ((bool)e.NewValue)
                persianDatePicker.Loaded += new RoutedEventHandler(PersianDatePickerHelper.OnLoaded);
            else
                persianDatePicker.Loaded -= new RoutedEventHandler(PersianDatePickerHelper.OnLoaded);
        }

        private static void OnLoaded(object sender, RoutedEventArgs e)
        {
            PersianDatePicker control = (PersianDatePicker)sender;
            control.Loaded -= new RoutedEventHandler(PersianDatePickerHelper.OnLoaded);
            DatePickerTextBox templateChild1 = control.GetTemplateChild<DatePickerTextBox>("PART_TextBox");
            if (templateChild1 == null)
                return;
            System.Windows.Controls.ContentControl templateChild2 = templateChild1.GetTemplateChild<System.Windows.Controls.ContentControl>("PART_Watermark");
            if (templateChild2 == null)
                return;
            Binding binding1 = new Binding()
            {
                Path = new PropertyPath((object)ControlHelper.PlaceholderTextProperty),
                Source = (object)control
            };
            BindingExpression bindingExpression = templateChild2.GetBindingExpression(System.Windows.Controls.ContentControl.ContentProperty);
            BindingBase binding2;
            if (bindingExpression != null)
                binding2 = (BindingBase)new MultiBinding()
                {
                    Bindings = {
            (BindingBase) binding1,
            (BindingBase) bindingExpression.ParentBinding
          },
                    Converter = (IMultiValueConverter)PersianDatePickerHelper._watermarkConverter
                };
            else
                binding2 = (BindingBase)binding1;
            templateChild2.SetBinding(ContentControl.ContentProperty, binding2);
        }

        private class FirstNotNullOrEmptyConverter : IMultiValueConverter
        {
            public object Convert(
              object[] values,
              Type targetType,
              object parameter,
              CultureInfo culture)
            {
                foreach (object obj in values)
                {
                    if (obj is string str)
                    {
                        if (!string.IsNullOrEmpty(str))
                            return (object)str;
                    }
                    else if (obj != null)
                        return obj;
                }
                return (object)null;
            }

            public object[] ConvertBack(
              object value,
              Type[] targetTypes,
              object parameter,
              CultureInfo culture)
            {
                throw new NotImplementedException();
            }
        }
    }
}
