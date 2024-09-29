
using PersianDateTimeWPFTools.Tools;
using PersianDateTimeWPFTools.Windows.Controls.Primitives;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
#if NET8_0_OR_GREATER
#nullable disable
#endif
namespace PersianDateTimeWPFTools.Windows.Controls.Primitives
{
  public class ThemeShadowChrome : Decorator
  {
    public static readonly DependencyProperty IsShadowEnabledProperty = DependencyProperty.Register(nameof (IsShadowEnabled), typeof (bool), typeof (ThemeShadowChrome), new PropertyMetadata((object) true, new PropertyChangedCallback(ThemeShadowChrome.OnIsShadowEnabledChanged)));
    public static readonly DependencyProperty DepthProperty = DependencyProperty.Register(nameof (Depth), typeof (double), typeof (ThemeShadowChrome), new PropertyMetadata((object) 32.0, new PropertyChangedCallback(ThemeShadowChrome.OnDepthChanged)));
    public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register(nameof (CornerRadius), typeof (CornerRadius), typeof (ThemeShadowChrome), new PropertyMetadata((object) new CornerRadius(), new PropertyChangedCallback(ThemeShadowChrome.OnCornerRadiusChanged)));
    private static readonly DependencyProperty PopupMarginProperty = DependencyProperty.Register(nameof (PopupMargin), typeof (Thickness), typeof (ThemeShadowChrome), new PropertyMetadata((object) new Thickness(), new PropertyChangedCallback(ThemeShadowChrome.OnPopupMarginChanged)));
    private readonly Grid _background;
    private readonly BitmapCache _bitmapCache;
    private Border _shadow1;
    private Border _shadow2;
    private ThemeShadowChrome.PopupControl _parentPopupControl;
    private TranslateTransform _transform;
    private PopupPositioner _popupPositioner;
    private static readonly Brush s_bg1;
    private static readonly Brush s_bg2;
    private static readonly Brush s_bg3;
    private static readonly Brush s_bg4;
    private static readonly Vector s_noTranslation = new Vector(0.0, 0.0);

    static ThemeShadowChrome()
    {
      SolidColorBrush solidColorBrush1 = new SolidColorBrush(Colors.Black);
      solidColorBrush1.Opacity = 0.11;
      ThemeShadowChrome.s_bg1 = (Brush) solidColorBrush1;
      SolidColorBrush solidColorBrush2 = new SolidColorBrush(Colors.Black);
      solidColorBrush2.Opacity = 0.13;
      ThemeShadowChrome.s_bg2 = (Brush) solidColorBrush2;
      SolidColorBrush solidColorBrush3 = new SolidColorBrush(Colors.Black);
      solidColorBrush3.Opacity = 0.18;
      ThemeShadowChrome.s_bg3 = (Brush) solidColorBrush3;
      SolidColorBrush solidColorBrush4 = new SolidColorBrush(Colors.Black);
      solidColorBrush4.Opacity = 0.22;
      ThemeShadowChrome.s_bg4 = (Brush) solidColorBrush4;
      ThemeShadowChrome.s_bg1.Freeze();
      ThemeShadowChrome.s_bg2.Freeze();
      ThemeShadowChrome.s_bg3.Freeze();
      ThemeShadowChrome.s_bg4.Freeze();
    }

    public ThemeShadowChrome()
    {
      this._bitmapCache = new BitmapCache();
      Grid grid = new Grid();
      grid.CacheMode = (CacheMode) this._bitmapCache;
      grid.Focusable = false;
      grid.IsHitTestVisible = false;
      grid.SnapsToDevicePixels = false;
      this._background = grid;
      this.AddVisualChild((Visual) this._background);
      this.SizeChanged += new SizeChangedEventHandler(this.OnSizeChanged);
      this.Loaded += new RoutedEventHandler(this.OnLoaded);
    }

    public bool IsShadowEnabled
    {
      get => (bool) this.GetValue(ThemeShadowChrome.IsShadowEnabledProperty);
      set => this.SetValue(ThemeShadowChrome.IsShadowEnabledProperty, (object) value);
    }

    private static void OnIsShadowEnabledChanged(
      DependencyObject d,
      DependencyPropertyChangedEventArgs e)
    {
      ((ThemeShadowChrome) d).OnIsShadowEnabledChanged();
    }

    private void OnIsShadowEnabledChanged()
    {
      if (!this.IsInitialized)
        return;
      if (this.IsShadowEnabled)
      {
        this.EnsureShadows();
        this._background.Children.Add((UIElement) this._shadow1);
        this._background.Children.Add((UIElement) this._shadow2);
        this._background.Visibility = Visibility.Visible;
      }
      else
      {
        this._background.Children.Clear();
        this._background.Visibility = Visibility.Collapsed;
      }
      this.OnVisualParentChanged();
      this.UpdatePopupMargin();
    }

    public double Depth
    {
      get => (double) this.GetValue(ThemeShadowChrome.DepthProperty);
      set => this.SetValue(ThemeShadowChrome.DepthProperty, (object) value);
    }

    private static void OnDepthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((ThemeShadowChrome) d).OnDepthChanged();
    }

    private void OnDepthChanged()
    {
      if (!this.IsInitialized)
        return;
      this.UpdateShadow1();
      this.UpdateShadow2();
      this.UpdatePopupMargin();
    }

    public CornerRadius CornerRadius
    {
      get => (CornerRadius) this.GetValue(ThemeShadowChrome.CornerRadiusProperty);
      set => this.SetValue(ThemeShadowChrome.CornerRadiusProperty, (object) value);
    }

    private static void OnCornerRadiusChanged(
      DependencyObject d,
      DependencyPropertyChangedEventArgs e)
    {
      ((ThemeShadowChrome) d).OnCornerRadiusChanged(e);
    }

    private void OnCornerRadiusChanged(DependencyPropertyChangedEventArgs e)
    {
      CornerRadius newValue = (CornerRadius) e.NewValue;
      if (this._shadow1 != null)
        this._shadow1.CornerRadius = newValue;
      if (this._shadow2 == null)
        return;
      this._shadow2.CornerRadius = newValue;
    }

    private Thickness PopupMargin
    {
      get => (Thickness) this.GetValue(ThemeShadowChrome.PopupMarginProperty);
      set => this.SetValue(ThemeShadowChrome.PopupMarginProperty, (object) value);
    }

    private static void OnPopupMarginChanged(
      DependencyObject d,
      DependencyPropertyChangedEventArgs e)
    {
      ((ThemeShadowChrome) d).OnPopupMarginChanged(e);
    }

    private void OnPopupMarginChanged(DependencyPropertyChangedEventArgs e)
    {
      this.ApplyPopupMargin();
    }

    private void UpdatePopupMargin()
    {
      if (this.IsShadowEnabled)
      {
        double depth = this.Depth;
        double num1 = 0.9 * depth;
        double num2 = 0.4 * depth;
        this.PopupMargin = new Thickness(num1, num1, num1, num1 + num2);
      }
      else
        this.ClearValue(ThemeShadowChrome.PopupMarginProperty);
    }

    private void ApplyPopupMargin()
    {
      if (this._parentPopupControl == null)
        return;
      if (this.ReadLocalValue(ThemeShadowChrome.PopupMarginProperty) == DependencyProperty.UnsetValue)
        this._parentPopupControl.ClearMargin();
      else
        this._parentPopupControl.SetMargin(this.PopupMargin);
    }

    protected override int VisualChildrenCount
    {
      get
      {
        if (!this.IsShadowEnabled)
          return base.VisualChildrenCount;
        return this.Child != null ? 2 : 1;
      }
    }

    protected override void OnVisualParentChanged(DependencyObject oldParent)
    {
      base.OnVisualParentChanged(oldParent);
      if (!this.IsInitialized)
        return;
      this.OnVisualParentChanged();
    }

    protected override Visual GetVisualChild(int index)
    {
      if (!this.IsShadowEnabled)
        return base.GetVisualChild(index);
      if (index == 0)
        return (Visual) this._background;
      return index == 1 && this.Child != null ? (Visual) this.Child : throw new ArgumentOutOfRangeException(nameof (index));
    }

    protected override void OnInitialized(EventArgs e)
    {
      base.OnInitialized(e);
      this.OnIsShadowEnabledChanged();
    }

    protected override Size MeasureOverride(Size constraint)
    {
      if (this.IsShadowEnabled)
        this._background.Measure(constraint);
      return base.MeasureOverride(constraint);
    }

    protected override Size ArrangeOverride(Size arrangeSize)
    {
      if (this.IsShadowEnabled)
        this._background.Arrange(new Rect(arrangeSize));
      return base.ArrangeOverride(arrangeSize);
    }

    private void OnVisualParentChanged()
    {
      if (this.IsShadowEnabled)
      {
        ThemeShadowChrome.PopupControl popupControl = (ThemeShadowChrome.PopupControl) null;
        switch (this.VisualParent)
        {
          case ContextMenu contextMenu:
            popupControl = new ThemeShadowChrome.PopupControl(contextMenu);
            break;
          case System.Windows.Controls.ToolTip toolTip:
            popupControl = new ThemeShadowChrome.PopupControl(toolTip);
            break;
          default:
            Popup parentPopup = this.FindParentPopup((FrameworkElement) this);
            if (parentPopup != null)
            {
              popupControl = new ThemeShadowChrome.PopupControl(parentPopup);
              break;
            }
            break;
        }
        this.SetParentPopupControl(popupControl);
      }
      else
        this.SetParentPopupControl((ThemeShadowChrome.PopupControl) null);
    }

    private void EnsureShadows()
    {
      if (this._shadow1 == null)
      {
        this._shadow1 = this.CreateShadowElement();
        this.UpdateShadow1();
      }
      if (this._shadow2 != null)
        return;
      this._shadow2 = this.CreateShadowElement();
      this.UpdateShadow2();
    }

    private Border CreateShadowElement()
    {
      Border shadowElement = new Border();
      shadowElement.CornerRadius = this.CornerRadius;
      shadowElement.Effect = (Effect) new BlurEffect();
      shadowElement.RenderTransform = (Transform) new TranslateTransform();
      return shadowElement;
    }

    private void UpdateShadow1()
    {
      if (this._shadow1 == null)
        return;
      double depth = this.Depth;
      ((BlurEffect) this._shadow1.Effect).Radius = 0.9 * depth;
      ((TranslateTransform) this._shadow1.RenderTransform).Y = 0.4 * depth;
      this._shadow1.Background = depth >= 32.0 ? ThemeShadowChrome.s_bg4 : ThemeShadowChrome.s_bg2;
    }

    private void UpdateShadow2()
    {
      if (this._shadow2 == null)
        return;
      double depth = this.Depth;
      ((BlurEffect) this._shadow2.Effect).Radius = 0.225 * depth;
      ((TranslateTransform) this._shadow2.RenderTransform).Y = 0.075 * depth;
      this._shadow2.Background = depth >= 32.0 ? ThemeShadowChrome.s_bg3 : ThemeShadowChrome.s_bg1;
    }

    private void OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
      this.ClearMarginAdjustment();
      this.UpdateLayout();
      this.AdjustMargin();
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
      if (!this.IsVisible)
        return;
      this.AdjustMargin();
    }

    private void AdjustMargin()
    {
      if (this._parentPopupControl == null)
        return;
      Thickness margin = this.Margin;
      if (!(margin != new Thickness()) || !(this.VisualParent is UIElement visualParent))
        return;
      double width = visualParent.RenderSize.Width;
      double actualWidth = this.ActualWidth;
      if (width <= 0.0 || actualWidth <= 0.0 || width >= actualWidth + margin.Left + margin.Right)
        return;
      double num = (width - actualWidth) / 2.0;
      ThicknessAnimation animation = new ThicknessAnimation(new Thickness(num, margin.Top, num, margin.Bottom), (Duration) TimeSpan.Zero);
      this.BeginAnimation(FrameworkElement.MarginProperty, (AnimationTimeline) animation);
      this.UpdateLayout();
    }

    private void ClearMarginAdjustment()
    {
      this.BeginAnimation(FrameworkElement.MarginProperty, (AnimationTimeline) null);
    }

    private void SetParentPopupControl(ThemeShadowChrome.PopupControl value)
    {
      if (this._parentPopupControl == value)
        return;
      if (this._popupPositioner != null)
      {
        this._popupPositioner.Dispose();
        this._popupPositioner = (PopupPositioner) null;
      }
      if (this._parentPopupControl != null)
      {
        this._parentPopupControl.Opened -= new EventHandler(this.OnParentPopupControlOpened);
        this._parentPopupControl.Closed -= new EventHandler(this.OnParentPopupControlClosed);
        this._parentPopupControl.ClearMargin();
        this._parentPopupControl.Dispose();
      }
      this._parentPopupControl = value;
      if (this._parentPopupControl == null)
        return;
      this._parentPopupControl.Opened += new EventHandler(this.OnParentPopupControlOpened);
      this._parentPopupControl.Closed += new EventHandler(this.OnParentPopupControlClosed);
      this.ApplyPopupMargin();
    }

    private void OnParentPopupControlOpened(object sender, EventArgs e)
    {
      if (this._popupPositioner != null)
        return;
      if (this._parentPopupControl != null)
      {
        FrameworkElement control = this._parentPopupControl.Control;
        switch (control)
        {
          case null:
            break;
          case System.Windows.Controls.ToolTip toolTip when toolTip.PlacementTarget is Thumb placementTarget && placementTarget.TemplatedParent is Slider:
            return;
          case Popup parent:
//label_6:
            Popup popup = parent;
            if (popup != null && PopupPositioner.IsSupported)
            {
              this._popupPositioner = new PopupPositioner(popup);
              break;
            }
            break;
          /*default:
            parent = control.Parent as Popup;
            goto label_6;*/
        }
      }
      if (this._popupPositioner != null)
        return;
      this.PositionParentPopupControl();
    }

    private void OnParentPopupControlClosed(object sender, EventArgs e)
    {
      this.ClearMarginAdjustment();
      this.ResetTransform();
    }

    private void PositionParentPopupControl()
    {
      ThemeShadowChrome.PopupControl popup = this._parentPopupControl;
      if (popup == null)
        return;
      CustomPlacementMode? nullable1 = new CustomPlacementMode?();
      switch (popup.Placement)
      {
        case PlacementMode.Bottom:
          nullable1 = new CustomPlacementMode?(CustomPlacementMode.BottomEdgeAlignedLeft);
          break;
        case PlacementMode.Top:
          nullable1 = new CustomPlacementMode?(CustomPlacementMode.TopEdgeAlignedLeft);
          break;
        case PlacementMode.Custom:
          CustomPlacementMode placement;
          if (this.TryGetCustomPlacementMode(out placement))
          {
            nullable1 = new CustomPlacementMode?(placement);
            break;
          }
          break;
      }
      if (!nullable1.HasValue || this.EnsureEdgesAligned(nullable1.Value))
        return;
      CustomPlacementMode? nullable2 = nullable1;
      CustomPlacementMode customPlacementMode1 = CustomPlacementMode.BottomEdgeAlignedLeft;
      if (nullable2.GetValueOrDefault() == customPlacementMode1 & nullable2.HasValue)
      {
        if (!shouldAlignRightEdges())
          return;
        this.EnsureEdgesAligned(CustomPlacementMode.BottomEdgeAlignedRight);
      }
      else
      {
        CustomPlacementMode? nullable3 = nullable1;
        CustomPlacementMode customPlacementMode2 = CustomPlacementMode.TopEdgeAlignedLeft;
        if (!(nullable3.GetValueOrDefault() == customPlacementMode2 & nullable3.HasValue) || !shouldAlignRightEdges())
          return;
        this.EnsureEdgesAligned(CustomPlacementMode.TopEdgeAlignedRight);
      }

      bool shouldAlignRightEdges()
      {
        UIElement placementTarget = popup.PlacementTarget;
        if (placementTarget != null)
        {
          double width = placementTarget.RenderSize.Width;
          Vector offset;
          if (this.ActualWidth > 0.0 && width > 0.0 && (this.ActualWidth == width || this.ActualWidth > width && this.TryGetOffsetToTarget(InterestPoint.TopRight, InterestPoint.TopRight, out offset) && offset.X < 0.0))
            return true;
        }
        return false;
      }
    }

    private bool TryGetCustomPlacementMode(out CustomPlacementMode placement)
    {
      return this.TryGetCustomPlacementMode((DependencyObject) this._parentPopupControl?.Control, out placement) || this.TryGetCustomPlacementMode(this.VisualParent, out placement);
    }

    private bool TryGetCustomPlacementMode(
      DependencyObject element,
      out CustomPlacementMode placement)
    {
      if (element != null && element.ReadLocalValue(CustomPopupPlacementHelper.PlacementProperty) != DependencyProperty.UnsetValue)
      {
        placement = CustomPopupPlacementHelper.GetPlacement(element);
        return true;
      }
      placement = CustomPlacementMode.Top;
      return false;
    }

    private bool TryGetOffsetToTarget(
      InterestPoint targetInterestPoint,
      InterestPoint childInterestPoint,
      out Vector offset)
    {
      ThemeShadowChrome.PopupControl parentPopupControl = this._parentPopupControl;
      if (parentPopupControl != null)
      {
        UIElement placementTarget = parentPopupControl.PlacementTarget;
        if (placementTarget != null && this.IsVisible && placementTarget.IsVisible)
        {
          offset = Helper.GetOffset((UIElement) this, childInterestPoint, placementTarget, targetInterestPoint, parentPopupControl.PlacementRectangle);
          if (Math.Abs(offset.X) < 0.5)
            offset.X = 0.0;
          if (Math.Abs(offset.Y) < 0.5)
            offset.Y = 0.0;
          return true;
        }
      }
      offset = new Vector();
      return false;
    }

    private bool EnsureEdgesAligned(CustomPlacementMode placement)
    {
      Vector translation = ThemeShadowChrome.s_noTranslation;
      switch (placement)
      {
        case CustomPlacementMode.TopEdgeAlignedLeft:
          Vector offset1;
          if (this.TryGetOffsetToTarget(InterestPoint.TopLeft, InterestPoint.BottomLeft, out offset1))
          {
            translation = getTranslation(true, true, offset1);
            break;
          }
          break;
        case CustomPlacementMode.TopEdgeAlignedRight:
          Vector offset2;
          if (this.TryGetOffsetToTarget(InterestPoint.TopRight, InterestPoint.BottomRight, out offset2))
          {
            translation = getTranslation(true, false, offset2);
            break;
          }
          break;
        case CustomPlacementMode.BottomEdgeAlignedLeft:
          Vector offset3;
          if (this.TryGetOffsetToTarget(InterestPoint.BottomLeft, InterestPoint.TopLeft, out offset3))
          {
            translation = getTranslation(false, true, offset3);
            break;
          }
          break;
        case CustomPlacementMode.BottomEdgeAlignedRight:
          Vector offset4;
          if (this.TryGetOffsetToTarget(InterestPoint.BottomRight, InterestPoint.TopRight, out offset4))
          {
            translation = getTranslation(false, false, offset4);
            break;
          }
          break;
      }
      if (translation != ThemeShadowChrome.s_noTranslation)
      {
        this.SetupTransform(translation);
        return true;
      }
      this.ResetTransform();
      return false;

      Vector getTranslation(bool top, bool left, Vector offset)
      {
        double x = 0.0;
        double y1 = 0.0;
        if (left && offset.X > 0.0 || !left && offset.X < 0.0 || Math.Abs(offset.X) < 0.5)
          x = -offset.X;
        Thickness popupMargin;
        if (top)
        {
          double y2 = offset.Y;
          popupMargin = this.PopupMargin;
          double top1 = popupMargin.Top;
          if (y2 < top1)
            goto label_7;
        }
        if (!top)
        {
          double y3 = offset.Y;
          popupMargin = this.PopupMargin;
          double num = -popupMargin.Bottom;
          if (y3 > num)
            goto label_7;
        }
        if (Math.Abs(offset.Y) >= 0.5)
          goto label_8;
label_7:
        y1 = -offset.Y;
label_8:
        return new Vector(x, y1);
      }
    }

    private void SetupTransform(Vector translation)
    {
      if (this._transform == null)
      {
        this._transform = new TranslateTransform();
        this.RenderTransform = (Transform) this._transform;
      }
      this._transform.X = translation.X;
      this._transform.Y = translation.Y;
    }

    private void ResetTransform()
    {
      if (this._transform == null)
        return;
      this._transform.ClearValue(TranslateTransform.XProperty);
      this._transform.ClearValue(TranslateTransform.YProperty);
    }

    private Popup FindParentPopup(FrameworkElement element)
    {
      DependencyObject parent1 = element.Parent;
      if (parent1 is Popup parentPopup)
        return parentPopup;
      if (parent1 is FrameworkElement element1)
        return this.FindParentPopup(element1);
      return VisualTreeHelper.GetParent((DependencyObject) element) is FrameworkElement parent2 ? this.FindParentPopup(parent2) : (Popup) null;
    }

    private class PopupControl : IDisposable
    {
      private ContextMenu _contextMenu;
      private System.Windows.Controls.ToolTip _toolTip;
      private Popup _popup;

      public PopupControl(ContextMenu contextMenu)
      {
        this._contextMenu = contextMenu;
        this._contextMenu.Opened += new RoutedEventHandler(this.OnOpened);
        this._contextMenu.Closed += new RoutedEventHandler(this.OnClosed);
      }

      public PopupControl(System.Windows.Controls.ToolTip toolTip)
      {
        this._toolTip = toolTip;
        this._toolTip.Opened += new RoutedEventHandler(this.OnOpened);
        this._toolTip.Closed += new RoutedEventHandler(this.OnClosed);
      }

      public PopupControl(Popup popup)
      {
        this._popup = popup;
        this._popup.Opened += new EventHandler(this.OnOpened);
        this._popup.Closed += new EventHandler(this.OnClosed);
      }

      public FrameworkElement Control
      {
        get
        {
          return (FrameworkElement) this._contextMenu ?? (FrameworkElement) this._toolTip ?? (FrameworkElement) this._popup;
        }
      }

      public PlacementMode Placement
      {
        get
        {
          if (this._contextMenu != null)
            return this._contextMenu.Placement;
          if (this._toolTip != null)
            return this._toolTip.Placement;
          return this._popup != null ? this._popup.Placement : PlacementMode.Absolute;
        }
      }

      public UIElement PlacementTarget
      {
        get
        {
          if (this._contextMenu != null)
            return this._contextMenu.PlacementTarget;
          if (this._toolTip != null)
            return this._toolTip.PlacementTarget;
          return this._popup != null ? this._popup.PlacementTarget ?? VisualTreeHelper.GetParent((DependencyObject) this._popup) as UIElement : (UIElement) null;
        }
      }

      public Rect PlacementRectangle
      {
        get
        {
          if (this._contextMenu != null)
            return this._contextMenu.PlacementRectangle;
          if (this._toolTip != null)
            return this._toolTip.PlacementRectangle;
          return this._popup != null ? this._popup.PlacementRectangle : Rect.Empty;
        }
      }

      private FrameworkElement ChildAsFE
      {
        get
        {
          return (FrameworkElement) this._contextMenu ?? (FrameworkElement) this._toolTip ?? this._popup?.Child as FrameworkElement;
        }
      }

      public event EventHandler Opened;

      public event EventHandler Closed;

      public void SetMargin(Thickness margin)
      {
        FrameworkElement childAsFe = this.ChildAsFE;
        if (childAsFe == null)
          return;
        childAsFe.Margin = margin;
      }

      public void ClearMargin() => this.ChildAsFE?.ClearValue(FrameworkElement.MarginProperty);

      public void Dispose()
      {
        if (this._contextMenu != null)
        {
          this._contextMenu.Opened -= new RoutedEventHandler(this.OnOpened);
          this._contextMenu.Closed -= new RoutedEventHandler(this.OnClosed);
          this._contextMenu = (ContextMenu) null;
        }
        else if (this._toolTip != null)
        {
          this._toolTip.Opened -= new RoutedEventHandler(this.OnOpened);
          this._toolTip.Closed -= new RoutedEventHandler(this.OnClosed);
          this._toolTip = (System.Windows.Controls.ToolTip) null;
        }
        else
        {
          if (this._popup == null)
            return;
          this._popup.Opened -= new EventHandler(this.OnOpened);
          this._popup.Closed -= new EventHandler(this.OnClosed);
          this._popup = (Popup) null;
        }
      }

      private void OnOpened(object sender, EventArgs e)
      {
        EventHandler opened = this.Opened;
        if (opened == null)
          return;
        opened((object) this, e);
      }

      private void OnClosed(object sender, EventArgs e)
      {
        EventHandler closed = this.Closed;
        if (closed == null)
          return;
        closed((object) this, e);
      }
    }
  }
}
