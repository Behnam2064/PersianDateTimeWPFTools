using System.Windows;
using System.Windows.Media;

#if NET8_0_OR_GREATER
#nullable disable
#endif
namespace PersianDateTimeWPFTools.Controls
{
  public class RatingItemPathInfo : RatingItemInfo
  {
    public static readonly DependencyProperty DisabledDataProperty = DependencyProperty.Register(nameof (DisabledData), typeof (Geometry), typeof (RatingItemPathInfo), (PropertyMetadata) null);
    public static readonly DependencyProperty DataProperty = DependencyProperty.Register(nameof (Data), typeof (Geometry), typeof (RatingItemPathInfo), (PropertyMetadata) null);
    public static readonly DependencyProperty PlaceholderDataProperty = DependencyProperty.Register(nameof (PlaceholderData), typeof (Geometry), typeof (RatingItemPathInfo), (PropertyMetadata) null);
    public static readonly DependencyProperty PointerOverDataProperty = DependencyProperty.Register(nameof (PointerOverData), typeof (Geometry), typeof (RatingItemPathInfo), (PropertyMetadata) null);
    public static readonly DependencyProperty PointerOverPlaceholderDataProperty = DependencyProperty.Register(nameof (PointerOverPlaceholderData), typeof (Geometry), typeof (RatingItemPathInfo), (PropertyMetadata) null);
    public static readonly DependencyProperty UnsetDataProperty = DependencyProperty.Register(nameof (UnsetData), typeof (Geometry), typeof (RatingItemPathInfo), (PropertyMetadata) null);

    public Geometry DisabledData
    {
      get => (Geometry) this.GetValue(RatingItemPathInfo.DisabledDataProperty);
      set => this.SetValue(RatingItemPathInfo.DisabledDataProperty, (object) value);
    }

    public Geometry Data
    {
      get => (Geometry) this.GetValue(RatingItemPathInfo.DataProperty);
      set => this.SetValue(RatingItemPathInfo.DataProperty, (object) value);
    }

    public Geometry PlaceholderData
    {
      get => (Geometry) this.GetValue(RatingItemPathInfo.PlaceholderDataProperty);
      set => this.SetValue(RatingItemPathInfo.PlaceholderDataProperty, (object) value);
    }

    public Geometry PointerOverData
    {
      get => (Geometry) this.GetValue(RatingItemPathInfo.PointerOverDataProperty);
      set => this.SetValue(RatingItemPathInfo.PointerOverDataProperty, (object) value);
    }

    public Geometry PointerOverPlaceholderData
    {
      get => (Geometry) this.GetValue(RatingItemPathInfo.PointerOverPlaceholderDataProperty);
      set => this.SetValue(RatingItemPathInfo.PointerOverPlaceholderDataProperty, (object) value);
    }

    public Geometry UnsetData
    {
      get => (Geometry) this.GetValue(RatingItemPathInfo.UnsetDataProperty);
      set => this.SetValue(RatingItemPathInfo.UnsetDataProperty, (object) value);
    }

    protected override Freezable CreateInstanceCore() => (Freezable) new RatingItemPathInfo();
  }
}
