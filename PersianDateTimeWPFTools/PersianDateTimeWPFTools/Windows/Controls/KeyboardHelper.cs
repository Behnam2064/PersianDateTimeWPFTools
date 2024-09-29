using System.Windows.Input;
#if NET8_0_OR_GREATER
#nullable disable
#endif
namespace PersianDateTimeWPFTools.Windows.Controls
{
  internal static class KeyboardHelper
  {
    public static void GetMetaKeyState(out bool ctrl, out bool shift)
    {
      ctrl = (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control;
      shift = (Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift;
    }
  }
}
