using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

#if NET8_0_OR_GREATER
#nullable disable
#endif
namespace PersianDateTimeWPFTools.Controls
{
  public class ContentPresenterEx : ContentPresenter
  {
    public static readonly DependencyProperty FontFamilyProperty = TextElement.FontFamilyProperty.AddOwner(typeof (ContentPresenterEx));
    public static readonly DependencyProperty FontStyleProperty = TextElement.FontStyleProperty.AddOwner(typeof (ContentPresenterEx));
    public static readonly DependencyProperty FontWeightProperty = TextElement.FontWeightProperty.AddOwner(typeof (ContentPresenterEx));
    public static readonly DependencyProperty FontStretchProperty = TextElement.FontStretchProperty.AddOwner(typeof (ContentPresenterEx));
    public static readonly DependencyProperty FontSizeProperty = TextElement.FontSizeProperty.AddOwner(typeof (ContentPresenterEx));
    public static readonly DependencyProperty ForegroundProperty = TextElement.ForegroundProperty.AddOwner(typeof (ContentPresenterEx));
    public static readonly DependencyProperty LineHeightProperty = Block.LineHeightProperty.AddOwner(typeof (ContentPresenterEx));
    public static readonly DependencyProperty LineStackingStrategyProperty = Block.LineStackingStrategyProperty.AddOwner(typeof (ContentPresenterEx));
    public static readonly DependencyProperty TextWrappingProperty = TextBlock.TextWrappingProperty.AddOwner(typeof (ContentPresenterEx), (PropertyMetadata) new FrameworkPropertyMetadata((object) TextWrapping.NoWrap, new PropertyChangedCallback(ContentPresenterEx.OnTextWrappingChanged)));
    private TextBlock _textBlock;
    private AccessText _accessText;

    [Localizability(LocalizationCategory.Font)]
    public FontFamily FontFamily
    {
      get => (FontFamily) this.GetValue(ContentPresenterEx.FontFamilyProperty);
      set => this.SetValue(ContentPresenterEx.FontFamilyProperty, (object) value);
    }

    public FontStyle FontStyle
    {
      get => (FontStyle) this.GetValue(ContentPresenterEx.FontStyleProperty);
      set => this.SetValue(ContentPresenterEx.FontStyleProperty, (object) value);
    }

    public FontWeight FontWeight
    {
      get => (FontWeight) this.GetValue(ContentPresenterEx.FontWeightProperty);
      set => this.SetValue(ContentPresenterEx.FontWeightProperty, (object) value);
    }

    public FontStretch FontStretch
    {
      get => (FontStretch) this.GetValue(ContentPresenterEx.FontStretchProperty);
      set => this.SetValue(ContentPresenterEx.FontStretchProperty, (object) value);
    }

    [TypeConverter(typeof (FontSizeConverter))]
    [Localizability(LocalizationCategory.None)]
    public double FontSize
    {
      get => (double) this.GetValue(ContentPresenterEx.FontSizeProperty);
      set => this.SetValue(ContentPresenterEx.FontSizeProperty, (object) value);
    }

    public Brush Foreground
    {
      get => (Brush) this.GetValue(ContentPresenterEx.ForegroundProperty);
      set => this.SetValue(ContentPresenterEx.ForegroundProperty, (object) value);
    }

    [TypeConverter(typeof (LengthConverter))]
    public double LineHeight
    {
      get => (double) this.GetValue(ContentPresenterEx.LineHeightProperty);
      set => this.SetValue(ContentPresenterEx.LineHeightProperty, (object) value);
    }

    public LineStackingStrategy LineStackingStrategy
    {
      get => (LineStackingStrategy) this.GetValue(ContentPresenterEx.LineStackingStrategyProperty);
      set => this.SetValue(ContentPresenterEx.LineStackingStrategyProperty, (object) value);
    }

    public TextWrapping TextWrapping
    {
      get => (TextWrapping) this.GetValue(ContentPresenterEx.TextWrappingProperty);
      set => this.SetValue(ContentPresenterEx.TextWrappingProperty, (object) value);
    }

    private static void OnTextWrappingChanged(
      DependencyObject d,
      DependencyPropertyChangedEventArgs e)
    {
      ContentPresenterEx contentPresenterEx = (ContentPresenterEx) d;
      if (contentPresenterEx.TextBlock != null)
      {
        contentPresenterEx.TextBlock.TextWrapping = (TextWrapping) e.NewValue;
      }
      else
      {
        if (contentPresenterEx.AccessText == null)
          return;
        contentPresenterEx.AccessText.TextWrapping = (TextWrapping) e.NewValue;
      }
    }

    private bool IsUsingDefaultTemplate { get; set; }

    private TextBlock TextBlock
    {
      get => this._textBlock;
      set
      {
        if (this._textBlock != null)
          this._textBlock.ClearValue(TextBlock.TextWrappingProperty);
        this._textBlock = value;
        if (this._textBlock == null)
          return;
        this._textBlock.TextWrapping = this.TextWrapping;
      }
    }

    private AccessText AccessText
    {
      get => this._accessText;
      set
      {
        if (this._accessText != null)
          this._accessText.ClearValue(AccessText.TextWrappingProperty);
        this._accessText = value;
        if (this._accessText == null)
          return;
        this._accessText.TextWrapping = this.TextWrapping;
      }
    }

    protected override DataTemplate ChooseTemplate()
    {
      object content = this.Content;
      DataTemplate dataTemplate = this.ContentTemplate;
      if (dataTemplate == null && this.ContentTemplateSelector != null)
        dataTemplate = this.ContentTemplateSelector.SelectTemplate(content, (DependencyObject) this);
      if (dataTemplate == null)
      {
        dataTemplate = base.ChooseTemplate();
        this.IsUsingDefaultTemplate = true;
      }
      else
        this.IsUsingDefaultTemplate = false;
      return dataTemplate;
    }

    protected override void OnVisualChildrenChanged(
      DependencyObject visualAdded,
      DependencyObject visualRemoved)
    {
      base.OnVisualChildrenChanged(visualAdded, visualRemoved);
      if (visualAdded != null && this.IsUsingDefaultTemplate)
      {
        switch (visualAdded)
        {
          case TextBlock textBlock:
            this.TextBlock = textBlock;
            break;
          case AccessText accessText:
            this.AccessText = accessText;
            break;
        }
      }
      else
      {
        if (visualRemoved == null)
          return;
        if (visualRemoved == this.TextBlock)
        {
          this.TextBlock = (TextBlock) null;
        }
        else
        {
          if (visualRemoved != this.AccessText)
            return;
          this.AccessText = (AccessText) null;
        }
      }
    }
  }
}
