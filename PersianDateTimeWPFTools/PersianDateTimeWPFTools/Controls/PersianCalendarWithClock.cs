using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows;
using PersianDateTimeWPFTools.Tools;
using System.Globalization;
using PersianDateTimeWPFTools.Windows.Controls;
using System.Windows.Data;
using System.Collections.ObjectModel;
using CalendarDateRange = PersianDateTimeWPFTools.Windows.Controls.CalendarDateRange;
using System.Runtime.CompilerServices;

namespace PersianDateTimeWPFTools.Controls
{
    [TemplatePart(Name = ElementClockPresenter, Type = typeof(ContentPresenter))]
    [TemplatePart(Name = ElementCalendarPresenter, Type = typeof(ContentPresenter))]
    public class PersianCalendarWithClock : Control
    {
        #region Constants

        private const string ElementClockPresenter = "PART_ClockPresenter";

        private const string ElementCalendarPresenter = "PART_CalendarPresenter";

        #endregion Constants

        #region Data

        private ContentPresenter _clockPresenter;

        private ContentPresenter _calendarPresenter;

        private Clock _clock;

        private PersianCalendar _calendar;

        private bool _isLoaded;

        private IDictionary<DependencyProperty, bool> _isHandlerSuspended;

        #endregion Data

        #region Public Events

        public static readonly RoutedEvent SelectedDateTimeChangedEvent =
            EventManager.RegisterRoutedEvent("SelectedDateTimeChanged", RoutingStrategy.Direct,
                typeof(EventHandler<PersianDateTimeWPFTools.Windows.Controls.CalendarDateChangedEventArgs>), typeof(PersianCalendarWithClock));

        public event EventHandler<PersianDateTimeWPFTools.Windows.Controls.CalendarDateChangedEventArgs> SelectedDateTimeChanged
        {
            add => AddHandler(SelectedDateTimeChangedEvent, value);
            remove => RemoveHandler(SelectedDateTimeChangedEvent, value);
        }

        [Obsolete]
        public event EventHandler<PersianDateTimeWPFTools.Windows.Controls.CalendarDateChangedEventArgs> DisplayDateTimeChanged;

        #endregion Public Events

        public ObservableCollection<CalendarDateRange> BlackoutDates
        {
            get => _calendar?.BlackoutDates;
        }

        public static readonly DependencyProperty AllowSelectBlackedOutDayProperty = DependencyProperty.Register(nameof(AllowSelectBlackedOutDay), typeof(bool), typeof(PersianCalendarWithClock), (PropertyMetadata)new FrameworkPropertyMetadata((object)false));
        public static readonly DependencyProperty CustomCultureProperty = DependencyProperty.Register(nameof(CustomCulture), typeof(CultureInfo), typeof(PersianCalendarWithClock), (PropertyMetadata)new FrameworkPropertyMetadata((object)null));
        public static readonly DependencyProperty CustomCultureNameProperty = DependencyProperty.Register(nameof(CustomCultureName), typeof(string), typeof(PersianCalendarWithClock), (PropertyMetadata)new FrameworkPropertyMetadata((object)null));
        public static readonly DependencyProperty ShowTodayButtonProperty = DependencyProperty.Register(nameof(ShowTodayButton), typeof(bool), typeof(PersianCalendarWithClock), (PropertyMetadata)new FrameworkPropertyMetadata(false));
        public static readonly DependencyProperty CalendarStyleProperty
            = DependencyProperty.Register(nameof(CalendarStyle), typeof(Style), typeof(PersianCalendarWithClock));

        public static readonly DependencyProperty ClockStyleProperty
            = DependencyProperty.Register(nameof(ClockStyle), typeof(Style), typeof(PersianCalendarWithClock));


        public static readonly DependencyProperty IsTodayHighlightedProperty =
            DependencyProperty.Register(nameof(IsTodayHighlighted),
                typeof(bool), typeof(PersianCalendarWithClock), (PropertyMetadata)new
                FrameworkPropertyMetadata((object)true));



        public static readonly DependencyProperty DisplayDateProperty = DependencyProperty.Register(nameof(DisplayDate), typeof(DateTime), typeof(PersianCalendarWithClock), (PropertyMetadata)new FrameworkPropertyMetadata((object)DateTime.MinValue, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public static readonly DependencyProperty DisplayDateEndProperty = DependencyProperty.Register(nameof(DisplayDateEnd), typeof(DateTime?), typeof(PersianCalendarWithClock), (PropertyMetadata)new FrameworkPropertyMetadata((object)null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public static readonly DependencyProperty DisplayDateStartProperty = DependencyProperty.Register(nameof(DisplayDateStart), typeof(DateTime?), typeof(PersianCalendarWithClock), (PropertyMetadata)new FrameworkPropertyMetadata((object)null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public static readonly DependencyProperty DisplayModeProperty = DependencyProperty.Register(nameof(DisplayMode), typeof(PersianDateTimeWPFTools.Windows.Controls.CalendarMode), typeof(PersianCalendarWithClock), (PropertyMetadata)new FrameworkPropertyMetadata((object)PersianDateTimeWPFTools.Windows.Controls.CalendarMode.Month, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public static readonly DependencyProperty FirstDayOfWeekProperty = DependencyProperty.Register(nameof(FirstDayOfWeek), typeof(DayOfWeek), typeof(PersianCalendarWithClock), (PropertyMetadata)new FrameworkPropertyMetadata((object)PersianDateTimeWPFTools.Windows.Controls.DateTimeHelper.GetCurrentDateFormat().FirstDayOfWeek));
        public static readonly DependencyProperty SelectionModeProperty = DependencyProperty.Register(nameof(SelectionMode), typeof(PersianDateTimeWPFTools.Windows.Controls.CalendarSelectionMode), typeof(PersianCalendarWithClock), (PropertyMetadata)new FrameworkPropertyMetadata((object)PersianDateTimeWPFTools.Windows.Controls.CalendarSelectionMode.SingleDate));


        public DateTime DisplayDate
        {
            get => (DateTime)this.GetValue(PersianCalendarWithClock.DisplayDateProperty);
            set => this.SetValue(PersianCalendarWithClock.DisplayDateProperty, (object)value);
        }


        public DateTime? DisplayDateEnd
        {
            get => (DateTime?)this.GetValue(PersianCalendarWithClock.DisplayDateEndProperty);
            set => this.SetValue(PersianCalendarWithClock.DisplayDateEndProperty, (object)value);
        }

        public DateTime? DisplayDateStart
        {
            get => (DateTime?)this.GetValue(PersianCalendarWithClock.DisplayDateStartProperty);
            set => this.SetValue(PersianCalendarWithClock.DisplayDateStartProperty, (object)value);
        }

        public PersianDateTimeWPFTools.Windows.Controls.CalendarMode DisplayMode
        {
            get => (PersianDateTimeWPFTools.Windows.Controls.CalendarMode)this.GetValue(PersianCalendarWithClock.DisplayModeProperty);
            set => this.SetValue(PersianCalendarWithClock.DisplayModeProperty, (object)value);
        }


        public DayOfWeek FirstDayOfWeek
        {
            get => (DayOfWeek)this.GetValue(PersianCalendarWithClock.FirstDayOfWeekProperty);
            set => this.SetValue(PersianCalendarWithClock.FirstDayOfWeekProperty, (object)value);
        }




        public bool IsTodayHighlighted
        {
            get => (bool)this.GetValue(PersianCalendar.IsTodayHighlightedProperty);
            set => this.SetValue(PersianCalendar.IsTodayHighlightedProperty, (object)value);
        }

        public Style ClockStyle
        {
            get => (Style)this.GetValue(ClockStyleProperty);
            set => this.SetValue(ClockStyleProperty, (object)value);
        }

        public Style CalendarStyle
        {
            get => (Style)this.GetValue(CalendarStyleProperty);
            set => this.SetValue(CalendarStyleProperty, (object)value);
        }


        public bool AllowSelectBlackedOutDay
        {
            get => (bool)this.GetValue(AllowSelectBlackedOutDayProperty);
            set => this.SetValue(AllowSelectBlackedOutDayProperty, (object)value);
        }

        public string CustomCultureName
        {
            get => (string)this.GetValue(CustomCultureNameProperty);
            set => this.SetValue(CustomCultureNameProperty, (object)value);
        }

        public bool ShowTodayButton
        {
            get => (bool)this.GetValue(ShowTodayButtonProperty);
            set => this.SetValue(ShowTodayButtonProperty, value);
        }

        public CultureInfo CustomCulture
        {
            get => this.GetValue(CustomCultureProperty) as CultureInfo;

            set => this.SetValue(CustomCultureProperty, (object)value);
        }


        public PersianCalendarWithClock()
        {
            InitCalendarAndClock();
            Loaded += (s, e) =>
            {
                if (_isLoaded) return;
                _isLoaded = true;
                DisplayDateTime = SelectedDateTime ?? DateTime.Now;
            };
        }

        #region Public Properties
        [Obsolete("This feature will be removed soon and is currently of no use.")]
        public static readonly DependencyProperty DateTimeFormatProperty = DependencyProperty.Register(
            "DateTimeFormat", typeof(string), typeof(PersianCalendarWithClock), new PropertyMetadata("yyyy-MM-dd HH:mm:ss"));

        [Obsolete("This feature will be removed soon and is currently of no use.")]
        public string DateTimeFormat
        {
            get => (string)GetValue(DateTimeFormatProperty);
            set => SetValue(DateTimeFormatProperty, value);
        }

        public static readonly DependencyProperty SelectedDateTimeProperty = DependencyProperty.Register(
            "SelectedDateTime", typeof(DateTime?), typeof(PersianCalendarWithClock), new PropertyMetadata(default(DateTime?), OnSelectedDateTimeChanged));

        private static void OnSelectedDateTimeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            /*            var ctl = (PersianCalendarWithClock)d;
                        var v = (DateTime?)e.NewValue;

                        ctl.OnSelectedDateTimeChanged(new PersianDateTimeWPFTools.Windows.Controls.CalendarDateChangedEventArgs
                            (SelectedDateTimeChangedEvent, v, e.OldValue as DateTime?));*/
        }

        public DateTime? SelectedDateTime
        {
            get => (DateTime?)GetValue(SelectedDateTimeProperty);
            set => SetValue(SelectedDateTimeProperty, value);
        }
        [Obsolete]
        public static readonly DependencyProperty DisplayDateTimeProperty = DependencyProperty.Register(
            "DisplayDateTime", typeof(DateTime), typeof(PersianCalendarWithClock), new FrameworkPropertyMetadata(DateTime.MinValue, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnDisplayDateTimeChanged));

        private static void OnDisplayDateTimeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            /*var ctl = (PersianCalendarWithClock)d;
            if (ctl.IsHandlerSuspended(DisplayDateTimeProperty))
                return;
            var v = (DateTime)e.NewValue;
            ctl._clock.SelectedTime = v;
            ctl._calendar.SelectedDate = v;
            ctl._calendar.DisplayDate = v;
            ctl.OnDisplayDateTimeChanged(new PersianDateTimeWPFTools.Windows.Controls.CalendarDateChangedEventArgs(e.OldValue as DateTime?, v));*/
        }
        [Obsolete]
        public DateTime DisplayDateTime
        {
            get => (DateTime)GetValue(DisplayDateTimeProperty);
            set => SetValue(DisplayDateTimeProperty, value);
        }

        #endregion

        #region Public Methods

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();


            _clockPresenter = GetTemplateChild(ElementClockPresenter) as ContentPresenter;
            _calendarPresenter = GetTemplateChild(ElementCalendarPresenter) as ContentPresenter;

            CheckNull();

            _clockPresenter.Content = _clock;
            _calendarPresenter.Content = _calendar;

        }

        #endregion

        #region Protected Methods

        protected virtual void OnSelectedDateTimeChanged(PersianDateTimeWPFTools.Windows.Controls.CalendarDateChangedEventArgs e) => RaiseEvent(e);

        protected virtual void OnDisplayDateTimeChanged(PersianDateTimeWPFTools.Windows.Controls.CalendarDateChangedEventArgs e)
        {
            var handler = DisplayDateTimeChanged;
            handler?.Invoke(this, e);
        }

        #endregion Protected Methods

        #region Private Methods

        private void SetIsHandlerSuspended(DependencyProperty property, bool value)
        {
            if (value)
            {
                if (_isHandlerSuspended == null)
                {
                    _isHandlerSuspended = new Dictionary<DependencyProperty, bool>(2);
                }

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

        private bool IsHandlerSuspended(DependencyProperty property)
        {
            return _isHandlerSuspended != null && _isHandlerSuspended.ContainsKey(property);
        }

        private void CheckNull()
        {
            if (_clockPresenter == null || _calendarPresenter == null) throw new Exception();
        }


        private void InitCalendarAndClock()
        {
            _clock = new Clock
            {
                BorderThickness = new Thickness(),
                Background = Brushes.Transparent
            };

            _clock.SetBinding(FrameworkElement.StyleProperty, this.GetDatePickerBinding(ClockStyleProperty));
            this.ClockStyle = (Style)Application.Current.Resources["ClockBaseStyle"];

            TitleElement.SetBackground(_clock, Brushes.Transparent);
            _clock.DisplayTimeChanged += Clock_DisplayTimeChanged;

            _calendar = new PersianCalendar
            {
                BorderThickness = new Thickness(),
                Background = Brushes.Transparent,
                Focusable = false,
            };


            _calendar.SetBinding(PersianCalendar.FirstDayOfWeekProperty,
                new Binding(FirstDayOfWeekProperty.Name) { Source = this, Mode = BindingMode.TwoWay });

            _calendar.SetBinding(PersianCalendar.DisplayModeProperty,
                new Binding(DisplayModeProperty.Name) { Source = this, Mode = BindingMode.TwoWay });
            
            _calendar.SetBinding(PersianCalendar.DisplayDateStartProperty,
                new Binding(DisplayDateStartProperty.Name) { Source = this, Mode = BindingMode.TwoWay });

            _calendar.SetBinding(PersianCalendar.DisplayDateEndProperty,
                new Binding(DisplayDateEndProperty.Name) { Source = this, Mode = BindingMode.TwoWay });


            _calendar.SetBinding(PersianCalendar.DisplayDateProperty,
                new Binding(DisplayDateProperty.Name) { Source = this, Mode = BindingMode.TwoWay });


            _calendar.SetBinding(PersianCalendar.StyleProperty,
                new Binding(CalendarStyleProperty.Name) { Source = this, Mode = BindingMode.TwoWay });


            _calendar.SetBinding(PersianCalendar.IsTodayHighlightedProperty,
                new Binding(IsTodayHighlightedProperty.Name) { Source = this, Mode = BindingMode.TwoWay });


            this.CalendarStyle = (Style)Application.Current.Resources["DefaultPersianCalendarStyle"];

            _calendar.SetBinding(PersianCalendar.AllowSelectBlackedOutDayProperty,
                new Binding(AllowSelectBlackedOutDayProperty.Name) { Source = this, Mode = BindingMode.TwoWay });

            _calendar.SetBinding(PersianCalendar.CustomCultureProperty,
                new Binding(CustomCultureProperty.Name) { Source = this, Mode = BindingMode.TwoWay });

            _calendar.SetBinding(PersianCalendar.CustomCultureNameProperty,
                new Binding(CustomCultureNameProperty.Name) { Source = this, Mode = BindingMode.TwoWay });

            _calendar.SetBinding(PersianCalendar.ShowTodayButtonProperty,
                new Binding(ShowTodayButtonProperty.Name) { Source = this, Mode = BindingMode.TwoWay });

            DisplayDate = DateTime.Now;//If there is no current code and you select the year or decade button, you will encounter an error.
            TitleElement.SetBackground(_calendar, Brushes.Transparent);
            _calendar.SelectedDatesChanged += Calendar_SelectedDatesChanged;
        }

        private BindingBase GetDatePickerBinding(DependencyProperty property)
        {
            return (BindingBase)new Binding(property.Name)
            {
                Source = (object)this,
                Mode = BindingMode.TwoWay,
            };
        }

        private void Calendar_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            Mouse.Capture(null);
            UpdateDisplayTime();
        }

        private void Clock_DisplayTimeChanged(object sender, PersianDateTimeWPFTools.Windows.Controls.ClockTimeChangedEventArgs e)
        {
            UpdateDisplayTime();
        }

        private void UpdateDisplayTime()
        {
            if (_calendar.SelectedDate != null)
            {
                var date = _calendar.SelectedDate.Value;
                var time = _clock.DisplayTime;

                var result = new DateTime(date.Year, date.Month, date.Day, time.Hour, time.Minute, time.Second);
                SetValueNoCallback(DisplayDateTimeProperty, result);
                SelectedDateTime = result;

                OnSelectedDateTimeChanged(new PersianDateTimeWPFTools.Windows.Controls.CalendarDateChangedEventArgs
                (SelectedDateTimeChangedEvent, null, result));

                OnDisplayDateTimeChanged(new PersianDateTimeWPFTools.Windows.Controls.CalendarDateChangedEventArgs(null, result));
            }
        }

        #endregion
    }

}
