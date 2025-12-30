using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PersianDateTimeWPFTools.Controls
{
#if NET6_0_OR_GREATER
#nullable enable
#endif
    public sealed class CalendarDayInfo
    {
#if NET6_0_OR_GREATER
        public object? ToolTip { get; set; }

        public DataTemplate? ToolTipTemplate { get; set; }
#elif NET40_OR_GREATER
        public object ToolTip { get; set; }
        public DataTemplate ToolTipTemplate { get; set; }
#endif
        public bool HasIndicator { get; set; }
#if NET6_0_OR_GREATER
        public Style? IndicatorStyle { get; set; }
#elif NET40_OR_GREATER
        public Style IndicatorStyle { get; set; }
#endif
        public bool IsDisabled { get; set; }

        public bool ShowToolTipWhenDisabled { get; set; } = true;

    }

}
