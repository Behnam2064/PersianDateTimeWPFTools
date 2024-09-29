
using PersianDateTimeWPFTools.Windows.Controls.Primitives;
using PersianDateTimeWPFTools.Controls;
using System;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
#if NET8_0_OR_GREATER
#nullable disable
#endif

namespace PersianDateTimeWPFTools.Windows.Automation.Peers
{
    public sealed class CalendarDayButtonAutomationPeer :
      ButtonAutomationPeer,
      IGridItemProvider,
      ISelectionItemProvider,
      ITableItemProvider
    {
        public CalendarDayButtonAutomationPeer(CalendarDayButton owner)
          : base((Button)owner)
        {
        }

        private PersianCalendar OwningPersianCalendar => this.OwningCalendarDayButton.Owner;

        private IRawElementProviderSimple OwningCalendarAutomationPeer
        {
            get
            {
                if (this.OwningPersianCalendar != null)
                {
                    AutomationPeer peerForElement = UIElementAutomationPeer.CreatePeerForElement((UIElement)this.OwningPersianCalendar);
                    if (peerForElement != null)
                        return this.ProviderFromPeer(peerForElement);
                }
                return (IRawElementProviderSimple)null;
            }
        }

        private CalendarDayButton OwningCalendarDayButton => this.Owner as CalendarDayButton;

        private DateTime? Date
        {
            get
            {
                return this.OwningCalendarDayButton != null && this.OwningCalendarDayButton.DataContext is DateTime ? (DateTime?)this.OwningCalendarDayButton.DataContext : new DateTime?();
            }
        }

        public override object GetPattern(PatternInterface patternInterface)
        {
            return patternInterface == PatternInterface.GridItem || patternInterface == PatternInterface.SelectionItem || patternInterface == PatternInterface.TableItem ? (this.OwningPersianCalendar == null || this.OwningCalendarDayButton == null ? base.GetPattern(patternInterface) : (object)this) : base.GetPattern(patternInterface);
        }

        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.Button;
        }

        protected override string GetClassNameCore() => this.Owner.GetType().Name;

        protected override string GetHelpTextCore()
        {
            if (!this.Date.HasValue)
                return base.GetHelpTextCore();
            var culture = OwningPersianCalendar.CustomCulture ?? PersianDateTimeWPFTools.Windows.Controls.DateTimeHelper.GetCulture((FrameworkElement)this.OwningCalendarDayButton);
            string longDateString = PersianDateTimeWPFTools.Windows.Controls.DateTimeHelper.ToLongDateString(this.Date, culture);
            if (!this.OwningCalendarDayButton.IsBlackedOut)
                return longDateString;
            return string.Format((IFormatProvider)PersianDateTimeWPFTools.Windows.Controls.DateTimeHelper.GetCurrentDateFormat(), "Blackout Day - {0}", new object[1]
            {
        (object) longDateString
            });
        }

        protected override string GetLocalizedControlTypeCore() => "Day button";

        protected override string GetNameCore()
        {
            var culture = OwningPersianCalendar.CustomCulture ?? PersianDateTimeWPFTools.Windows.Controls.DateTimeHelper.GetCulture((FrameworkElement)this.OwningCalendarDayButton);
            return !this.Date.HasValue ? base.GetNameCore() : PersianDateTimeWPFTools.Windows.Controls.DateTimeHelper.ToLongDateString(this.Date, culture);
        }

        int IGridItemProvider.Column
        {
            get => (int)this.OwningCalendarDayButton.GetValue(Grid.ColumnProperty);
        }

        int IGridItemProvider.ColumnSpan
        {
            get => (int)this.OwningCalendarDayButton.GetValue(Grid.ColumnSpanProperty);
        }

        IRawElementProviderSimple IGridItemProvider.ContainingGrid => this.OwningCalendarAutomationPeer;

        int IGridItemProvider.Row => (int)this.OwningCalendarDayButton.GetValue(Grid.RowProperty) - 1;

        int IGridItemProvider.RowSpan
        {
            get => (int)this.OwningCalendarDayButton.GetValue(Grid.RowSpanProperty);
        }

        bool ISelectionItemProvider.IsSelected => this.OwningCalendarDayButton.IsSelected;

        IRawElementProviderSimple ISelectionItemProvider.SelectionContainer
        {
            get => this.OwningCalendarAutomationPeer;
        }

        void ISelectionItemProvider.AddToSelection()
        {
            if (((ISelectionItemProvider)this).IsSelected || !this.EnsureSelection() || !(this.OwningCalendarDayButton.DataContext is DateTime))
                return;
            if (this.OwningPersianCalendar.SelectionMode == PersianDateTimeWPFTools.Windows.Controls.CalendarSelectionMode.SingleDate)
                this.OwningPersianCalendar.SelectedDate = new DateTime?((DateTime)this.OwningCalendarDayButton.DataContext);
            else
                this.OwningPersianCalendar.SelectedDates.Add((DateTime)this.OwningCalendarDayButton.DataContext);
        }

        void ISelectionItemProvider.RemoveFromSelection()
        {
            if (!((ISelectionItemProvider)this).IsSelected || !(this.OwningCalendarDayButton.DataContext is DateTime))
                return;
            this.OwningPersianCalendar.SelectedDates.Remove((DateTime)this.OwningCalendarDayButton.DataContext);
        }

        void ISelectionItemProvider.Select()
        {
            if (!this.EnsureSelection())
                return;
            this.OwningPersianCalendar.SelectedDates.Clear();
            if (!(this.OwningCalendarDayButton.DataContext is DateTime))
                return;
            this.OwningPersianCalendar.SelectedDates.Add((DateTime)this.OwningCalendarDayButton.DataContext);
        }

        IRawElementProviderSimple[] ITableItemProvider.GetColumnHeaderItems()
        {
            if (this.OwningPersianCalendar != null && this.OwningCalendarAutomationPeer != null)
            {
                IRawElementProviderSimple[] columnHeaders = ((ITableProvider)UIElementAutomationPeer.CreatePeerForElement((UIElement)this.OwningPersianCalendar)).GetColumnHeaders();
                if (columnHeaders != null)
                {
                    int column = ((IGridItemProvider)this).Column;
                    return new IRawElementProviderSimple[1]
                    {
            columnHeaders[column]
                    };
                }
            }
            return (IRawElementProviderSimple[])null;
        }

        IRawElementProviderSimple[] ITableItemProvider.GetRowHeaderItems()
        {
            return (IRawElementProviderSimple[])null;
        }

        private bool EnsureSelection()
        {
            if (!this.OwningCalendarDayButton.IsEnabled)
                throw new ElementNotEnabledException();
            return !this.OwningCalendarDayButton.IsBlackedOut && this.OwningPersianCalendar.SelectionMode != PersianDateTimeWPFTools.Windows.Controls.CalendarSelectionMode.None;
        }
    }
}
