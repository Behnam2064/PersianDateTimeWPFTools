#if NET8_0_OR_GREATER
#nullable disable
#endif
namespace MS.Internal
{
  internal static class DoubleUtil
  {
    public static int DoubleToInt(double val) => 0.0 >= val ? (int) (val - 0.5) : (int) (val + 0.5);
  }
}
