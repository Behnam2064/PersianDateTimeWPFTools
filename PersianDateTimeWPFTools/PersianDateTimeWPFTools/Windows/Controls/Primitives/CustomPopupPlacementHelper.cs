
using PersianDateTimeWPFTools.Tools;
using PersianDateTimeWPFTools.Windows.Controls.Primitives;
using System;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
#if NET8_0_OR_GREATER
#nullable disable
#endif
namespace PersianDateTimeWPFTools.Windows.Controls.Primitives
{
  internal static class CustomPopupPlacementHelper
  {
    public static readonly DependencyProperty PlacementProperty = DependencyProperty.RegisterAttached("Placement", typeof (CustomPlacementMode), typeof (CustomPopupPlacementHelper), new PropertyMetadata((object) CustomPlacementMode.Top));

    public static CustomPlacementMode GetPlacement(DependencyObject element)
    {
      return (CustomPlacementMode) element.GetValue(CustomPopupPlacementHelper.PlacementProperty);
    }

    public static void SetPlacement(DependencyObject element, CustomPlacementMode value)
    {
      element.SetValue(CustomPopupPlacementHelper.PlacementProperty, (object) value);
    }

    internal static CustomPopupPlacement[] PositionPopup(
      CustomPlacementMode placement,
      Size popupSize,
      Size targetSize,
      Point offset,
      FrameworkElement child = null)
    {
      Matrix transformToDevice = new Matrix();
      if (child != null)
        Helper.TryGetTransformToDevice((Visual) child, out transformToDevice);
      CustomPopupPlacement popupPlacement = CustomPopupPlacementHelper.CalculatePopupPlacement(placement, popupSize, targetSize, offset, child, transformToDevice);
      CustomPopupPlacement? nullable = new CustomPopupPlacement?();
      CustomPlacementMode? alternativePlacementMode = CustomPopupPlacementHelper.GetAlternativePlacementMode(placement);
      if (alternativePlacementMode.HasValue)
        nullable = new CustomPopupPlacement?(CustomPopupPlacementHelper.CalculatePopupPlacement(alternativePlacementMode.Value, popupSize, targetSize, offset, child, transformToDevice));
      return nullable.HasValue ? new CustomPopupPlacement[2]
      {
        popupPlacement,
        nullable.Value
      } : new CustomPopupPlacement[1]{ popupPlacement };
    }

    private static CustomPopupPlacement CalculatePopupPlacement(
      CustomPlacementMode placement,
      Size popupSize,
      Size targetSize,
      Point offset,
      FrameworkElement child = null,
      Matrix transformToDevice = default (Matrix))
    {
      Point point;
      PopupPrimaryAxis primaryAxis;
      switch (placement)
      {
        case CustomPlacementMode.Top:
          point = new Point((targetSize.Width - popupSize.Width) / 2.0, -popupSize.Height);
          primaryAxis = PopupPrimaryAxis.Horizontal;
          break;
        case CustomPlacementMode.Bottom:
          point = new Point((targetSize.Width - popupSize.Width) / 2.0, targetSize.Height);
          primaryAxis = PopupPrimaryAxis.Horizontal;
          break;
        case CustomPlacementMode.Left:
          point = new Point(-popupSize.Width, (targetSize.Height - popupSize.Height) / 2.0);
          primaryAxis = PopupPrimaryAxis.Vertical;
          break;
        case CustomPlacementMode.Right:
          point = new Point(targetSize.Width, (targetSize.Height - popupSize.Height) / 2.0);
          primaryAxis = PopupPrimaryAxis.Vertical;
          break;
        case CustomPlacementMode.Full:
          point = new Point((targetSize.Width - popupSize.Width) / 2.0, (targetSize.Height - popupSize.Height) / 2.0);
          primaryAxis = PopupPrimaryAxis.None;
          break;
        case CustomPlacementMode.TopEdgeAlignedLeft:
          point = new Point(0.0, -popupSize.Height);
          primaryAxis = PopupPrimaryAxis.Horizontal;
          break;
        case CustomPlacementMode.TopEdgeAlignedRight:
          point = new Point(targetSize.Width - popupSize.Width, -popupSize.Height);
          primaryAxis = PopupPrimaryAxis.Horizontal;
          break;
        case CustomPlacementMode.BottomEdgeAlignedLeft:
          point = new Point(0.0, targetSize.Height);
          primaryAxis = PopupPrimaryAxis.Horizontal;
          break;
        case CustomPlacementMode.BottomEdgeAlignedRight:
          point = new Point(targetSize.Width - popupSize.Width, targetSize.Height);
          primaryAxis = PopupPrimaryAxis.Horizontal;
          break;
        case CustomPlacementMode.LeftEdgeAlignedTop:
          point = new Point(-popupSize.Width, 0.0);
          primaryAxis = PopupPrimaryAxis.Vertical;
          break;
        case CustomPlacementMode.LeftEdgeAlignedBottom:
          point = new Point(-popupSize.Width, targetSize.Height - popupSize.Height);
          primaryAxis = PopupPrimaryAxis.Vertical;
          break;
        case CustomPlacementMode.RightEdgeAlignedTop:
          point = new Point(targetSize.Width, 0.0);
          primaryAxis = PopupPrimaryAxis.Vertical;
          break;
        case CustomPlacementMode.RightEdgeAlignedBottom:
          point = new Point(targetSize.Width, targetSize.Height - popupSize.Height);
          primaryAxis = PopupPrimaryAxis.Vertical;
          break;
        default:
          throw new ArgumentOutOfRangeException(nameof (placement));
      }
      if (child != null)
      {
        Vector vector = VisualTreeHelper.GetOffset((Visual) child);
        if (transformToDevice != new Matrix())
          vector = transformToDevice.Transform(vector);
        point -= vector;
      }
      return new CustomPopupPlacement(point, primaryAxis);
    }

    private static CustomPlacementMode? GetAlternativePlacementMode(CustomPlacementMode placement)
    {
      switch (placement)
      {
        case CustomPlacementMode.Top:
          return new CustomPlacementMode?(CustomPlacementMode.Bottom);
        case CustomPlacementMode.Bottom:
          return new CustomPlacementMode?(CustomPlacementMode.Top);
        case CustomPlacementMode.Left:
          return new CustomPlacementMode?(CustomPlacementMode.Right);
        case CustomPlacementMode.Right:
          return new CustomPlacementMode?(CustomPlacementMode.Left);
        case CustomPlacementMode.Full:
          return new CustomPlacementMode?();
        case CustomPlacementMode.TopEdgeAlignedLeft:
          return new CustomPlacementMode?(CustomPlacementMode.BottomEdgeAlignedLeft);
        case CustomPlacementMode.TopEdgeAlignedRight:
          return new CustomPlacementMode?(CustomPlacementMode.BottomEdgeAlignedRight);
        case CustomPlacementMode.BottomEdgeAlignedLeft:
          return new CustomPlacementMode?(CustomPlacementMode.TopEdgeAlignedLeft);
        case CustomPlacementMode.BottomEdgeAlignedRight:
          return new CustomPlacementMode?(CustomPlacementMode.TopEdgeAlignedRight);
        case CustomPlacementMode.LeftEdgeAlignedTop:
          return new CustomPlacementMode?(CustomPlacementMode.RightEdgeAlignedTop);
        case CustomPlacementMode.LeftEdgeAlignedBottom:
          return new CustomPlacementMode?(CustomPlacementMode.RightEdgeAlignedBottom);
        case CustomPlacementMode.RightEdgeAlignedTop:
          return new CustomPlacementMode?(CustomPlacementMode.RightEdgeAlignedTop);
        case CustomPlacementMode.RightEdgeAlignedBottom:
          return new CustomPlacementMode?(CustomPlacementMode.LeftEdgeAlignedBottom);
        default:
          return new CustomPlacementMode?();
      }
    }
  }
}
