using PersianDateTimeWPFTools.Controls;
using PersianDateTimeWPFTools.Tools;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using PersianCalendar = PersianDateTimeWPFTools.Controls.PersianCalendar;
#if NET8_0_OR_GREATER
#nullable disable
#endif
namespace PersianDateTimeWPFTools.Windows.Controls.Primitives
{

    [TemplatePart(Name = "PART_Root", Type = typeof(FrameworkElement))]
    [TemplatePart(Name = "PART_HeaderButton", Type = typeof(Button))]
    [TemplatePart(Name = "PART_PreviousButton", Type = typeof(Button))]
    [TemplatePart(Name = "PART_GoToTodayButton", Type = typeof(Button))]
    [TemplatePart(Name = "PART_NextButton", Type = typeof(Button))]
    [TemplatePart(Name = "DayTitleTemplate", Type = typeof(DataTemplate))]
    [TemplatePart(Name = "PART_MonthView", Type = typeof(Grid))]
    [TemplatePart(Name = "PART_YearView", Type = typeof(Grid))]
    [TemplatePart(Name = "PART_DisabledVisual", Type = typeof(FrameworkElement))]
    [TemplatePart(Name = "PART_ButtonConfirm", Type = typeof(Button))]
    [TemplateVisualState(Name = "Normal", GroupName = "CommonStates")]
    [TemplateVisualState(Name = "Disabled", GroupName = "CommonStates")]
    public sealed class CalendarItem : Control
    {
        private const string ElementRoot = "PART_Root";
        private const string ElementHeaderButton = "PART_HeaderButton";
        private const string ElementPreviousButton = "PART_PreviousButton";
        private const string ElementGoToTodayButton = "PART_GoToTodayButton";
        private const string ElementNextButton = "PART_NextButton";
        private const string ElementDayTitleTemplate = "DayTitleTemplate";
        private const string ElementMonthView = "PART_MonthView";
        private const string ElementYearView = "PART_YearView";
        private const string ElementDisabledVisual = "PART_DisabledVisual";
        private const int COLS = 7;
        private const int ROWS = 7;
        private const int YEAR_COLS = 4;
        private const int YEAR_ROWS = 3;
        private const int NUMBER_OF_DAYS_IN_WEEK = 7;
        internal System.Globalization.Calendar _calendar; //= PersianCalendarHelper.GetCurrentCalendar();
        private DataTemplate _dayTitleTemplate;
        private FrameworkElement _disabledVisual;
        private Button _headerButton;
        private Grid _monthView;
        private Button _nextButton;
        private Button _confirmButton;
        private Button _previousButton;
        private Button _goToTodayButton;
        private Grid _yearView;
        private bool _isMonthPressed;
        private bool _isDayPressed;
        private bool _ShowTodayButton { get; set; }
        public bool ShowTodayButton
        {
            get => _ShowTodayButton;
            set
            {
                _ShowTodayButton = value;
                if (_goToTodayButton != null)
                    _goToTodayButton.Visibility = value ? Visibility.Visible : Visibility.Hidden;
            }
        }




        public bool ShowConfirmButton
        {
            get { return (bool)GetValue(ShowConfirmButtonProperty); }
            set { SetValue(ShowConfirmButtonProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShowConfirmButton.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowConfirmButtonProperty =
            DependencyProperty.Register("ShowConfirmButton", typeof(bool), typeof(CalendarItem), new PropertyMetadata(false));




        private CultureInfo _CustomCulture { get; set; }
        public CultureInfo CustomCulture
        {
            get => _CustomCulture;
            set
            {
                _CustomCulture = value;

                if (value != null)
                    _calendar = value.Calendar;
                else
                    _calendar = DateTimeHelper.GetCulture(this).Calendar;

                dateTimeHelper = new DateTimeHelper(_calendar);
            }
        }

        private DateTimeHelper dateTimeHelper;

        static CalendarItem()
        {
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(CalendarItem), (PropertyMetadata)new FrameworkPropertyMetadata((object)typeof(CalendarItem)));
            UIElement.FocusableProperty.OverrideMetadata(typeof(CalendarItem), (PropertyMetadata)new FrameworkPropertyMetadata((object)false));
            KeyboardNavigation.TabNavigationProperty.OverrideMetadata(typeof(CalendarItem), (PropertyMetadata)new FrameworkPropertyMetadata((object)KeyboardNavigationMode.Once));
            KeyboardNavigation.DirectionalNavigationProperty.OverrideMetadata(typeof(CalendarItem), (PropertyMetadata)new FrameworkPropertyMetadata((object)KeyboardNavigationMode.Contained));
        }

        public CalendarItem()
        {
            //_calendar = DateTimeHelper.GetCultureInfo((FrameworkElement)this).Calendar;
            this._calendar = DateTimeHelper.GetCulture((FrameworkElement)this).Calendar;
            dateTimeHelper = new DateTimeHelper(_calendar);
        }

        private void InitCalendar()
        {
            if (Owner != null)
            {
                if (Owner.CustomCulture != null)
                {
                    CustomCulture = Owner.CustomCulture;
                }
            }
        }
        internal Grid MonthView => this._monthView;

        internal PersianCalendar Owner { get; set; }

        internal Grid YearView => this._yearView;

        private PersianDateTimeWPFTools.Windows.Controls.CalendarMode DisplayMode
        {
            get => this.Owner == null ? PersianDateTimeWPFTools.Windows.Controls.CalendarMode.Month : this.Owner.DisplayMode;
        }

        private Button HeaderButton => this._headerButton;

        private Button NextButton => this._nextButton;

        private Button PreviousButton => this._previousButton;

        private DateTime DisplayDate => this.Owner == null ? DateTime.Today : this.Owner.DisplayDate;

        public override void OnApplyTemplate()
        {
            InitCalendar();
            base.OnApplyTemplate();
            if (this._previousButton != null)
                this._previousButton.Click -= new RoutedEventHandler(this.PreviousButton_Click);
            if (this._nextButton != null)
                this._nextButton.Click -= new RoutedEventHandler(this.NextButton_Click);
            if (this._goToTodayButton != null)
                this._goToTodayButton.Click -= new RoutedEventHandler(this.GoToTodayButton_Click);
            if (this._headerButton != null)
                this._headerButton.Click -= new RoutedEventHandler(this.HeaderButton_Click);
            if (this._confirmButton != null)
                this._confirmButton.Click -= new RoutedEventHandler(this.ConfirmButton_Click);


            this._monthView = this.GetTemplateChild("PART_MonthView") as Grid;
            this._yearView = this.GetTemplateChild("PART_YearView") as Grid;
            this._previousButton = this.GetTemplateChild("PART_PreviousButton") as Button;
            this._goToTodayButton = this.GetTemplateChild("PART_GoToTodayButton") as Button;
            this._nextButton = this.GetTemplateChild("PART_NextButton") as Button;
            this._confirmButton = this.GetTemplateChild("PART_ButtonConfirm") as Button;
            this._headerButton = this.GetTemplateChild("PART_HeaderButton") as Button;
            this._disabledVisual = this.GetTemplateChild("PART_DisabledVisual") as FrameworkElement;
            this._dayTitleTemplate = (DataTemplate)null;
            if (this.Template != null && this.Template.Resources.Contains((object)"DayTitleTemplate"))
                this._dayTitleTemplate = this.Template.Resources[(object)"DayTitleTemplate"] as DataTemplate;
            if (this._previousButton != null)
            {
                if (this._previousButton.Content == null)
                    this._previousButton.Content = (object)"Previous button";
                this._previousButton.Click += new RoutedEventHandler(this.PreviousButton_Click);
            }

            if (this._goToTodayButton != null)
            {
                if (this._goToTodayButton.Content == null)
                    this._goToTodayButton.Content = (object)"Got to today button";
                this._goToTodayButton.Click += new RoutedEventHandler(this.GoToTodayButton_Click);
            }

            if (this._confirmButton != null)
            {
                if (this._confirmButton.Content == null)
                    this._confirmButton.Content = "Confirm";
                this._confirmButton.Click += new RoutedEventHandler(this.ConfirmButton_Click);

                Binding binding = new Binding()
                {
                    Source = this,
                    Converter = new Bool2VisibilityConverter(),
                    Path = new PropertyPath(ShowConfirmButtonProperty.Name),
                    Mode = BindingMode.OneWay,
                };

                //_confirmButton.SetBinding(Button.VisibilityProperty, new Binding(ShowConfirmButtonProperty.Name) { Source = this, Converter = new BooleanToVisibilityConverter() });
                _confirmButton.SetBinding(Button.VisibilityProperty, binding);

            }

            if (this._nextButton != null)
            {
                if (this._nextButton.Content == null)
                    this._nextButton.Content = (object)"Next button";
                this._nextButton.Click += new RoutedEventHandler(this.NextButton_Click);
            }
            if (this._headerButton != null)
                this._headerButton.Click += new RoutedEventHandler(this.HeaderButton_Click);

            if (_goToTodayButton != null)
                _goToTodayButton.Visibility = ShowTodayButton ? Visibility.Visible : Visibility.Hidden;
            this.PopulateGrids();
            if (this.Owner != null)
            {
                switch (this.Owner.DisplayMode)
                {
                    case PersianDateTimeWPFTools.Windows.Controls.CalendarMode.Month:
                        this.UpdateMonthMode();
                        break;
                    case PersianDateTimeWPFTools.Windows.Controls.CalendarMode.Year:
                        this.UpdateYearMode();
                        break;
                    case PersianDateTimeWPFTools.Windows.Controls.CalendarMode.Decade:
                        this.UpdateDecadeMode();
                        break;
                }
            }
            else
                this.UpdateMonthMode();
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            if (Owner != null)
            {
                Owner.RaiseConfirmButtonClick();
            }
        }

        private void GoToTodayButton_Click(object sender, RoutedEventArgs e)
        {
            Button btnToday = null;
            if (this._goToTodayButton != null)
            {
                btnToday = _goToTodayButton;
            }
            else
            {
                btnToday = sender as Button;
            }

            if (btnToday == null || Owner == null)
                return;

            //this.Owner.DisplayMode = this.Owner.DisplayMode != CalendarMode.Month ? CalendarMode.Decade : CalendarMode.Year;
            if (Owner.DisplayMode != CalendarMode.Month)
                Owner.DisplayMode = CalendarMode.Month;

            this.Owner.MoveDisplayTo(new DateTime?(dateTimeHelper.DiscardDayTime(DateTime.Now)));

        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);
            if (this.IsMouseCaptured)
                this.ReleaseMouseCapture();
            this._isMonthPressed = false;
            this._isDayPressed = false;
            if (e.Handled || this.Owner.DisplayMode != PersianDateTimeWPFTools.Windows.Controls.CalendarMode.Month || !this.Owner.HoverEnd.HasValue)
                return;
            this.FinishSelection(this.Owner.HoverEnd.Value);
        }

        protected override void OnLostMouseCapture(MouseEventArgs e)
        {
            base.OnLostMouseCapture(e);
            if (this.IsMouseCaptured)
                return;
            this._isDayPressed = false;
            this._isMonthPressed = false;
        }

        internal void UpdateDecadeMode()
        {
            DateTime decadeForDecadeMode = this.GetDecadeForDecadeMode(this.Owner == null ? DateTime.Today : this.Owner.DisplayYear);
            DateTime decadeEnd = this._calendar.AddYears(decadeForDecadeMode, 9);
            this.SetDecadeModeHeaderButton(decadeForDecadeMode);
            this.SetDecadeModePreviousButton(decadeForDecadeMode);
            this.SetDecadeModeNextButton(decadeEnd);
            if (this._yearView == null)
                return;
            this.SetYearButtons(decadeForDecadeMode, decadeEnd);
        }

        internal void UpdateMonthMode()
        {
            /*if (_goToTodayButton != null)
                _goToTodayButton.Visibility = Visibility.Visible;*/
            this.SetMonthModeHeaderButton();
            this.SetMonthModePreviousButton();
            this.SetMonthModeNextButton();
            if (this._monthView == null)
                return;
            this.SetMonthModeDayTitles();
            this.SetMonthModeCalendarDayButtons();
            this.AddMonthModeHighlight();
        }

        internal void UpdateYearMode()
        {
            /*if (_goToTodayButton != null)
                _goToTodayButton.Visibility = Visibility.Collapsed;*/
            this.SetYearModeHeaderButton();
            this.SetYearModePreviousButton();
            this.SetYearModeNextButton();
            if (this._yearView == null)
                return;
            this.SetYearModeMonthButtons();
        }

        internal IEnumerable<CalendarDayButton> GetCalendarDayButtons()
        {
            int count = 49;
            if (this.MonthView != null)
            {
                UIElementCollection dayButtonsHost = this.MonthView.Children;
                for (int childIndex = 7; childIndex < count; ++childIndex)
                {
                    if (dayButtonsHost[childIndex] is CalendarDayButton calendarDayButton)
                        yield return calendarDayButton;
                }
                dayButtonsHost = (UIElementCollection)null;
            }
        }

        internal CalendarDayButton GetFocusedCalendarDayButton()
        {
            foreach (CalendarDayButton calendarDayButton in this.GetCalendarDayButtons())
            {
                if (calendarDayButton != null && calendarDayButton.IsFocused)
                    return calendarDayButton;
            }
            return (CalendarDayButton)null;
        }

        internal CalendarDayButton GetCalendarDayButton(DateTime date)
        {
            foreach (CalendarDayButton calendarDayButton in this.GetCalendarDayButtons())
            {
                if (calendarDayButton != null && calendarDayButton.DataContext is DateTime && PersianDateTimeWPFTools.Windows.Controls.DateTimeHelper.CompareDays(date, (DateTime)calendarDayButton.DataContext) == 0)
                    return calendarDayButton;
            }
            return (CalendarDayButton)null;
        }

        internal CalendarButton GetCalendarButton(DateTime date, PersianDateTimeWPFTools.Windows.Controls.CalendarMode mode)
        {
            foreach (CalendarButton calendarButton in this.GetCalendarButtons())
            {
                if (calendarButton != null && calendarButton.DataContext is DateTime)
                {
                    if (mode == PersianDateTimeWPFTools.Windows.Controls.CalendarMode.Year)
                    {
                        if (dateTimeHelper.CompareYearMonth(date, (DateTime)calendarButton.DataContext) == 0)
                            return calendarButton;
                    }
                    else if (this._calendar.GetYear(date) == this._calendar.GetYear((DateTime)calendarButton.DataContext))
                        return calendarButton;
                }
            }
            return (CalendarButton)null;
        }

        internal CalendarButton GetFocusedCalendarButton()
        {
            foreach (CalendarButton calendarButton in this.GetCalendarButtons())
            {
                if (calendarButton != null && calendarButton.IsFocused)
                    return calendarButton;
            }
            return (CalendarButton)null;
        }

        private IEnumerable<CalendarButton> GetCalendarButtons()
        {
            //If DisplayMode is anything other than Month, a null error is thrown for this.YearView.Children.
            //Unfortunately, there is no value displayed for calendar buttons in design mode.
            if (this.YearView?.Children == null)
                yield break;

            foreach (UIElement child in this.YearView.Children)
            {
                if (child is CalendarButton calendarButton)
                    yield return calendarButton;
            }
        }

        internal void FocusDate(DateTime date)
        {
            FrameworkElement frameworkElement = (FrameworkElement)null;
            switch (this.DisplayMode)
            {
                case PersianDateTimeWPFTools.Windows.Controls.CalendarMode.Month:
                    frameworkElement = (FrameworkElement)this.GetCalendarDayButton(date);
                    break;
                case PersianDateTimeWPFTools.Windows.Controls.CalendarMode.Year:
                case PersianDateTimeWPFTools.Windows.Controls.CalendarMode.Decade:
                    frameworkElement = (FrameworkElement)this.GetCalendarButton(date, this.DisplayMode);
                    break;
            }
            if (frameworkElement == null || frameworkElement.IsFocused)
                return;
            frameworkElement.MoveFocus(new TraversalRequest(FocusNavigationDirection.First));
        }

        private DateTime GetDecadeForDecadeMode(DateTime selectedYear)
        {
            DateTime time = dateTimeHelper.DecadeOfDate(selectedYear);
            if (this._isMonthPressed && this._yearView != null)
            {
                UIElementCollection children = this._yearView.Children;
                int count = children.Count;
                if (count > 0 && children[0] is CalendarButton calendarButton1 && calendarButton1.DataContext is DateTime && this._calendar.GetYear((DateTime)calendarButton1.DataContext) == this._calendar.GetYear(selectedYear))
                    return this._calendar.AddYears(time, 10);
                if (count > 1 && children[count - 1] is CalendarButton calendarButton2 && calendarButton2.DataContext is DateTime && ((DateTime)calendarButton2.DataContext).Year == selectedYear.Year)
                    return this._calendar.AddYears(time, -10);
            }
            return time;
        }

        private void EndDrag(bool ctrl, DateTime selectedDate)
        {
            if (this.Owner == null)
                return;
            this.Owner.CurrentDate = selectedDate;
            if (!this.Owner.HoverStart.HasValue)
                return;
            if (ctrl && DateTime.Compare(this.Owner.HoverStart.Value, selectedDate) == 0 && (this.Owner.SelectionMode == PersianDateTimeWPFTools.Windows.Controls.CalendarSelectionMode.SingleDate || this.Owner.SelectionMode == PersianDateTimeWPFTools.Windows.Controls.CalendarSelectionMode.MultipleRange))
                this.Owner.SelectedDates.Toggle(selectedDate);
            else
                this.Owner.SelectedDates.AddRangeInternal(this.Owner.HoverStart.Value, selectedDate);
            this.Owner.OnDayClick(selectedDate);
        }

        private void CellOrMonth_PreviewKeyDown(object sender, RoutedEventArgs e)
        {
            if (this.Owner == null)
                return;
            this.Owner.OnDayOrMonthPreviewKeyDown(e);
        }

        private void Cell_Clicked(object sender, RoutedEventArgs e)
        {
            if (this.Owner == null)
                return;

            CalendarDayButton calendarDayButton = sender as CalendarDayButton;
            if (!(calendarDayButton.DataContext is DateTime) || (calendarDayButton.IsBlackedOut && !Owner.AllowSelectBlackedOutDay))
                return;
            DateTime dataContext = (DateTime)calendarDayButton.DataContext;
            bool ctrl;
            bool shift;
            KeyboardHelper.GetMetaKeyState(out ctrl, out shift);
            switch (this.Owner.SelectionMode)
            {
                case PersianDateTimeWPFTools.Windows.Controls.CalendarSelectionMode.SingleDate:
                    if (!ctrl)
                    {
                        this.Owner.SelectedDate = new DateTime?(dataContext);
                        break;
                    }
                    this.Owner.SelectedDates.Toggle(dataContext);
                    break;
                case PersianDateTimeWPFTools.Windows.Controls.CalendarSelectionMode.SingleRange:
                    DateTime? nullable = new DateTime?(this.Owner.CurrentDate);
                    this.Owner.SelectedDates.ClearInternal(true);
                    if (shift && nullable.HasValue)
                    {
                        this.Owner.SelectedDates.AddRangeInternal(nullable.Value, dataContext);
                        break;
                    }
                    this.Owner.SelectedDate = new DateTime?(dataContext);
                    this.Owner.HoverStart = new DateTime?();
                    this.Owner.HoverEnd = new DateTime?();
                    break;
                case PersianDateTimeWPFTools.Windows.Controls.CalendarSelectionMode.MultipleRange:
                    if (!ctrl)
                        this.Owner.SelectedDates.ClearInternal(true);
                    if (shift)
                    {
                        this.Owner.SelectedDates.AddRangeInternal(this.Owner.CurrentDate, dataContext);
                        break;
                    }
                    if (!ctrl)
                    {
                        this.Owner.SelectedDate = new DateTime?(dataContext);
                        break;
                    }
                    this.Owner.SelectedDates.Toggle(dataContext);
                    this.Owner.HoverStart = new DateTime?();
                    this.Owner.HoverEnd = new DateTime?();
                    break;
            }
            this.Owner.OnDayClick(dataContext);
        }

        private void Cell_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!(sender is CalendarDayButton calendarDayButton) || this.Owner == null || !(calendarDayButton.DataContext is DateTime))
                return;
            if (calendarDayButton.IsBlackedOut && !Owner.AllowSelectBlackedOutDay)
            {
                this.Owner.HoverStart = new DateTime?();
            }
            else
            {
                this._isDayPressed = true;
                Mouse.Capture((IInputElement)this, CaptureMode.SubTree);
                calendarDayButton.MoveFocus(new TraversalRequest(FocusNavigationDirection.First));
                bool ctrl;
                bool shift;
                KeyboardHelper.GetMetaKeyState(out ctrl, out shift);
                DateTime dataContext = (DateTime)calendarDayButton.DataContext;
                switch (this.Owner.SelectionMode)
                {
                    case PersianDateTimeWPFTools.Windows.Controls.CalendarSelectionMode.SingleDate:
                        this.Owner.DatePickerDisplayDateFlag = true;
                        if (!ctrl)
                        {
                            this.Owner.SelectedDate = new DateTime?(dataContext);
                            break;
                        }
                        this.Owner.SelectedDates.Toggle(dataContext);
                        break;
                    case PersianDateTimeWPFTools.Windows.Controls.CalendarSelectionMode.SingleRange:
                        this.Owner.SelectedDates.ClearInternal();
                        if (shift)
                        {
                            DateTime? nullable1 = this.Owner.HoverStart;
                            if (!nullable1.HasValue)
                            {
                                PersianCalendar owner1 = this.Owner;
                                PersianCalendar owner2 = this.Owner;
                                nullable1 = new DateTime?(this.Owner.CurrentDate);
                                DateTime? nullable2 = nullable1;
                                owner2.HoverEnd = nullable2;
                                DateTime? nullable3 = nullable1;
                                owner1.HoverStart = nullable3;
                                break;
                            }
                            break;
                        }
                        PersianCalendar owner3 = this.Owner;
                        PersianCalendar owner4 = this.Owner;
                        DateTime? nullable4 = new DateTime?(dataContext);
                        DateTime? nullable5 = nullable4;
                        owner4.HoverEnd = nullable5;
                        DateTime? nullable6 = nullable4;
                        owner3.HoverStart = nullable6;
                        break;
                    case PersianDateTimeWPFTools.Windows.Controls.CalendarSelectionMode.MultipleRange:
                        if (!ctrl)
                            this.Owner.SelectedDates.ClearInternal();
                        if (shift)
                        {
                            DateTime? nullable7 = this.Owner.HoverStart;
                            if (!nullable7.HasValue)
                            {
                                PersianCalendar owner5 = this.Owner;
                                PersianCalendar owner6 = this.Owner;
                                nullable7 = new DateTime?(this.Owner.CurrentDate);
                                DateTime? nullable8 = nullable7;
                                owner6.HoverEnd = nullable8;
                                DateTime? nullable9 = nullable7;
                                owner5.HoverStart = nullable9;
                                break;
                            }
                            break;
                        }
                        PersianCalendar owner7 = this.Owner;
                        PersianCalendar owner8 = this.Owner;
                        DateTime? nullable10 = new DateTime?(dataContext);
                        DateTime? nullable11 = nullable10;
                        owner8.HoverEnd = nullable11;
                        DateTime? nullable12 = nullable10;
                        owner7.HoverStart = nullable12;
                        break;
                }
                this.Owner.CurrentDate = dataContext;
                this.Owner.UpdateCellItems();
            }
        }

        private void Cell_MouseEnter(object sender, MouseEventArgs e)
        {
            if (!(sender is CalendarDayButton calendarDayButton) || (calendarDayButton.IsBlackedOut && !Owner.AllowSelectBlackedOutDay) || e.LeftButton != MouseButtonState.Pressed || !this._isDayPressed)
                return;
            calendarDayButton.MoveFocus(new TraversalRequest(FocusNavigationDirection.First));
            if (this.Owner == null || !(calendarDayButton.DataContext is DateTime))
                return;
            DateTime dataContext = (DateTime)calendarDayButton.DataContext;
            if (this.Owner.SelectionMode == PersianDateTimeWPFTools.Windows.Controls.CalendarSelectionMode.SingleDate)
            {
                this.Owner.DatePickerDisplayDateFlag = true;
                PersianCalendar owner1 = this.Owner;
                PersianCalendar owner2 = this.Owner;
                DateTime? nullable1 = new DateTime?();
                DateTime? nullable2 = nullable1;
                owner2.HoverEnd = nullable2;
                DateTime? nullable3 = nullable1;
                owner1.HoverStart = nullable3;
                if (this.Owner.SelectedDates.Count == 0)
                    this.Owner.SelectedDates.Add(dataContext);
                else
                    this.Owner.SelectedDates[0] = dataContext;
            }
            else
            {
                this.Owner.HoverEnd = new DateTime?(dataContext);
                this.Owner.CurrentDate = dataContext;
                this.Owner.UpdateCellItems();
            }
        }

        private void Cell_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!(sender is CalendarDayButton calendarDayButton) || this.Owner == null)
                return;
            if (!calendarDayButton.IsBlackedOut)
                this.Owner.OnDayButtonMouseUp(e);
            else if (this.Owner != null && this.Owner.AllowSelectBlackedOutDay) // Close PersianDatePicker Popup
                this.Owner.OnDayButtonMouseUp(e);

            if (!(calendarDayButton.DataContext is DateTime))
                return;
            this.FinishSelection((DateTime)calendarDayButton.DataContext);
            e.Handled = true;
        }

        private void FinishSelection(DateTime selectedDate)
        {
            bool ctrl;
            KeyboardHelper.GetMetaKeyState(out ctrl, out bool _);
            if (this.Owner.SelectionMode == PersianDateTimeWPFTools.Windows.Controls.CalendarSelectionMode.None || this.Owner.SelectionMode == PersianDateTimeWPFTools.Windows.Controls.CalendarSelectionMode.SingleDate)
                this.Owner.OnDayClick(selectedDate);
            else if (this.Owner.HoverStart.HasValue)
            {
                switch (this.Owner.SelectionMode)
                {
                    case PersianDateTimeWPFTools.Windows.Controls.CalendarSelectionMode.SingleRange:
                        this.Owner.SelectedDates.ClearInternal();
                        this.EndDrag(ctrl, selectedDate);
                        break;
                    case PersianDateTimeWPFTools.Windows.Controls.CalendarSelectionMode.MultipleRange:
                        this.EndDrag(ctrl, selectedDate);
                        break;
                }
            }
            else
            {
                CalendarDayButton calendarDayButton = this.GetCalendarDayButton(selectedDate);
                if (calendarDayButton == null || !calendarDayButton.IsInactive || !calendarDayButton.IsBlackedOut)
                    return;
                this.Owner.OnDayClick(selectedDate);
            }
        }

        private void Month_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!(sender is CalendarButton b))
                return;
            this._isMonthPressed = true;
            Mouse.Capture((IInputElement)this, CaptureMode.SubTree);
            if (this.Owner == null)
                return;
            this.Owner.OnCalendarButtonPressed(b, false);
        }

        private void Month_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!(sender is CalendarButton b) || this.Owner == null)
                return;
            this.Owner.OnCalendarButtonPressed(b, true);
        }

        private void Month_MouseEnter(object sender, MouseEventArgs e)
        {
            if (!(sender is CalendarButton b) || !this._isMonthPressed || this.Owner == null)
                return;
            this.Owner.OnCalendarButtonPressed(b, false);
        }

        private void Month_Clicked(object sender, RoutedEventArgs e)
        {
            if (!(sender is CalendarButton b))
                return;
            this.Owner.OnCalendarButtonPressed(b, true);
        }

        private void HeaderButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.Owner == null)
                return;
            this.Owner.DisplayMode = this.Owner.DisplayMode != PersianDateTimeWPFTools.Windows.Controls.CalendarMode.Month ? PersianDateTimeWPFTools.Windows.Controls.CalendarMode.Decade : PersianDateTimeWPFTools.Windows.Controls.CalendarMode.Year;
            this.FocusDate(this.DisplayDate);
        }

        private void PreviousButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.Owner == null)
                return;
            this.Owner.OnPreviousClick();
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.Owner == null)
                return;
            this.Owner.OnNextClick();
        }

        private void PopulateGrids()
        {
            if (this._monthView != null)
            {
                if (this._dayTitleTemplate != null)
                {
                    for (int index = 0; index < 7; ++index)
                    {
                        FrameworkElement element = (FrameworkElement)this._dayTitleTemplate.LoadContent();
                        element.SetValue(Grid.RowProperty, (object)0);
                        element.SetValue(Grid.ColumnProperty, (object)index);
                        this._monthView.Children.Add((UIElement)element);
                    }
                }
                for (int index1 = 1; index1 < 7; ++index1)
                {
                    for (int index2 = 0; index2 < 7; ++index2)
                    {
                        CalendarDayButton element = new CalendarDayButton();
                        element.Owner = this.Owner;
                        element.SetValue(Grid.RowProperty, (object)index1);
                        element.SetValue(Grid.ColumnProperty, (object)index2);
                        element.SetBinding(FrameworkElement.StyleProperty, this.GetOwnerBinding("CalendarDayButtonStyle"));
                        element.AddHandler(UIElement.MouseLeftButtonDownEvent, (Delegate)new MouseButtonEventHandler(this.Cell_MouseLeftButtonDown), true);
                        element.AddHandler(UIElement.MouseLeftButtonUpEvent, (Delegate)new MouseButtonEventHandler(this.Cell_MouseLeftButtonUp), true);
                        element.AddHandler(UIElement.MouseEnterEvent, (Delegate)new MouseEventHandler(this.Cell_MouseEnter), true);
                        element.Click += new RoutedEventHandler(this.Cell_Clicked);
                        element.AddHandler(UIElement.PreviewKeyDownEvent, (Delegate)new RoutedEventHandler(this.CellOrMonth_PreviewKeyDown), true);
                        this._monthView.Children.Add((UIElement)element);
                    }
                }
            }
            if (this._yearView == null)
                return;
            int num = 0;
            for (int index3 = 0; index3 < 3; ++index3)
            {
                for (int index4 = 0; index4 < 4; ++index4)
                {
                    CalendarButton element = new CalendarButton();
                    element.Owner = this.Owner;
                    element.SetValue(Grid.RowProperty, (object)index3);
                    element.SetValue(Grid.ColumnProperty, (object)index4);
                    element.SetBinding(FrameworkElement.StyleProperty, this.GetOwnerBinding("CalendarButtonStyle"));
                    element.AddHandler(UIElement.MouseLeftButtonDownEvent, (Delegate)new MouseButtonEventHandler(this.Month_MouseLeftButtonDown), true);
                    element.AddHandler(UIElement.MouseLeftButtonUpEvent, (Delegate)new MouseButtonEventHandler(this.Month_MouseLeftButtonUp), true);
                    element.AddHandler(UIElement.MouseEnterEvent, (Delegate)new MouseEventHandler(this.Month_MouseEnter), true);
                    element.AddHandler(UIElement.PreviewKeyDownEvent, (Delegate)new RoutedEventHandler(this.CellOrMonth_PreviewKeyDown), true);
                    element.Click += new RoutedEventHandler(this.Month_Clicked);
                    this._yearView.Children.Add((UIElement)element);
                    ++num;
                }
            }
        }

        private void SetMonthModeDayTitles()
        {
            if (this._monthView == null)
                return;
            var culture = CustomCulture ?? PersianDateTimeWPFTools.Windows.Controls.DateTimeHelper.GetCulture((FrameworkElement)this);
            string[] shortestDayNames = PersianDateTimeWPFTools.Windows.Controls.DateTimeHelper.GetDateFormat(culture).ShortestDayNames;
            for (int index = 0; index < 7; ++index)
            {
                if (this._monthView.Children[index] is FrameworkElement child && shortestDayNames != null && shortestDayNames.Length != 0)
                {
                    child.DataContext = this.Owner == null ? (object)shortestDayNames[(int)(index + PersianDateTimeWPFTools.Windows.Controls.DateTimeHelper.GetDateFormat(culture).FirstDayOfWeek) % shortestDayNames.Length] : (object)shortestDayNames[(int)(index + this.Owner.FirstDayOfWeek) % shortestDayNames.Length];
                }
            }
        }

        private void SetMonthModeCalendarDayButtons()
        {
            DateTime firstDayOfMonth = dateTimeHelper.GetFirstDayOfMonth(this.DisplayDate);
            int fromPreviousMonth = this.GetNumberOfDisplayedDaysFromPreviousMonth(firstDayOfMonth);
            bool flag1 = dateTimeHelper.CompareYearMonth(firstDayOfMonth, this._calendar.MinSupportedDateTime) <= 0;
            bool flag2 = dateTimeHelper.CompareYearMonth(firstDayOfMonth, this._calendar.MaxSupportedDateTime) >= 0;
            int daysInMonth = this._calendar.GetDaysInMonth(firstDayOfMonth.Year, firstDayOfMonth.Month);
            var culture = CustomCulture ?? PersianDateTimeWPFTools.Windows.Controls.DateTimeHelper.GetCulture((FrameworkElement)this);
            int num = 49;
            for (int index = 7; index < num; ++index)
            {
                CalendarDayButton child = this._monthView.Children[index] as CalendarDayButton;
                int days = index - fromPreviousMonth - 7;
                if ((!flag1 || days >= 0) && (!flag2 || days < daysInMonth))
                {
                    DateTime dateTime = this._calendar.AddDays(firstDayOfMonth, days);
                    this.SetMonthModeDayButtonState(child, new DateTime?(dateTime));
                    child.DataContext = (object)dateTime;
                    child.SetContentInternal(dateTimeHelper.ToDayString(new DateTime?(dateTime), culture));

                    #region Tooltip feature

                    if (Owner?.DayToolTips != null)
                    {
                        var key = dateTime;
                        if (Owner.DayToolTips.TryGetValue(key, out var tooltip))
                        {
                            #region Tooltip feature
                            CalendarDayButtonExtensions.SetDayToolTip(child, tooltip);
                            #endregion

                            #region Tooltip template feature

                            CalendarDayButtonExtensions.SetDayToolTipTemplate(
                                child,
                                Owner.DayToolTipTemplate); 
                            #endregion
                        }
                        else
                        {
                            #region Tooltip feature
                            CalendarDayButtonExtensions.SetDayToolTip(child, null);
                            #endregion

                            #region Tooltip template feature
                            CalendarDayButtonExtensions.SetDayToolTipTemplate(child, null);
                            #endregion
                        }
                    }

                    #endregion

                    #region Day Indicators featuer

                    bool hasIndicator = false;

                    if (Owner?.DayIndicators != null)
                    {
                        var key = dateTime.Date;

                        if (Owner.DayIndicators.TryGetValue(key, out hasIndicator))
                        {
                            //CalendarDayButtonExtensions.SetHasDayIndicator(child, hasIndicator);
                        }
                        else
                        {
                            //CalendarDayButtonExtensions.SetHasDayIndicator(child, false);
                            hasIndicator = false;
                        }
                    }
                    else
                    {
                        //CalendarDayButtonExtensions.SetHasDayIndicator(child, false);
                        hasIndicator = false;
                    }

                    CalendarDayButtonExtensions.SetHasDayIndicator(child, hasIndicator);
                    #endregion
                }
                else
                {
                    this.SetMonthModeDayButtonState(child, new DateTime?());
                    child.DataContext = (object)null;
                    child.SetContentInternal(dateTimeHelper.ToDayString(new DateTime?(), culture));
                }
            }
        }

        private void SetMonthModeDayButtonState(CalendarDayButton childButton, DateTime? dateToAdd)
        {
            if (this.Owner == null)
                return;
            if (dateToAdd.HasValue)
            {
                childButton.Visibility = Visibility.Visible;
                if (PersianDateTimeWPFTools.Windows.Controls.DateTimeHelper.CompareDays(dateToAdd.Value, this.Owner.DisplayDateStartInternal) < 0 || PersianDateTimeWPFTools.Windows.Controls.DateTimeHelper.CompareDays(dateToAdd.Value, this.Owner.DisplayDateEndInternal) > 0)
                {
                    childButton.IsEnabled = false;
                    childButton.Visibility = Visibility.Hidden;
                }
                else
                {
                    childButton.IsEnabled = true;
                    childButton.SetValue(CalendarDayButton.IsBlackedOutPropertyKey, (object)this.Owner.BlackoutDates.Contains(dateToAdd.Value));
                    childButton.SetValue(CalendarDayButton.IsInactivePropertyKey, (object)(dateTimeHelper.CompareYearMonth(dateToAdd.Value, this.Owner.DisplayDateInternal) != 0));
                    if (PersianDateTimeWPFTools.Windows.Controls.DateTimeHelper.CompareDays(dateToAdd.Value, DateTime.Today) == 0)
                    {
                        childButton.SetValue(CalendarDayButton.IsTodayPropertyKey, (object)true);
                        childButton.ChangeVisualState(true);
                    }
                    else
                        childButton.SetValue(CalendarDayButton.IsTodayPropertyKey, (object)false);
                    bool flag = false;
                    foreach (DateTime selectedDate in (Collection<DateTime>)this.Owner.SelectedDates)
                        flag |= PersianDateTimeWPFTools.Windows.Controls.DateTimeHelper.CompareDays(dateToAdd.Value, selectedDate) == 0;
                    childButton.SetValue(CalendarDayButton.IsSelectedPropertyKey, (object)flag);
                }
            }
            else
            {
                childButton.Visibility = Visibility.Hidden;
                childButton.IsEnabled = false;
                childButton.SetValue(CalendarDayButton.IsBlackedOutPropertyKey, (object)false);
                childButton.SetValue(CalendarDayButton.IsInactivePropertyKey, (object)true);
                childButton.SetValue(CalendarDayButton.IsTodayPropertyKey, (object)false);
                childButton.SetValue(CalendarDayButton.IsSelectedPropertyKey, (object)false);
            }
        }

        private void AddMonthModeHighlight()
        {
            PersianCalendar owner = this.Owner;
            if (owner == null)
                return;
            if (owner.HoverStart.HasValue && owner.HoverEnd.HasValue)
            {
                DateTime start = owner.HoverEnd.Value;
                DateTime end = owner.HoverEnd.Value;
                DateTime dt1 = owner.HoverEnd.Value;
                DateTime? hoverStart = owner.HoverStart;
                DateTime dt2 = hoverStart.Value;
                int num1 = PersianDateTimeWPFTools.Windows.Controls.DateTimeHelper.CompareDays(dt1, dt2);
                if (num1 < 0)
                {
                    hoverStart = this.Owner.HoverStart;
                    end = hoverStart.Value;
                }
                else
                {
                    hoverStart = this.Owner.HoverStart;
                    start = hoverStart.Value;
                }
                int num2 = 49;
                for (int index = 7; index < num2; ++index)
                {
                    CalendarDayButton child = this._monthView.Children[index] as CalendarDayButton;
                    if (child.DataContext is DateTime)
                    {
                        DateTime dataContext = (DateTime)child.DataContext;
                        //Original code
                        //child.SetValue(CalendarDayButton.IsHighlightedPropertyKey, (object) (bool) (num1 == 0 ? 0 : (PersianDateTimeWPFTools.Windows.Controls.DateTimeHelper.InRange(dataContext, start, end) ? 1 : 0)));
                        var result = (num1 == 0 ? 0 : (PersianDateTimeWPFTools.Windows.Controls.DateTimeHelper.InRange(dataContext, start, end) ? 1 : 0));
                        child.SetValue(CalendarDayButton.IsHighlightedPropertyKey, result == 0);
                    }
                    else
                        child.SetValue(CalendarDayButton.IsHighlightedPropertyKey, (object)false);
                }
            }
            else
            {
                int num = 49;
                for (int index = 7; index < num; ++index)
                    (this._monthView.Children[index] as CalendarDayButton).SetValue(CalendarDayButton.IsHighlightedPropertyKey, (object)false);
            }
        }

        private void SetMonthModeHeaderButton()
        {
            if (this._headerButton == null)
                return;
            var currentCul = CustomCulture ?? PersianDateTimeWPFTools.Windows.Controls.DateTimeHelper.GetCulture((FrameworkElement)this);
            this._headerButton.Content = (object)PersianDateTimeWPFTools.Windows.Controls.DateTimeHelper.ToYearMonthPatternString(new DateTime?(this.DisplayDate), currentCul);
            if (this.Owner == null)
                return;
            this._headerButton.IsEnabled = true;
        }

        private void SetMonthModeNextButton()
        {
            if (this.Owner == null || this._nextButton == null)
                return;
            DateTime dateTime = dateTimeHelper.DiscardDayTime(this.DisplayDate);
            if (dateTimeHelper.CompareYearMonth(dateTime, DateTime.MaxValue) == 0)
                this._nextButton.IsEnabled = false;
            else
                this._nextButton.IsEnabled = PersianDateTimeWPFTools.Windows.Controls.DateTimeHelper.CompareDays(this.Owner.DisplayDateEndInternal, this._calendar.AddMonths(dateTime, 1)) > -1;
        }

        private void SetMonthModePreviousButton()
        {
            if (this.Owner == null || this._previousButton == null)
                return;
            this._previousButton.IsEnabled = PersianDateTimeWPFTools.Windows.Controls.DateTimeHelper.CompareDays(this.Owner.DisplayDateStartInternal, dateTimeHelper.DiscardDayTime(this.DisplayDate)) < 0;
        }

        private void SetYearButtons(DateTime decade, DateTime decadeEnd)
        {
            int year1 = this._calendar.GetYear(decade);
            int year2 = this._calendar.GetYear(decadeEnd);
            int num1 = -1;
            var culture = CustomCulture ?? PersianDateTimeWPFTools.Windows.Controls.DateTimeHelper.GetCulture((FrameworkElement)this);
            foreach (object child in this._yearView.Children)
            {
                CalendarButton calendarButton = child as CalendarButton;
                int year3 = year1 + num1;
                int num2 = year3;
                DateTime dateTime1 = this._calendar.MaxSupportedDateTime;
                int year4 = dateTime1.Year;
                if (num2 <= year4)
                {
                    int num3 = year3;
                    dateTime1 = this._calendar.MinSupportedDateTime;
                    int year5 = dateTime1.Year;
                    if (num3 >= year5)
                    {
                        DateTime dateTime2 = this._calendar.ToDateTime(year3, 1, 1, 0, 0, 0, 0);
                        calendarButton.DataContext = (object)dateTime2;
                        calendarButton.SetContentInternal(dateTimeHelper.ToYearString(
                            new DateTime?(dateTime2), culture));
                        calendarButton.Visibility = Visibility.Visible;
                        if (this.Owner != null)
                        {
                            calendarButton.HasSelectedDays = this._calendar.GetYear(this.Owner.DisplayDate) == year3;
                            int num4 = year3;
                            dateTime1 = this.Owner.DisplayDateStartInternal;
                            int year6 = dateTime1.Year;
                            if (num4 >= year6)
                            {
                                int num5 = year3;
                                dateTime1 = this.Owner.DisplayDateEndInternal;
                                int year7 = dateTime1.Year;
                                if (num5 <= year7)
                                {
                                    calendarButton.IsEnabled = true;
                                    calendarButton.Opacity = 1.0;
                                    goto label_9;
                                }
                            }
                            calendarButton.IsEnabled = false;
                            calendarButton.Opacity = 0.0;
                        }
                    label_9:
                        calendarButton.IsInactive = year3 < year1 || year3 > year2;
                        goto label_11;
                    }
                }
                calendarButton.DataContext = (object)null;
                calendarButton.IsEnabled = false;
                calendarButton.Opacity = 0.0;
            label_11:
                ++num1;
            }
        }

        private void SetYearModeMonthButtons()
        {
            int num = 0;
            var culture = CustomCulture ?? PersianDateTimeWPFTools.Windows.Controls.DateTimeHelper.GetCulture((FrameworkElement)this);
            foreach (object child in this._yearView.Children)
            {
                CalendarButton calendarButton = child as CalendarButton;
                DateTime dateTime = this._calendar.ToDateTime(this._calendar.GetYear(this.DisplayDate), num + 1, 1, 0, 0, 0, 0);
                calendarButton.DataContext = (object)dateTime;
                calendarButton.SetContentInternal(PersianDateTimeWPFTools.Windows.Controls.DateTimeHelper.ToAbbreviatedMonthString(new DateTime?(dateTime), culture));
                calendarButton.Visibility = Visibility.Visible;
                if (this.Owner != null)
                {
                    calendarButton.HasSelectedDays = dateTimeHelper.CompareYearMonth(dateTime, this.Owner.DisplayDateInternal) == 0;
                    if (dateTimeHelper.CompareYearMonth(dateTime, this.Owner.DisplayDateStartInternal) < 0 || dateTimeHelper.CompareYearMonth(dateTime, this.Owner.DisplayDateEndInternal) > 0)
                    {
                        calendarButton.IsEnabled = false;
                        calendarButton.Opacity = 0.0;
                    }
                    else
                    {
                        calendarButton.IsEnabled = true;
                        calendarButton.Opacity = 1.0;
                    }
                }
                calendarButton.IsInactive = false;
                ++num;
            }
        }

        private void SetYearModeHeaderButton()
        {
            if (this._headerButton == null)
                return;
            this._headerButton.IsEnabled = true;
            var culture = CustomCulture ?? PersianDateTimeWPFTools.Windows.Controls.DateTimeHelper.GetCulture((FrameworkElement)this);
            this._headerButton.Content = (object)dateTimeHelper.ToYearString(new DateTime?(this.DisplayDate), culture);
        }

        private void SetYearModeNextButton()
        {
            if (this.Owner == null || this._nextButton == null)
                return;
            Button nextButton = this._nextButton;
            DateTime dateTime = this.Owner.DisplayDateEndInternal;
            int year1 = dateTime.Year;
            dateTime = this.DisplayDate;
            int year2 = dateTime.Year;
            int num = year1 != year2 ? 1 : 0;
            nextButton.IsEnabled = num != 0;
        }

        private void SetYearModePreviousButton()
        {
            if (this.Owner == null || this._previousButton == null)
                return;
            Button previousButton = this._previousButton;
            DateTime dateTime = this.Owner.DisplayDateStartInternal;
            int year1 = dateTime.Year;
            dateTime = this.DisplayDate;
            int year2 = dateTime.Year;
            int num = year1 != year2 ? 1 : 0;
            previousButton.IsEnabled = num != 0;
        }

        private void SetDecadeModeHeaderButton(DateTime decade)
        {
            if (this._headerButton == null)
                return;
            var culture = CustomCulture ?? PersianDateTimeWPFTools.Windows.Controls.DateTimeHelper.GetCulture((FrameworkElement)this);
            this._headerButton.Content = (object)dateTimeHelper.ToDecadeRangeString(decade, culture);
            this._headerButton.IsEnabled = false;
        }

        private void SetDecadeModeNextButton(DateTime decadeEnd)
        {
            if (this.Owner == null || this._nextButton == null)
                return;
            this._nextButton.IsEnabled = this._calendar.GetYear(this.Owner.DisplayDateEndInternal) > this._calendar.GetYear(decadeEnd);
        }

        private void SetDecadeModePreviousButton(DateTime decade)
        {
            if (this.Owner == null || this._previousButton == null)
                return;
            int year = this._calendar.GetYear(this.Owner.DisplayDateStartInternal);
            this._previousButton.IsEnabled = this._calendar.GetYear(decade) > year;
        }

        private int GetNumberOfDisplayedDaysFromPreviousMonth(DateTime firstOfMonth)
        {
            DayOfWeek dayOfWeek = this._calendar.GetDayOfWeek(firstOfMonth);
            var cul = CustomCulture ?? PersianDateTimeWPFTools.Windows.Controls.DateTimeHelper.GetCulture((FrameworkElement)this);
            int num = this.Owner == null ? (dayOfWeek - PersianDateTimeWPFTools.Windows.Controls.DateTimeHelper.GetDateFormat(cul).FirstDayOfWeek + 7) % 7 : (dayOfWeek - this.Owner.FirstDayOfWeek + 7) % 7;
            return num == 0 ? 7 : num;
        }

        private BindingBase GetOwnerBinding(string propertyName)
        {
            return (BindingBase)new Binding(propertyName)
            {
                Source = (object)this.Owner
            };
        }
    }
}