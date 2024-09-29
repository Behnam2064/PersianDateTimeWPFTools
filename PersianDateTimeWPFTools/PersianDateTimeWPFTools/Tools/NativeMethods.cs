using System.Runtime.InteropServices;

#if NET8_0_OR_GREATER
#nullable disable
#endif
namespace MS.Win32
{
  internal class NativeMethods
  {
    public const int SWP_NOSIZE = 1;
    public const int SWP_NOMOVE = 2;
    public const int SWP_NOZORDER = 4;
    public const int SWP_NOACTIVATE = 16;
    public const int SWP_SHOWWINDOW = 64;
    public const int SWP_HIDEWINDOW = 128;
    public const int SWP_DRAWFRAME = 32;

    [StructLayout(LayoutKind.Sequential)]
    public class POINT
    {
      public int x;
      public int y;

      public POINT()
      {
      }

      public POINT(int x, int y)
      {
        this.x = x;
        this.y = y;
      }
    }

    public struct RECT
    {
      public int left;
      public int top;
      public int right;
      public int bottom;

      public RECT(int left, int top, int right, int bottom)
      {
        this.left = left;
        this.top = top;
        this.right = right;
        this.bottom = bottom;
      }

      public int Width => this.right - this.left;

      public int Height => this.bottom - this.top;

      public void Offset(int dx, int dy)
      {
        this.left += dx;
        this.top += dy;
        this.right += dx;
        this.bottom += dy;
      }

      public bool IsEmpty => this.left >= this.right || this.top >= this.bottom;
    }
  }
}
