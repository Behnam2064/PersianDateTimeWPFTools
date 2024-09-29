using PersianDateTimeWPFTools.Windows.Controls.Primitives;
using System;
using System.Windows;
using System.Windows.Media;

#if NET8_0_OR_GREATER
#nullable disable
#endif
namespace PersianDateTimeWPFTools.Tools
{
  internal static class Helper
  {
    public static bool IsAnimationsEnabled
    {
      get => SystemParameters.ClientAreaAnimation && RenderCapability.Tier > 0;
    }

    public static bool TryGetTransformToDevice(Visual visual, out Matrix value)
    {
      PresentationSource presentationSource = PresentationSource.FromVisual(visual);
      if (presentationSource != null)
      {
        value = presentationSource.CompositionTarget.TransformToDevice;
        return true;
      }
      value = new Matrix();
      return false;
    }

    public static Vector GetOffset(
      UIElement element1,
      InterestPoint interestPoint1,
      UIElement element2,
      InterestPoint interestPoint2,
      Rect element2Bounds)
    {
      Point point = element1.TranslatePoint(Helper.GetPoint(element1, interestPoint1), element2);
      return element2Bounds.IsEmpty ? point - Helper.GetPoint(element2, interestPoint2) : point - Helper.GetPoint(element2Bounds, interestPoint2);
    }

    private static Point GetPoint(UIElement element, InterestPoint interestPoint)
    {
      return Helper.GetPoint(new Rect(element.RenderSize), interestPoint);
    }

    private static Point GetPoint(Rect rect, InterestPoint interestPoint)
    {
      switch (interestPoint)
      {
        case InterestPoint.TopLeft:
          return rect.TopLeft;
        case InterestPoint.TopRight:
          return rect.TopRight;
        case InterestPoint.BottomLeft:
          return rect.BottomLeft;
        case InterestPoint.BottomRight:
          return rect.BottomRight;
        case InterestPoint.Center:
          return new Point(rect.Left + rect.Width / 2.0, rect.Top + rect.Height / 2.0);
        default:
          throw new ArgumentOutOfRangeException(nameof (interestPoint));
      }
    }

    public static bool HasDefaultValue(this DependencyObject d, DependencyProperty dp)
    {
      return DependencyPropertyHelper.GetValueSource(d, dp).BaseValueSource == BaseValueSource.Default;
    }

    public static bool HasNonDefaultValue(this DependencyObject d, DependencyProperty dp)
    {
      return !d.HasDefaultValue(dp);
    }

    public static bool HasLocalValue(this DependencyObject d, DependencyProperty dp)
    {
      return d.ReadLocalValue(dp) != DependencyProperty.UnsetValue;
    }
  }
}
