using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using PersianDateTimeWPFTools.Windows.Controls;

namespace PersianDateTimeWPFTools.Controls
{
    [TemplatePart(Name = ElementButtonConfirm, Type = typeof(Button))]
    public abstract class ClockBase : Control
    {
        protected const string ElementButtonConfirm = "PART_ButtonConfirm";

        protected Button ButtonConfirm;

        protected bool AppliedTemplate;

        public event Action Confirmed;

        public event EventHandler<ClockTimeChangedEventArgs> DisplayTimeChanged;


        public static readonly RoutedEvent SelectedTimeChangedEvent =
            EventManager.RegisterRoutedEvent("SelectedTimeChanged", RoutingStrategy.Direct,
                 typeof(EventHandler<ClockTimeChangedEventArgs>), typeof(ClockBase));

        public event EventHandler<ClockTimeChangedEventArgs> SelectedTimeChanged
        {
            add => AddHandler(SelectedTimeChangedEvent, value);
            remove => RemoveHandler(SelectedTimeChangedEvent, value);
        }

        public static readonly DependencyProperty TimeFormatProperty = DependencyProperty.Register(
            nameof(TimeFormat), typeof(string), typeof(ClockBase), new PropertyMetadata("HH:mm:ss"));

        public string TimeFormat
        {
            get => (string)GetValue(TimeFormatProperty);
            set => SetValue(TimeFormatProperty, value);
        }

        public static readonly DependencyProperty SelectedTimeProperty = DependencyProperty.Register(
            nameof(SelectedTime), typeof(DateTime?), typeof(ClockBase), new PropertyMetadata(default(DateTime?), OnSelectedTimeChanged));

        private static void OnSelectedTimeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctl = (ClockBase)d;
            var v = (DateTime?)e.NewValue;
            ctl.DisplayTime = v ?? DateTime.Now;
            ctl.OnSelectedTimeChanged(new ClockTimeChangedEventArgs(SelectedTimeChangedEvent, e.OldValue as DateTime?, v));
        }

        public DateTime? SelectedTime
        {
            get => (DateTime?)GetValue(SelectedTimeProperty);
            set => SetValue(SelectedTimeProperty, value);
        }

        public static readonly DependencyProperty DisplayTimeProperty = DependencyProperty.Register(
            nameof(DisplayTime), typeof(DateTime), typeof(ClockBase),
            new FrameworkPropertyMetadata(DateTime.Now, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                OnDisplayTimeChanged));

        private static void OnDisplayTimeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctl = (ClockBase)d;
            var v = (DateTime)e.NewValue;
            ctl.Update(v);
            ctl.OnDisplayTimeChanged(new ClockTimeChangedEventArgs(SelectedTimeChangedEvent, e.OldValue as DateTime?, v));
        }

        public DateTime DisplayTime
        {
            get => (DateTime)GetValue(DisplayTimeProperty);
            set => SetValue(DisplayTimeProperty, value);
        }



        protected virtual void OnSelectedTimeChanged(ClockTimeChangedEventArgs e)
        {
            RaiseEvent(e);
        }

        protected virtual void OnDisplayTimeChanged(ClockTimeChangedEventArgs e)
        {
            if (IsLoaded)
            {
                //If the user writes code in the SelectedTimeChangedEvent event before the control is fully loaded and the main thread is busy, the following error will occur.
                //InvalidOperationException: Dispatcher processing has been suspended, but messages are still being processed.
                var handler = DisplayTimeChanged;
                handler?.Invoke(this, (ClockTimeChangedEventArgs)e);
            }
        }

        protected void ButtonConfirm_OnClick(object sender, RoutedEventArgs e)
        {
            SelectedTime = DisplayTime;
            Confirmed?.Invoke();
        }

        internal abstract void Update(DateTime time);
    }
}
