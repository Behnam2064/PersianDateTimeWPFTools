using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;
#if NET8_0_OR_GREATER
#nullable disable
#endif
namespace PersianDateTimeWPFTools.Tools
{
  [TypeConverter(typeof (DynamicColorExtensionConverter))]
  public class DynamicColorExtension : DynamicResourceExtension
  {
    public DynamicColorExtension()
    {
    }

    public DynamicColorExtension(object resourceKey)
      : base(resourceKey)
    {
    }

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
      object obj = base.ProvideValue(serviceProvider);
      if (!(serviceProvider?.GetService(typeof (IProvideValueTarget)) is IProvideValueTarget service) || !(service.TargetObject is SolidColorBrush targetObject))
        return obj;
      ThemeResourceHelper.SetColorKey(targetObject, this.ResourceKey);
      return obj;
    }
  }
}
