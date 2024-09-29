using System;
using System.ComponentModel;
using System.Windows;

#if NET8_0_OR_GREATER
#nullable disable
#endif
namespace PersianDateTimeWPFTools.Windows.Controls.Primitives
{
  internal static class DesignMode
  {
    private static readonly Lazy<bool> _designModeEnabled = new Lazy<bool>((Func<bool>) (() => DesignerProperties.GetIsInDesignMode(new DependencyObject())));

    public static bool DesignModeEnabled => DesignMode._designModeEnabled.Value;
  }
}
