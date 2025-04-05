using PersianDateTimeWPFTools.Controls;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
#if NET8_0_OR_GREATER
#nullable disable
#endif
namespace PersianDateTimeWPFTools.Controls
{
  [TypeConverter(typeof (IconElementConverter))]
  public abstract class IconElement : FrameworkElement
  {
    public static readonly DependencyProperty ForegroundProperty = TextElement.ForegroundProperty.AddOwner(typeof (IconElement), (PropertyMetadata) new FrameworkPropertyMetadata((object) SystemColors.ControlTextBrush, FrameworkPropertyMetadataOptions.Inherits, new PropertyChangedCallback(IconElement.OnForegroundPropertyChanged)));
    private static readonly DependencyProperty VisualParentForegroundProperty = DependencyProperty.Register(nameof (VisualParentForeground), typeof (Brush), typeof (IconElement), new PropertyMetadata((object) null, new PropertyChangedCallback(IconElement.OnVisualParentForegroundPropertyChanged)));
    private Grid _layoutRoot;
    private bool _isForegroundDefaultOrInherited = true;
    private bool _shouldInheritForegroundFromVisualParent;

    private protected IconElement()
    {
    }

    private static void OnForegroundPropertyChanged(
      DependencyObject sender,
      DependencyPropertyChangedEventArgs args)
    {
      ((IconElement) sender).OnForegroundPropertyChanged(args);
    }

    private void OnForegroundPropertyChanged(DependencyPropertyChangedEventArgs args)
    {
      this._isForegroundDefaultOrInherited = DependencyPropertyHelper.GetValueSource((DependencyObject) this, args.Property).BaseValueSource <= BaseValueSource.Inherited;
      this.UpdateShouldInheritForegroundFromVisualParent();
    }

    [Bindable(true)]
    [Category("Appearance")]
    public Brush Foreground
    {
      get => (Brush) this.GetValue(IconElement.ForegroundProperty);
      set => this.SetValue(IconElement.ForegroundProperty, (object) value);
    }

    private protected Brush VisualParentForeground
    {
      get => (Brush) this.GetValue(IconElement.VisualParentForegroundProperty);
      set => this.SetValue(IconElement.VisualParentForegroundProperty, (object) value);
    }

    private static void OnVisualParentForegroundPropertyChanged(
      DependencyObject sender,
      DependencyPropertyChangedEventArgs args)
    {
      ((IconElement) sender).OnVisualParentForegroundPropertyChanged(args);
    }

    private protected virtual void OnVisualParentForegroundPropertyChanged(
      DependencyPropertyChangedEventArgs args)
    {
    }

    private protected bool ShouldInheritForegroundFromVisualParent
    {
      get => this._shouldInheritForegroundFromVisualParent;
      private set
      {
        if (this._shouldInheritForegroundFromVisualParent == value)
          return;
        this._shouldInheritForegroundFromVisualParent = value;
        if (this._shouldInheritForegroundFromVisualParent)
          this.SetBinding(IconElement.VisualParentForegroundProperty, (BindingBase) new Binding()
          {
            Path = new PropertyPath((object) TextElement.ForegroundProperty),
            Source = (object) this.VisualParent
          });
        else
          this.ClearValue(IconElement.VisualParentForegroundProperty);
        this.OnShouldInheritForegroundFromVisualParentChanged();
      }
    }

    private protected virtual void OnShouldInheritForegroundFromVisualParentChanged()
    {
    }

    private void UpdateShouldInheritForegroundFromVisualParent()
    {
      this.ShouldInheritForegroundFromVisualParent = this._isForegroundDefaultOrInherited && this.Parent != null && this.VisualParent != null && this.Parent != this.VisualParent;
    }

    private protected UIElementCollection Children
    {
      get
      {
        this.EnsureLayoutRoot();
        return this._layoutRoot.Children;
      }
    }

    private protected abstract void InitializeChildren();

    protected override int VisualChildrenCount => 1;

    protected override Visual GetVisualChild(int index)
    {
      if (index != 0)
        throw new ArgumentOutOfRangeException(nameof (index));
      this.EnsureLayoutRoot();
      return (Visual) this._layoutRoot;
    }

    protected override Size MeasureOverride(Size availableSize)
    {
      this.EnsureLayoutRoot();
      this._layoutRoot.Measure(availableSize);
      return this._layoutRoot.DesiredSize;
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
      this.EnsureLayoutRoot();
      this._layoutRoot.Arrange(new Rect(new System.Windows.Point(), finalSize));
      return finalSize;
    }

    protected override void OnVisualParentChanged(DependencyObject oldParent)
    {
      base.OnVisualParentChanged(oldParent);
      this.UpdateShouldInheritForegroundFromVisualParent();
    }

    private void EnsureLayoutRoot()
    {
      if (this._layoutRoot != null)
        return;
      Grid grid = new Grid();
      grid.Background = (Brush) Brushes.Transparent;
      grid.SnapsToDevicePixels = true;
      this._layoutRoot = grid;
      this.InitializeChildren();
      this.AddVisualChild((Visual) this._layoutRoot);
    }


     #region Second job
        public static readonly DependencyProperty GeometryProperty = DependencyProperty.RegisterAttached(
"Geometry", typeof(Geometry), typeof(IconElement), new PropertyMetadata(default(Geometry)));

        public static void SetGeometry(DependencyObject element, Geometry value)
            => element.SetValue(GeometryProperty, value);

        public static Geometry GetGeometry(DependencyObject element)
            => (Geometry)element.GetValue(GeometryProperty);

        public static readonly DependencyProperty WidthProperty = DependencyProperty.RegisterAttached(
            "Width", typeof(double), typeof(IconElement), new PropertyMetadata(double.NaN));

        public static void SetWidth(DependencyObject element, double value)
            => element.SetValue(WidthProperty, value);

        public static double GetWidth(DependencyObject element)
            => (double)element.GetValue(WidthProperty);

        public static readonly DependencyProperty HeightProperty = DependencyProperty.RegisterAttached(
            "Height", typeof(double), typeof(IconElement), new PropertyMetadata(double.NaN));

        public static void SetHeight(DependencyObject element, double value)
            => element.SetValue(HeightProperty, value);

        public static double GetHeight(DependencyObject element)
            => (double)element.GetValue(HeightProperty); 
        #endregion

    }
}
