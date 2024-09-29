using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

#if NET8_0_OR_GREATER
#nullable disable
#endif
namespace PersianDateTimeWPFTools.Tools
{
    internal static class ControlExtensions
    {
        public static FrameworkElement GetTemplateRoot(this Control control)
        {
            return VisualTreeHelper.GetChildrenCount((DependencyObject)control) > 0 ? VisualTreeHelper.GetChild((DependencyObject)control, 0) as FrameworkElement : (FrameworkElement)null;
        }

        public static T GetTemplateChild<T>(this Control control, string childName) where T : DependencyObject
        {
            return control.Template?.FindName(childName, (FrameworkElement)control) as T;
        }
    }
}
