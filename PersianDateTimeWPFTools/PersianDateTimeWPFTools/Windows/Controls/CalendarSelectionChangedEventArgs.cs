using System;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
#if NET8_0_OR_GREATER
#nullable disable
#endif
namespace PersianDateTimeWPFTools.Windows.Controls
{
    internal class CalendarSelectionChangedEventArgs : SelectionChangedEventArgs
    {
        public CalendarSelectionChangedEventArgs(
          RoutedEvent eventId,
          IList removedItems,
          IList addedItems)
          : base(eventId, removedItems, addedItems)
        {
        }

        protected override void InvokeEventHandler(Delegate genericHandler, object genericTarget)
        {
            if (genericHandler is EventHandler<SelectionChangedEventArgs> eventHandler)
                eventHandler(genericTarget, (SelectionChangedEventArgs)this);
            else
                base.InvokeEventHandler(genericHandler, genericTarget);
        }
    }
}
