using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

#if NET8_0_OR_GREATER
#nullable disable
#endif
namespace PersianDateTimeWPFTools.Windows.Controls.Primitives
{
    public static class ControlHelper
    {
        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.RegisterAttached("CornerRadius", typeof(CornerRadius), typeof(ControlHelper), (PropertyMetadata)null);
        public static readonly DependencyProperty HeaderProperty = DependencyProperty.RegisterAttached("Header", typeof(object), typeof(ControlHelper), (PropertyMetadata)new FrameworkPropertyMetadata(new PropertyChangedCallback(ControlHelper.OnHeaderChanged)));
        public static readonly DependencyProperty HeaderTemplateProperty = DependencyProperty.RegisterAttached("HeaderTemplate", typeof(DataTemplate), typeof(ControlHelper), (PropertyMetadata)new FrameworkPropertyMetadata(new PropertyChangedCallback(ControlHelper.OnHeaderTemplateChanged)));
        private static readonly DependencyPropertyKey HeaderVisibilityPropertyKey = DependencyProperty.RegisterAttachedReadOnly("HeaderVisibility", typeof(Visibility), typeof(ControlHelper), (PropertyMetadata)new FrameworkPropertyMetadata((object)Visibility.Collapsed));
        public static readonly DependencyProperty HeaderVisibilityProperty = ControlHelper.HeaderVisibilityPropertyKey.DependencyProperty;
        public static readonly DependencyProperty PlaceholderTextProperty = DependencyProperty.RegisterAttached("PlaceholderText", typeof(string), typeof(ControlHelper), (PropertyMetadata)new FrameworkPropertyMetadata((object)string.Empty, new PropertyChangedCallback(ControlHelper.OnPlaceholderTextChanged)));
        private static readonly DependencyPropertyKey PlaceholderTextVisibilityPropertyKey = DependencyProperty.RegisterAttachedReadOnly("PlaceholderTextVisibility", typeof(Visibility), typeof(ControlHelper), (PropertyMetadata)new FrameworkPropertyMetadata((object)Visibility.Collapsed));
        public static readonly DependencyProperty PlaceholderTextVisibilityProperty = ControlHelper.PlaceholderTextVisibilityPropertyKey.DependencyProperty;
        public static readonly DependencyProperty PlaceholderForegroundProperty = DependencyProperty.RegisterAttached("PlaceholderForeground", typeof(Brush), typeof(ControlHelper), (PropertyMetadata)null);
        public static readonly DependencyProperty DescriptionProperty = DependencyProperty.RegisterAttached("Description", typeof(object), typeof(ControlHelper), (PropertyMetadata)new FrameworkPropertyMetadata(new PropertyChangedCallback(ControlHelper.OnDescriptionChanged)));
        private static readonly DependencyPropertyKey DescriptionVisibilityPropertyKey = DependencyProperty.RegisterAttachedReadOnly("DescriptionVisibility", typeof(Visibility), typeof(ControlHelper), (PropertyMetadata)new FrameworkPropertyMetadata((object)Visibility.Collapsed));
        public static readonly DependencyProperty DescriptionVisibilityProperty = ControlHelper.DescriptionVisibilityPropertyKey.DependencyProperty;

        public static CornerRadius GetCornerRadius(Control control)
        {
            return (CornerRadius)control.GetValue(ControlHelper.CornerRadiusProperty);
        }

        public static void SetCornerRadius(Control control, CornerRadius value)
        {
            control.SetValue(ControlHelper.CornerRadiusProperty, (object)value);
        }

        public static object GetHeader(Control control)
        {
            return control.GetValue(ControlHelper.HeaderProperty);
        }

        public static void SetHeader(Control control, object value)
        {
            control.SetValue(ControlHelper.HeaderProperty, value);
        }

        private static void OnHeaderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ControlHelper.UpdateHeaderVisibility((Control)d);
        }

        public static DataTemplate GetHeaderTemplate(Control control)
        {
            return (DataTemplate)control.GetValue(ControlHelper.HeaderTemplateProperty);
        }

        public static void SetHeaderTemplate(Control control, DataTemplate value)
        {
            control.SetValue(ControlHelper.HeaderTemplateProperty, (object)value);
        }

        private static void OnHeaderTemplateChanged(
          DependencyObject d,
          DependencyPropertyChangedEventArgs e)
        {
            ControlHelper.UpdateHeaderVisibility((Control)d);
        }

        public static Visibility GetHeaderVisibility(Control control)
        {
            return (Visibility)control.GetValue(ControlHelper.HeaderVisibilityProperty);
        }

        private static void SetHeaderVisibility(Control control, Visibility value)
        {
            control.SetValue(ControlHelper.HeaderVisibilityPropertyKey, (object)value);
        }

        private static void UpdateHeaderVisibility(Control control)
        {
            Visibility visibility = ControlHelper.GetHeaderTemplate(control) == null ? (ControlHelper.IsNullOrEmptyString(ControlHelper.GetHeader(control)) ? Visibility.Collapsed : Visibility.Visible) : Visibility.Visible;
            ControlHelper.SetHeaderVisibility(control, visibility);
        }

        public static string GetPlaceholderText(Control control)
        {
            return (string)control.GetValue(ControlHelper.PlaceholderTextProperty);
        }

        public static void SetPlaceholderText(Control control, string value)
        {
            control.SetValue(ControlHelper.PlaceholderTextProperty, (object)value);
        }

        private static void OnPlaceholderTextChanged(
          DependencyObject d,
          DependencyPropertyChangedEventArgs e)
        {
            ControlHelper.UpdatePlaceholderTextVisibility((Control)d);
        }

        public static Visibility GetPlaceholderTextVisibility(Control control)
        {
            return (Visibility)control.GetValue(ControlHelper.PlaceholderTextVisibilityProperty);
        }

        private static void SetPlaceholderTextVisibility(Control control, Visibility value)
        {
            control.SetValue(ControlHelper.PlaceholderTextVisibilityPropertyKey, (object)value);
        }

        private static void UpdatePlaceholderTextVisibility(Control control)
        {
            ControlHelper.SetPlaceholderTextVisibility(control, string.IsNullOrEmpty(ControlHelper.GetPlaceholderText(control)) ? Visibility.Collapsed : Visibility.Visible);
        }

        public static Brush GetPlaceholderForeground(Control control)
        {
            return (Brush)control.GetValue(ControlHelper.PlaceholderForegroundProperty);
        }

        public static void SetPlaceholderForeground(Control control, Brush value)
        {
            control.SetValue(ControlHelper.PlaceholderForegroundProperty, (object)value);
        }

        public static object GetDescription(Control control)
        {
            return control.GetValue(ControlHelper.DescriptionProperty);
        }

        public static void SetDescription(Control control, object value)
        {
            control.SetValue(ControlHelper.DescriptionProperty, value);
        }

        private static void OnDescriptionChanged(
          DependencyObject d,
          DependencyPropertyChangedEventArgs e)
        {
            ControlHelper.UpdateDescriptionVisibility((Control)d);
        }

        public static Visibility GetDescriptionVisibility(Control control)
        {
            return (Visibility)control.GetValue(ControlHelper.DescriptionVisibilityProperty);
        }

        private static void SetDescriptionVisibility(Control control, Visibility value)
        {
            control.SetValue(ControlHelper.DescriptionVisibilityPropertyKey, (object)value);
        }

        private static void UpdateDescriptionVisibility(Control control)
        {
            ControlHelper.SetDescriptionVisibility(control, ControlHelper.IsNullOrEmptyString(ControlHelper.GetDescription(control)) ? Visibility.Collapsed : Visibility.Visible);
        }

        internal static bool IsNullOrEmptyString(object obj)
        {
            if (obj == null)
                return true;
            return obj is string str && string.IsNullOrEmpty(str);
        }
    }
}
