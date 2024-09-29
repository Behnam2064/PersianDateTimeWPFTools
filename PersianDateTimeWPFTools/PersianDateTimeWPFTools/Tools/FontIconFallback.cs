
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
#if NET8_0_OR_GREATER
#nullable disable
#endif
namespace PersianDateTimeWPFTools.Windows.Controls.Primitives
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class FontIconFallback : Control
  {
    public static readonly DependencyProperty DataProperty = DependencyProperty.Register(nameof (Data), typeof (Geometry), typeof (FontIconFallback), (PropertyMetadata) null);

    static FontIconFallback()
    {
      FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof (FontIconFallback), (PropertyMetadata) new FrameworkPropertyMetadata((object) typeof (FontIconFallback)));
      UIElement.FocusableProperty.OverrideMetadata(typeof (FontIconFallback), (PropertyMetadata) new FrameworkPropertyMetadata((object) false));
    }

    public Geometry Data
    {
      get => (Geometry) this.GetValue(FontIconFallback.DataProperty);
      set => this.SetValue(FontIconFallback.DataProperty, (object) value);
    }
  }
}
