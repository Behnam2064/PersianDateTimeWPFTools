﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows;
using PersianDateTimeWPFTools.Tools;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Security.Policy;
using System.Windows.Threading;

namespace PersianDateTimeWPFTools.Controls
{
    [TemplatePart(Name = ElementButtonAm, Type = typeof(RadioButton))]
    [TemplatePart(Name = ElementButtonPm, Type = typeof(RadioButton))]
    [TemplatePart(Name = ElementCanvas, Type = typeof(Canvas))]
    [TemplatePart(Name = ElementBorderTitle, Type = typeof(Border))]
    [TemplatePart(Name = ElementBorderClock, Type = typeof(Border))]
    [TemplatePart(Name = ElementPanelNum, Type = typeof(ClockPanel))]
    [TemplatePart(Name = ElementTimeStr, Type = typeof(TextBlock))]
    public class Clock : ClockBase
    {
        #region Constants

        private const string ElementButtonAm = "PART_ButtonAm";
        private const string ElementButtonPm = "PART_ButtonPm";
        private const string ElementCanvas = "PART_Canvas";
        private const string ElementBorderTitle = "PART_BorderTitle";
        private const string ElementBorderClock = "PART_BorderClock";
        private const string ElementPanelNum = "PART_PanelNum";
        private const string ElementTimeStr = "PART_TimeStr";

        #endregion Constants

        #region Data

        private RadioButton _buttonAm;

        private RadioButton _buttonPm;

        private Canvas _canvas;

        private Border _borderTitle;

        private Border _borderClock;

        private RadioButton _currentButton;

        private RotateTransform _rotateTransformClock;

        private ClockPanel _circlePanel;

        private List<RadioButton> _radioButtonList;

        private TextBlock _blockTime;

        private int _secValue;

        public Clock()
        {
            InitResources.SetControlStyle(this);
            
        }

        #endregion Data

        #region Public Properties

        public static readonly DependencyProperty ClockRadioButtonStyleProperty = DependencyProperty.Register(
            nameof(ClockRadioButtonStyle), typeof(Style), typeof(Clock), new PropertyMetadata(default(Style)));

        public Style ClockRadioButtonStyle
        {
            get => (Style)GetValue(ClockRadioButtonStyleProperty);
            set => SetValue(ClockRadioButtonStyleProperty, value);
        }


        public static readonly DependencyProperty ClockRadioButtonHourStyleProperty = DependencyProperty.Register(
            nameof(ClockRadioButtonHourStyle), typeof(Style), typeof(Clock), new PropertyMetadata(default(Style)));

        public Style ClockRadioButtonHourStyle
        {
            get => (Style)GetValue(ClockRadioButtonHourStyleProperty);
            set => SetValue(ClockRadioButtonHourStyleProperty, value);
        }

        private int SecValue
        {
            get => _secValue;
            set
            {
                if (value < 0)
                {
                    _secValue = 59;
                }
                else if (value > 59)
                {
                    _secValue = 0;
                }
                else
                {
                    _secValue = value;
                }
            }
        }

        #endregion Public Properties

        #region Public Methods

        public override void OnApplyTemplate()
        {
            AppliedTemplate = false;
            if (_buttonAm != null)
            {
                _buttonAm.Click -= ButtonAm_OnClick;
            }

            if (_buttonPm != null)
            {
                _buttonPm.Click -= ButtonPm_OnClick;
            }


            if (_borderTitle != null)
            {
                _borderTitle.MouseWheel -= BorderTitle_OnMouseWheel;
            }

            if (_canvas != null)
            {
                _canvas.MouseWheel -= Canvas_OnMouseWheel;
                _canvas.RemoveHandler(ButtonBase.ClickEvent, new RoutedEventHandler(Canvas_OnClick));
                _canvas.MouseMove -= Canvas_OnMouseMove;
            }

            base.OnApplyTemplate();

            _buttonAm = GetTemplateChild(ElementButtonAm) as RadioButton;
            _buttonPm = GetTemplateChild(ElementButtonPm) as RadioButton;
            _borderTitle = GetTemplateChild(ElementBorderTitle) as Border;
            _canvas = GetTemplateChild(ElementCanvas) as Canvas;
            _borderClock = GetTemplateChild(ElementBorderClock) as Border;
            _circlePanel = GetTemplateChild(ElementPanelNum) as ClockPanel;
            _blockTime = GetTemplateChild(ElementTimeStr) as TextBlock;

            if (!CheckNull()) return;

            _buttonAm.Click += ButtonAm_OnClick;
            _buttonPm.Click += ButtonPm_OnClick;
            _borderTitle.MouseWheel += BorderTitle_OnMouseWheel;

            _canvas.MouseWheel += Canvas_OnMouseWheel;
            _canvas.AddHandler(ButtonBase.ClickEvent, new RoutedEventHandler(Canvas_OnClick));
            _canvas.MouseMove += Canvas_OnMouseMove;

            _rotateTransformClock = new RotateTransform();
            _borderClock.RenderTransform = _rotateTransformClock;

            _radioButtonList = new List<RadioButton>();

            if (ClockRadioButtonHourStyle == null)
            {
                ClockRadioButtonHourStyle = ClockRadioButtonStyle;
            }


            for (var i = 0; i < 12; i++)
            {
                var num = i + 1;
                var button = new RadioButton
                {
                    //Num = num,
                    Tag = num,
                    Content = num
                };
                //button.SetBinding(StyleProperty, new Binding(ClockRadioButtonStyleProperty.Name) { Source = this });
                button.SetBinding(StyleProperty, new Binding(ClockRadioButtonHourStyleProperty.Name) { Source = this });
                _radioButtonList.Add(button);
                _circlePanel.Children.Add(button);
            }

            AppliedTemplate = true;
            if (SelectedTime.HasValue)
            {
                Update(SelectedTime.Value);
            }
            else
            {
                DisplayTime = DateTime.Now;
                Update(DisplayTime);
            }
        }

        #endregion Public Methods

        #region Private Methods

        private bool CheckNull()
        {
            if (_buttonPm == null || _buttonAm == null || _canvas == null ||
                _borderTitle == null || _borderClock == null || _circlePanel == null ||
                _blockTime == null) return false;

            return true;
        }

        private void BorderTitle_OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta < 0)
            {
                SecValue--;
                Update();
            }
            else
            {
                SecValue++;
                Update();
            }
            e.Handled = true;
        }

        private void Canvas_OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            var value = (int)_rotateTransformClock.Angle;
            if (e.Delta < 0)
            {
                value += 6;
            }
            else
            {
                value -= 6;
            }
            if (value < 0)
            {
                value = value + 360;
            }
            _rotateTransformClock.Angle = value;

            Update();
            e.Handled = true;
        }

        private void Canvas_OnClick(object sender, RoutedEventArgs e)
        {
            _currentButton = e.OriginalSource as RadioButton;
            if (_currentButton != null)
            {
                Update();
                //If the user writes code in the SelectedTimeChangedEvent event before the control is fully loaded and the main thread is busy, the following error will occur.
                //InvalidOperationException: Dispatcher processing has been suspended, but messages are still being processed.
                if (IsLoaded)
                    //If you do not want the SelectedTimeChangedEvent to be triggered when the time is changed
                    //(for example, to confirm the time selection using another button), please remove the following code:
                    SelectedTime = DisplayTime;

            }
        }

        private void Canvas_OnMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var center = new Point(85, 85);
                var p = e.GetPosition(_canvas);

                var value = (Math.Atan2(p.Y - center.Y, p.X - center.X) * 180 / Math.PI) + 90;
                if (value < 0)
                {
                    value = value + 360;
                }
                value = value - value % 6;
                _rotateTransformClock.Angle = value;
                Update();
            }
        }

        private void Update()
        {
            if (!AppliedTemplate) return;
            var hValue = (int)_currentButton.Tag;
            if (_buttonPm.IsChecked == true)
            {
                hValue += 12;
                if (hValue == 24) hValue = 12;
            }
            else if (hValue == 12)
            {
                hValue = 0;
            }
            if (hValue == 12 && _buttonAm.IsChecked == true)
            {
                _buttonPm.IsChecked = true;
                _buttonAm.IsChecked = false;
            }

            if (_blockTime != null)
            {
                DisplayTime = GetDisplayTime();
                _blockTime.Text = DisplayTime.ToString(TimeFormat);
            }
        }

        internal override void Update(DateTime time)
        {
            if (!AppliedTemplate) return;
            var h = time.Hour;
            var m = time.Minute;

            if (h >= 12)
            {
                _buttonPm.IsChecked = true;
                _buttonAm.IsChecked = false;
            }
            else
            {
                _buttonPm.IsChecked = false;
                _buttonAm.IsChecked = true;
            }

            _rotateTransformClock.Angle = m * 6;

            var hRest = h % 12;
            if (hRest == 0) hRest = 12;
            var ctl = _radioButtonList[hRest - 1];
            ctl.IsChecked = true;
            ctl.RaiseEvent(new RoutedEventArgs { RoutedEvent = ButtonBase.ClickEvent });

            _secValue = time.Second;
            Update();
        }

        private DateTime GetDisplayTime()
        {
            var hValue = (int)_currentButton.Tag;
            if (_buttonPm.IsChecked == true)
            {
                hValue += 12;
                if (hValue == 24) hValue = 12;
            }
            else if (hValue == 12)
            {
                hValue = 0;
            }
            var now = DateTime.Now;
            return new DateTime(now.Year, now.Month, now.Day, hValue, (int)Math.Abs(_rotateTransformClock.Angle) % 360 / 6, _secValue);
        }

        private void ButtonAm_OnClick(object sender, RoutedEventArgs e) => Update();

        private void ButtonPm_OnClick(object sender, RoutedEventArgs e) => Update();

        #endregion Private Methods       
    }
}
