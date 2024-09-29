using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
#if NET8_0_OR_GREATER
#nullable disable
#endif

namespace PersianDateTimeWPFTools.Windows.Controls.Primitives
{
  public class PlacementRectangleConverter : IMultiValueConverter
  {
    public Thickness Margin { get; set; }

    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
      if (values.Length != 2 || !(values[0] is double num1) || !(values[1] is double num2))
        return (object) Rect.Empty;
      Thickness margin = this.Margin;
      return (object) new Rect(new Point(margin.Left, margin.Top), new Point(num1 - margin.Right, num2 - margin.Bottom));
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
