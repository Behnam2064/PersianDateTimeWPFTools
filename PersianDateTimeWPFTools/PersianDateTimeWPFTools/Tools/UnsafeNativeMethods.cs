using System;
using System.Runtime.InteropServices;

#if NET8_0_OR_GREATER
#nullable disable
#endif
namespace MS.Win32
{
  internal class UnsafeNativeMethods
  {
    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    public static extern bool SetWindowPos(
      HandleRef hWnd,
      HandleRef hWndInsertAfter,
      int x,
      int y,
      int cx,
      int cy,
      int flags);

    [DllImport("user32.dll", EntryPoint = "ClientToScreen", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern int IntClientToScreen(HandleRef hWnd, [In, Out] NativeMethods.POINT pt);

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    public static extern IntPtr GetActiveWindow();
  }
}
