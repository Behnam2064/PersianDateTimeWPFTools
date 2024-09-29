using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

#if NET8_0_OR_GREATER
#nullable disable
#endif
namespace MS.Win32
{
  internal static class SafeNativeMethods
  {
    public static IntPtr MonitorFromRect(ref NativeMethods.RECT rect, int flags)
    {
      return SafeNativeMethods.SafeNativeMethodsPrivate.MonitorFromRect(ref rect, flags);
    }

    internal static void GetWindowRect(HandleRef hWnd, [In, Out] ref NativeMethods.RECT rect)
    {
      if (!SafeNativeMethods.SafeNativeMethodsPrivate.IntGetWindowRect(hWnd, ref rect))
        throw new Win32Exception();
    }

    private class SafeNativeMethodsPrivate
    {
      [DllImport("user32.dll", EntryPoint = "GetWindowRect", CharSet = CharSet.Auto, SetLastError = true)]
      public static extern bool IntGetWindowRect(HandleRef hWnd, [In, Out] ref NativeMethods.RECT rect);

      [DllImport("user32.dll")]
      public static extern IntPtr MonitorFromRect(ref NativeMethods.RECT rect, int flags);

      [DllImport("user32.dll", EntryPoint = "ScreenToClient", CharSet = CharSet.Auto, SetLastError = true)]
      public static extern int IntScreenToClient(HandleRef hWnd, [In, Out] NativeMethods.POINT pt);
    }
  }
}
