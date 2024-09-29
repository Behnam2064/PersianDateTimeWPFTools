
using PersianDateTimeWPFTools.Tools;
using PersianDateTimeWPFTools.Windows.Controls.Primitives;
using System.Windows;
using System.Windows.Media.Animation;
#if NET8_0_OR_GREATER
#nullable disable
#endif
namespace PersianDateTimeWPFTools.Windows.Controls.Primitives
{
  public static class OpeningAnimationHelper
  {
    public static readonly DependencyProperty StoryboardProperty = DependencyProperty.RegisterAttached("Storyboard", typeof (Storyboard), typeof (OpeningAnimationHelper), new PropertyMetadata(new PropertyChangedCallback(OpeningAnimationHelper.OnStoryboardChanged)));

    public static Storyboard GetStoryboard(FrameworkElement element)
    {
      return (Storyboard) element.GetValue(OpeningAnimationHelper.StoryboardProperty);
    }

    public static void SetStoryboard(FrameworkElement element, Storyboard value)
    {
      element.SetValue(OpeningAnimationHelper.StoryboardProperty, (object) value);
    }

    private static void OnStoryboardChanged(
      DependencyObject d,
      DependencyPropertyChangedEventArgs e)
    {
      FrameworkElement frameworkElement = (FrameworkElement) d;
      if (e.OldValue != null)
        frameworkElement.Loaded -= new RoutedEventHandler(OpeningAnimationHelper.OnElementLoaded);
      if (e.NewValue == null)
        return;
      frameworkElement.Loaded += new RoutedEventHandler(OpeningAnimationHelper.OnElementLoaded);
    }

    private static void OnElementLoaded(object sender, RoutedEventArgs e)
    {
      FrameworkElement element = (FrameworkElement) sender;
      if (!element.IsVisible || !Helper.IsAnimationsEnabled || DesignMode.DesignModeEnabled)
        return;
      OpeningAnimationHelper.GetStoryboard(element)?.Begin();
    }
  }
}
