using PersianDateTimeWPFTools.Windows.Controls;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;
using static System.Net.Mime.MediaTypeNames;
#if NET8_0_OR_GREATER
#nullable disable
#endif
namespace PersianDateTimeWPFTools.Controls
{
    [TemplatePart(Name = "PART_Root", Type = typeof(Grid))]
    [TemplatePart(Name = "PART_TextBox", Type = typeof(PersianDateTimeWPFTools.Windows.Controls.Primitives.DatePickerTextBox))]
    [TemplatePart(Name = "PART_Button", Type = typeof(Button))]
    [TemplatePart(Name = "PART_Popup", Type = typeof(Popup))]
    [TemplateVisualState(Name = "Normal", GroupName = "CommonStates")]
    [TemplateVisualState(Name = "Disabled", GroupName = "CommonStates")]
    public class PersianDatePicker : Control
    {
        private const string ElementRoot = "PART_Root";
        private const string ElementTextBox = "PART_TextBox";
        private const string ElementButton = "PART_Button";
        private const string ElementPopup = "PART_Popup";
        private PersianCalendar _persianCalendar;
        private string _defaultText;
        private ButtonBase _dropDownButton;
        private Popup _popUp;
        private bool _disablePopupReopen;
        private bool _shouldCoerceText;
        private string _coercedTextValue;
        private PersianDateTimeWPFTools.Windows.Controls.Primitives.DatePickerTextBox _textBox;
        private IDictionary<DependencyProperty, bool> _isHandlerSuspended;
        private DateTime? _originalSelectedDate;
        public static readonly RoutedEvent SelectedDateChangedEvent = EventManager.RegisterRoutedEvent("SelectedDateChanged", RoutingStrategy.Direct, typeof(EventHandler<SelectionChangedEventArgs>), typeof(PersianDatePicker));
        public static readonly DependencyProperty CalendarStyleProperty = DependencyProperty.Register(nameof(CalendarStyle), typeof(Style), typeof(PersianDatePicker));
        public static readonly DependencyProperty DisplayDateProperty = DependencyProperty.Register(nameof(DisplayDate), typeof(DateTime), typeof(PersianDatePicker), (PropertyMetadata)new FrameworkPropertyMetadata((object)DateTime.Now, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, (PropertyChangedCallback)null, new CoerceValueCallback(PersianDatePicker.CoerceDisplayDate)));
        public static readonly DependencyProperty DisplayDateEndProperty = DependencyProperty.Register(nameof(DisplayDateEnd), typeof(DateTime?), typeof(PersianDatePicker), (PropertyMetadata)new FrameworkPropertyMetadata((object)null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(PersianDatePicker.OnDisplayDateEndChanged), new CoerceValueCallback(PersianDatePicker.CoerceDisplayDateEnd)));
        public static readonly DependencyProperty DisplayDateStartProperty = DependencyProperty.Register(nameof(DisplayDateStart), typeof(DateTime?), typeof(PersianDatePicker), (PropertyMetadata)new FrameworkPropertyMetadata((object)null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(PersianDatePicker.OnDisplayDateStartChanged), new CoerceValueCallback(PersianDatePicker.CoerceDisplayDateStart)));
        public static readonly DependencyProperty FirstDayOfWeekProperty = DependencyProperty.Register(nameof(FirstDayOfWeek), typeof(DayOfWeek), typeof(PersianDatePicker), (PropertyMetadata)null, new ValidateValueCallback(PersianCalendar.IsValidFirstDayOfWeek));
        public static readonly DependencyProperty IsDropDownOpenProperty = DependencyProperty.Register(nameof(IsDropDownOpen), typeof(bool), typeof(PersianDatePicker), (PropertyMetadata)new FrameworkPropertyMetadata((object)false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(PersianDatePicker.OnIsDropDownOpenChanged), new CoerceValueCallback(PersianDatePicker.OnCoerceIsDropDownOpen)));
        public static readonly DependencyProperty IsTodayHighlightedProperty = DependencyProperty.Register(nameof(IsTodayHighlighted), typeof(bool), typeof(PersianDatePicker));
        public static readonly DependencyProperty SelectedDateProperty = DependencyProperty.Register(nameof(SelectedDate), typeof(DateTime?), typeof(PersianDatePicker), (PropertyMetadata)new FrameworkPropertyMetadata((object)null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(PersianDatePicker.OnSelectedDateChanged), new CoerceValueCallback(PersianDatePicker.CoerceSelectedDate)));
        public static readonly DependencyProperty SelectedDateFormatProperty = DependencyProperty.Register(nameof(SelectedDateFormat), typeof(PersianDateTimeWPFTools.Windows.Controls.DatePickerFormat), typeof(PersianDatePicker), (PropertyMetadata)new FrameworkPropertyMetadata((object)PersianDateTimeWPFTools.Windows.Controls.DatePickerFormat.Long, new PropertyChangedCallback(PersianDatePicker.OnSelectedDateFormatChanged)), new ValidateValueCallback(PersianDatePicker.IsValidSelectedDateFormat));
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(nameof(Text), typeof(string), typeof(PersianDatePicker), (PropertyMetadata)new FrameworkPropertyMetadata((object)string.Empty, new PropertyChangedCallback(PersianDatePicker.OnTextChanged), new CoerceValueCallback(PersianDatePicker.OnCoerceText)));

        #region New feature

        public static readonly DependencyProperty AllowSelectBlackedOutDayProperty = DependencyProperty.Register(nameof(AllowSelectBlackedOutDay), typeof(bool), typeof(PersianDatePicker), (PropertyMetadata)new FrameworkPropertyMetadata((object)false, new PropertyChangedCallback(PersianDatePicker.OnAllowSelectBlackedOutDayChanged)));
        public static readonly DependencyProperty CustomCultureProperty = DependencyProperty.Register(nameof(CustomCulture), typeof(CultureInfo), typeof(PersianDatePicker), (PropertyMetadata)new FrameworkPropertyMetadata((object)null, new PropertyChangedCallback(PersianDatePicker.OnCustomCultureChanged)));
        public static readonly DependencyProperty CustomCultureNameProperty = DependencyProperty.Register(nameof(CustomCultureName), typeof(string), typeof(PersianDatePicker), (PropertyMetadata)new FrameworkPropertyMetadata((object)null, new PropertyChangedCallback(PersianDatePicker.OnCustomCultureNameChanged)));
        public static readonly DependencyProperty ShowTodayButtonProperty = DependencyProperty.Register(nameof(ShowTodayButton), typeof(bool), typeof(PersianDatePicker), (PropertyMetadata)new FrameworkPropertyMetadata(false, new PropertyChangedCallback(PersianDatePicker.OnShowTodayButtonChanged)));

        #endregion

        public event RoutedEventHandler CalendarClosed;

        public event RoutedEventHandler CalendarOpened;

        public event EventHandler<PersianDateTimeWPFTools.Windows.Controls.DatePickerDateValidationErrorEventArgs> DateValidationError;

        public event EventHandler<SelectionChangedEventArgs> SelectedDateChanged
        {
            add => this.AddHandler(PersianDatePicker.SelectedDateChangedEvent, (Delegate)value);
            remove => this.RemoveHandler(PersianDatePicker.SelectedDateChangedEvent, (Delegate)value);
        }

        static PersianDatePicker()
        {
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(PersianDatePicker), (PropertyMetadata)new FrameworkPropertyMetadata((object)typeof(PersianDatePicker)));
            EventManager.RegisterClassHandler(typeof(PersianDatePicker), UIElement.GotFocusEvent, (Delegate)new RoutedEventHandler(PersianDatePicker.OnGotFocus));
            KeyboardNavigation.TabNavigationProperty.OverrideMetadata(typeof(PersianDatePicker), (PropertyMetadata)new FrameworkPropertyMetadata((object)KeyboardNavigationMode.Once));
            KeyboardNavigation.IsTabStopProperty.OverrideMetadata(typeof(PersianDatePicker), (PropertyMetadata)new FrameworkPropertyMetadata((object)false));
            UIElement.IsEnabledProperty.OverrideMetadata(typeof(PersianDatePicker), (PropertyMetadata)new UIPropertyMetadata(new PropertyChangedCallback(PersianDatePicker.OnIsEnabledChanged)));
        }

        public PersianDatePicker()
        {
            this.InitializeCalendar();
            this._defaultText = string.Empty;
            this.FirstDayOfWeek = PersianDateTimeWPFTools.Windows.Controls.DateTimeHelper.GetCurrentDateFormat().FirstDayOfWeek;
            this.DisplayDate = DateTime.Today;
        }

        public PersianDateTimeWPFTools.Windows.Controls.CalendarBlackoutDatesCollection BlackoutDates
        {
            get => this._persianCalendar.BlackoutDates;
        }

        public Style CalendarStyle
        {
            get => (Style)this.GetValue(PersianDatePicker.CalendarStyleProperty);
            set => this.SetValue(PersianDatePicker.CalendarStyleProperty, (object)value);
        }

        public DateTime DisplayDate
        {
            get => (DateTime)this.GetValue(PersianDatePicker.DisplayDateProperty);
            set => this.SetValue(PersianDatePicker.DisplayDateProperty, (object)value);
        }

        #region New feature

        public bool AllowSelectBlackedOutDay
        {
            get => (bool)this.GetValue(PersianDatePicker.AllowSelectBlackedOutDayProperty);
            set => this.SetValue(PersianDatePicker.AllowSelectBlackedOutDayProperty, (object)value);
        }

        public string CustomCultureName
        {
            get => (string)this.GetValue(PersianDatePicker.CustomCultureNameProperty);
            set => this.SetValue(PersianDatePicker.CustomCultureNameProperty, (object)value);
        }

        public CultureInfo CustomCulture
        {
            get => this.GetValue(PersianDatePicker.CustomCultureProperty) as CultureInfo;

            set => this.SetValue(PersianDatePicker.CustomCultureProperty, (object)value);
        }

        public bool ShowTodayButton
        {
            get => (bool)this.GetValue(PersianDatePicker.ShowTodayButtonProperty);
            set => this.SetValue(PersianDatePicker.ShowTodayButtonProperty, value);
        }


        private static void OnAllowSelectBlackedOutDayChanged(
          DependencyObject d,
          DependencyPropertyChangedEventArgs e)
        {
            //PersianDatePicker persianCalendar = d as PersianDatePicker;
            //persianCalendar.UpdateCellItems();
        }

        private static void OnCustomCultureChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
        {
            /* PersianCalendar persianCalendar = d as PersianCalendar;

             #region Default culture

             var NewCultureInfo = e.NewValue as CultureInfo;
             if (NewCultureInfo != null)
             {
                 persianCalendar.dateTimeHelper = new DateTimeHelper(NewCultureInfo.Calendar);
             }
             else
             {
                 persianCalendar.dateTimeHelper = new DateTimeHelper(DateTimeHelper.GetCulture(persianCalendar).Calendar);
             }

             #endregion

             if (persianCalendar._monthControl != null)
             {
                 persianCalendar._monthControl.CustomCulture = e.NewValue as CultureInfo;
                 persianCalendar._calendar = persianCalendar._monthControl.CustomCulture.Calendar;
                 // Like OnLanguageChanged(...);
                 persianCalendar.CoerceValue(PersianCalendar.FirstDayOfWeekProperty);
                 persianCalendar.UpdateCellItems();
             }*/


        }

        private static void OnCustomCultureNameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PersianDatePicker persianCalendar = d as PersianDatePicker;
            if (e.NewValue != null)
            {
                string CultureName = e.NewValue?.ToString();
                if (!string.IsNullOrEmpty(CultureName) || !string.IsNullOrWhiteSpace(CultureName))
                {

                    persianCalendar.CustomCulture = new CultureInfo(CultureName);
                }
                else
                {
                    persianCalendar.CustomCulture = null;
                }
            }
            else
            {
                persianCalendar.CustomCulture = null;
            }
        }

        private static void OnShowTodayButtonChanged(
  DependencyObject d,
  DependencyPropertyChangedEventArgs e)
        {
            PersianDatePicker persianCalendar = d as PersianDatePicker;
            if (persianCalendar?._persianCalendar != null)
                persianCalendar._persianCalendar.ShowTodayButton = (bool)e.NewValue;
        }



        #endregion

        private static object CoerceDisplayDate(DependencyObject d, object value)
        {
            PersianDatePicker persianDatePicker = d as PersianDatePicker;
            persianDatePicker._persianCalendar.DisplayDate = (DateTime)value;
            return (object)persianDatePicker._persianCalendar.DisplayDate;
        }

        public DateTime? DisplayDateEnd
        {
            get => (DateTime?)this.GetValue(PersianDatePicker.DisplayDateEndProperty);
            set => this.SetValue(PersianDatePicker.DisplayDateEndProperty, (object)value);
        }

        private static void OnDisplayDateEndChanged(
          DependencyObject d,
          DependencyPropertyChangedEventArgs e)
        {
            (d as PersianDatePicker).CoerceValue(PersianDatePicker.DisplayDateProperty);
        }

        private static object CoerceDisplayDateEnd(DependencyObject d, object value)
        {
            PersianDatePicker persianDatePicker = d as PersianDatePicker;
            persianDatePicker._persianCalendar.DisplayDateEnd = (DateTime?)value;
            return (object)persianDatePicker._persianCalendar.DisplayDateEnd;
        }

        public DateTime? DisplayDateStart
        {
            get => (DateTime?)this.GetValue(PersianDatePicker.DisplayDateStartProperty);
            set => this.SetValue(PersianDatePicker.DisplayDateStartProperty, (object)value);
        }

        private static void OnDisplayDateStartChanged(
          DependencyObject d,
          DependencyPropertyChangedEventArgs e)
        {
            PersianDatePicker persianDatePicker = d as PersianDatePicker;
            persianDatePicker.CoerceValue(PersianDatePicker.DisplayDateEndProperty);
            persianDatePicker.CoerceValue(PersianDatePicker.DisplayDateProperty);
        }

        private static object CoerceDisplayDateStart(DependencyObject d, object value)
        {
            PersianDatePicker persianDatePicker = d as PersianDatePicker;
            persianDatePicker._persianCalendar.DisplayDateStart = (DateTime?)value;
            return (object)persianDatePicker._persianCalendar.DisplayDateStart;
        }

        public DayOfWeek FirstDayOfWeek
        {
            get => (DayOfWeek)this.GetValue(PersianDatePicker.FirstDayOfWeekProperty);
            set => this.SetValue(PersianDatePicker.FirstDayOfWeekProperty, (object)value);
        }

        public bool IsDropDownOpen
        {
            get => (bool)this.GetValue(PersianDatePicker.IsDropDownOpenProperty);
            set => this.SetValue(PersianDatePicker.IsDropDownOpenProperty, (object)value);
        }

        private static object OnCoerceIsDropDownOpen(DependencyObject d, object baseValue)
        {
            return !(d as PersianDatePicker).IsEnabled ? (object)false : baseValue;
        }

        private static void OnIsDropDownOpenChanged(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            PersianDatePicker dp = d as PersianDatePicker;
            bool newValue = (bool)e.NewValue;
            if (dp._popUp == null || dp._popUp.IsOpen == newValue)
                return;
            dp._popUp.IsOpen = newValue;
            if (!newValue)
                return;
            dp._originalSelectedDate = dp.SelectedDate;
            dp.Dispatcher.Invoke(() =>
            {
                dp._persianCalendar.Focus();
            });
            dp.Dispatcher.Invoke(() => dp._persianCalendar.Focus());
            //dp.Dispatcher.BeginInvoke(DispatcherPriority.Input, (Delegate)(() => dp._persianCalendar.Focus()));
        }

        private static void OnIsEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PersianDatePicker dp = d as PersianDatePicker;
            dp.CoerceValue(PersianDatePicker.IsDropDownOpenProperty);
            PersianDatePicker.OnVisualStatePropertyChanged(dp);
        }

        private static void OnVisualStatePropertyChanged(PersianDatePicker dp)
        {
            if (Validation.GetHasError((DependencyObject)dp))
            {
                if (dp.IsKeyboardFocused)
                    VisualStateManager.GoToState((FrameworkElement)dp, "InvalidFocused", true);
                else
                    VisualStateManager.GoToState((FrameworkElement)dp, "InvalidUnfocused", true);
            }
            else
                VisualStateManager.GoToState((FrameworkElement)dp, "Valid", true);
            VisualStateManager.GoToState((FrameworkElement)dp, dp.IsEnabled ? "Normal" : "Disabled", true);
        }

        public bool IsTodayHighlighted
        {
            get => (bool)this.GetValue(PersianDatePicker.IsTodayHighlightedProperty);
            set => this.SetValue(PersianDatePicker.IsTodayHighlightedProperty, (object)value);
        }

        public DateTime? SelectedDate
        {
            get => (DateTime?)this.GetValue(PersianDatePicker.SelectedDateProperty);
            set => this.SetValue(PersianDatePicker.SelectedDateProperty, (object)value);
        }

        private static void OnSelectedDateChanged(
          DependencyObject d,
          DependencyPropertyChangedEventArgs e)
        {
            PersianDatePicker element = d as PersianDatePicker;
            Collection<DateTime> addedItems = new Collection<DateTime>();
            Collection<DateTime> removedItems = new Collection<DateTime>();
            element.CoerceValue(PersianDatePicker.DisplayDateStartProperty);
            element.CoerceValue(PersianDatePicker.DisplayDateEndProperty);
            element.CoerceValue(PersianDatePicker.DisplayDateProperty);
            DateTime? newValue1 = (DateTime?)e.NewValue;
            DateTime? oldValue1 = (DateTime?)e.OldValue;
            if (element.SelectedDate.HasValue)
            {
                DateTime d1 = element.SelectedDate.Value;
                element.SetTextInternal(element.DateTimeToString(d1));
                int month1 = d1.Month;
                DateTime displayDate = element.DisplayDate;
                int month2 = displayDate.Month;
                if (month1 == month2)
                {
                    int year1 = d1.Year;
                    displayDate = element.DisplayDate;
                    int year2 = displayDate.Year;
                    if (year1 == year2)
                        goto label_5;
                }
                if (!element._persianCalendar.DatePickerDisplayDateFlag)
                    element.DisplayDate = d1;
                label_5:
                element._persianCalendar.DatePickerDisplayDateFlag = false;
            }
            else
                element.SetWaterMarkText();
            if (newValue1.HasValue)
                addedItems.Add(newValue1.Value);
            if (oldValue1.HasValue)
                removedItems.Add(oldValue1.Value);
            element.OnSelectedDateChanged((SelectionChangedEventArgs)new PersianDateTimeWPFTools.Windows.Controls.CalendarSelectionChangedEventArgs(PersianDatePicker.SelectedDateChangedEvent, (IList)removedItems, (IList)addedItems));
            if (!(UIElementAutomationPeer.FromElement((UIElement)element) is PersianDateTimeWPFTools.Windows.Controls.DatePickerAutomationPeer pickerAutomationPeer))
                return;
            string newValue2 = newValue1.HasValue ? element.DateTimeToString(newValue1.Value) : "";
            string oldValue2 = oldValue1.HasValue ? element.DateTimeToString(oldValue1.Value) : "";
            pickerAutomationPeer.RaiseValuePropertyChangedEvent(oldValue2, newValue2);
        }

        private static object CoerceSelectedDate(DependencyObject d, object value)
        {
            PersianDatePicker persianDatePicker = d as PersianDatePicker;
            persianDatePicker._persianCalendar.SelectedDate = (DateTime?)value;
            return (object)persianDatePicker._persianCalendar.SelectedDate;
        }

        public PersianDateTimeWPFTools.Windows.Controls.DatePickerFormat SelectedDateFormat
        {
            get => (PersianDateTimeWPFTools.Windows.Controls.DatePickerFormat)this.GetValue(PersianDatePicker.SelectedDateFormatProperty);
            set => this.SetValue(PersianDatePicker.SelectedDateFormatProperty, (object)value);
        }

        private static void OnSelectedDateFormatChanged(
          DependencyObject d,
          DependencyPropertyChangedEventArgs e)
        {
            PersianDatePicker persianDatePicker = d as PersianDatePicker;
            if (persianDatePicker._textBox == null)
                return;
            if (string.IsNullOrEmpty(persianDatePicker._textBox.Text))
            {
                persianDatePicker.SetWaterMarkText();
            }
            else
            {
                DateTime? text = persianDatePicker.ParseText(persianDatePicker._textBox.Text);
                if (!text.HasValue)
                    return;
                persianDatePicker.SetTextInternal(persianDatePicker.DateTimeToString(text.Value));
            }
        }

        public string Text
        {
            get => (string)this.GetValue(PersianDatePicker.TextProperty);
            set => this.SetValue(PersianDatePicker.TextProperty, (object)value);
        }

        private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PersianDatePicker persianDatePicker = d as PersianDatePicker;
            if (persianDatePicker.IsHandlerSuspended(PersianDatePicker.TextProperty))
                return;
            if (e.NewValue is string newValue)
            {
                if (persianDatePicker._textBox != null)
                    persianDatePicker._textBox.Text = newValue;
                else
                    persianDatePicker._defaultText = newValue;
                persianDatePicker.SetSelectedDate();
            }
            else
                persianDatePicker.SetValueNoCallback(PersianDatePicker.SelectedDateProperty, (object)null);
        }

        private static object OnCoerceText(DependencyObject dObject, object baseValue)
        {
            PersianDatePicker persianDatePicker = (PersianDatePicker)dObject;
            if (!persianDatePicker._shouldCoerceText)
                return baseValue;
            persianDatePicker._shouldCoerceText = false;
            return (object)persianDatePicker._coercedTextValue;
        }

        private void SetTextInternal(string value)
        {
            if (BindingOperations.GetBindingExpressionBase((DependencyObject)this, PersianDatePicker.TextProperty) != null)
            {
                this.Text = value;
            }
            else
            {
                this._shouldCoerceText = true;
                this._coercedTextValue = value;
                this.CoerceValue(PersianDatePicker.TextProperty);
            }
        }

        public override void OnApplyTemplate()
        {
            if (this._popUp != null)
            {
                this._popUp.RemoveHandler(UIElement.PreviewMouseLeftButtonDownEvent, (Delegate)new MouseButtonEventHandler(this.PopUp_PreviewMouseLeftButtonDown));
                this._popUp.Opened -= new EventHandler(this.PopUp_Opened);
                this._popUp.Closed -= new EventHandler(this.PopUp_Closed);
                this._popUp.Child = (UIElement)null;
            }
            if (this._dropDownButton != null)
            {
                this._dropDownButton.Click -= new RoutedEventHandler(this.DropDownButton_Click);
                this._dropDownButton.RemoveHandler(UIElement.MouseLeaveEvent, (Delegate)new MouseEventHandler(this.DropDownButton_MouseLeave));
            }
            if (this._textBox != null)
            {
                this._textBox.RemoveHandler(UIElement.KeyDownEvent, (Delegate)new KeyEventHandler(this.TextBox_KeyDown));
                this._textBox.RemoveHandler(TextBoxBase.TextChangedEvent, (Delegate)new TextChangedEventHandler(this.TextBox_TextChanged));
                this._textBox.RemoveHandler(UIElement.LostFocusEvent, (Delegate)new RoutedEventHandler(this.TextBox_LostFocus));
            }
            base.OnApplyTemplate();
            this._popUp = this.GetTemplateChild("PART_Popup") as Popup;
            if (this._popUp != null)
            {
                this._popUp.AddHandler(UIElement.PreviewMouseLeftButtonDownEvent, (Delegate)new MouseButtonEventHandler(this.PopUp_PreviewMouseLeftButtonDown));
                this._popUp.Opened += new EventHandler(this.PopUp_Opened);
                this._popUp.Closed += new EventHandler(this.PopUp_Closed);
                this._popUp.Child = (UIElement)this._persianCalendar;
                if (this.IsDropDownOpen)
                    this._popUp.IsOpen = true;
            }
            this._dropDownButton = (ButtonBase)(this.GetTemplateChild("PART_Button") as Button);
            if (this._dropDownButton != null)
            {
                this._dropDownButton.Click += new RoutedEventHandler(this.DropDownButton_Click);
                this._dropDownButton.AddHandler(UIElement.MouseLeaveEvent, (Delegate)new MouseEventHandler(this.DropDownButton_MouseLeave), true);
                if (this._dropDownButton.Content == null)
                    this._dropDownButton.Content = (object)"Show PersianCalendar";
            }
            this._textBox = this.GetTemplateChild("PART_TextBox") as PersianDateTimeWPFTools.Windows.Controls.Primitives.DatePickerTextBox;
            this.UpdateDisabledVisual();
            DateTime? selectedDate = this.SelectedDate;
            if (!selectedDate.HasValue)
                this.SetWaterMarkText();
            if (this._textBox == null)
                return;
            this._textBox.AddHandler(UIElement.KeyDownEvent, (Delegate)new KeyEventHandler(this.TextBox_KeyDown), true);
            this._textBox.AddHandler(TextBoxBase.TextChangedEvent, (Delegate)new TextChangedEventHandler(this.TextBox_TextChanged), true);
            this._textBox.AddHandler(UIElement.LostFocusEvent, (Delegate)new RoutedEventHandler(this.TextBox_LostFocus), true);
            selectedDate = this.SelectedDate;
            if (!selectedDate.HasValue)
            {
                if (string.IsNullOrEmpty(this._defaultText))
                    return;
                this._textBox.Text = this._defaultText;
                this.SetSelectedDate();
            }
            else
            {
                PersianDateTimeWPFTools.Windows.Controls.Primitives.DatePickerTextBox textBox = this._textBox;
                selectedDate = this.SelectedDate;
                string str = this.DateTimeToString(selectedDate.Value);
                textBox.Text = str;
            }
        }

        public override string ToString()
        {
            var culture = CustomCulture ?? PersianDateTimeWPFTools.Windows.Controls.DateTimeHelper.GetCulture((FrameworkElement)this);
            return this.SelectedDate.HasValue ? this.SelectedDate.Value.ToString((IFormatProvider)PersianDateTimeWPFTools.Windows.Controls.DateTimeHelper.GetDateFormat(culture)) : string.Empty;
        }

        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return (AutomationPeer)new PersianDateTimeWPFTools.Windows.Controls.DatePickerAutomationPeer(this);
        }

        protected virtual void OnCalendarClosed(RoutedEventArgs e)
        {
            RoutedEventHandler calendarClosed = this.CalendarClosed;
            if (calendarClosed == null)
                return;
            calendarClosed((object)this, e);
        }

        protected virtual void OnCalendarOpened(RoutedEventArgs e)
        {
            RoutedEventHandler calendarOpened = this.CalendarOpened;
            if (calendarOpened == null)
                return;
            calendarOpened((object)this, e);
        }

        protected virtual void OnSelectedDateChanged(SelectionChangedEventArgs e)
        {
            this.RaiseEvent((RoutedEventArgs)e);
        }

        protected virtual void OnDateValidationError(PersianDateTimeWPFTools.Windows.Controls.DatePickerDateValidationErrorEventArgs e)
        {
            EventHandler<PersianDateTimeWPFTools.Windows.Controls.DatePickerDateValidationErrorEventArgs> dateValidationError = this.DateValidationError;
            if (dateValidationError == null)
                return;
            dateValidationError((object)this, e);
        }

        private static void OnGotFocus(object sender, RoutedEventArgs e)
        {
            PersianDatePicker persianDatePicker = (PersianDatePicker)sender;
            if (e.Handled || persianDatePicker._textBox == null)
                return;
            if (e.OriginalSource == persianDatePicker)
            {
                persianDatePicker._textBox.Focus();
                e.Handled = true;
            }
            else
            {
                if (e.OriginalSource != persianDatePicker._textBox)
                    return;
                persianDatePicker._textBox.SelectAll();
                e.Handled = true;
            }
        }

        private void SetValueNoCallback(DependencyProperty property, object value)
        {
            this.SetIsHandlerSuspended(property, true);
            try
            {
                this.SetValue(property, value);
            }
            finally
            {
                this.SetIsHandlerSuspended(property, false);
            }
        }

        private bool IsHandlerSuspended(DependencyProperty property)
        {
            return this._isHandlerSuspended != null && this._isHandlerSuspended.ContainsKey(property);
        }

        private void SetIsHandlerSuspended(DependencyProperty property, bool value)
        {
            if (value)
            {
                if (this._isHandlerSuspended == null)
                    this._isHandlerSuspended = (IDictionary<DependencyProperty, bool>)new Dictionary<DependencyProperty, bool>(2);
                this._isHandlerSuspended[property] = true;
            }
            else
            {
                if (this._isHandlerSuspended == null)
                    return;
                this._isHandlerSuspended.Remove(property);
            }
        }

        private void PopUp_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!(sender is Popup popup) || popup.StaysOpen || this._dropDownButton == null || this._dropDownButton.InputHitTest(e.GetPosition((IInputElement)this._dropDownButton)) == null)
                return;
            this._disablePopupReopen = true;
        }

        private void PopUp_Opened(object sender, EventArgs e)
        {
            if (!this.IsDropDownOpen)
                this.IsDropDownOpen = true;
            if (this._persianCalendar != null)
            {
                this._persianCalendar.DisplayMode = PersianDateTimeWPFTools.Windows.Controls.CalendarMode.Month;
                this._persianCalendar.MoveFocus(new TraversalRequest(FocusNavigationDirection.First));
            }
            this.OnCalendarOpened(new RoutedEventArgs());
        }

        private void PopUp_Closed(object sender, EventArgs e)
        {
            if (this.IsDropDownOpen)
                this.IsDropDownOpen = false;
            if (this._persianCalendar.IsKeyboardFocusWithin)
                this.MoveFocus(new TraversalRequest(FocusNavigationDirection.First));
            this.OnCalendarClosed(new RoutedEventArgs());
        }

        private void Calendar_DayButtonMouseUp(object sender, MouseButtonEventArgs e)
        {
            this.IsDropDownOpen = false;
        }

        private void Calendar_DisplayDateChanged(object sender, PersianDateTimeWPFTools.Windows.Controls.CalendarDateChangedEventArgs e)
        {
            DateTime? addedDate = e.AddedDate;
            DateTime displayDate = this.DisplayDate;
            if ((addedDate.HasValue ? (addedDate.HasValue ? (addedDate.GetValueOrDefault() != displayDate ? 1 : 0) : 0) : 1) == 0)
                return;
            this.SetValue(PersianDatePicker.DisplayDateProperty, (object)e.AddedDate.Value);
        }

        private void CalendarDayOrMonthButton_PreviewKeyDown(object sender, RoutedEventArgs e)
        {
            PersianCalendar persianCalendar = sender as PersianCalendar;
            KeyEventArgs keyEventArgs = (KeyEventArgs)e;
            if (keyEventArgs.Key != Key.Escape && (keyEventArgs.Key != Key.Return && keyEventArgs.Key != Key.Space || persianCalendar.DisplayMode != PersianDateTimeWPFTools.Windows.Controls.CalendarMode.Month))
                return;
            this.IsDropDownOpen = false;
            if (keyEventArgs.Key != Key.Escape)
                return;
            this.SelectedDate = this._originalSelectedDate;
        }

        private void Calendar_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0 && this.SelectedDate.HasValue && DateTime.Compare((DateTime)e.AddedItems[0], this.SelectedDate.Value) != 0)
                this.SelectedDate = (DateTime?)e.AddedItems[0];
            else if (e.AddedItems.Count == 0)
            {
                this.SelectedDate = new DateTime?();
            }
            else
            {
                if (this.SelectedDate.HasValue || e.AddedItems.Count <= 0)
                    return;
                this.SelectedDate = (DateTime?)e.AddedItems[0];
            }
        }

        private string DateTimeToString(DateTime d)
        {
            var culture = CustomCulture ?? PersianDateTimeWPFTools.Windows.Controls.DateTimeHelper.GetCulture((FrameworkElement)this);
            //PersianDateTimeWPFTools.Windows.Controls.DateTimeHelper.GetDateFormat(culture);
            System.Globalization.Calendar persianCalendar = culture.Calendar;
            switch (this.SelectedDateFormat)
            {
                case PersianDateTimeWPFTools.Windows.Controls.DatePickerFormat.Long:
                    return string.Format("{0:0000}/{1:00}/{2:00}", (object)persianCalendar.GetYear(d), (object)persianCalendar.GetMonth(d), (object)persianCalendar.GetDayOfMonth(d));
                case PersianDateTimeWPFTools.Windows.Controls.DatePickerFormat.Short:
                    return string.Format("{0:0000}/{1:00}/{2:00}", (object)(persianCalendar.GetYear(d) /*% 100*/), (object)persianCalendar.GetMonth(d), (object)persianCalendar.GetDayOfMonth(d));
                default:
                    return (string)null;
            }
        }

        private static DateTime DiscardDayTime(DateTime d) => new DateTime(d.Year, d.Month, 1, 0, 0, 0);

        private static DateTime? DiscardTime(DateTime? d)
        {
            if (!d.HasValue)
                return new DateTime?();
            DateTime dateTime = d.Value;
            return new DateTime?(new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 0, 0, 0));
        }

        private void DropDownButton_Click(object sender, RoutedEventArgs e) => this.TogglePopUp();

        private void DropDownButton_MouseLeave(object sender, MouseEventArgs e)
        {
            this._disablePopupReopen = false;
        }

        private void TogglePopUp()
        {
            if (this.IsDropDownOpen)
                this.IsDropDownOpen = false;
            else if (this._disablePopupReopen)
            {
                this._disablePopupReopen = false;
            }
            else
            {
                this.SetSelectedDate();
                this.IsDropDownOpen = true;
            }
        }

        private void InitializeCalendar()
        {
            this._persianCalendar = new PersianCalendar();
            this._persianCalendar.DayButtonMouseUp += new MouseButtonEventHandler(this.Calendar_DayButtonMouseUp);
            this._persianCalendar.DisplayDateChanged += new EventHandler<PersianDateTimeWPFTools.Windows.Controls.CalendarDateChangedEventArgs>(this.Calendar_DisplayDateChanged);
            this._persianCalendar.SelectedDatesChanged += new EventHandler<SelectionChangedEventArgs>(this.Calendar_SelectedDatesChanged);
            this._persianCalendar.DayOrMonthPreviewKeyDown += new RoutedEventHandler(this.CalendarDayOrMonthButton_PreviewKeyDown);
            this._persianCalendar.HorizontalAlignment = HorizontalAlignment.Left;
            this._persianCalendar.VerticalAlignment = VerticalAlignment.Top;
            this._persianCalendar.SelectionMode = PersianDateTimeWPFTools.Windows.Controls.CalendarSelectionMode.SingleDate;
            this._persianCalendar.SetBinding(Control.ForegroundProperty, this.GetDatePickerBinding(Control.ForegroundProperty));
            this._persianCalendar.SetBinding(FrameworkElement.StyleProperty, this.GetDatePickerBinding(PersianDatePicker.CalendarStyleProperty));
            this._persianCalendar.SetBinding(PersianCalendar.IsTodayHighlightedProperty, this.GetDatePickerBinding(PersianDatePicker.IsTodayHighlightedProperty));
            this._persianCalendar.SetBinding(PersianCalendar.FirstDayOfWeekProperty, this.GetDatePickerBinding(PersianDatePicker.FirstDayOfWeekProperty));
            this._persianCalendar.SetBinding(PersianCalendar.AllowSelectBlackedOutDayProperty, this.GetDatePickerBinding(PersianDatePicker.AllowSelectBlackedOutDayProperty));
            this._persianCalendar.SetBinding(PersianCalendar.CustomCultureProperty, this.GetDatePickerBinding(PersianDatePicker.CustomCultureProperty));
            this._persianCalendar.SetBinding(PersianCalendar.CustomCultureNameProperty, this.GetDatePickerBinding(PersianDatePicker.CustomCultureNameProperty));
        }

        private BindingBase GetDatePickerBinding(DependencyProperty property)
        {
            return (BindingBase)new Binding(property.Name)
            {
                Source = (object)this
            };
        }

        private static bool IsValidSelectedDateFormat(object value)
        {
            PersianDateTimeWPFTools.Windows.Controls.DatePickerFormat datePickerFormat = (PersianDateTimeWPFTools.Windows.Controls.DatePickerFormat)value;
            return datePickerFormat == PersianDateTimeWPFTools.Windows.Controls.DatePickerFormat.Long || datePickerFormat == PersianDateTimeWPFTools.Windows.Controls.DatePickerFormat.Short;
        }

        private DateTime? ParseText(string text)
        {
            try
            {
                DateTime persianDate = this.ParsePersianDate(text);
                if (PersianCalendar.IsValidDateSelection(this._persianCalendar, (object)persianDate))
                    return new DateTime?(persianDate);
                PersianDateTimeWPFTools.Windows.Controls.DatePickerDateValidationErrorEventArgs e = new PersianDateTimeWPFTools.Windows.Controls.DatePickerDateValidationErrorEventArgs((Exception)new ArgumentOutOfRangeException(nameof(text), "SelectedDate value is not valid."), text);
                this.OnDateValidationError(e);
                if (e.ThrowException)
                    throw e.Exception;
            }
            catch (FormatException ex)
            {
                string text1 = text;
                PersianDateTimeWPFTools.Windows.Controls.DatePickerDateValidationErrorEventArgs e = new PersianDateTimeWPFTools.Windows.Controls.DatePickerDateValidationErrorEventArgs((Exception)ex, text1);
                this.OnDateValidationError(e);
                if (e.ThrowException)
                {
                    if (e.Exception != null)
                        throw e.Exception;
                }
            }
            return new DateTime?();
        }

        private DateTime ParsePersianDate(string date)
        {
            var culture = CustomCulture ?? PersianDateTimeWPFTools.Windows.Controls.DateTimeHelper.GetCulture((FrameworkElement)this);
            if (culture.LCID == CalendarHelper.PersianCultureLCID)
            {
                string[] strArray = date.Split('/');
                int year = strArray.Length == 3 ? int.Parse(strArray[0]) : throw new FormatException("Could not parse the given string as a Persian date.");
                int month = int.Parse(strArray[1]);
                int day = int.Parse(strArray[2]);
                if (year >= 100 && year <= 999 || year > 9999 || year < 0)
                    throw new FormatException("Could not parse the given string as a Persian date. Year part of the date string is incorrect format.");
                if (month < 1 || month > 12)
                    throw new FormatException("Could not parse the given string as a Persian date. Month part of the date string is incorrect format.");
                if (day < 1 || day > 31)
                    throw new FormatException("Could not parse the given string as a Persian date. Day part of the date string is incorrect format.");
                if (year < 1000)
                    year += 1300;
                //return new System.Globalization.PersianCalendar().ToDateTime(year, month, day, 0, 0, 0, 0);
                return culture.Calendar.ToDateTime(year, month, day, 0, 0, 0, 0);
            }
            else
            {
                DateTime? newSelectedDate;

                // TryParse is not used in order to be able to pass the exception to the TextParseError event
                newSelectedDate = DateTime.Parse(date, culture);

                if (newSelectedDate != null)
                {
                    return (DateTime)newSelectedDate;
                }
                else
                {
                    throw new ArgumentOutOfRangeException(nameof(date));
                }
            }
        }

        private bool ProcessDatePickerKey(KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Return:
                    this.SetSelectedDate();
                    return true;
                case Key.System:
                    if (e.SystemKey == Key.Down && (Keyboard.Modifiers & ModifierKeys.Alt) == ModifierKeys.Alt)
                    {
                        this.TogglePopUp();
                        return true;
                    }
                    break;
            }
            return false;
        }

        private void SetSelectedDate()
        {
            if (this._textBox != null)
            {
                if (!string.IsNullOrEmpty(this._textBox.Text))
                {
                    string text = this._textBox.Text;
                    DateTime? selectedDate = this.SelectedDate;
                    if (selectedDate.HasValue)
                    {
                        selectedDate = this.SelectedDate;
                        if (this.DateTimeToString(selectedDate.Value) == text)
                            return;
                    }
                    DateTime? nullable = this.SetTextBoxValue(text);
                    selectedDate = this.SelectedDate;
                    if (selectedDate.Equals((object)nullable))
                        return;
                    this.SelectedDate = nullable;
                    this.DisplayDate = nullable.Value;
                }
                else
                {
                    DateTime? nullable = this.SelectedDate;
                    if (!nullable.HasValue)
                        return;
                    nullable = new DateTime?();
                    this.SelectedDate = nullable;
                }
            }
            else
            {
                DateTime? nullable = this.SetTextBoxValue(this._defaultText);
                if (this.SelectedDate.Equals((object)nullable))
                    return;
                this.SelectedDate = nullable;
            }
        }

        private DateTime? SetTextBoxValue(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                this.SetValue(PersianDatePicker.TextProperty, (object)s);
                return this.SelectedDate;
            }
            DateTime? text = this.ParseText(s);
            if (text.HasValue)
            {
                this.SetValue(PersianDatePicker.TextProperty, (object)this.DateTimeToString(text.Value));
                return text;
            }
            DateTime? nullable = this.SelectedDate;
            if (nullable.HasValue)
            {
                nullable = this.SelectedDate;
                string str = this.DateTimeToString(nullable.Value);
                this.SetValue(PersianDatePicker.TextProperty, (object)str);
                return this.SelectedDate;
            }
            this.SetWaterMarkText();
            nullable = new DateTime?();
            return nullable;
        }

        private void SetWaterMarkText()
        {
            if (this._textBox == null)
                return;
            var culture = CustomCulture ?? PersianDateTimeWPFTools.Windows.Controls.DateTimeHelper.GetCulture((FrameworkElement)this);
            DateTimeFormatInfo dateFormat = PersianDateTimeWPFTools.Windows.Controls.DateTimeHelper.GetDateFormat(culture);
            this.SetTextInternal(string.Empty);
            this._defaultText = string.Empty;
            switch (this.SelectedDateFormat)
            {
                case PersianDateTimeWPFTools.Windows.Controls.DatePickerFormat.Long:
                    this._textBox.Watermark = (object)string.Format((IFormatProvider)CultureInfo.CurrentCulture, "Select a date", new object[1]
                    {
            (object) dateFormat.LongDatePattern.ToString()
                    });
                    break;
                case PersianDateTimeWPFTools.Windows.Controls.DatePickerFormat.Short:
                    this._textBox.Watermark = (object)string.Format((IFormatProvider)CultureInfo.CurrentCulture, "Select a date", new object[1]
                    {
            (object) dateFormat.ShortDatePattern.ToString()
                    });
                    break;
            }
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e) => this.SetSelectedDate();

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = this.ProcessDatePickerKey(e) || e.Handled;
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.SetValueNoCallback(PersianDatePicker.TextProperty, (object)this._textBox.Text);
        }

        private void UpdateDisabledVisual()
        {
            if (!this.IsEnabled)
                PersianDateTimeWPFTools.Windows.Controls.VisualStates.GoToState((Control)this, true, "Disabled", "Normal");
            else
                PersianDateTimeWPFTools.Windows.Controls.VisualStates.GoToState((Control)this, true, "Normal");
        }
    }
}
