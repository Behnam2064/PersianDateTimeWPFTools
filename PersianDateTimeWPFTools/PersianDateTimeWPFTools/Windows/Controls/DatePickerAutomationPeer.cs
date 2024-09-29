
using PersianDateTimeWPFTools.Controls;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
#if NET8_0_OR_GREATER
#nullable disable
#endif
namespace PersianDateTimeWPFTools.Windows.Controls
{
  public sealed class DatePickerAutomationPeer : 
    FrameworkElementAutomationPeer,
    IExpandCollapseProvider,
    IValueProvider
  {
    public DatePickerAutomationPeer(PersianDatePicker owner)
      : base((FrameworkElement) owner)
    {
    }

    private PersianDatePicker OwningPersianDatePicker => this.Owner as PersianDatePicker;

    public override object GetPattern(PatternInterface patternInterface)
    {
      return patternInterface == PatternInterface.ExpandCollapse || patternInterface == PatternInterface.Value ? (object) this : base.GetPattern(patternInterface);
    }

    protected override AutomationControlType GetAutomationControlTypeCore()
    {
      return AutomationControlType.Custom;
    }

    protected override string GetClassNameCore() => this.Owner.GetType().Name;

    protected override string GetLocalizedControlTypeCore() => "date picker";

    ExpandCollapseState IExpandCollapseProvider.ExpandCollapseState
    {
      get
      {
        return this.OwningPersianDatePicker.IsDropDownOpen ? ExpandCollapseState.Expanded : ExpandCollapseState.Collapsed;
      }
    }

    void IExpandCollapseProvider.Collapse() => this.OwningPersianDatePicker.IsDropDownOpen = false;

    void IExpandCollapseProvider.Expand() => this.OwningPersianDatePicker.IsDropDownOpen = true;

    bool IValueProvider.IsReadOnly => false;

    string IValueProvider.Value => this.OwningPersianDatePicker.ToString();

    void IValueProvider.SetValue(string value) => this.OwningPersianDatePicker.Text = value;

    [MethodImpl(MethodImplOptions.NoInlining)]
    internal void RaiseValuePropertyChangedEvent(string oldValue, string newValue)
    {
      if (!(oldValue != newValue))
        return;
      this.RaisePropertyChangedEvent(ValuePatternIdentifiers.ValueProperty, (object) oldValue, (object) newValue);
    }
  }
}
