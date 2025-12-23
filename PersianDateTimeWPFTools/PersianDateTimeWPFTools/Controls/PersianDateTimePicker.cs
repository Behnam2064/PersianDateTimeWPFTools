using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using System.Windows;
using System.Globalization;
using System.Collections.ObjectModel;
using CalendarDateRange = PersianDateTimeWPFTools.Windows.Controls.CalendarDateRange;
using System.Diagnostics;
using PersianDateTimeWPFTools.Tools;
using System.Runtime.CompilerServices;

namespace PersianDateTimeWPFTools.Controls
{
    [TemplatePart(Name = ElementRoot, Type = typeof(Grid))]
    [TemplatePart(Name = ElementTextBox, Type = typeof(PersianDateTimeWPFTools.Windows.Controls.Primitives.DatePickerTextBox))]
    [TemplatePart(Name = ElementButton, Type = typeof(Button))]
    [TemplatePart(Name = ElementPopup, Type = typeof(Popup))]
    public class PersianDateTimePicker : Control
    {
        #region Constants

        private const string ElementRoot = "PART_Root";

        private const string ElementTextBox = "PART_TextBox";

        private const string ElementButton = "PART_Button";

        private const string ElementPopup = "PART_Popup";

        #endregion Constants

        #region Data

        private PersianCalendarWithClock _calendarWithClock;

        private string _defaultText;

        private ButtonBase _dropDownButton;

        private Popup _popup;

        private bool _disablePopupReopen;

        private PersianDateTimeWPFTools.Windows.Controls.Primitives.DatePickerTextBox _textBox;

        private IDictionary<DependencyProperty, bool> _isHandlerSuspended;

        private DateTime? _originalSelectedDateTime;


        public bool ShowConfirmButton
        {
            get { return (bool)GetValue(ShowConfirmButtonProperty); }
            set { SetValue(ShowConfirmButtonProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShowConfirmButton.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowConfirmButtonProperty =
            DependencyProperty.Register("ShowConfirmButton", typeof(bool), typeof(PersianDateTimePicker), new PropertyMetadata(true));


        public event EventHandler ConfirmButtonClicked
        {
            add => _calendarWithClock.ConfirmButtonClicked += value;
            remove => _calendarWithClock.ConfirmButtonClicked -= value;
        }


        public ObservableCollection<CalendarDateRange> BlackoutDates
        {
            get => _calendarWithClock?.BlackoutDates;
        }


        #region Tooltip feature
        public IDictionary<DateTime, object> DayToolTips
        {
            get => (IDictionary<DateTime, object>)GetValue(DayToolTipsProperty);
            set => SetValue(DayToolTipsProperty, value);
        }

        public static readonly DependencyProperty DayToolTipsProperty =
            DependencyProperty.Register(
                nameof(DayToolTips),
                typeof(IDictionary<DateTime, object>),
                typeof(PersianDateTimePicker),
                new PropertyMetadata(null));

        #endregion

        #region Tooltip template feature
        public DataTemplate DayToolTipTemplate
        {
            get => (DataTemplate)GetValue(DayToolTipTemplateProperty);
            set => SetValue(DayToolTipTemplateProperty, value);
        }

        public static readonly DependencyProperty DayToolTipTemplateProperty =
            DependencyProperty.Register(
                nameof(DayToolTipTemplate),
                typeof(DataTemplate),
                typeof(PersianDateTimePicker),
                new PropertyMetadata(null));

        #endregion

        #endregion Data

        public static readonly DependencyProperty SelectedDateFormatProperty
            = DependencyProperty.Register(nameof(SelectedDateFormat), typeof(PersianDateTimeWPFTools.Windows.Controls.DatePickerFormat),
                typeof(PersianDateTimePicker), (PropertyMetadata)new
                FrameworkPropertyMetadata((object)PersianDateTimeWPFTools.Windows.Controls.DatePickerFormat.Long,
                    new PropertyChangedCallback(PersianDateTimePicker.OnSelectedDateFormatChanged)),
                new ValidateValueCallback(PersianDateTimePicker.IsValidSelectedDateFormat));
        public PersianDateTimeWPFTools.Windows.Controls.DatePickerFormat SelectedDateFormat
        {
            get => (PersianDateTimeWPFTools.Windows.Controls.DatePickerFormat)this.GetValue(SelectedDateFormatProperty);
            set => this.SetValue(SelectedDateFormatProperty, (object)value);
        }

        private static void OnSelectedDateFormatChanged(
  DependencyObject d,
  DependencyPropertyChangedEventArgs e)
        {
            PersianDateTimePicker persianDatePicker = d as PersianDateTimePicker;
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


        public static readonly DependencyProperty AllowSelectBlackedOutDayProperty
            = DependencyProperty.Register(nameof(AllowSelectBlackedOutDay), typeof(bool), typeof(PersianDateTimePicker), (PropertyMetadata)new FrameworkPropertyMetadata((object)false));
        public static readonly DependencyProperty CustomCultureProperty
            = DependencyProperty.Register(nameof(CustomCulture), typeof(CultureInfo), typeof(PersianDateTimePicker), (PropertyMetadata)new FrameworkPropertyMetadata((object)null
                , new PropertyChangedCallback((d, e) =>
            {
                PersianDateTimePicker persianCalendar = d as PersianDateTimePicker;
                //Changing how the date is displayed in the PersianDatePicker TextBox when changing Culture
                if (persianCalendar.IsLoaded && persianCalendar.SelectedDateTime != null)
                {
                    persianCalendar.SetTextInternal(persianCalendar.DateTimeToString((DateTime)persianCalendar.SelectedDateTime));
                }

            })));
        public static readonly DependencyProperty CustomCultureNameProperty
            = DependencyProperty.Register(nameof(CustomCultureName), typeof(string), typeof(PersianDateTimePicker), (PropertyMetadata)new FrameworkPropertyMetadata((object)null,
                new PropertyChangedCallback((d, e) =>
            {
                /* PersianDateTimePicker persianCalendar = d as PersianDateTimePicker;
                 //Changing how the date is displayed in the PersianDatePicker TextBox when changing Culture
                 if (persianCalendar.IsLoaded && persianCalendar.SelectedDateTime != null)
                 {
                     persianCalendar.SetTextInternal(persianCalendar.DateTimeToString((DateTime)persianCalendar.SelectedDateTime));
                 }*/

            })));




        public static readonly DependencyProperty DisplayDateProperty = DependencyProperty.Register(nameof(DisplayDate), typeof(DateTime), typeof(PersianDateTimePicker), (PropertyMetadata)new FrameworkPropertyMetadata((object)DateTime.MinValue, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public static readonly DependencyProperty DisplayDateEndProperty = DependencyProperty.Register(nameof(DisplayDateEnd), typeof(DateTime?), typeof(PersianDateTimePicker), (PropertyMetadata)new FrameworkPropertyMetadata((object)null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public static readonly DependencyProperty DisplayDateStartProperty = DependencyProperty.Register(nameof(DisplayDateStart), typeof(DateTime?), typeof(PersianDateTimePicker), (PropertyMetadata)new FrameworkPropertyMetadata((object)null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public static readonly DependencyProperty DisplayModeProperty = DependencyProperty.Register(nameof(DisplayMode), typeof(PersianDateTimeWPFTools.Windows.Controls.CalendarMode), typeof(PersianDateTimePicker), (PropertyMetadata)new FrameworkPropertyMetadata((object)PersianDateTimeWPFTools.Windows.Controls.CalendarMode.Month, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public static readonly DependencyProperty FirstDayOfWeekProperty = DependencyProperty.Register(nameof(FirstDayOfWeek), typeof(DayOfWeek), typeof(PersianDateTimePicker), (PropertyMetadata)new FrameworkPropertyMetadata((object)PersianDateTimeWPFTools.Windows.Controls.DateTimeHelper.GetCurrentDateFormat().FirstDayOfWeek));
        public static readonly DependencyProperty SelectionModeProperty = DependencyProperty.Register(nameof(SelectionMode), typeof(PersianDateTimeWPFTools.Windows.Controls.CalendarSelectionMode), typeof(PersianDateTimePicker), (PropertyMetadata)new FrameworkPropertyMetadata((object)PersianDateTimeWPFTools.Windows.Controls.CalendarSelectionMode.SingleDate));


        public DateTime DisplayDate
        {
            get => (DateTime)this.GetValue(PersianDateTimePicker.DisplayDateProperty);
            set => this.SetValue(PersianDateTimePicker.DisplayDateProperty, (object)value);
        }


        public DateTime? DisplayDateEnd
        {
            get => (DateTime?)this.GetValue(PersianDateTimePicker.DisplayDateEndProperty);
            set => this.SetValue(PersianDateTimePicker.DisplayDateEndProperty, (object)value);
        }

        public DateTime? DisplayDateStart
        {
            get => (DateTime?)this.GetValue(PersianDateTimePicker.DisplayDateStartProperty);
            set => this.SetValue(PersianDateTimePicker.DisplayDateStartProperty, (object)value);
        }

        public PersianDateTimeWPFTools.Windows.Controls.CalendarMode DisplayMode
        {
            get => (PersianDateTimeWPFTools.Windows.Controls.CalendarMode)this.GetValue(PersianDateTimePicker.DisplayModeProperty);
            set => this.SetValue(PersianDateTimePicker.DisplayModeProperty, (object)value);
        }


        public DayOfWeek FirstDayOfWeek
        {
            get => (DayOfWeek)this.GetValue(PersianDateTimePicker.FirstDayOfWeekProperty);
            set => this.SetValue(PersianDateTimePicker.FirstDayOfWeekProperty, (object)value);
        }


        public static readonly DependencyProperty ShowTodayButtonProperty
            = DependencyProperty.Register(nameof(ShowTodayButton), typeof(bool), typeof(PersianDateTimePicker), (PropertyMetadata)new FrameworkPropertyMetadata(false));


        public static readonly DependencyProperty IsTodayHighlightedProperty =
          DependencyProperty.Register(nameof(IsTodayHighlighted),
              typeof(bool), typeof(PersianDateTimePicker), (PropertyMetadata)new
              FrameworkPropertyMetadata((object)true));

        public bool IsTodayHighlighted
        {
            get => (bool)this.GetValue(IsTodayHighlightedProperty);
            set => this.SetValue(IsTodayHighlightedProperty, (object)value);
        }



        public bool AllowSelectBlackedOutDay
        {
            get => (bool)this.GetValue(PersianDateTimePicker.AllowSelectBlackedOutDayProperty);
            set => this.SetValue(PersianDateTimePicker.AllowSelectBlackedOutDayProperty, (object)value);
        }

        public string CustomCultureName
        {
            get => (string)this.GetValue(PersianDateTimePicker.CustomCultureNameProperty);
            set => this.SetValue(PersianDateTimePicker.CustomCultureNameProperty, (object)value);
        }

        public bool ShowTodayButton
        {
            get => (bool)this.GetValue(PersianDateTimePicker.ShowTodayButtonProperty);
            set => this.SetValue(PersianDateTimePicker.ShowTodayButtonProperty, value);
        }

        public CultureInfo CustomCulture
        {
            get => this.GetValue(PersianDateTimePicker.CustomCultureProperty) as CultureInfo;

            set => this.SetValue(PersianDateTimePicker.CustomCultureProperty, (object)value);
        }


        #region Public Events

        public static readonly RoutedEvent SelectedDateTimeChangedEvent =
            EventManager.RegisterRoutedEvent("SelectedDateTimeChanged", RoutingStrategy.Direct,
                typeof(EventHandler<PersianDateTimeWPFTools.Windows.Controls.CalendarDateChangedEventArgs>), typeof(PersianDateTimePicker));

        public event EventHandler<PersianDateTimeWPFTools.Windows.Controls.CalendarDateChangedEventArgs> SelectedDateTimeChanged
        {
            add => AddHandler(SelectedDateTimeChangedEvent, value);
            remove => RemoveHandler(SelectedDateTimeChangedEvent, value);
        }

        public event RoutedEventHandler PickerClosed;

        public event RoutedEventHandler PickerOpened;

        #endregion Public Events

        static PersianDateTimePicker()
        {
            EventManager.RegisterClassHandler(typeof(PersianDateTimePicker), GotFocusEvent, new RoutedEventHandler(OnGotFocus));
            KeyboardNavigation.TabNavigationProperty.OverrideMetadata(typeof(PersianDateTimePicker), new FrameworkPropertyMetadata(KeyboardNavigationMode.Once));
            KeyboardNavigation.IsTabStopProperty.OverrideMetadata(typeof(PersianDateTimePicker), new FrameworkPropertyMetadata(false));
        }

        public PersianDateTimePicker()
        {
            InitResources.SetControlStyle(this);
            DisplayDate = DateTime.Today;
            InitCalendarWithClock();
            //CommandBindings.Add(new CommandBinding(ControlCommands.Clear, (s, e) =>
            //{
            //    SetCurrentValue(SelectedDateTimeProperty, null);
            //    SetCurrentValue(TextProperty, "");
            //    _textBox.Text = string.Empty;
            //}));
        }

        #region Public Properties
        [ObsoleteAttribute]
        public static readonly DependencyProperty CalendarStyleProperty = DependencyProperty.Register(
            "CalendarStyle", typeof(Style), typeof(PersianDateTimePicker), new PropertyMetadata(default(Style)));
        [ObsoleteAttribute]
        public Style CalendarStyle
        {
            get => (Style)GetValue(CalendarStyleProperty);
            set => SetValue(CalendarStyleProperty, value);
        }


        public static readonly DependencyProperty PersianCalendarWithClockStyleProperty = DependencyProperty.Register(
            "PersianCalendarWithClockStyle", typeof(Style), typeof(PersianDateTimePicker), new PropertyMetadata(default(Style)));

        public Style PersianCalendarWithClockStyle
        {
            get => (Style)GetValue(PersianCalendarWithClockStyleProperty);
            set => SetValue(PersianCalendarWithClockStyleProperty, value);
        }



        public static readonly DependencyProperty DisplayDateTimeProperty = DependencyProperty.Register(
            "DisplayDateTime", typeof(DateTime), typeof(PersianDateTimePicker), new FrameworkPropertyMetadata(DateTime.Now, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, null, CoerceDisplayDateTime));

        private static object CoerceDisplayDateTime(DependencyObject d, object value)
        {
            var dp = (PersianDateTimePicker)d;
            dp._calendarWithClock.DisplayDateTime = (DateTime)value;

            return dp._calendarWithClock.DisplayDateTime;
        }

        public DateTime DisplayDateTime
        {
            get => (DateTime)GetValue(DisplayDateTimeProperty);
            set => SetValue(DisplayDateTimeProperty, value);
        }

        public static readonly DependencyProperty IsDropDownOpenProperty = DependencyProperty.Register(
            "IsDropDownOpen", typeof(bool), typeof(PersianDateTimePicker), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnIsDropDownOpenChanged, OnCoerceIsDropDownOpen));

        private static object OnCoerceIsDropDownOpen(DependencyObject d, object baseValue)
        {
            var picker = d as PersianDateTimePicker;
            if (picker != null && !picker.IsEnabled)
                return false;

            return baseValue;
        }


        private static void OnIsDropDownOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var dp = d as PersianDateTimePicker;

            var newValue = (bool)e.NewValue;
            if (dp?._popup != null && dp._popup.IsOpen != newValue)
            {
                dp._popup.IsOpen = newValue;
                if (newValue)
                {
                    dp._originalSelectedDateTime = dp.SelectedDateTime;

                    dp.Dispatcher.BeginInvoke(DispatcherPriority.Input, (Action)delegate
                    {
                        dp._calendarWithClock.Focus();
                    });
                }
            }
        }

        public bool IsDropDownOpen
        {
            get => (bool)GetValue(IsDropDownOpenProperty);
            set => SetValue(IsDropDownOpenProperty, value);
        }

        public static readonly DependencyProperty SelectedDateTimeProperty = DependencyProperty.Register(
            "SelectedDateTime", typeof(DateTime?), typeof(PersianDateTimePicker), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnSelectedDateTimeChanged, CoerceSelectedDateTime));

        private static object CoerceSelectedDateTime(DependencyObject d, object value)
        {
            var dp = (PersianDateTimePicker)d;
            dp._calendarWithClock.SelectedDateTime = (DateTime?)value;
            return dp._calendarWithClock.SelectedDateTime;
        }

        private static void OnSelectedDateTimeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is PersianDateTimePicker dp)) return;

            if (dp.SelectedDateTime.HasValue)
            {
                var time = dp.SelectedDateTime.Value;
                dp.SetTextInternal(dp.DateTimeToString(time));
            }

            dp.RaiseEvent(new PersianDateTimeWPFTools.Windows.Controls.CalendarDateChangedEventArgs(SelectedDateTimeChangedEvent, e.OldValue as DateTime?, e.NewValue as DateTime?));
        }

        public DateTime? SelectedDateTime
        {
            get => (DateTime?)GetValue(SelectedDateTimeProperty);
            set => SetValue(SelectedDateTimeProperty, value);
        }

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            "Text", typeof(string), typeof(PersianDateTimePicker), new FrameworkPropertyMetadata(string.Empty, OnTextChanged));

        private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is PersianDateTimePicker dp && !dp.IsHandlerSuspended(TextProperty))
            {
                if (e.NewValue is string newValue)
                {
                    if (dp._textBox != null)
                    {
                        dp._textBox.Text = newValue;
                    }
                    else
                    {
                        dp._defaultText = newValue;
                    }

                    dp.SetSelectedDateTime();
                }
                else
                {
                    dp.SetValueNoCallback(SelectedDateTimeProperty, null);
                }
            }
        }

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        /// <summary>
        /// Sets the local Text property without breaking bindings
        /// </summary>
        /// <param name="value"></param>
        private void SetTextInternal(string value)
        {
            SetCurrentValue(TextProperty, value);
        }

        public static readonly DependencyProperty SelectionBrushProperty =
            TextBoxBase.SelectionBrushProperty.AddOwner(typeof(PersianDateTimePicker));

        public Brush SelectionBrush
        {
            get => (Brush)GetValue(SelectionBrushProperty);
            set => SetValue(SelectionBrushProperty, value);
        }

#if !(NET40 || NET45 || NET451 || NET452 || NET46 || NET461 || NET462 || NET47 || NET471 || NET472)

        public static readonly DependencyProperty SelectionTextBrushProperty =
            TextBoxBase.SelectionTextBrushProperty.AddOwner(typeof(PersianDateTimePicker));

        public Brush SelectionTextBrush
        {
            get => (Brush)GetValue(SelectionTextBrushProperty);
            set => SetValue(SelectionTextBrushProperty, value);
        }

#endif

        public static readonly DependencyProperty SelectionOpacityProperty =
            TextBoxBase.SelectionOpacityProperty.AddOwner(typeof(PersianDateTimePicker));

        public double SelectionOpacity
        {
            get => (double)GetValue(SelectionOpacityProperty);
            set => SetValue(SelectionOpacityProperty, value);
        }

        public static readonly DependencyProperty CaretBrushProperty =
            TextBoxBase.CaretBrushProperty.AddOwner(typeof(PersianDateTimePicker));

        public Brush CaretBrush
        {
            get => (Brush)GetValue(CaretBrushProperty);
            set => SetValue(CaretBrushProperty, value);
        }

        #endregion

        #region Public Methods

        public override void OnApplyTemplate()
        {
            if (DesignerProperties.GetIsInDesignMode(this)) return;

            _calendarWithClock.SetBinding(PersianCalendarWithClock.DayToolTipsProperty,
              new Binding(DayToolTipsProperty.Name) { Source = this, Mode = BindingMode.TwoWay });

            _calendarWithClock.SetBinding(PersianCalendarWithClock.DayToolTipTemplateProperty,
                new Binding(DayToolTipTemplateProperty.Name) { Source = this, Mode = BindingMode.TwoWay });


            if (_popup != null)
            {
                _popup.PreviewMouseLeftButtonDown -= PopupPreviewMouseLeftButtonDown;
                _popup.Opened -= PopupOpened;
                _popup.Closed -= PopupClosed;
                _popup.Child = null;
            }

            if (_dropDownButton != null)
            {
                _dropDownButton.Click -= DropDownButton_Click;
                _dropDownButton.MouseLeave -= DropDownButton_MouseLeave;
            }

            if (_textBox != null)
            {
                _textBox.KeyDown -= TextBox_KeyDown;
                _textBox.TextChanged -= TextBox_TextChanged;
                _textBox.LostFocus -= TextBox_LostFocus;
            }

            base.OnApplyTemplate();

            _popup = GetTemplateChild(ElementPopup) as Popup;
            _dropDownButton = GetTemplateChild(ElementButton) as Button;
            _textBox = GetTemplateChild(ElementTextBox) as PersianDateTimeWPFTools.Windows.Controls.Primitives.DatePickerTextBox;

            CheckNull();

            _popup.PreviewMouseLeftButtonDown += PopupPreviewMouseLeftButtonDown;
            _popup.PreviewKeyDown += _popup_PreviewKeyDown;
            _popup.Opened += PopupOpened;
            _popup.Closed += PopupClosed;
            _popup.Child = _calendarWithClock;

            if (IsDropDownOpen)
            {
                _popup.IsOpen = true;
            }

            _dropDownButton.Click += DropDownButton_Click;
            _dropDownButton.MouseLeave += DropDownButton_MouseLeave;

            var selectedDateTime = SelectedDateTime;

            if (_textBox != null)
            {
                /*if (selectedDateTime == null)
                {
                    _textBox.Text = DateTimeToString(DateTime.Now);
                }*/

                _textBox.SetBinding(SelectionBrushProperty, new Binding(SelectionBrushProperty.Name) { Source = this });
#if !(NET40 || NET45 || NET451 || NET452 || NET46 || NET461 || NET462 || NET47 || NET471 || NET472)
                _textBox.SetBinding(SelectionTextBrushProperty, new Binding(SelectionTextBrushProperty.Name) { Source = this });
#endif
                _textBox.SetBinding(SelectionOpacityProperty, new Binding(SelectionOpacityProperty.Name) { Source = this });
                _textBox.SetBinding(CaretBrushProperty, new Binding(CaretBrushProperty.Name) { Source = this });

                _textBox.KeyDown += TextBox_KeyDown;
                _textBox.TextChanged += TextBox_TextChanged;
                _textBox.LostFocus += TextBox_LostFocus;

                if (selectedDateTime == null)
                {
                    if (!string.IsNullOrEmpty(_defaultText))
                    {
                        _textBox.Text = _defaultText;
                        SetSelectedDateTime();
                    }
                }
                else
                {
                    _textBox.Text = DateTimeToString(selectedDateTime.Value);
                }
            }

            if (selectedDateTime is null)
            {
                if (_originalSelectedDateTime == null)
                    _originalSelectedDateTime = DateTime.Now;
                SetCurrentValue(DisplayDateTimeProperty, _originalSelectedDateTime);
            }
            else
            {
                SetCurrentValue(DisplayDateTimeProperty, selectedDateTime);
            }
        }

        private void _popup_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Escape)
            {
                if (_popup != null && _popup.IsOpen)
                {
                    _popup.IsOpen = false;
                }
            }
        }

        public override string ToString() => SelectedDateTime != null ? DateTimeToString((DateTime)SelectedDateTime) : string.Empty;

        #endregion

        #region Protected Methods

        protected virtual void OnPickerClosed(RoutedEventArgs e)
        {
            var handler = PickerClosed;
            handler?.Invoke(this, e);
        }

        protected virtual void OnPickerOpened(RoutedEventArgs e)
        {
            var handler = PickerOpened;
            handler?.Invoke(this, e);
        }

        #endregion Protected Methods

        #region Private Methods

        private void CheckNull()
        {
            if (_dropDownButton == null || _popup == null || _textBox == null)
                throw new Exception();
        }

        private void InitCalendarWithClock()
        {
            _calendarWithClock = new PersianCalendarWithClock
            {
                //ShowConfirmButton = true
            };


            _calendarWithClock.SetBinding(PersianCalendarWithClock.StyleProperty,
                new Binding(PersianCalendarWithClockStyleProperty.Name) { Source = this, Mode = BindingMode.TwoWay });

            _calendarWithClock.SetBinding(PersianCalendarWithClock.ShowConfirmButtonProperty,
                new Binding(ShowConfirmButtonProperty.Name) { Source = this, Mode = BindingMode.TwoWay });

            _calendarWithClock.SetBinding(PersianCalendarWithClock.FirstDayOfWeekProperty,
                new Binding(FirstDayOfWeekProperty.Name) { Source = this, Mode = BindingMode.TwoWay });

            _calendarWithClock.SetBinding(PersianCalendarWithClock.DisplayModeProperty,
                new Binding(DisplayModeProperty.Name) { Source = this, Mode = BindingMode.TwoWay });

            _calendarWithClock.SetBinding(PersianCalendarWithClock.DisplayDateStartProperty,
                new Binding(DisplayDateStartProperty.Name) { Source = this, Mode = BindingMode.TwoWay });

            _calendarWithClock.SetBinding(PersianCalendarWithClock.DisplayDateEndProperty,
                new Binding(DisplayDateEndProperty.Name) { Source = this, Mode = BindingMode.TwoWay });


            _calendarWithClock.SetBinding(PersianCalendarWithClock.DisplayDateProperty,
                new Binding(DisplayDateProperty.Name) { Source = this, Mode = BindingMode.TwoWay });



            _calendarWithClock.SetBinding(PersianCalendarWithClock.IsTodayHighlightedProperty,
                new Binding(IsTodayHighlightedProperty.Name) { Source = this, Mode = BindingMode.TwoWay });

            _calendarWithClock.SetBinding(PersianCalendarWithClock.AllowSelectBlackedOutDayProperty,
                new Binding(AllowSelectBlackedOutDayProperty.Name) { Source = this, Mode = BindingMode.TwoWay });

            _calendarWithClock.SetBinding(PersianCalendarWithClock.CustomCultureProperty,
                new Binding(CustomCultureProperty.Name) { Source = this, Mode = BindingMode.TwoWay });

            _calendarWithClock.SetBinding(PersianCalendarWithClock.CustomCultureNameProperty,
                new Binding(CustomCultureNameProperty.Name) { Source = this, Mode = BindingMode.TwoWay });

            _calendarWithClock.SetBinding(PersianCalendarWithClock.ShowTodayButtonProperty,
                new Binding(ShowTodayButtonProperty.Name) { Source = this, Mode = BindingMode.TwoWay });

            DisplayDate = DateTime.Now;//If there is no current code and you select the year or decade button, you will encounter an error.
            _calendarWithClock.SelectedDateTimeChanged += CalendarWithClock_SelectedDateTimeChanged;
            _calendarWithClock.ConfirmButtonClicked += CalendarWithClock_Confirmed;
        }

        private void CalendarWithClock_Confirmed(object sender, EventArgs e) => TogglePopup();

        private void CalendarWithClock_SelectedDateTimeChanged(object sender, PersianDateTimeWPFTools.Windows.Controls.CalendarDateChangedEventArgs e)
        {
            SelectedDateTime = e.AddedDate;
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            SetSelectedDateTime();
        }

        private void SetIsHandlerSuspended(DependencyProperty property, bool value)
        {
            if (value)
            {
                if (_isHandlerSuspended == null)
                    _isHandlerSuspended = new Dictionary<DependencyProperty, bool>(2);

                _isHandlerSuspended[property] = true;
            }
            else
            {
                _isHandlerSuspended?.Remove(property);
            }
        }

        private void SetValueNoCallback(DependencyProperty property, object value)
        {
            SetIsHandlerSuspended(property, true);
            try
            {
                SetCurrentValue(property, value);
            }
            finally
            {
                SetIsHandlerSuspended(property, false);
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            SetValueNoCallback(TextProperty, _textBox.Text);
        }

        private bool ProcessPersianDateTimePickerKey(KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.System:
                    {
                        switch (e.SystemKey)
                        {
                            case Key.Down:
                                {
                                    if ((Keyboard.Modifiers & ModifierKeys.Alt) == ModifierKeys.Alt)
                                    {
                                        TogglePopup();
                                        return true;
                                    }

                                    break;
                                }
                        }

                        break;
                    }

                case Key.Enter:
                    {
                        SetSelectedDateTime();
                        return true;
                    }
            }

            return false;
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = ProcessPersianDateTimePickerKey(e) || e.Handled;
        }

        private void DropDownButton_MouseLeave(object sender, MouseEventArgs e)
        {
            _disablePopupReopen = false;
        }

        private bool IsHandlerSuspended(DependencyProperty property)
        {
            return _isHandlerSuspended != null && _isHandlerSuspended.ContainsKey(property);
        }

        private void PopupPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var popup = sender as Popup;
            if (popup != null && popup.StaysOpen == false)
            {
                if (_dropDownButton != null && _dropDownButton.InputHitTest(e.GetPosition(_dropDownButton)) != null)
                {
                    _disablePopupReopen = true;
                }
            }

        }

        private void PopupOpened(object sender, EventArgs e)
        {
            if (!IsDropDownOpen)
            {
                SetCurrentValue(IsDropDownOpenProperty, true);
            }

            _calendarWithClock?.MoveFocus(new TraversalRequest(FocusNavigationDirection.First));

            OnPickerOpened(new RoutedEventArgs());
        }

        private void PopupClosed(object sender, EventArgs e)
        {
            if (IsDropDownOpen)
            {
                SetCurrentValue(IsDropDownOpenProperty, false);
            }

            if (_calendarWithClock.IsKeyboardFocusWithin)
            {
                MoveFocus(new TraversalRequest(FocusNavigationDirection.First));
            }

            OnPickerClosed(new RoutedEventArgs());
        }

        private void DropDownButton_Click(object sender, RoutedEventArgs e) => TogglePopup();

        private void TogglePopup()
        {
            if (IsDropDownOpen)
            {
                SetCurrentValue(IsDropDownOpenProperty, false);
            }
            else
            {
                if (_disablePopupReopen)
                {
                    _disablePopupReopen = false;
                }
                else
                {
                    SetSelectedDateTime();
                    SetCurrentValue(IsDropDownOpenProperty, true);
                }
            }
        }

        private void SafeSetText(string s)
        {
            if (string.Compare(Text, s, StringComparison.Ordinal) != 0)
            {
                SetCurrentValue(TextProperty, s);
            }
        }

        private DateTime? ParseText(string text)
        {
            try
            {
                return DateTime.Parse(text);
            }
            catch
            {
                // ignored
            }

            return null;
        }

        private DateTime? SetTextBoxValue(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                SafeSetText(s);
                return SelectedDateTime;
            }

            var d = ParseText(s);

            if (d != null)
            {
                SafeSetText(DateTimeToString((DateTime)d));
                return d;
            }

            if (SelectedDateTime != null)
            {
                var newtext = DateTimeToString(SelectedDateTime.Value);
                SafeSetText(newtext);
                return SelectedDateTime;
            }
            SafeSetText(DateTimeToString(DisplayDateTime));
            return DisplayDateTime;
        }

        private void SetSelectedDateTime()
        {
            if (_textBox != null)
            {
                if (!string.IsNullOrEmpty(_textBox.Text))
                {
                    var s = _textBox.Text;

                    if (SelectedDateTime != null)
                    {
                        if (SelectedDateTime != DisplayDateTime)
                        {
                            SetCurrentValue(DisplayDateTimeProperty, SelectedDateTime);
                        }

                        var selectedTime = DateTimeToString(SelectedDateTime.Value);

                        if (string.Compare(selectedTime, s, StringComparison.Ordinal) == 0)
                        {
                            return;
                        }
                    }

                    var d = SetTextBoxValue(s);
                    if (!SelectedDateTime.Equals(d))
                    {
                        SetCurrentValue(SelectedDateTimeProperty, d);
                        SetCurrentValue(DisplayDateTimeProperty, d);
                    }
                }
                else
                {
                    if (SelectedDateTime.HasValue)
                    {
                        SetCurrentValue(SelectedDateTimeProperty, null);
                    }
                }
            }
            else
            {
                var d = SetTextBoxValue(_defaultText);
                if (!SelectedDateTime.Equals(d))
                {
                    SetCurrentValue(SelectedDateTimeProperty, d);
                }
            }
        }

        //private string DateTimeToString(DateTime d) => d.ToString(DateTimeFormat);

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
        private void SetWaterMarkText()
        {
            if (this._textBox == null)
                return;
            var culture = _calendarWithClock?.CustomCulture ?? PersianDateTimeWPFTools.Windows.Controls.DateTimeHelper.GetCulture((FrameworkElement)this);
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

        private static void OnGotFocus(object sender, RoutedEventArgs e)
        {
            var picker = (PersianDateTimePicker)sender;
            if (!e.Handled && picker._textBox != null)
            {
                if (Equals(e.OriginalSource, picker))
                {
                    picker._textBox.Focus();
                    e.Handled = true;
                }
                else if (Equals(e.OriginalSource, picker._textBox))
                {
                    picker._textBox.SelectAll();
                    e.Handled = true;
                }
            }
        }

        private static bool IsValidSelectedDateFormat(object value)
        {
            PersianDateTimeWPFTools.Windows.Controls.DatePickerFormat datePickerFormat = (PersianDateTimeWPFTools.Windows.Controls.DatePickerFormat)value;
            return datePickerFormat == PersianDateTimeWPFTools.Windows.Controls.DatePickerFormat.Long || datePickerFormat == PersianDateTimeWPFTools.Windows.Controls.DatePickerFormat.Short;
        }
        #endregion
    }
}
