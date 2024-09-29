using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Threading;
using PersianDateTimeWPFTools.Windows.Controls;
using PersianDateTimeWPFTools.Windows.Controls.Primitives;
using CalendarButton = PersianDateTimeWPFTools.Windows.Controls.Primitives.CalendarButton;
using CalendarDayButton = PersianDateTimeWPFTools.Windows.Controls.Primitives.CalendarDayButton;
using CalendarItem = PersianDateTimeWPFTools.Windows.Controls.Primitives.CalendarItem;

namespace PersianDateTimeWPFTools.Controls
{
    [TemplatePart(Name = "PART_Root", Type = typeof(Panel))]
    [TemplatePart(Name = "PART_CalendarItem", Type = typeof(PersianDateTimeWPFTools.Windows.Controls.Primitives.CalendarItem))]
    public class PersianCalendar : Control
    {
        private const string ElementRoot = "PART_Root";
        private const string ElementMonth = "PART_CalendarItem";
        private const int COLS = 7;
        private const int ROWS = 7;
        private const int YEAR_ROWS = 3;
        private const int YEAR_COLS = 4;
        private const int YEARS_PER_DECADE = 10;
        private DateTime? _hoverStart;
        private DateTime? _hoverEnd;
        private bool _isShiftPressed;
        private DateTime? _currentDate;
        internal System.Globalization.Calendar _calendar;
        private DateTimeHelper dateTimeHelper;
        private PersianDateTimeWPFTools.Windows.Controls.Primitives.CalendarItem _monthControl;
        private PersianDateTimeWPFTools.Windows.Controls.CalendarBlackoutDatesCollection _blackoutDates;
        private PersianDateTimeWPFTools.Windows.Controls.SelectedDatesCollection _selectedDates;
        public static readonly RoutedEvent SelectedDatesChangedEvent = EventManager.RegisterRoutedEvent("SelectedDatesChanged", RoutingStrategy.Direct, typeof(EventHandler<SelectionChangedEventArgs>), typeof(PersianCalendar));
        public static readonly DependencyProperty CalendarButtonStyleProperty = DependencyProperty.Register(nameof(CalendarButtonStyle), typeof(Style), typeof(PersianCalendar));
        public static readonly DependencyProperty CalendarDayButtonStyleProperty = DependencyProperty.Register(nameof(CalendarDayButtonStyle), typeof(Style), typeof(PersianCalendar));
        public static readonly DependencyProperty CalendarItemStyleProperty = DependencyProperty.Register(nameof(CalendarItemStyle), typeof(Style), typeof(PersianCalendar));
        public static readonly DependencyProperty DisplayDateProperty = DependencyProperty.Register(nameof(DisplayDate), typeof(DateTime), typeof(PersianCalendar), (PropertyMetadata)new FrameworkPropertyMetadata((object)DateTime.MinValue, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(PersianCalendar.OnDisplayDateChanged), new CoerceValueCallback(PersianCalendar.CoerceDisplayDate)));
        public static readonly DependencyProperty DisplayDateEndProperty = DependencyProperty.Register(nameof(DisplayDateEnd), typeof(DateTime?), typeof(PersianCalendar), (PropertyMetadata)new FrameworkPropertyMetadata((object)null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(PersianCalendar.OnDisplayDateEndChanged), new CoerceValueCallback(PersianCalendar.CoerceDisplayDateEnd)));
        public static readonly DependencyProperty DisplayDateStartProperty = DependencyProperty.Register(nameof(DisplayDateStart), typeof(DateTime?), typeof(PersianCalendar), (PropertyMetadata)new FrameworkPropertyMetadata((object)null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(PersianCalendar.OnDisplayDateStartChanged), new CoerceValueCallback(PersianCalendar.CoerceDisplayDateStart)));
        public static readonly DependencyProperty DisplayModeProperty = DependencyProperty.Register(nameof(DisplayMode), typeof(PersianDateTimeWPFTools.Windows.Controls.CalendarMode), typeof(PersianCalendar), (PropertyMetadata)new FrameworkPropertyMetadata((object)PersianDateTimeWPFTools.Windows.Controls.CalendarMode.Month, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(PersianCalendar.OnDisplayModePropertyChanged)), new ValidateValueCallback(PersianCalendar.IsValidDisplayMode));
        public static readonly DependencyProperty FirstDayOfWeekProperty = DependencyProperty.Register(nameof(FirstDayOfWeek), typeof(DayOfWeek), typeof(PersianCalendar), (PropertyMetadata)new FrameworkPropertyMetadata((object)PersianDateTimeWPFTools.Windows.Controls.DateTimeHelper.GetCurrentDateFormat().FirstDayOfWeek, new PropertyChangedCallback(PersianCalendar.OnFirstDayOfWeekChanged)), new ValidateValueCallback(PersianCalendar.IsValidFirstDayOfWeek));
        public static readonly DependencyProperty IsTodayHighlightedProperty = DependencyProperty.Register(nameof(IsTodayHighlighted), typeof(bool), typeof(PersianCalendar), (PropertyMetadata)new FrameworkPropertyMetadata((object)true, new PropertyChangedCallback(PersianCalendar.OnIsTodayHighlightedChanged)));
        public static readonly DependencyProperty SelectedDateProperty = DependencyProperty.Register(nameof(SelectedDate), typeof(DateTime?), typeof(PersianCalendar), (PropertyMetadata)new FrameworkPropertyMetadata((object)null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(PersianCalendar.OnSelectedDateChanged)));
        public static readonly DependencyProperty SelectionModeProperty = DependencyProperty.Register(nameof(SelectionMode), typeof(PersianDateTimeWPFTools.Windows.Controls.CalendarSelectionMode), typeof(PersianCalendar), (PropertyMetadata)new FrameworkPropertyMetadata((object)PersianDateTimeWPFTools.Windows.Controls.CalendarSelectionMode.SingleDate, new PropertyChangedCallback(PersianCalendar.OnSelectionModeChanged)), new ValidateValueCallback(PersianCalendar.IsValidSelectionMode));

        public static readonly DependencyProperty AllowSelectBlackedOutDayProperty = DependencyProperty.Register(nameof(AllowSelectBlackedOutDay), typeof(bool), typeof(PersianCalendar), (PropertyMetadata)new FrameworkPropertyMetadata((object)false, new PropertyChangedCallback(PersianCalendar.OnAllowSelectBlackedOutDayChanged)));
        public static readonly DependencyProperty CustomCultureProperty = DependencyProperty.Register(nameof(CustomCulture), typeof(CultureInfo), typeof(PersianCalendar), (PropertyMetadata)new FrameworkPropertyMetadata((object)null, new PropertyChangedCallback(PersianCalendar.OnCustomCultureChanged)));
        public static readonly DependencyProperty CustomCultureNameProperty = DependencyProperty.Register(nameof(CustomCultureName), typeof(string), typeof(PersianCalendar), (PropertyMetadata)new FrameworkPropertyMetadata((object)null, new PropertyChangedCallback(PersianCalendar.OnCustomCultureNameChanged)));



        public event EventHandler<SelectionChangedEventArgs> SelectedDatesChanged
        {
            add => this.AddHandler(PersianCalendar.SelectedDatesChangedEvent, (Delegate)value);
            remove => this.RemoveHandler(PersianCalendar.SelectedDatesChangedEvent, (Delegate)value);
        }

        public event EventHandler<PersianDateTimeWPFTools.Windows.Controls.CalendarDateChangedEventArgs> DisplayDateChanged;

        public event EventHandler<PersianDateTimeWPFTools.Windows.Controls.CalendarModeChangedEventArgs> DisplayModeChanged;

        public event EventHandler<EventArgs> SelectionModeChanged;



        static PersianCalendar()
        {
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(PersianCalendar), (PropertyMetadata)new FrameworkPropertyMetadata((object)typeof(PersianCalendar)));
            KeyboardNavigation.TabNavigationProperty.OverrideMetadata(typeof(PersianCalendar), (PropertyMetadata)new FrameworkPropertyMetadata((object)KeyboardNavigationMode.Once));
            KeyboardNavigation.DirectionalNavigationProperty.OverrideMetadata(typeof(PersianCalendar), (PropertyMetadata)new FrameworkPropertyMetadata((object)KeyboardNavigationMode.Contained));
            EventManager.RegisterClassHandler(typeof(PersianCalendar), UIElement.GotFocusEvent, (Delegate)new RoutedEventHandler(PersianCalendar.OnGotFocus));
            FrameworkElement.LanguageProperty.OverrideMetadata(typeof(PersianCalendar), (PropertyMetadata)new FrameworkPropertyMetadata(new PropertyChangedCallback(PersianCalendar.OnLanguageChanged)));
        }

        public PersianCalendar()
        {
            this._calendar = DateTimeHelper.GetCulture((FrameworkElement)this).Calendar;
            dateTimeHelper = new DateTimeHelper(_calendar);
            this._blackoutDates = new PersianDateTimeWPFTools.Windows.Controls.CalendarBlackoutDatesCollection(this);
            this._selectedDates = new PersianDateTimeWPFTools.Windows.Controls.SelectedDatesCollection(this);
            this.DisplayDate = DateTime.Today;
        }

        public PersianDateTimeWPFTools.Windows.Controls.CalendarBlackoutDatesCollection BlackoutDates
        {
            get => this._blackoutDates;
        }

        public Style CalendarButtonStyle
        {
            get => (Style)this.GetValue(PersianCalendar.CalendarButtonStyleProperty);
            set => this.SetValue(PersianCalendar.CalendarButtonStyleProperty, (object)value);
        }

        public Style CalendarDayButtonStyle
        {
            get => (Style)this.GetValue(PersianCalendar.CalendarDayButtonStyleProperty);
            set => this.SetValue(PersianCalendar.CalendarDayButtonStyleProperty, (object)value);
        }

        public Style CalendarItemStyle
        {
            get => (Style)this.GetValue(PersianCalendar.CalendarItemStyleProperty);
            set => this.SetValue(PersianCalendar.CalendarItemStyleProperty, (object)value);
        }

        public DateTime DisplayDate
        {
            get => (DateTime)this.GetValue(PersianCalendar.DisplayDateProperty);
            set => this.SetValue(PersianCalendar.DisplayDateProperty, (object)value);
        }

        public bool AllowSelectBlackedOutDay
        {
            get => (bool)this.GetValue(PersianCalendar.AllowSelectBlackedOutDayProperty);
            set => this.SetValue(PersianCalendar.AllowSelectBlackedOutDayProperty, (object)value);
        }

        public string CustomCultureName
        {
            get => (string)this.GetValue(PersianCalendar.CustomCultureNameProperty);
            set => this.SetValue(PersianCalendar.CustomCultureNameProperty, (object)value);
        }

        private static void OnDisplayDateChanged(
          DependencyObject d,
          DependencyPropertyChangedEventArgs e)
        {
            PersianCalendar persianCalendar = d as PersianCalendar;
            var cal = persianCalendar.MonthControl?._calendar ?? persianCalendar._calendar;
            persianCalendar.DisplayDateInternal = new PersianDateTimeWPFTools.Windows.Controls.DateTimeHelper(cal).DiscardDayTime((DateTime)e.NewValue);
            persianCalendar.UpdateCellItems();
            persianCalendar.OnDisplayDateChanged(new PersianDateTimeWPFTools.Windows.Controls.CalendarDateChangedEventArgs(new DateTime?((DateTime)e.OldValue), new DateTime?((DateTime)e.NewValue)));
        }

        private static object CoerceDisplayDate(DependencyObject d, object value)
        {

            PersianCalendar persianCalendar = d as PersianCalendar;
            DateTime dateTime = (DateTime)value;
            if (persianCalendar.DisplayDateStart.HasValue && dateTime < persianCalendar.DisplayDateStart.Value)
                value = (object)persianCalendar.DisplayDateStart.Value;
            else if (persianCalendar.DisplayDateEnd.HasValue && dateTime > persianCalendar.DisplayDateEnd.Value)
                value = (object)persianCalendar.DisplayDateEnd.Value;
            return value;
        }

        public DateTime? DisplayDateEnd
        {
            get => (DateTime?)this.GetValue(PersianCalendar.DisplayDateEndProperty);
            set => this.SetValue(PersianCalendar.DisplayDateEndProperty, (object)value);
        }

        private static void OnDisplayDateEndChanged(
          DependencyObject d,
          DependencyPropertyChangedEventArgs e)
        {
            PersianCalendar persianCalendar = d as PersianCalendar;
            persianCalendar.CoerceValue(PersianCalendar.DisplayDateProperty);
            persianCalendar.UpdateCellItems();
        }

        private static object CoerceDisplayDateEnd(DependencyObject d, object value)
        {
            PersianCalendar persianCalendar = d as PersianCalendar;
            DateTime? nullable = (DateTime?)value;
            if (nullable.HasValue)
            {
                if (persianCalendar.DisplayDateStart.HasValue && nullable.Value < persianCalendar.DisplayDateStart.Value)
                    value = (object)persianCalendar.DisplayDateStart;
                DateTime? maximumDate = persianCalendar.SelectedDates.MaximumDate;
                if (maximumDate.HasValue && nullable.Value < maximumDate.Value)
                    value = (object)maximumDate;
            }
            return value;
        }

        public DateTime? DisplayDateStart
        {
            get => (DateTime?)this.GetValue(PersianCalendar.DisplayDateStartProperty);
            set => this.SetValue(PersianCalendar.DisplayDateStartProperty, (object)value);
        }

        private static void OnDisplayDateStartChanged(
          DependencyObject d,
          DependencyPropertyChangedEventArgs e)
        {
            PersianCalendar persianCalendar = d as PersianCalendar;
            persianCalendar.CoerceValue(PersianCalendar.DisplayDateEndProperty);
            persianCalendar.CoerceValue(PersianCalendar.DisplayDateProperty);
            persianCalendar.UpdateCellItems();
        }

        private static object CoerceDisplayDateStart(DependencyObject d, object value)
        {
            PersianCalendar persianCalendar = d as PersianCalendar;
            DateTime? nullable = (DateTime?)value;
            if (nullable.HasValue)
            {
                DateTime? minimumDate = persianCalendar.SelectedDates.MinimumDate;
                if (minimumDate.HasValue && nullable.Value > minimumDate.Value)
                    value = (object)minimumDate;
            }
            return value;
        }

        public PersianDateTimeWPFTools.Windows.Controls.CalendarMode DisplayMode
        {
            get => (PersianDateTimeWPFTools.Windows.Controls.CalendarMode)this.GetValue(PersianCalendar.DisplayModeProperty);
            set => this.SetValue(PersianCalendar.DisplayModeProperty, (object)value);
        }

        private static void OnDisplayModePropertyChanged(
          DependencyObject d,
          DependencyPropertyChangedEventArgs e)
        {
            PersianCalendar persianCalendar1 = d as PersianCalendar;
            PersianDateTimeWPFTools.Windows.Controls.CalendarMode newValue = (PersianDateTimeWPFTools.Windows.Controls.CalendarMode)e.NewValue;
            PersianDateTimeWPFTools.Windows.Controls.CalendarMode oldValue = (PersianDateTimeWPFTools.Windows.Controls.CalendarMode)e.OldValue;
            CalendarItem monthControl = persianCalendar1.MonthControl;
            switch (newValue)
            {
                case PersianDateTimeWPFTools.Windows.Controls.CalendarMode.Month:
                    if (oldValue == PersianDateTimeWPFTools.Windows.Controls.CalendarMode.Year || oldValue == PersianDateTimeWPFTools.Windows.Controls.CalendarMode.Decade)
                    {
                        PersianCalendar persianCalendar2 = persianCalendar1;
                        PersianCalendar persianCalendar3 = persianCalendar1;
                        DateTime? nullable1 = new DateTime?();
                        DateTime? nullable2 = nullable1;
                        persianCalendar3.HoverEnd = nullable2;
                        DateTime? nullable3 = nullable1;
                        persianCalendar2.HoverStart = nullable3;
                        persianCalendar1.CurrentDate = persianCalendar1.DisplayDate;
                    }
                    persianCalendar1.UpdateCellItems();
                    break;
                case PersianDateTimeWPFTools.Windows.Controls.CalendarMode.Year:
                case PersianDateTimeWPFTools.Windows.Controls.CalendarMode.Decade:
                    if (oldValue == PersianDateTimeWPFTools.Windows.Controls.CalendarMode.Month)
                        persianCalendar1.DisplayDate = persianCalendar1.CurrentDate;
                    persianCalendar1.UpdateCellItems();
                    break;
            }
            persianCalendar1.OnDisplayModeChanged(new PersianDateTimeWPFTools.Windows.Controls.CalendarModeChangedEventArgs((PersianDateTimeWPFTools.Windows.Controls.CalendarMode)e.OldValue, newValue));
        }

        public DayOfWeek FirstDayOfWeek
        {
            get => (DayOfWeek)this.GetValue(PersianCalendar.FirstDayOfWeekProperty);
            set => this.SetValue(PersianCalendar.FirstDayOfWeekProperty, (object)value);
        }

        private static void OnFirstDayOfWeekChanged(
          DependencyObject d,
          DependencyPropertyChangedEventArgs e)
        {
            (d as PersianCalendar).UpdateCellItems();
        }

        public bool IsTodayHighlighted
        {
            get => (bool)this.GetValue(PersianCalendar.IsTodayHighlightedProperty);
            set => this.SetValue(PersianCalendar.IsTodayHighlightedProperty, (object)value);
        }


        private static void OnIsTodayHighlightedChanged(
          DependencyObject d,
          DependencyPropertyChangedEventArgs e)
        {
            PersianCalendar persianCalendar = d as PersianCalendar;
            int num = new PersianDateTimeWPFTools.Windows.Controls.DateTimeHelper(persianCalendar._calendar).CompareYearMonth(persianCalendar.DisplayDateInternal, DateTime.Today);
            if (num <= -2 || num >= 2)
                return;
            persianCalendar.UpdateCellItems();
        }

        private static void OnAllowSelectBlackedOutDayChanged(
          DependencyObject d,
          DependencyPropertyChangedEventArgs e)
        {
            PersianCalendar persianCalendar = d as PersianCalendar;
            persianCalendar.UpdateCellItems();
        }

        public CultureInfo CustomCulture
        {
            get => this.GetValue(PersianCalendar.CustomCultureProperty) as CultureInfo;

            set => this.SetValue(PersianCalendar.CustomCultureProperty, (object)value);
        }


        private static void OnCustomCultureNameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PersianCalendar persianCalendar = d as PersianCalendar;
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

        private static void OnCustomCultureChanged(
          DependencyObject d,
          DependencyPropertyChangedEventArgs e)
        {
            PersianCalendar persianCalendar = d as PersianCalendar;

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
            }


        }

        private static void OnLanguageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PersianCalendar persianCalendar = d as PersianCalendar;
            if (DependencyPropertyHelper.GetValueSource(d, PersianCalendar.FirstDayOfWeekProperty).BaseValueSource != BaseValueSource.Default)
                return;
            persianCalendar.CoerceValue(PersianCalendar.FirstDayOfWeekProperty);
            persianCalendar.UpdateCellItems();
        }

        public DateTime? SelectedDate
        {
            get => (DateTime?)this.GetValue(PersianCalendar.SelectedDateProperty);
            set => this.SetValue(PersianCalendar.SelectedDateProperty, (object)value);
        }

        private static void OnSelectedDateChanged(
          DependencyObject d,
          DependencyPropertyChangedEventArgs e)
        {
            PersianCalendar cal = d as PersianCalendar;
            if (cal.SelectionMode == PersianDateTimeWPFTools.Windows.Controls.CalendarSelectionMode.None && e.NewValue != null)
                throw new InvalidOperationException("The SelectedDate property cannot be set when the selection mode is None.");
            DateTime? newValue = (DateTime?)e.NewValue;
            if (!PersianCalendar.IsValidDateSelection(cal, (object)newValue))
                throw new ArgumentOutOfRangeException(nameof(d), "SelectedDate value is not valid.");
            if (!newValue.HasValue)
                cal.SelectedDates.ClearInternal(true);
            else if (newValue.HasValue && (cal.SelectedDates.Count <= 0 || !(cal.SelectedDates[0] == newValue.Value)))
            {
                cal.SelectedDates.ClearInternal();
                cal.SelectedDates.Add(newValue.Value);
            }
            if (cal.SelectionMode != PersianDateTimeWPFTools.Windows.Controls.CalendarSelectionMode.SingleDate)
            {
                return;
            }
            if (newValue.HasValue)
                cal.CurrentDate = newValue.Value;
            cal.UpdateCellItems();
        }

        public PersianDateTimeWPFTools.Windows.Controls.SelectedDatesCollection SelectedDates => this._selectedDates;

        public PersianDateTimeWPFTools.Windows.Controls.CalendarSelectionMode SelectionMode
        {
            get => (PersianDateTimeWPFTools.Windows.Controls.CalendarSelectionMode)this.GetValue(PersianCalendar.SelectionModeProperty);
            set => this.SetValue(PersianCalendar.SelectionModeProperty, (object)value);
        }

        private static void OnSelectionModeChanged(
          DependencyObject d,
          DependencyPropertyChangedEventArgs e)
        {
            PersianCalendar persianCalendar = d as PersianCalendar;
            DateTime? nullable = new DateTime?();
            persianCalendar.HoverEnd = nullable;
            persianCalendar.HoverStart = nullable;
            persianCalendar.SelectedDates.ClearInternal(true);
            persianCalendar.OnSelectionModeChanged(EventArgs.Empty);
        }

        internal event MouseButtonEventHandler DayButtonMouseUp;

        internal event RoutedEventHandler DayOrMonthPreviewKeyDown;

        internal bool DatePickerDisplayDateFlag { get; set; }

        internal DateTime DisplayDateInternal { get; private set; }

        internal DateTime DisplayDateEndInternal
        {
            get
            {

                return this.MonthControl._calendar.MaxSupportedDateTime;

                //return this.DisplayDateEnd.GetValueOrDefault(PersianCalendarHelper.GetCurrentCalendar().MaxSupportedDateTime);
            }
        }

        internal DateTime DisplayDateStartInternal
        {
            get
            {
                //return this.DisplayDateStart.GetValueOrDefault(PersianCalendarHelper.GetCurrentCalendar().MinSupportedDateTime);
                return this.MonthControl._calendar.MinSupportedDateTime;
            }
        }

        internal DateTime CurrentDate
        {
            get => this._currentDate.GetValueOrDefault(this.DisplayDateInternal);
            set => this._currentDate = new DateTime?(value);
        }

        internal DateTime? HoverStart
        {
            get => this.SelectionMode != PersianDateTimeWPFTools.Windows.Controls.CalendarSelectionMode.None ? this._hoverStart : new DateTime?();
            set => this._hoverStart = value;
        }

        internal DateTime? HoverEnd
        {
            get => this.SelectionMode != PersianDateTimeWPFTools.Windows.Controls.CalendarSelectionMode.None ? this._hoverEnd : new DateTime?();
            set => this._hoverEnd = value;
        }

        internal CalendarItem MonthControl => this._monthControl;

        internal DateTime DisplayMonth => dateTimeHelper.DiscardDayTime(this.DisplayDate);

        internal DateTime DisplayYear => dateTimeHelper.DiscardMonthDayTime(this.DisplayDate);

        private new bool IsRightToLeft => this.FlowDirection == FlowDirection.RightToLeft;

        public override void OnApplyTemplate()
        {
            if (this._monthControl != null)
                this._monthControl.Owner = this;
            base.OnApplyTemplate();
            this._monthControl = this.GetTemplateChild("PART_CalendarItem") as CalendarItem;
            if (this._monthControl != null)
                this._monthControl.Owner = this;

            _monthControl.CustomCulture = CustomCulture;
            this.CurrentDate = this.DisplayDate;
            this.UpdateCellItems();

            if (CustomCulture != null)
            {
                DisplayDate = DateTime.Today;
                OnDayClick(DateTime.Today);
            }
        }

        public override string ToString()
        {
            var culture = CustomCulture ?? PersianDateTimeWPFTools.Windows.Controls.DateTimeHelper.GetCulture((FrameworkElement)this);
            return this.SelectedDate.HasValue ? this.SelectedDate.Value.ToString((IFormatProvider)PersianDateTimeWPFTools.Windows.Controls.DateTimeHelper.GetDateFormat(culture)) : string.Empty;
        }

        protected virtual void OnSelectedDatesChanged(SelectionChangedEventArgs e)
        {
            this.RaiseEvent((RoutedEventArgs)e);
        }

        protected virtual void OnDisplayDateChanged(PersianDateTimeWPFTools.Windows.Controls.CalendarDateChangedEventArgs e)
        {
            EventHandler<PersianDateTimeWPFTools.Windows.Controls.CalendarDateChangedEventArgs> displayDateChanged = this.DisplayDateChanged;
            if (displayDateChanged == null)
                return;
            displayDateChanged((object)this, e);
        }

        protected virtual void OnDisplayModeChanged(PersianDateTimeWPFTools.Windows.Controls.CalendarModeChangedEventArgs e)
        {
            EventHandler<PersianDateTimeWPFTools.Windows.Controls.CalendarModeChangedEventArgs> displayModeChanged = this.DisplayModeChanged;
            if (displayModeChanged == null)
                return;
            displayModeChanged((object)this, e);
        }

        protected virtual void OnSelectionModeChanged(EventArgs e)
        {
            EventHandler<EventArgs> selectionModeChanged = this.SelectionModeChanged;
            if (selectionModeChanged == null)
                return;
            selectionModeChanged((object)this, e);
        }

        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return (AutomationPeer)new PersianDateTimeWPFTools.Windows.Automation.Peers.CalendarAutomationPeer(this);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Handled)
                return;
            e.Handled = this.ProcessCalendarKey(e);
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            if (e.Handled || e.Key != Key.LeftShift && e.Key != Key.RightShift)
                return;
            this.ProcessShiftKeyUp();
        }

        internal CalendarDayButton FindDayButtonFromDay(DateTime day)
        {
            if (this.MonthControl != null)
            {
                foreach (CalendarDayButton calendarDayButton in this.MonthControl.GetCalendarDayButtons())
                {
                    if (calendarDayButton.DataContext is DateTime && PersianDateTimeWPFTools.Windows.Controls.DateTimeHelper.CompareDays((DateTime)calendarDayButton.DataContext, day) == 0)
                        return calendarDayButton;
                }
            }
            return (CalendarDayButton)null;
        }

        internal static bool IsValidDateSelection(PersianCalendar cal, object value)
        {
            return value == null || (cal.AllowSelectBlackedOutDay ? true : !cal.BlackoutDates.Contains((DateTime)value));
        }

        internal void OnDayButtonMouseUp(MouseButtonEventArgs e)
        {
            MouseButtonEventHandler dayButtonMouseUp = this.DayButtonMouseUp;
            if (dayButtonMouseUp == null)
                return;
            dayButtonMouseUp((object)this, e);
        }

        internal void OnDayOrMonthPreviewKeyDown(RoutedEventArgs e)
        {
            RoutedEventHandler monthPreviewKeyDown = this.DayOrMonthPreviewKeyDown;
            if (monthPreviewKeyDown == null)
                return;
            monthPreviewKeyDown((object)this, e);
        }

        internal void OnDayClick(DateTime selectedDate)
        {
            if (this.SelectionMode == PersianDateTimeWPFTools.Windows.Controls.CalendarSelectionMode.None)
                this.CurrentDate = selectedDate;
            if (dateTimeHelper.CompareYearMonth(selectedDate, this.DisplayDateInternal) != 0)
            {
                this.MoveDisplayTo(new DateTime?(selectedDate));
            }
            else
            {
                this.UpdateCellItems();
                this.FocusDate(selectedDate);
            }
        }

        internal void OnCalendarButtonPressed(CalendarButton b, bool switchDisplayMode)
        {
            if (!(b.DataContext is DateTime))
                return;
            DateTime dataContext = (DateTime)b.DataContext;
            DateTime? nullable = new DateTime?();
            PersianDateTimeWPFTools.Windows.Controls.CalendarMode calendarMode = PersianDateTimeWPFTools.Windows.Controls.CalendarMode.Month;
            switch (this.DisplayMode)
            {
                case PersianDateTimeWPFTools.Windows.Controls.CalendarMode.Year:
                    nullable = new PersianDateTimeWPFTools.Windows.Controls.DateTimeHelper(this.MonthControl._calendar).SetYearMonth(this.DisplayDate, dataContext);
                    calendarMode = PersianDateTimeWPFTools.Windows.Controls.CalendarMode.Month;
                    break;
                case PersianDateTimeWPFTools.Windows.Controls.CalendarMode.Decade:
                    nullable = new PersianDateTimeWPFTools.Windows.Controls.DateTimeHelper(this.MonthControl._calendar).SetYear(this.DisplayDate, dataContext);
                    calendarMode = PersianDateTimeWPFTools.Windows.Controls.CalendarMode.Year;
                    break;
            }
            if (!nullable.HasValue)
                return;
            this.DisplayDate = nullable.Value;
            if (!switchDisplayMode)
                return;
            this.DisplayMode = calendarMode;
            this.FocusDate(this.DisplayMode == PersianDateTimeWPFTools.Windows.Controls.CalendarMode.Month ? this.CurrentDate : this.DisplayDate);
        }

        private DateTime? GetDateOffset(DateTime date, int offset, PersianDateTimeWPFTools.Windows.Controls.CalendarMode displayMode)
        {
            DateTime? dateOffset = new DateTime?();
            switch (displayMode)
            {
                case PersianDateTimeWPFTools.Windows.Controls.CalendarMode.Month:
                    dateOffset = dateTimeHelper.AddMonths(date, offset);
                    break;
                case PersianDateTimeWPFTools.Windows.Controls.CalendarMode.Year:
                    dateOffset = dateTimeHelper.AddYears(date, offset);
                    break;
                case PersianDateTimeWPFTools.Windows.Controls.CalendarMode.Decade:
                    dateOffset = dateTimeHelper.AddYears(this.DisplayDate, offset * 10);
                    break;
            }
            return dateOffset;
        }

        private void MoveDisplayTo(DateTime? date)
        {
            if (!date.HasValue)
                return;
            DateTime date1 = date.Value.Date;
            switch (this.DisplayMode)
            {
                case PersianDateTimeWPFTools.Windows.Controls.CalendarMode.Month:
                    this.DisplayDate = dateTimeHelper.DiscardDayTime(date1);
                    this.CurrentDate = date1;
                    this.UpdateCellItems();
                    break;
                case PersianDateTimeWPFTools.Windows.Controls.CalendarMode.Year:
                case PersianDateTimeWPFTools.Windows.Controls.CalendarMode.Decade:
                    this.DisplayDate = date1;
                    this.UpdateCellItems();
                    break;
            }
            this.FocusDate(date1);
        }

        internal void OnNextClick()
        {
            DateTime? dateOffset = this.GetDateOffset(this.DisplayDate, 1, this.DisplayMode);
            if (!dateOffset.HasValue)
                return;
            this.MoveDisplayTo(new DateTime?(dateTimeHelper.DiscardDayTime(dateOffset.Value)));
        }

        internal void OnPreviousClick()
        {
            DateTime? dateOffset = this.GetDateOffset(this.DisplayDate, -1, this.DisplayMode);
            if (!dateOffset.HasValue)
                return;
            this.MoveDisplayTo(new DateTime?(dateTimeHelper.DiscardDayTime(dateOffset.Value)));
        }

        internal void OnSelectedDatesCollectionChanged(SelectionChangedEventArgs e)
        {
            if (!PersianCalendar.IsSelectionChanged(e))
                return;
            if ((AutomationPeer.ListenerExists(AutomationEvents.SelectionItemPatternOnElementSelected) || AutomationPeer.ListenerExists(AutomationEvents.SelectionItemPatternOnElementAddedToSelection) || AutomationPeer.ListenerExists(AutomationEvents.SelectionItemPatternOnElementRemovedFromSelection)) && UIElementAutomationPeer.FromElement((UIElement)this) is PersianDateTimeWPFTools.Windows.Automation.Peers.CalendarAutomationPeer calendarAutomationPeer)
                calendarAutomationPeer.RaiseSelectionEvents(e);
            this.CoerceFromSelection();
            this.OnSelectedDatesChanged(e);
        }

        internal void UpdateCellItems()
        {
            CalendarItem monthControl = this.MonthControl;
            if (monthControl == null)
                return;
            switch (this.DisplayMode)
            {
                case PersianDateTimeWPFTools.Windows.Controls.CalendarMode.Month:
                    monthControl.UpdateMonthMode();
                    break;
                case PersianDateTimeWPFTools.Windows.Controls.CalendarMode.Year:
                    monthControl.UpdateYearMode();
                    break;
                case PersianDateTimeWPFTools.Windows.Controls.CalendarMode.Decade:
                    monthControl.UpdateDecadeMode();
                    break;
            }
        }

        private void CoerceFromSelection()
        {
            this.CoerceValue(PersianCalendar.DisplayDateStartProperty);
            this.CoerceValue(PersianCalendar.DisplayDateEndProperty);
            this.CoerceValue(PersianCalendar.DisplayDateProperty);
        }

        private void AddKeyboardSelection()
        {
            if (!this.HoverStart.HasValue)
                return;
            this.SelectedDates.ClearInternal();
            this.SelectedDates.AddRange(this.HoverStart.Value, this.CurrentDate);
        }

        private static bool IsSelectionChanged(SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != e.RemovedItems.Count)
                return true;
            foreach (DateTime addedItem in (IEnumerable)e.AddedItems)
            {
                if (!e.RemovedItems.Contains((object)addedItem))
                    return true;
            }
            return false;
        }

        private static bool IsValidDisplayMode(object value)
        {
            PersianDateTimeWPFTools.Windows.Controls.CalendarMode calendarMode = (PersianDateTimeWPFTools.Windows.Controls.CalendarMode)value;
            switch (calendarMode)
            {
                case PersianDateTimeWPFTools.Windows.Controls.CalendarMode.Month:
                case PersianDateTimeWPFTools.Windows.Controls.CalendarMode.Year:
                    return true;
                default:
                    return calendarMode == PersianDateTimeWPFTools.Windows.Controls.CalendarMode.Decade;
            }
        }

        internal static bool IsValidFirstDayOfWeek(object value)
        {
            DayOfWeek dayOfWeek = (DayOfWeek)value;
            switch (dayOfWeek)
            {
                case DayOfWeek.Sunday:
                case DayOfWeek.Monday:
                case DayOfWeek.Tuesday:
                case DayOfWeek.Wednesday:
                case DayOfWeek.Thursday:
                case DayOfWeek.Friday:
                    return true;
                default:
                    return dayOfWeek == DayOfWeek.Saturday;
            }
        }

        private static bool IsValidKeyboardSelection(PersianCalendar cal, object value)
        {
            if (value == null)
                return true;
            return !cal.BlackoutDates.Contains((DateTime)value) && DateTime.Compare((DateTime)value, cal.DisplayDateStartInternal) >= 0 && DateTime.Compare((DateTime)value, cal.DisplayDateEndInternal) <= 0;
        }

        private static bool IsValidSelectionMode(object value)
        {
            PersianDateTimeWPFTools.Windows.Controls.CalendarSelectionMode calendarSelectionMode = (PersianDateTimeWPFTools.Windows.Controls.CalendarSelectionMode)value;
            switch (calendarSelectionMode)
            {
                case PersianDateTimeWPFTools.Windows.Controls.CalendarSelectionMode.SingleDate:
                case PersianDateTimeWPFTools.Windows.Controls.CalendarSelectionMode.SingleRange:
                case PersianDateTimeWPFTools.Windows.Controls.CalendarSelectionMode.MultipleRange:
                    return true;
                default:
                    return calendarSelectionMode == PersianDateTimeWPFTools.Windows.Controls.CalendarSelectionMode.None;
            }
        }

        private void OnSelectedMonthChanged(DateTime? selectedMonth)
        {
            if (!selectedMonth.HasValue)
                return;
            this.DisplayDate = selectedMonth.Value;
            this.UpdateCellItems();
            this.FocusDate(selectedMonth.Value);
        }

        private void OnSelectedYearChanged(DateTime? selectedYear)
        {
            if (!selectedYear.HasValue)
                return;
            this.DisplayDate = selectedYear.Value;
            this.UpdateCellItems();
            this.FocusDate(selectedYear.Value);
        }

        internal void FocusDate(DateTime date)
        {
            if (this.MonthControl == null)
                return;
            this.MonthControl.FocusDate(date);
        }

        private static void OnGotFocus(object sender, RoutedEventArgs e)
        {
            PersianCalendar persianCalendar = (PersianCalendar)sender;
            if (e.Handled || e.OriginalSource != persianCalendar)
                return;
            if (persianCalendar.SelectedDate.HasValue && new PersianDateTimeWPFTools.Windows.Controls.DateTimeHelper(persianCalendar.MonthControl._calendar).CompareYearMonth(persianCalendar.SelectedDate.Value, persianCalendar.DisplayDateInternal) == 0)
                persianCalendar.FocusDate(persianCalendar.SelectedDate.Value);
            else
                persianCalendar.FocusDate(persianCalendar.DisplayDate);
            e.Handled = true;
        }

        private bool ProcessCalendarKey(KeyEventArgs e)
        {
            if (this.DisplayMode == PersianDateTimeWPFTools.Windows.Controls.CalendarMode.Month)
            {
                CalendarDayButton calendarDayButton = this.MonthControl != null ? this.MonthControl.GetCalendarDayButton(this.CurrentDate) : (CalendarDayButton)null;
                if (dateTimeHelper.CompareYearMonth(this.CurrentDate, this.DisplayDateInternal) != 0 && calendarDayButton != null && !calendarDayButton.IsInactive)
                    return false;
            }
            bool ctrl;
            bool shift;
            KeyboardHelper.GetMetaKeyState(out ctrl, out shift);
            switch (e.Key)
            {
                case Key.Return:
                case Key.Space:
                    return this.ProcessEnterKey();
                case Key.Prior:
                    this.ProcessPageUpKey(shift);
                    return true;
                case Key.Next:
                    this.ProcessPageDownKey(shift);
                    return true;
                case Key.End:
                    this.ProcessEndKey(shift);
                    return true;
                case Key.Home:
                    this.ProcessHomeKey(shift);
                    return true;
                case Key.Left:
                    this.ProcessLeftKey(shift);
                    return true;
                case Key.Up:
                    this.ProcessUpKey(ctrl, shift);
                    return true;
                case Key.Right:
                    this.ProcessRightKey(shift);
                    return true;
                case Key.Down:
                    this.ProcessDownKey(ctrl, shift);
                    return true;
                default:
                    return false;
            }
        }

        private void ProcessDownKey(bool ctrl, bool shift)
        {
            switch (this.DisplayMode)
            {
                case PersianDateTimeWPFTools.Windows.Controls.CalendarMode.Month:
                    if (!(!ctrl | shift))
                        break;
                    DateTime? nonBlackoutDate = this._blackoutDates.GetNonBlackoutDate(new PersianDateTimeWPFTools.Windows.Controls.DateTimeHelper(_monthControl._calendar).AddDays(this.CurrentDate, 7), 1);
                    this.ProcessSelection(shift, nonBlackoutDate);
                    break;
                case PersianDateTimeWPFTools.Windows.Controls.CalendarMode.Year:
                    if (ctrl)
                    {
                        this.DisplayMode = PersianDateTimeWPFTools.Windows.Controls.CalendarMode.Month;
                        this.FocusDate(this.DisplayDate);
                        break;
                    }
                    this.OnSelectedMonthChanged(dateTimeHelper.AddMonths(this.DisplayDate, 4));
                    break;
                case PersianDateTimeWPFTools.Windows.Controls.CalendarMode.Decade:
                    if (ctrl)
                    {
                        this.DisplayMode = PersianDateTimeWPFTools.Windows.Controls.CalendarMode.Year;
                        this.FocusDate(this.DisplayDate);
                        break;
                    }
                    this.OnSelectedYearChanged(dateTimeHelper.AddYears(this.DisplayDate, 4));
                    break;
            }
        }

        private void ProcessEndKey(bool shift)
        {
            switch (this.DisplayMode)
            {
                case PersianDateTimeWPFTools.Windows.Controls.CalendarMode.Month:
                    DateTime displayDate = this.DisplayDate;
                    DateTime? lastSelectedDate = new DateTime?(dateTimeHelper.DiscardDayTime(this.DisplayDateInternal));
                    if (dateTimeHelper.CompareYearMonth(DateTime.MaxValue, lastSelectedDate.Value) > 0)
                    {
                        lastSelectedDate = new DateTime?(dateTimeHelper.AddMonths(lastSelectedDate.Value, 1).Value);
                        lastSelectedDate = new DateTime?(new PersianDateTimeWPFTools.Windows.Controls.DateTimeHelper(_monthControl._calendar).AddDays(lastSelectedDate.Value, -1).Value);
                    }
                    else
                        lastSelectedDate = new DateTime?(DateTime.MaxValue);
                    this.ProcessSelection(shift, lastSelectedDate);
                    break;
                case PersianDateTimeWPFTools.Windows.Controls.CalendarMode.Year:
                    this.OnSelectedMonthChanged(new DateTime?(dateTimeHelper.GetLastMonth(this.DisplayDate)));
                    break;
                case PersianDateTimeWPFTools.Windows.Controls.CalendarMode.Decade:
                    this.OnSelectedYearChanged(new DateTime?(dateTimeHelper.EndOfDecade(this.DisplayDate)));
                    break;
            }
        }

        private bool ProcessEnterKey()
        {
            switch (this.DisplayMode)
            {
                case PersianDateTimeWPFTools.Windows.Controls.CalendarMode.Year:
                    this.DisplayMode = PersianDateTimeWPFTools.Windows.Controls.CalendarMode.Month;
                    this.FocusDate(this.DisplayDate);
                    return true;
                case PersianDateTimeWPFTools.Windows.Controls.CalendarMode.Decade:
                    this.DisplayMode = PersianDateTimeWPFTools.Windows.Controls.CalendarMode.Year;
                    this.FocusDate(this.DisplayDate);
                    return true;
                default:
                    return false;
            }
        }

        private void ProcessHomeKey(bool shift)
        {
            switch (this.DisplayMode)
            {
                case PersianDateTimeWPFTools.Windows.Controls.CalendarMode.Month:
                    DateTime? lastSelectedDate = new DateTime?(dateTimeHelper.DiscardDayTime(this.DisplayDateInternal));
                    this.ProcessSelection(shift, lastSelectedDate);
                    break;
                case PersianDateTimeWPFTools.Windows.Controls.CalendarMode.Year:
                    this.OnSelectedMonthChanged(new DateTime?(dateTimeHelper.DiscardMonthDayTime(this.DisplayDate)));
                    break;
                case PersianDateTimeWPFTools.Windows.Controls.CalendarMode.Decade:
                    this.OnSelectedYearChanged(new DateTime?(new PersianDateTimeWPFTools.Windows.Controls.DateTimeHelper(this.MonthControl._calendar).DecadeOfDate(this.DisplayDate)));
                    break;
            }
        }

        private void ProcessLeftKey(bool shift)
        {
            int num = !this.IsRightToLeft ? -1 : 1;
            switch (this.DisplayMode)
            {
                case PersianDateTimeWPFTools.Windows.Controls.CalendarMode.Month:
                    DateTime? nonBlackoutDate = this._blackoutDates.GetNonBlackoutDate(new PersianDateTimeWPFTools.Windows.Controls.DateTimeHelper(_monthControl._calendar).AddDays(this.CurrentDate, num), num);
                    this.ProcessSelection(shift, nonBlackoutDate);
                    break;
                case PersianDateTimeWPFTools.Windows.Controls.CalendarMode.Year:
                    this.OnSelectedMonthChanged(dateTimeHelper.AddMonths(this.DisplayDate, num));
                    break;
                case PersianDateTimeWPFTools.Windows.Controls.CalendarMode.Decade:
                    this.OnSelectedYearChanged(dateTimeHelper.AddYears(this.DisplayDate, num));
                    break;
            }
        }

        private void ProcessPageDownKey(bool shift)
        {
            switch (this.DisplayMode)
            {
                case PersianDateTimeWPFTools.Windows.Controls.CalendarMode.Month:
                    DateTime? nonBlackoutDate = this._blackoutDates.GetNonBlackoutDate(dateTimeHelper.AddMonths(this.CurrentDate, 1), 1);
                    this.ProcessSelection(shift, nonBlackoutDate);
                    break;
                case PersianDateTimeWPFTools.Windows.Controls.CalendarMode.Year:
                    this.OnSelectedMonthChanged(dateTimeHelper.AddYears(this.DisplayDate, 1));
                    break;
                case PersianDateTimeWPFTools.Windows.Controls.CalendarMode.Decade:
                    this.OnSelectedYearChanged(dateTimeHelper.AddYears(this.DisplayDate, 10));
                    break;
            }
        }

        private void ProcessPageUpKey(bool shift)
        {
            switch (this.DisplayMode)
            {
                case PersianDateTimeWPFTools.Windows.Controls.CalendarMode.Month:
                    DateTime? nonBlackoutDate = this._blackoutDates.GetNonBlackoutDate(dateTimeHelper.AddMonths(this.CurrentDate, -1), -1);
                    this.ProcessSelection(shift, nonBlackoutDate);
                    break;
                case PersianDateTimeWPFTools.Windows.Controls.CalendarMode.Year:
                    this.OnSelectedMonthChanged(dateTimeHelper.AddYears(this.DisplayDate, -1));
                    break;
                case PersianDateTimeWPFTools.Windows.Controls.CalendarMode.Decade:
                    this.OnSelectedYearChanged(dateTimeHelper.AddYears(this.DisplayDate, -10));
                    break;
            }
        }

        private void ProcessRightKey(bool shift)
        {
            int num = !this.IsRightToLeft ? 1 : -1;
            switch (this.DisplayMode)
            {
                case PersianDateTimeWPFTools.Windows.Controls.CalendarMode.Month:
                    DateTime? nonBlackoutDate = this._blackoutDates.GetNonBlackoutDate(new PersianDateTimeWPFTools.Windows.Controls.DateTimeHelper(_monthControl._calendar).AddDays(this.CurrentDate, num), num);
                    this.ProcessSelection(shift, nonBlackoutDate);
                    break;
                case PersianDateTimeWPFTools.Windows.Controls.CalendarMode.Year:
                    this.OnSelectedMonthChanged(dateTimeHelper.AddMonths(this.DisplayDate, num));
                    break;
                case PersianDateTimeWPFTools.Windows.Controls.CalendarMode.Decade:
                    this.OnSelectedYearChanged(dateTimeHelper.AddYears(this.DisplayDate, num));
                    break;
            }
        }

        private void ProcessSelection(bool shift, DateTime? lastSelectedDate)
        {
            if (this.SelectionMode == PersianDateTimeWPFTools.Windows.Controls.CalendarSelectionMode.None && lastSelectedDate.HasValue)
            {
                this.OnDayClick(lastSelectedDate.Value);
            }
            else
            {
                if (!lastSelectedDate.HasValue || !PersianCalendar.IsValidKeyboardSelection(this, (object)lastSelectedDate.Value))
                    return;
                if (this.SelectionMode == PersianDateTimeWPFTools.Windows.Controls.CalendarSelectionMode.SingleRange || this.SelectionMode == PersianDateTimeWPFTools.Windows.Controls.CalendarSelectionMode.MultipleRange)
                {
                    this.SelectedDates.ClearInternal();
                    if (shift)
                    {
                        this._isShiftPressed = true;
                        DateTime? nullable = this.HoverStart;
                        if (!nullable.HasValue)
                        {
                            nullable = new DateTime?(this.CurrentDate);
                            this.HoverEnd = nullable;
                            this.HoverStart = nullable;
                        }
                        nullable = this.HoverStart;
                        PersianDateTimeWPFTools.Windows.Controls.CalendarDateRange range;
                        if (DateTime.Compare(nullable.Value, lastSelectedDate.Value) < 0)
                        {
                            nullable = this.HoverStart;
                            range = new PersianDateTimeWPFTools.Windows.Controls.CalendarDateRange(nullable.Value, lastSelectedDate.Value);
                        }
                        else
                        {
                            DateTime start = lastSelectedDate.Value;
                            nullable = this.HoverStart;
                            DateTime end = nullable.Value;
                            range = new PersianDateTimeWPFTools.Windows.Controls.CalendarDateRange(start, end);
                        }
                        if (!this.BlackoutDates.ContainsAny(range))
                        {
                            this._currentDate = lastSelectedDate;
                            this.HoverEnd = lastSelectedDate;
                        }
                        this.OnDayClick(this.CurrentDate);
                    }
                    else
                    {
                        DateTime? nullable = new DateTime?(this.CurrentDate = lastSelectedDate.Value);
                        this.HoverEnd = nullable;
                        this.HoverStart = nullable;
                        this.AddKeyboardSelection();
                        this.OnDayClick(lastSelectedDate.Value);
                    }
                }
                else
                {
                    this.CurrentDate = lastSelectedDate.Value;
                    DateTime? nullable = new DateTime?();
                    this.HoverEnd = nullable;
                    this.HoverStart = nullable;
                    if (this.SelectedDates.Count > 0)
                        this.SelectedDates[0] = lastSelectedDate.Value;
                    else
                        this.SelectedDates.Add(lastSelectedDate.Value);
                    this.OnDayClick(lastSelectedDate.Value);
                }
                this.UpdateCellItems();
            }
        }

        private void ProcessShiftKeyUp()
        {
            if (!this._isShiftPressed || this.SelectionMode != PersianDateTimeWPFTools.Windows.Controls.CalendarSelectionMode.SingleRange && this.SelectionMode != PersianDateTimeWPFTools.Windows.Controls.CalendarSelectionMode.MultipleRange)
                return;
            this.AddKeyboardSelection();
            this._isShiftPressed = false;
            DateTime? nullable = new DateTime?();
            this.HoverEnd = nullable;
            this.HoverStart = nullable;
        }

        private void ProcessUpKey(bool ctrl, bool shift)
        {
            switch (this.DisplayMode)
            {
                case PersianDateTimeWPFTools.Windows.Controls.CalendarMode.Month:
                    if (ctrl)
                    {
                        this.DisplayMode = PersianDateTimeWPFTools.Windows.Controls.CalendarMode.Year;
                        this.FocusDate(this.DisplayDate);
                        break;
                    }
                    DateTime? nonBlackoutDate = this._blackoutDates.GetNonBlackoutDate(new PersianDateTimeWPFTools.Windows.Controls.DateTimeHelper(_monthControl._calendar).AddDays(this.CurrentDate, -7), -1);
                    this.ProcessSelection(shift, nonBlackoutDate);
                    break;
                case PersianDateTimeWPFTools.Windows.Controls.CalendarMode.Year:
                    if (ctrl)
                    {
                        this.DisplayMode = PersianDateTimeWPFTools.Windows.Controls.CalendarMode.Decade;
                        this.FocusDate(this.DisplayDate);
                        break;
                    }
                    this.OnSelectedMonthChanged(dateTimeHelper.AddMonths(this.DisplayDate, -4));
                    break;
                case PersianDateTimeWPFTools.Windows.Controls.CalendarMode.Decade:
                    if (ctrl)
                        break;
                    this.OnSelectedYearChanged(dateTimeHelper.AddYears(this.DisplayDate, -4));
                    break;
            }
        }
    }

}
