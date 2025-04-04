﻿using System;
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
            TitleElement.SetBackground(_clock, Brushes.Transparent);
            _clock.DisplayTimeChanged += Clock_DisplayTimeChanged;

            _calendar = new PersianCalendar
            {
                BorderThickness = new Thickness(),
                Background = Brushes.Transparent,
                Focusable = false
            };

            _calendar.SetBinding(PersianCalendar.AllowSelectBlackedOutDayProperty,
                new Binding(AllowSelectBlackedOutDayProperty.Name) { Source = this, Mode = BindingMode.TwoWay });

            _calendar.SetBinding(PersianCalendar.CustomCultureProperty,
                new Binding(CustomCultureProperty.Name) { Source = this, Mode = BindingMode.TwoWay });

            _calendar.SetBinding(PersianCalendar.CustomCultureNameProperty,
                new Binding(CustomCultureNameProperty.Name) { Source = this, Mode = BindingMode.TwoWay });

            _calendar.SetBinding(PersianCalendar.ShowTodayButtonProperty,
                new Binding(ShowTodayButtonProperty.Name) { Source = this, Mode = BindingMode.TwoWay });


            TitleElement.SetBackground(_calendar, Brushes.Transparent);
            _calendar.SelectedDatesChanged += Calendar_SelectedDatesChanged;
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
