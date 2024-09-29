using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading;
using System.Windows.Controls;
using PersianDateTimeWPFTools.Controls;
#if NET8_0_OR_GREATER
#nullable disable
#endif
namespace PersianDateTimeWPFTools.Windows.Controls
{
    public sealed class CalendarBlackoutDatesCollection : ObservableCollection<CalendarDateRange>
    {
        private Thread _dispatcherThread;
        private PersianCalendar _owner;

        public CalendarBlackoutDatesCollection(PersianCalendar owner)
        {
            this._owner = owner;
            this._dispatcherThread = Thread.CurrentThread;
        }

        public void AddDatesInPast()
        {
            this.Add(new CalendarDateRange(DateTime.MinValue, DateTime.Today.AddDays(-1.0)));
        }

        public bool Contains(DateTime date) => this.GetContainingDateRange(date) != null;

        public bool Contains(DateTime start, DateTime end)
        {
            int count = this.Count;
            DateTime t2_1;
            DateTime t2_2;
            if (DateTime.Compare(end, start) > -1)
            {
                t2_1 = DateTimeHelper.DiscardTime(new DateTime?(start)).Value;
                t2_2 = DateTimeHelper.DiscardTime(new DateTime?(end)).Value;
            }
            else
            {
                t2_1 = DateTimeHelper.DiscardTime(new DateTime?(end)).Value;
                t2_2 = DateTimeHelper.DiscardTime(new DateTime?(start)).Value;
            }
            for (int index = 0; index < count; ++index)
            {
                if (DateTime.Compare(this[index].Start, t2_1) == 0 && DateTime.Compare(this[index].End, t2_2) == 0)
                    return true;
            }
            return false;
        }

        public bool ContainsAny(CalendarDateRange range)
        {
            foreach (CalendarDateRange calendarDateRange in (Collection<CalendarDateRange>)this)
            {
                if (calendarDateRange.ContainsAny(range))
                    return true;
            }
            return false;
        }

        internal DateTime? GetNonBlackoutDate(DateTime? requestedDate, int dayInterval)
        {
            DateTime? nonBlackoutDate = requestedDate;
            if (!requestedDate.HasValue)
                return new DateTime?();
            CalendarDateRange containingDateRange;
            if ((containingDateRange = this.GetContainingDateRange(nonBlackoutDate.Value)) == null)
                return requestedDate;
            var calendar = _owner.MonthControl._calendar;
            var dateTimeHelper = new DateTimeHelper(calendar);
            do
            {
                
                nonBlackoutDate = dayInterval <= 0 ? dateTimeHelper.AddDays(containingDateRange.Start, dayInterval) : dateTimeHelper.AddDays(containingDateRange.End, dayInterval);
            }
            while (nonBlackoutDate.HasValue && (containingDateRange = this.GetContainingDateRange(nonBlackoutDate.Value)) != null);
            return nonBlackoutDate;
        }

        protected override void ClearItems()
        {
            if (!this.IsValidThread())
                throw new NotSupportedException("This type of Collection does not support changes to its SourceCollection from a thread different from the Dispatcher thread.");
            foreach (CalendarDateRange calendarDateRange in (IEnumerable<CalendarDateRange>)this.Items)
                this.UnRegisterItem(calendarDateRange);
            base.ClearItems();
            this._owner.UpdateCellItems();
        }

        protected override void InsertItem(int index, CalendarDateRange item)
        {
            if (!this.IsValidThread())
                throw new NotSupportedException("This type of Collection does not support changes to its SourceCollection from a thread different from the Dispatcher thread.");
            if (!this.IsValid(item))
                throw new ArgumentOutOfRangeException("Value is not valid.");
            this.RegisterItem(item);
            base.InsertItem(index, item);
            this._owner.UpdateCellItems();
        }

        protected override void RemoveItem(int index)
        {
            if (!this.IsValidThread())
                throw new NotSupportedException("This type of Collection does not support changes to its SourceCollection from a thread different from the Dispatcher thread.");
            if (index >= 0 && index < this.Count)
                this.UnRegisterItem(this.Items[index]);
            base.RemoveItem(index);
            this._owner.UpdateCellItems();
        }

        protected override void SetItem(int index, CalendarDateRange item)
        {
            if (!this.IsValidThread())
                throw new NotSupportedException("This type of Collection does not support changes to its SourceCollection from a thread different from the Dispatcher thread.");
            if (!this.IsValid(item))
                throw new ArgumentOutOfRangeException("Value is not valid.");
            CalendarDateRange calendarDateRange = (CalendarDateRange)null;
            if (index >= 0 && index < this.Count)
                calendarDateRange = this.Items[index];
            base.SetItem(index, item);
            this.UnRegisterItem(calendarDateRange);
            this.RegisterItem(this.Items[index]);
            this._owner.UpdateCellItems();
        }

        private void RegisterItem(CalendarDateRange item)
        {
            if (item == null)
                return;
            item.Changing += new EventHandler<CalendarDateRangeChangingEventArgs>(this.Item_Changing);
            item.PropertyChanged += new PropertyChangedEventHandler(this.Item_PropertyChanged);
        }

        private void UnRegisterItem(CalendarDateRange item)
        {
            if (item == null)
                return;
            item.Changing -= new EventHandler<CalendarDateRangeChangingEventArgs>(this.Item_Changing);
            item.PropertyChanged -= new PropertyChangedEventHandler(this.Item_PropertyChanged);
        }

        private void Item_Changing(object sender, CalendarDateRangeChangingEventArgs e)
        {
            if (sender is CalendarDateRange && !this.IsValid(e.Start, e.End))
                throw new ArgumentOutOfRangeException("Value is not valid.");
        }

        private void Item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!(sender is CalendarDateRange))
                return;
            this._owner.UpdateCellItems();
        }

        private bool IsValid(CalendarDateRange item) => this.IsValid(item.Start, item.End);

        private bool IsValid(DateTime start, DateTime end)
        {
            foreach (DateTime selectedDate in (Collection<DateTime>)this._owner.SelectedDates)
            {
                if (DateTimeHelper.InRange(((ValueType)selectedDate as DateTime?).Value, start, end))
                    return false;
            }
            return true;
        }

        private bool IsValidThread() => Thread.CurrentThread == this._dispatcherThread;

        private CalendarDateRange GetContainingDateRange(DateTime date)
        {
            for (int index = 0; index < this.Count; ++index)
            {
                if (DateTimeHelper.InRange(date, this[index]))
                    return this[index];
            }
            return (CalendarDateRange)null;
        }
    }
}
