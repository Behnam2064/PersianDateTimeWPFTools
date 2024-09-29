
using PersianDateTimeWPFTools.Windows.Controls.Primitives;
using PersianDateTimeWPFTools.Controls;
using System;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Input;

#if NET8_0_OR_GREATER
#nullable disable
#endif
namespace PersianDateTimeWPFTools.Windows.Automation.Peers
{
    public sealed class CalendarButtonAutomationPeer :
      ButtonAutomationPeer,
      IGridItemProvider,
      ISelectionItemProvider
    {
        public CalendarButtonAutomationPeer(CalendarButton owner)
          : base((Button)owner)
        {
        }

        private PersianCalendar OwningPersianCalendar => this.OwningCalendarButton.Owner;

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

        private CalendarButton OwningCalendarButton => this.Owner as CalendarButton;

        private DateTime? Date
        {
            get
            {
                return this.OwningCalendarButton != null && this.OwningCalendarButton.DataContext is DateTime ? (DateTime?)this.OwningCalendarButton.DataContext : new DateTime?();
            }
        }

        public override object GetPattern(PatternInterface patternInterface)
        {
            return patternInterface == PatternInterface.GridItem || patternInterface == PatternInterface.SelectionItem ? (this.OwningPersianCalendar == null || this.OwningPersianCalendar.MonthControl == null || this.OwningCalendarButton == null ? base.GetPattern(patternInterface) : (object)this) : base.GetPattern(patternInterface);
        }

        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.Button;
        }

        protected override string GetClassNameCore() => this.Owner.GetType().Name;

        protected override string GetLocalizedControlTypeCore() => "PersianCalendar button";

        protected override string GetHelpTextCore()
        {
            DateTime? date = this.Date;
            var culture = OwningPersianCalendar.CustomCulture ?? PersianDateTimeWPFTools.Windows.Controls.DateTimeHelper.GetCulture((FrameworkElement)this.OwningCalendarButton);
            return !date.HasValue ? base.GetHelpTextCore() : PersianDateTimeWPFTools.Windows.Controls.DateTimeHelper.ToLongDateString(date, culture);
        }

        protected override string GetNameCore()
        {
            var culture = OwningPersianCalendar.CustomCulture ?? PersianDateTimeWPFTools.Windows.Controls.DateTimeHelper.GetCulture((FrameworkElement)this.OwningCalendarButton);
            DateTime? date = this.Date;
            if (!date.HasValue)
                return base.GetNameCore();
            return this.OwningPersianCalendar.DisplayMode == PersianDateTimeWPFTools.Windows.Controls.CalendarMode.Decade ?
                      new PersianDateTimeWPFTools.Windows.Controls.DateTimeHelper(OwningPersianCalendar.MonthControl._calendar).ToYearString
                      (date, PersianDateTimeWPFTools.Windows.Controls.DateTimeHelper.GetCulture((FrameworkElement)this.OwningCalendarButton))
                      : PersianDateTimeWPFTools.Windows.Controls.DateTimeHelper.ToYearMonthPatternString(date, culture);
        }

        int IGridItemProvider.Column => (int)this.OwningCalendarButton.GetValue(Grid.ColumnProperty);

        int IGridItemProvider.ColumnSpan
        {
            get => (int)this.OwningCalendarButton.GetValue(Grid.ColumnSpanProperty);
        }

        IRawElementProviderSimple IGridItemProvider.ContainingGrid => this.OwningCalendarAutomationPeer;

        int IGridItemProvider.Row => (int)this.OwningCalendarButton.GetValue(Grid.RowSpanProperty);

        int IGridItemProvider.RowSpan => 1;

        bool ISelectionItemProvider.IsSelected => this.OwningCalendarButton.IsFocused;

        IRawElementProviderSimple ISelectionItemProvider.SelectionContainer
        {
            get => this.OwningCalendarAutomationPeer;
        }

        void ISelectionItemProvider.AddToSelection()
        {
        }

        void ISelectionItemProvider.RemoveFromSelection()
        {
        }

        void ISelectionItemProvider.Select()
        {
            if (!this.OwningCalendarButton.IsEnabled)
                throw new ElementNotEnabledException();
            this.OwningCalendarButton.MoveFocus(new TraversalRequest(FocusNavigationDirection.First));
        }
    }
}
