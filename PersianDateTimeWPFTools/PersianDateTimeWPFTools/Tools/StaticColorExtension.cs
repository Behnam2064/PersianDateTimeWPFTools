using System;
using System.Windows.Markup;
using System.Windows.Media;
#if NET8_0_OR_GREATER
#nullable disable
#endif
namespace PersianDateTimeWPFTools.Tools
{
  public class StaticColorExtension : System.Windows.StaticResourceExtension
  {
    public StaticColorExtension()
    {
    }

    public StaticColorExtension(object resourceKey)
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
