
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
#if NET8_0_OR_GREATER
#nullable disable
#endif
namespace PersianDateTimeWPFTools.Tools
{
  [TypeConverter(typeof (ThemeResouceExtensionConverter))]
  public class ThemeResourceExtension : DynamicResourceExtension
  {
    public ThemeResourceExtension()
    {
    }

    public ThemeResourceExtension(object resourceKey)
      : base(resourceKey)
    {
    }

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
      if (!(this.ResourceKey is string resourceKey) || !resourceKey.StartsWith("SystemColor", StringComparison.Ordinal))
        return base.ProvideValue(serviceProvider);
      return new Binding(resourceKey)
      {
        Source = ((object) ThemeResourceExtension.SystemColorsSource.Current)
      }.ProvideValue(serviceProvider);
    }

    private class SystemColorsSource : INotifyPropertyChanged
    {
      private SystemColorsSource()
      {
        SystemParameters.StaticPropertyChanged += new PropertyChangedEventHandler(this.OnSystemParametersPropertyChanged);
      }

      public static ThemeResourceExtension.SystemColorsSource Current { get; } = new ThemeResourceExtension.SystemColorsSource();

      public Color SystemColorButtonFaceColor => SystemColors.ControlColor;

      public Color SystemColorButtonTextColor => SystemColors.ControlTextColor;

      public Color SystemColorGrayTextColor => SystemColors.GrayTextColor;

      public Color SystemColorHighlightColor => SystemColors.HighlightColor;

      public Color SystemColorHighlightTextColor => SystemColors.HighlightTextColor;

      public Color SystemColorHotlightColor => SystemColors.HotTrackColor;

      public Color SystemColorWindowColor => SystemColors.WindowColor;

      public Color SystemColorWindowTextColor => SystemColors.WindowTextColor;

      public Color SystemColorActiveCaptionColor => SystemColors.ActiveCaptionColor;

      public Color SystemColorInactiveCaptionTextColor => SystemColors.InactiveCaptionTextColor;

      public event PropertyChangedEventHandler PropertyChanged;

      private void OnSystemParametersPropertyChanged(object sender, PropertyChangedEventArgs e)
      {
        if (!(e.PropertyName == "HighContrast") || !SystemParameters.HighContrast)
          return;
        PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
        if (propertyChanged == null)
          return;
        propertyChanged((object) this, new PropertyChangedEventArgs((string) null));
      }
    }
  }
}
