using PersianDateTimeWPFTools.Windows.Controls.Primitives;
using PersianDateTimeWPFTools.Controls;
using System;
using System.Collections;
using System.Collections.Generic;
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
  public sealed class CalendarAutomationPeer : 
    FrameworkElementAutomationPeer,
    IGridProvider,
    IMultipleViewProvider,
    ISelectionProvider,
    ITableProvider
  {
    public CalendarAutomationPeer(PersianCalendar owner)
      : base((FrameworkElement) owner)
    {
    }

    private PersianCalendar OwningPersianCalendar => this.Owner as PersianCalendar;

    private Grid OwningGrid
    {
      get
      {
        if (this.OwningPersianCalendar == null || this.OwningPersianCalendar.MonthControl == null)
          return (Grid) null;
        return this.OwningPersianCalendar.DisplayMode == PersianDateTimeWPFTools.Windows.Controls.CalendarMode.Month ? this.OwningPersianCalendar.MonthControl.MonthView : this.OwningPersianCalendar.MonthControl.YearView;
      }
    }

    public override object GetPattern(PatternInterface patternInterface)
    {
      if (patternInterface <= PatternInterface.Grid)
      {
        if (patternInterface != PatternInterface.Selection && patternInterface != PatternInterface.Grid)
          goto label_5;
      }
      else if (patternInterface != PatternInterface.MultipleView && patternInterface != PatternInterface.Table)
        goto label_5;
      if (this.OwningGrid != null)
        return (object) this;
label_5:
      return base.GetPattern(patternInterface);
    }

    protected override AutomationControlType GetAutomationControlTypeCore()
    {
      return AutomationControlType.Calendar;
    }

    protected override string GetClassNameCore() => this.Owner.GetType().Name;

    internal void RaiseSelectionEvents(SelectionChangedEventArgs e)
    {
      int count1 = this.OwningPersianCalendar.SelectedDates.Count;
      int count2 = e.AddedItems.Count;
      if (AutomationPeer.ListenerExists(AutomationEvents.SelectionItemPatternOnElementSelected) && count1 == 1 && count2 == 1)
      {
        CalendarDayButton dayButtonFromDay = this.OwningPersianCalendar.FindDayButtonFromDay((DateTime) e.AddedItems[0]);
        if (dayButtonFromDay == null)
          return;
        UIElementAutomationPeer.FromElement((UIElement) dayButtonFromDay)?.RaiseAutomationEvent(AutomationEvents.SelectionItemPatternOnElementSelected);
      }
      else
      {
        if (AutomationPeer.ListenerExists(AutomationEvents.SelectionItemPatternOnElementAddedToSelection))
        {
          foreach (DateTime addedItem in (IEnumerable) e.AddedItems)
          {
            CalendarDayButton dayButtonFromDay = this.OwningPersianCalendar.FindDayButtonFromDay(addedItem);
            if (dayButtonFromDay != null)
              UIElementAutomationPeer.FromElement((UIElement) dayButtonFromDay)?.RaiseAutomationEvent(AutomationEvents.SelectionItemPatternOnElementAddedToSelection);
          }
        }
        if (!AutomationPeer.ListenerExists(AutomationEvents.SelectionItemPatternOnElementRemovedFromSelection))
          return;
        foreach (DateTime removedItem in (IEnumerable) e.RemovedItems)
        {
          CalendarDayButton dayButtonFromDay = this.OwningPersianCalendar.FindDayButtonFromDay(removedItem);
          if (dayButtonFromDay != null)
            UIElementAutomationPeer.FromElement((UIElement) dayButtonFromDay)?.RaiseAutomationEvent(AutomationEvents.SelectionItemPatternOnElementRemovedFromSelection);
        }
      }
    }

    int IGridProvider.ColumnCount
    {
      get => this.OwningGrid != null ? this.OwningGrid.ColumnDefinitions.Count : 0;
    }

    int IGridProvider.RowCount
    {
      get
      {
        if (this.OwningGrid == null)
          return 0;
        return this.OwningPersianCalendar.DisplayMode == PersianDateTimeWPFTools.Windows.Controls.CalendarMode.Month ? Math.Max(0, this.OwningGrid.RowDefinitions.Count - 1) : this.OwningGrid.RowDefinitions.Count;
      }
    }

    IRawElementProviderSimple IGridProvider.GetItem(int row, int column)
    {
      if (this.OwningPersianCalendar.DisplayMode == PersianDateTimeWPFTools.Windows.Controls.CalendarMode.Month)
        ++row;
      if (this.OwningGrid != null && row >= 0 && row < this.OwningGrid.RowDefinitions.Count && column >= 0 && column < this.OwningGrid.ColumnDefinitions.Count)
      {
        foreach (UIElement child in this.OwningGrid.Children)
        {
          int num1 = (int) child.GetValue(Grid.RowProperty);
          int num2 = (int) child.GetValue(Grid.ColumnProperty);
          if (num1 == row && num2 == column)
          {
            AutomationPeer peerForElement = UIElementAutomationPeer.CreatePeerForElement(child);
            if (peerForElement != null)
              return this.ProviderFromPeer(peerForElement);
          }
        }
      }
      return (IRawElementProviderSimple) null;
    }

    int IMultipleViewProvider.CurrentView => (int) this.OwningPersianCalendar.DisplayMode;

    int[] IMultipleViewProvider.GetSupportedViews()
    {
      return new int[3]{ 0, 1, 2 };
    }

    string IMultipleViewProvider.GetViewName(int viewId)
    {
      switch (viewId)
      {
        case 0:
          return "Month";
        case 1:
          return "Year";
        case 2:
          return "Decade";
        default:
          return string.Empty;
      }
    }

    void IMultipleViewProvider.SetCurrentView(int viewId)
    {
      this.OwningPersianCalendar.DisplayMode = (PersianDateTimeWPFTools.Windows.Controls.CalendarMode) viewId;
    }

    bool ISelectionProvider.CanSelectMultiple
    {
      get
      {
        return this.OwningPersianCalendar.SelectionMode == PersianDateTimeWPFTools.Windows.Controls.CalendarSelectionMode.SingleRange || this.OwningPersianCalendar.SelectionMode == PersianDateTimeWPFTools.Windows.Controls.CalendarSelectionMode.MultipleRange;
      }
    }

    bool ISelectionProvider.IsSelectionRequired => false;

    IRawElementProviderSimple[] ISelectionProvider.GetSelection()
    {
      List<IRawElementProviderSimple> elementProviderSimpleList = new List<IRawElementProviderSimple>();
      if (this.OwningGrid != null)
      {
        if (this.OwningPersianCalendar.DisplayMode == PersianDateTimeWPFTools.Windows.Controls.CalendarMode.Month && this.OwningPersianCalendar.SelectedDates != null && this.OwningPersianCalendar.SelectedDates.Count != 0)
        {
          foreach (UIElement child in this.OwningGrid.Children)
          {
            if ((int) child.GetValue(Grid.RowProperty) != 0 && child is CalendarDayButton element && element.IsSelected)
            {
              AutomationPeer peerForElement = UIElementAutomationPeer.CreatePeerForElement((UIElement) element);
              if (peerForElement != null)
                elementProviderSimpleList.Add(this.ProviderFromPeer(peerForElement));
            }
          }
        }
        else
        {
          foreach (UIElement child in this.OwningGrid.Children)
          {
            if (child is CalendarButton element && element.IsFocused)
            {
              AutomationPeer peerForElement = UIElementAutomationPeer.CreatePeerForElement((UIElement) element);
              if (peerForElement != null)
              {
                elementProviderSimpleList.Add(this.ProviderFromPeer(peerForElement));
                break;
              }
              break;
            }
          }
        }
        if (elementProviderSimpleList.Count > 0)
          return elementProviderSimpleList.ToArray();
      }
      return (IRawElementProviderSimple[]) null;
    }

    RowOrColumnMajor ITableProvider.RowOrColumnMajor => RowOrColumnMajor.RowMajor;

    IRawElementProviderSimple[] ITableProvider.GetColumnHeaders()
    {
      if (this.OwningPersianCalendar.DisplayMode == PersianDateTimeWPFTools.Windows.Controls.CalendarMode.Month)
      {
        List<IRawElementProviderSimple> elementProviderSimpleList = new List<IRawElementProviderSimple>();
        foreach (UIElement child in this.OwningGrid.Children)
        {
          if ((int) child.GetValue(Grid.RowProperty) == 0)
          {
            AutomationPeer peerForElement = UIElementAutomationPeer.CreatePeerForElement(child);
            if (peerForElement != null)
              elementProviderSimpleList.Add(this.ProviderFromPeer(peerForElement));
          }
        }
        if (elementProviderSimpleList.Count > 0)
          return elementProviderSimpleList.ToArray();
      }
      return (IRawElementProviderSimple[]) null;
    }

    IRawElementProviderSimple[] ITableProvider.GetRowHeaders()
    {
      return (IRawElementProviderSimple[]) null;
    }
  }
}
