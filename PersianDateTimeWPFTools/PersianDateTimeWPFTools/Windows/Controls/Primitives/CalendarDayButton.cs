
using PersianDateTimeWPFTools.Windows.Automation.Peers;
using System.Globalization;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using PersianCalendar = PersianDateTimeWPFTools.Controls.PersianCalendar;
#if NET8_0_OR_GREATER
#nullable disable
#endif
namespace PersianDateTimeWPFTools.Windows.Controls.Primitives
{
  [TemplateVisualState(Name = "Normal", GroupName = "CommonStates")]
  [TemplateVisualState(Name = "MouseOver", GroupName = "CommonStates")]
  [TemplateVisualState(Name = "Pressed", GroupName = "CommonStates")]
  [TemplateVisualState(Name = "Disabled", GroupName = "CommonStates")]
  [TemplateVisualState(Name = "Unselected", GroupName = "SelectionStates")]
  [TemplateVisualState(Name = "Selected", GroupName = "SelectionStates")]
  [TemplateVisualState(Name = "CalendarButtonUnfocused", GroupName = "CalendarButtonFocusStates")]
  [TemplateVisualState(Name = "CalendarButtonFocused", GroupName = "CalendarButtonFocusStates")]
  [TemplateVisualState(Name = "Inactive", GroupName = "ActiveStates")]
  [TemplateVisualState(Name = "Active", GroupName = "ActiveStates")]
  [TemplateVisualState(Name = "RegularDay", GroupName = "DayStates")]
  [TemplateVisualState(Name = "Today", GroupName = "DayStates")]
  [TemplateVisualState(Name = "NormalDay", GroupName = "BlackoutDayStates")]
  [TemplateVisualState(Name = "BlackoutDay", GroupName = "BlackoutDayStates")]
  public sealed class CalendarDayButton : Button
  {
    private const int DEFAULTCONTENT = 1;
    internal const string StateToday = "Today";
    internal const string StateRegularDay = "RegularDay";
    internal const string GroupDay = "DayStates";
    internal const string StateBlackoutDay = "BlackoutDay";
    internal const string StateNormalDay = "NormalDay";
    internal const string GroupBlackout = "BlackoutDayStates";
    private bool _shouldCoerceContent;
    private object _coercedContent;
    internal static readonly DependencyPropertyKey IsTodayPropertyKey = DependencyProperty.RegisterReadOnly(nameof (IsToday), typeof (bool), typeof (CalendarDayButton), (PropertyMetadata) new FrameworkPropertyMetadata((object) false, new PropertyChangedCallback(CalendarDayButton.OnVisualStatePropertyChanged)));
    public static readonly DependencyProperty IsTodayProperty = CalendarDayButton.IsTodayPropertyKey.DependencyProperty;
    internal static readonly DependencyPropertyKey IsSelectedPropertyKey = DependencyProperty.RegisterReadOnly(nameof (IsSelected), typeof (bool), typeof (CalendarDayButton), (PropertyMetadata) new FrameworkPropertyMetadata((object) false, new PropertyChangedCallback(CalendarDayButton.OnVisualStatePropertyChanged)));
    public static readonly DependencyProperty IsSelectedProperty = CalendarDayButton.IsSelectedPropertyKey.DependencyProperty;
    internal static readonly DependencyPropertyKey IsInactivePropertyKey = DependencyProperty.RegisterReadOnly(nameof (IsInactive), typeof (bool), typeof (CalendarDayButton), (PropertyMetadata) new FrameworkPropertyMetadata((object) false, new PropertyChangedCallback(CalendarDayButton.OnVisualStatePropertyChanged)));
    public static readonly DependencyProperty IsInactiveProperty = CalendarDayButton.IsInactivePropertyKey.DependencyProperty;
    internal static readonly DependencyPropertyKey IsBlackedOutPropertyKey = DependencyProperty.RegisterReadOnly(nameof (IsBlackedOut), typeof (bool), typeof (CalendarDayButton), (PropertyMetadata) new FrameworkPropertyMetadata((object) false, new PropertyChangedCallback(CalendarDayButton.OnVisualStatePropertyChanged)));
    public static readonly DependencyProperty IsBlackedOutProperty = CalendarDayButton.IsBlackedOutPropertyKey.DependencyProperty;
    internal static readonly DependencyPropertyKey IsHighlightedPropertyKey = DependencyProperty.RegisterReadOnly(nameof (IsHighlighted), typeof (bool), typeof (CalendarDayButton), (PropertyMetadata) new FrameworkPropertyMetadata((object) false, new PropertyChangedCallback(CalendarDayButton.OnVisualStatePropertyChanged)));
    public static readonly DependencyProperty IsHighlightedProperty = CalendarDayButton.IsHighlightedPropertyKey.DependencyProperty;

    static CalendarDayButton()
    {
      FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof (CalendarDayButton), (PropertyMetadata) new FrameworkPropertyMetadata((object) typeof (CalendarDayButton)));
      ContentControl.ContentProperty.OverrideMetadata(typeof (CalendarDayButton), (PropertyMetadata) new FrameworkPropertyMetadata((PropertyChangedCallback) null, new CoerceValueCallback(CalendarDayButton.OnCoerceContent)));
    }

    public CalendarDayButton()
    {
      this.Loaded += (RoutedEventHandler) ((_param1, _param2) => this.ChangeVisualState(false));
    }

    public bool IsToday => (bool) this.GetValue(CalendarDayButton.IsTodayProperty);

    public bool IsSelected => (bool) this.GetValue(CalendarDayButton.IsSelectedProperty);

    public bool IsInactive => (bool) this.GetValue(CalendarDayButton.IsInactiveProperty);

    public bool IsBlackedOut => (bool) this.GetValue(CalendarDayButton.IsBlackedOutProperty);

    public bool IsHighlighted => (bool) this.GetValue(CalendarDayButton.IsHighlightedProperty);

    internal PersianCalendar Owner { get; set; }

    public override void OnApplyTemplate()
    {
      base.OnApplyTemplate();
      this.ChangeVisualState(false);
    }

    protected override AutomationPeer OnCreateAutomationPeer()
    {
      return (AutomationPeer) new CalendarDayButtonAutomationPeer(this);
    }

    protected override void OnGotKeyboardFocus(KeyboardFocusChangedEventArgs e)
    {
      this.ChangeVisualState(true);
      base.OnGotKeyboardFocus(e);
    }

    protected override void OnLostKeyboardFocus(KeyboardFocusChangedEventArgs e)
    {
      this.ChangeVisualState(true);
      base.OnLostKeyboardFocus(e);
    }

    internal new void ChangeVisualState(bool useTransitions)
    {
      if (this.IsEnabled)
        PersianDateTimeWPFTools.Windows.Controls.VisualStates.GoToState((Control) this, (useTransitions ? 1 : 0) != 0, "Normal");
      else
        PersianDateTimeWPFTools.Windows.Controls.VisualStates.GoToState((Control) this, (useTransitions ? 1 : 0) != 0, "Disabled");
      if (this.IsSelected || this.IsHighlighted)
        PersianDateTimeWPFTools.Windows.Controls.VisualStates.GoToState((Control) this, (useTransitions ? 1 : 0) != 0, "Selected", "Unselected");
      else
        PersianDateTimeWPFTools.Windows.Controls.VisualStates.GoToState((Control) this, (useTransitions ? 1 : 0) != 0, "Unselected");
      if (!this.IsInactive)
        PersianDateTimeWPFTools.Windows.Controls.VisualStates.GoToState((Control) this, (useTransitions ? 1 : 0) != 0, "Active", "Inactive");
      else
        PersianDateTimeWPFTools.Windows.Controls.VisualStates.GoToState((Control) this, (useTransitions ? 1 : 0) != 0, "Inactive");
      if (this.IsToday && this.Owner != null && this.Owner.IsTodayHighlighted)
        PersianDateTimeWPFTools.Windows.Controls.VisualStates.GoToState((Control) this, (useTransitions ? 1 : 0) != 0, "Today", "RegularDay");
      else
        PersianDateTimeWPFTools.Windows.Controls.VisualStates.GoToState((Control) this, (useTransitions ? 1 : 0) != 0, "RegularDay");
      if (this.IsBlackedOut)
        PersianDateTimeWPFTools.Windows.Controls.VisualStates.GoToState((Control) this, (useTransitions ? 1 : 0) != 0, "BlackoutDay", "NormalDay");
      else
        PersianDateTimeWPFTools.Windows.Controls.VisualStates.GoToState((Control) this, (useTransitions ? 1 : 0) != 0, "NormalDay");
      if (this.IsKeyboardFocused)
        PersianDateTimeWPFTools.Windows.Controls.VisualStates.GoToState((Control) this, (useTransitions ? 1 : 0) != 0, "CalendarButtonFocused", "CalendarButtonUnfocused");
      else
        VisualStateManager.GoToState((FrameworkElement) this, "CalendarButtonUnfocused", useTransitions);
    }

    internal void SetContentInternal(string value)
    {
      if (BindingOperations.GetBindingExpressionBase((DependencyObject) this, ContentControl.ContentProperty) != null)
      {
        this.Content = (object) value;
      }
      else
      {
        this._shouldCoerceContent = true;
        this._coercedContent = (object) value;
        this.CoerceValue(ContentControl.ContentProperty);
      }
    }

    private new static void OnVisualStatePropertyChanged(
      DependencyObject sender,
      DependencyPropertyChangedEventArgs e)
    {
      if (!(sender is CalendarDayButton calendarDayButton))
        return;
      calendarDayButton.ChangeVisualState(true);
    }

    private static object OnCoerceContent(DependencyObject sender, object baseValue)
    {
      CalendarDayButton calendarDayButton = (CalendarDayButton) sender;
      if (!calendarDayButton._shouldCoerceContent)
        return baseValue;
      calendarDayButton._shouldCoerceContent = false;
      return calendarDayButton._coercedContent;
    }
  }
}
