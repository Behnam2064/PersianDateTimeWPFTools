
using PersianDateTimeWPFTools.Tools;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
#if NET8_0_OR_GREATER
#nullable disable
#endif
namespace PersianDateTimeWPFTools.Windows.Controls.Primitives
{
    public static class FocusVisualHelper
    {
        public static readonly DependencyProperty FocusVisualPrimaryBrushProperty = DependencyProperty.RegisterAttached("FocusVisualPrimaryBrush", typeof(Brush), typeof(FocusVisualHelper));
        public static readonly DependencyProperty FocusVisualSecondaryBrushProperty = DependencyProperty.RegisterAttached("FocusVisualSecondaryBrush", typeof(Brush), typeof(FocusVisualHelper));
        public static readonly DependencyProperty FocusVisualPrimaryThicknessProperty = DependencyProperty.RegisterAttached("FocusVisualPrimaryThickness", typeof(Thickness), typeof(FocusVisualHelper), (PropertyMetadata)new FrameworkPropertyMetadata((object)new Thickness(2.0)));
        public static readonly DependencyProperty FocusVisualSecondaryThicknessProperty = DependencyProperty.RegisterAttached("FocusVisualSecondaryThickness", typeof(Thickness), typeof(FocusVisualHelper), (PropertyMetadata)new FrameworkPropertyMetadata((object)new Thickness(1.0)));
        public static readonly DependencyProperty FocusVisualMarginProperty = DependencyProperty.RegisterAttached("FocusVisualMargin", typeof(Thickness), typeof(FocusVisualHelper), (PropertyMetadata)new FrameworkPropertyMetadata((object)new Thickness()));
        public static readonly DependencyProperty UseSystemFocusVisualsProperty = DependencyProperty.RegisterAttached("UseSystemFocusVisuals", typeof(bool), typeof(FocusVisualHelper), new PropertyMetadata((object)false));
        public static readonly DependencyProperty IsTemplateFocusTargetProperty = DependencyProperty.RegisterAttached("IsTemplateFocusTarget", typeof(bool), typeof(FocusVisualHelper), new PropertyMetadata(new PropertyChangedCallback(FocusVisualHelper.OnIsTemplateFocusTargetChanged)));
        public static readonly DependencyProperty IsSystemFocusVisualProperty = DependencyProperty.RegisterAttached("IsSystemFocusVisual", typeof(bool), typeof(FocusVisualHelper), new PropertyMetadata(new PropertyChangedCallback(FocusVisualHelper.OnIsSystemFocusVisualChanged)));
        private static readonly DependencyPropertyKey ShowFocusVisualPropertyKey = DependencyProperty.RegisterAttachedReadOnly("ShowFocusVisual", typeof(bool), typeof(FocusVisualHelper), new PropertyMetadata(new PropertyChangedCallback(FocusVisualHelper.OnShowFocusVisualChanged)));
        public static readonly DependencyProperty ShowFocusVisualProperty = FocusVisualHelper.ShowFocusVisualPropertyKey.DependencyProperty;
        private static readonly DependencyProperty FocusedElementProperty = DependencyProperty.RegisterAttached("FocusedElement", typeof(FrameworkElement), typeof(FocusVisualHelper));
        private static readonly DependencyProperty TemplateFocusTargetProperty = DependencyProperty.RegisterAttached("TemplateFocusTarget", typeof(FrameworkElement), typeof(FocusVisualHelper));
        private static FocusVisualHelper.FocusVisualAdorner _focusVisualAdornerCache = (FocusVisualHelper.FocusVisualAdorner)null;

        public static Brush GetFocusVisualPrimaryBrush(FrameworkElement element)
        {
            return (Brush)element.GetValue(FocusVisualHelper.FocusVisualPrimaryBrushProperty);
        }

        public static void SetFocusVisualPrimaryBrush(FrameworkElement element, Brush value)
        {
            element.SetValue(FocusVisualHelper.FocusVisualPrimaryBrushProperty, (object)value);
        }

        public static Brush GetFocusVisualSecondaryBrush(FrameworkElement element)
        {
            return (Brush)element.GetValue(FocusVisualHelper.FocusVisualSecondaryBrushProperty);
        }

        public static void SetFocusVisualSecondaryBrush(FrameworkElement element, Brush value)
        {
            element.SetValue(FocusVisualHelper.FocusVisualSecondaryBrushProperty, (object)value);
        }

        public static Thickness GetFocusVisualPrimaryThickness(FrameworkElement element)
        {
            return (Thickness)element.GetValue(FocusVisualHelper.FocusVisualPrimaryThicknessProperty);
        }

        public static void SetFocusVisualPrimaryThickness(FrameworkElement element, Thickness value)
        {
            element.SetValue(FocusVisualHelper.FocusVisualPrimaryThicknessProperty, (object)value);
        }

        public static Thickness GetFocusVisualSecondaryThickness(FrameworkElement element)
        {
            return (Thickness)element.GetValue(FocusVisualHelper.FocusVisualSecondaryThicknessProperty);
        }

        public static void SetFocusVisualSecondaryThickness(FrameworkElement element, Thickness value)
        {
            element.SetValue(FocusVisualHelper.FocusVisualSecondaryThicknessProperty, (object)value);
        }

        public static Thickness GetFocusVisualMargin(FrameworkElement element)
        {
            return (Thickness)element.GetValue(FocusVisualHelper.FocusVisualMarginProperty);
        }

        public static void SetFocusVisualMargin(FrameworkElement element, Thickness value)
        {
            element.SetValue(FocusVisualHelper.FocusVisualMarginProperty, (object)value);
        }

        public static bool GetUseSystemFocusVisuals(Control control)
        {
            return (bool)control.GetValue(FocusVisualHelper.UseSystemFocusVisualsProperty);
        }

        public static void SetUseSystemFocusVisuals(Control control, bool value)
        {
            control.SetValue(FocusVisualHelper.UseSystemFocusVisualsProperty, (object)value);
        }

        public static bool GetIsTemplateFocusTarget(FrameworkElement element)
        {
            return (bool)element.GetValue(FocusVisualHelper.IsTemplateFocusTargetProperty);
        }

        public static void SetIsTemplateFocusTarget(FrameworkElement element, bool value)
        {
            element.SetValue(FocusVisualHelper.IsTemplateFocusTargetProperty, (object)value);
        }

        private static void OnIsTemplateFocusTargetChanged(
          DependencyObject d,
          DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement frameworkElement = (FrameworkElement)d;
            if (!(frameworkElement.TemplatedParent is Control templatedParent))
                return;
            if ((bool)e.NewValue)
                FocusVisualHelper.SetTemplateFocusTarget(templatedParent, frameworkElement);
            else
                templatedParent.ClearValue(FocusVisualHelper.TemplateFocusTargetProperty);
        }

        public static bool GetIsSystemFocusVisual(Control control)
        {
            return (bool)control.GetValue(FocusVisualHelper.IsSystemFocusVisualProperty);
        }

        public static void SetIsSystemFocusVisual(Control control, bool value)
        {
            control.SetValue(FocusVisualHelper.IsSystemFocusVisualProperty, (object)value);
        }

        private static void OnIsSystemFocusVisualChanged(
          DependencyObject d,
          DependencyPropertyChangedEventArgs e)
        {
            Control control = (Control)d;
            if ((bool)e.NewValue)
                control.IsVisibleChanged += new DependencyPropertyChangedEventHandler(FocusVisualHelper.OnFocusVisualIsVisibleChanged);
            else
                control.IsVisibleChanged -= new DependencyPropertyChangedEventHandler(FocusVisualHelper.OnFocusVisualIsVisibleChanged);
        }

        public static bool GetShowFocusVisual(FrameworkElement element)
        {
            return (bool)element.GetValue(FocusVisualHelper.ShowFocusVisualProperty);
        }

        private static void SetShowFocusVisual(FrameworkElement element, bool value)
        {
            element.SetValue(FocusVisualHelper.ShowFocusVisualPropertyKey, (object)value);
        }

        private static void OnShowFocusVisualChanged(
          DependencyObject d,
          DependencyPropertyChangedEventArgs e)
        {
            if (!(d is Control control1))
                return;
            FrameworkElement templateFocusTarget = FocusVisualHelper.GetTemplateFocusTarget(control1);
            if (templateFocusTarget == null)
                return;
            if ((bool)e.NewValue)
            {
                bool flag = true;
                if (templateFocusTarget is Control control2)
                    flag = FocusVisualHelper.GetUseSystemFocusVisuals(control2);
                if (!flag)
                    return;
                ShowFocusVisual(control1, templateFocusTarget);
            }
            else
                HideFocusVisual();


        }
        static void HideFocusVisual()
        {
            if (FocusVisualHelper._focusVisualAdornerCache == null)
                return;
            if (VisualTreeHelper.GetParent((DependencyObject)FocusVisualHelper._focusVisualAdornerCache) is AdornerLayer parent)
                parent.Remove((Adorner)FocusVisualHelper._focusVisualAdornerCache);
            FocusVisualHelper._focusVisualAdornerCache = (FocusVisualHelper.FocusVisualAdorner)null;
        }

        static void ShowFocusVisual(Control control, FrameworkElement target)
        {
            HideFocusVisual();
            AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer((Visual)target);
            if (adornerLayer == null)
                return;
            Style focusVisualStyle = target.FocusVisualStyle;
            if (focusVisualStyle != null && focusVisualStyle.BasedOn == null && focusVisualStyle.Setters.Count == 0)
                focusVisualStyle = target.TryFindResource((object)SystemParameters.FocusVisualStyleKey) as Style;
            if (focusVisualStyle == null)
                return;
            FocusVisualHelper._focusVisualAdornerCache = new FocusVisualHelper.FocusVisualAdorner(control, (UIElement)target, focusVisualStyle);
            adornerLayer.Add((Adorner)FocusVisualHelper._focusVisualAdornerCache);
            control.IsVisibleChanged += new DependencyPropertyChangedEventHandler(OnControlIsVisibleChanged);
        }

        static void OnControlIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            ((UIElement)sender).IsVisibleChanged -= new DependencyPropertyChangedEventHandler(OnControlIsVisibleChanged);
            if (FocusVisualHelper._focusVisualAdornerCache == null || FocusVisualHelper._focusVisualAdornerCache.FocusedElement != sender)
                return;
            HideFocusVisual();
        }
        private static FrameworkElement GetFocusedElement(Control focusVisual)
        {
            return (FrameworkElement)focusVisual.GetValue(FocusVisualHelper.FocusedElementProperty);
        }

        private static void SetFocusedElement(Control focusVisual, FrameworkElement value)
        {
            focusVisual.SetValue(FocusVisualHelper.FocusedElementProperty, (object)value);
        }

        private static FrameworkElement GetTemplateFocusTarget(Control control)
        {
            return (FrameworkElement)control.GetValue(FocusVisualHelper.TemplateFocusTargetProperty);
        }

        private static void SetTemplateFocusTarget(Control control, FrameworkElement value)
        {
            control.SetValue(FocusVisualHelper.TemplateFocusTargetProperty, (object)value);
        }

        private static void OnFocusVisualIsVisibleChanged(
          object sender,
          DependencyPropertyChangedEventArgs e)
        {
            Control control1 = (Control)sender;
            if ((bool)e.NewValue)
            {
                if (!((VisualTreeHelper.GetParent((DependencyObject)control1) is Adorner parent ? parent.AdornedElement : (UIElement)null) is FrameworkElement adornedElement))
                    return;
                FocusVisualHelper.SetShowFocusVisual(adornedElement, true);
                if (adornedElement is Control control2 && (!FocusVisualHelper.GetUseSystemFocusVisuals(control2) || FocusVisualHelper.GetTemplateFocusTarget(control2) != null))
                {
                    control1.Template = (ControlTemplate)null;
                }
                else
                {
                    FocusVisualHelper.TransferValue((DependencyObject)adornedElement, (DependencyObject)control1, FocusVisualHelper.FocusVisualPrimaryBrushProperty);
                    FocusVisualHelper.TransferValue((DependencyObject)adornedElement, (DependencyObject)control1, FocusVisualHelper.FocusVisualPrimaryThicknessProperty);
                    FocusVisualHelper.TransferValue((DependencyObject)adornedElement, (DependencyObject)control1, FocusVisualHelper.FocusVisualSecondaryBrushProperty);
                    FocusVisualHelper.TransferValue((DependencyObject)adornedElement, (DependencyObject)control1, FocusVisualHelper.FocusVisualSecondaryThicknessProperty);
                    control1.Margin = FocusVisualHelper.GetFocusVisualMargin(adornedElement);
                }
                FocusVisualHelper.SetFocusedElement(control1, adornedElement);
            }
            else
            {
                FrameworkElement focusedElement = FocusVisualHelper.GetFocusedElement(control1);
                if (focusedElement == null)
                    return;
                focusedElement.ClearValue(FocusVisualHelper.ShowFocusVisualPropertyKey);
                control1.ClearValue(FocusVisualHelper.FocusVisualPrimaryBrushProperty);
                control1.ClearValue(FocusVisualHelper.FocusVisualPrimaryThicknessProperty);
                control1.ClearValue(FocusVisualHelper.FocusVisualSecondaryBrushProperty);
                control1.ClearValue(FocusVisualHelper.FocusVisualSecondaryThicknessProperty);
                control1.ClearValue(FrameworkElement.MarginProperty);
                control1.ClearValue(Control.TemplateProperty);
                control1.ClearValue(FocusVisualHelper.FocusedElementProperty);
            }
        }

        private static void TransferValue(
          DependencyObject source,
          DependencyObject target,
          DependencyProperty dp)
        {
            if (source.HasDefaultValue(dp))
                return;
            target.SetValue(dp, source.GetValue(dp));
        }

        private sealed class FocusVisualAdorner : Adorner
        {
            private UIElement _adorderChild;

            public FocusVisualAdorner(
              Control focusedElement,
              UIElement adornedElement,
              Style focusVisualStyle)
              : base(adornedElement)
            {
                this.FocusedElement = focusedElement;
                Control control = new Control();
                FocusVisualHelper.SetIsSystemFocusVisual(control, false);
                control.Style = focusVisualStyle;
                control.Margin = FocusVisualHelper.GetFocusVisualMargin((FrameworkElement)focusedElement);
                FocusVisualHelper.TransferValue((DependencyObject)focusedElement, (DependencyObject)control, FocusVisualHelper.FocusVisualPrimaryBrushProperty);
                FocusVisualHelper.TransferValue((DependencyObject)focusedElement, (DependencyObject)control, FocusVisualHelper.FocusVisualPrimaryThicknessProperty);
                FocusVisualHelper.TransferValue((DependencyObject)focusedElement, (DependencyObject)control, FocusVisualHelper.FocusVisualSecondaryBrushProperty);
                FocusVisualHelper.TransferValue((DependencyObject)focusedElement, (DependencyObject)control, FocusVisualHelper.FocusVisualSecondaryThicknessProperty);
                this._adorderChild = (UIElement)control;
                this.IsClipEnabled = true;
                this.IsHitTestVisible = false;
                this.IsEnabled = false;
                this.AddVisualChild((Visual)this._adorderChild);
            }

            public Control FocusedElement { get; }

            protected override Size MeasureOverride(Size constraint)
            {
                Size renderSize = this.AdornedElement.RenderSize;
                ((UIElement)this.GetVisualChild(0)).Measure(renderSize);
                return renderSize;
            }

            protected override Size ArrangeOverride(Size size)
            {
                Size size1 = base.ArrangeOverride(size);
                ((UIElement)this.GetVisualChild(0)).Arrange(new Rect(new System.Windows.Point(), size1));
                return size1;
            }

            protected override int VisualChildrenCount => 1;

            protected override Visual GetVisualChild(int index)
            {
                if (index == 0)
                    return (Visual)this._adorderChild;
                throw new ArgumentOutOfRangeException(nameof(index));
            }
        }
    }
}
