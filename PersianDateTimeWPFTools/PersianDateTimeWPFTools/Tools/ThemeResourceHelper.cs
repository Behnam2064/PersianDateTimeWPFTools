
using System.Windows;
using System.Windows.Media;
#if NET8_0_OR_GREATER
#nullable disable
#endif
namespace PersianDateTimeWPFTools.Tools
{
  internal static class ThemeResourceHelper
  {
    private static readonly DependencyProperty ColorKeyProperty = DependencyProperty.RegisterAttached("ColorKey", typeof (object), typeof (ThemeResourceHelper));

    internal static object GetColorKey(SolidColorBrush element)
    {
      return element.GetValue(ThemeResourceHelper.ColorKeyProperty);
    }

    internal static void SetColorKey(SolidColorBrush element, object value)
    {
      element.SetValue(ThemeResourceHelper.ColorKeyProperty, value);
    }
  }
}
