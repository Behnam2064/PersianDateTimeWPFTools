using System;
using System.Windows;

#if NET8_0_OR_GREATER
#nullable disable
#endif
namespace PersianDateTimeWPFTools.Windows.Controls
{
    public class ClockTimeChangedEventArgs : RoutedEventArgs
    {
        internal ClockTimeChangedEventArgs(RoutedEvent routedEvent, DateTime? removedDate, DateTime? addedDate): base(routedEvent) 
        {
            this.RemovedDate = removedDate;
            this.AddedDate = addedDate;
        }


        public DateTime? AddedDate { get; private set; }

        public DateTime? RemovedDate { get; private set; }
    }


}

