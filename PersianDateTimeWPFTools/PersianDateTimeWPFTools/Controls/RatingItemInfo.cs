using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
#if NET8_0_OR_GREATER
#nullable disable
#endif
namespace PersianDateTimeWPFTools.Controls
{
    public class RatingItemInfo : Freezable
    {
        protected override Freezable CreateInstanceCore() => (Freezable)new RatingItemInfo();
    }
}