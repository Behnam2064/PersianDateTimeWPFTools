using System;
using System.Windows;

#if NET8_0_OR_GREATER
#nullable disable
#endif
namespace PersianDateTimeWPFTools.Windows.Controls
{
    public class CalendarDateChangedEventArgs : RoutedEventArgs
    {
        internal CalendarDateChangedEventArgs(DateTime? removedDate, DateTime? addedDate)
        {
            this.RemovedDate = removedDate;
            this.AddedDate = addedDate;
        }

        internal CalendarDateChangedEventArgs(RoutedEvent routed, DateTime? removedDate, DateTime? addedDate) : base(routed)
        {
            this.RemovedDate = removedDate;
            this.AddedDate = addedDate;
        }

        public DateTime? AddedDate { get; private set; }

        public DateTime? RemovedDate { get; private set; }
    }
}

