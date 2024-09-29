using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

#if NET8_0_OR_GREATER
#nullable disable
#endif
namespace PersianDateTimeWPFTools.Controls
{
  public sealed class SymbolIcon : IconElement
  {
    public static readonly DependencyProperty SymbolProperty = DependencyProperty.Register(nameof (Symbol), typeof (Symbol), typeof (SymbolIcon), new PropertyMetadata((object) Symbol.Emoji, new PropertyChangedCallback(SymbolIcon.OnSymbolChanged)));
    internal static readonly DependencyProperty FontSizeProperty = DependencyProperty.Register(nameof (FontSize), typeof (double), typeof (SymbolIcon), new PropertyMetadata((object) 20.0, new PropertyChangedCallback(SymbolIcon.OnFontSizeChanged)));
    private TextBlock _textBlock;

    public SymbolIcon()
    {
    }

    public SymbolIcon(Symbol symbol) => this.Symbol = symbol;

    public Symbol Symbol
    {
      get => (Symbol) this.GetValue(SymbolIcon.SymbolProperty);
      set => this.SetValue(SymbolIcon.SymbolProperty, (object) value);
    }

    private static void OnSymbolChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((SymbolIcon) d).OnSymbolChanged(e);
    }

    private void OnSymbolChanged(DependencyPropertyChangedEventArgs e)
    {
      if (this._textBlock == null)
        return;
      this._textBlock.Text = SymbolIcon.ConvertToString((Symbol) e.NewValue);
    }

    internal double FontSize
    {
      get => (double) this.GetValue(SymbolIcon.FontSizeProperty);
      set => this.SetValue(SymbolIcon.FontSizeProperty, (object) value);
    }

    private static void OnFontSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((SymbolIcon) d).OnFontSizeChanged(e);
    }

    private void OnFontSizeChanged(DependencyPropertyChangedEventArgs e)
    {
      if (this._textBlock == null)
        return;
      this._textBlock.FontSize = (double) e.NewValue;
    }

    private protected override void InitializeChildren()
    {
      TextBlock textBlock = new TextBlock();
      textBlock.Style = (Style) null;
      textBlock.HorizontalAlignment = HorizontalAlignment.Stretch;
      textBlock.VerticalAlignment = VerticalAlignment.Center;
      textBlock.TextAlignment = TextAlignment.Center;
      textBlock.FontSize = this.FontSize;
      textBlock.FontStyle = FontStyles.Normal;
      textBlock.FontWeight = FontWeights.Normal;
      textBlock.Text = SymbolIcon.ConvertToString(this.Symbol);
      this._textBlock = textBlock;
      this._textBlock.SetResourceReference(TextBlock.FontFamilyProperty, (object) "SymbolThemeFontFamily");
      if (this.ShouldInheritForegroundFromVisualParent)
        this._textBlock.Foreground = this.VisualParentForeground;
      this.Children.Add((UIElement) this._textBlock);
    }

    private protected override void OnShouldInheritForegroundFromVisualParentChanged()
    {
      if (this._textBlock == null)
        return;
      if (this.ShouldInheritForegroundFromVisualParent)
        this._textBlock.Foreground = this.VisualParentForeground;
      else
        this._textBlock.ClearValue(TextBlock.ForegroundProperty);
    }

    private protected override void OnVisualParentForegroundPropertyChanged(
      DependencyPropertyChangedEventArgs args)
    {
      if (!this.ShouldInheritForegroundFromVisualParent || this._textBlock == null)
        return;
      this._textBlock.Foreground = (Brush) args.NewValue;
    }

    private static string ConvertToString(Symbol symbol)
    {
      return char.ConvertFromUtf32((int) symbol).ToString();
    }
  }
}
