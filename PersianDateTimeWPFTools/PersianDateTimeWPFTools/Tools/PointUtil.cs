using MS.Win32;
using System.Windows;

#if NET8_0_OR_GREATER
#nullable disable
#endif
namespace MS.Internal
{
  internal static class PointUtil
  {
    internal static Rect ToRect(NativeMethods.RECT rc)
    {
      return new Rect()
      {
        X = (double) rc.left,
        Y = (double) rc.top,
        Width = (double) (rc.right - rc.left),
        Height = (double) (rc.bottom - rc.top)
      };
    }
  }
}
