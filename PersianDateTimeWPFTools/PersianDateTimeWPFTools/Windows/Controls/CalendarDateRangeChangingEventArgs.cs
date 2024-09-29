using System;

#if NET8_0_OR_GREATER
#nullable disable
#endif
namespace PersianDateTimeWPFTools.Windows.Controls
{
    internal class CalendarDateRangeChangingEventArgs : EventArgs
    {
        private DateTime _start;
        private DateTime _end;

        public CalendarDateRangeChangingEventArgs(DateTime start, DateTime end)
        {
            this._start = start;
            this._end = end;
        }

        public DateTime Start => this._start;

        public DateTime End => this._end;
    }
}
