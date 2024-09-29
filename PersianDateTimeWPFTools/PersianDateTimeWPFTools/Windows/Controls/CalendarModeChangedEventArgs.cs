using System.Windows;
#if NET8_0_OR_GREATER
#nullable disable
#endif
namespace PersianDateTimeWPFTools.Windows.Controls
{
    public class CalendarModeChangedEventArgs : RoutedEventArgs
    {
        public CalendarModeChangedEventArgs(CalendarMode oldMode, CalendarMode newMode)
        {
            this.OldMode = oldMode;
            this.NewMode = newMode;
        }

        public CalendarMode NewMode { get; private set; }

        public CalendarMode OldMode { get; private set; }
    }
}
