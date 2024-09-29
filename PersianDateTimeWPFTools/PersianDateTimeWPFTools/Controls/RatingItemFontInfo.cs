using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
#if NET8_0_OR_GREATER
#nullable disable
#endif
namespace PersianDateTimeWPFTools.Controls
{
    public class RatingItemFontInfo : RatingItemInfo
    {
        public static readonly DependencyProperty DisabledGlyphProperty = DependencyProperty.Register(nameof(DisabledGlyph), typeof(string), typeof(RatingItemFontInfo), new PropertyMetadata((object)string.Empty));
        public static readonly DependencyProperty GlyphProperty = DependencyProperty.Register(nameof(Glyph), typeof(string), typeof(RatingItemFontInfo), new PropertyMetadata((object)string.Empty));
        public static readonly DependencyProperty PlaceholderGlyphProperty = DependencyProperty.Register(nameof(PlaceholderGlyph), typeof(string), typeof(RatingItemFontInfo), new PropertyMetadata((object)string.Empty));
        public static readonly DependencyProperty PointerOverGlyphProperty = DependencyProperty.Register(nameof(PointerOverGlyph), typeof(string), typeof(RatingItemFontInfo), new PropertyMetadata((object)string.Empty));
        public static readonly DependencyProperty PointerOverPlaceholderGlyphProperty = DependencyProperty.Register(nameof(PointerOverPlaceholderGlyph), typeof(string), typeof(RatingItemFontInfo), new PropertyMetadata((object)string.Empty));
        public static readonly DependencyProperty UnsetGlyphProperty = DependencyProperty.Register(nameof(UnsetGlyph), typeof(string), typeof(RatingItemFontInfo), new PropertyMetadata((object)string.Empty));

        public string DisabledGlyph
        {
            get => (string)this.GetValue(RatingItemFontInfo.DisabledGlyphProperty);
            set => this.SetValue(RatingItemFontInfo.DisabledGlyphProperty, (object)value);
        }

        public string Glyph
        {
            get => (string)this.GetValue(RatingItemFontInfo.GlyphProperty);
            set => this.SetValue(RatingItemFontInfo.GlyphProperty, (object)value);
        }

        public string PlaceholderGlyph
        {
            get => (string)this.GetValue(RatingItemFontInfo.PlaceholderGlyphProperty);
            set => this.SetValue(RatingItemFontInfo.PlaceholderGlyphProperty, (object)value);
        }

        public string PointerOverGlyph
        {
            get => (string)this.GetValue(RatingItemFontInfo.PointerOverGlyphProperty);
            set => this.SetValue(RatingItemFontInfo.PointerOverGlyphProperty, (object)value);
        }

        public string PointerOverPlaceholderGlyph
        {
            get => (string)this.GetValue(RatingItemFontInfo.PointerOverPlaceholderGlyphProperty);
            set => this.SetValue(RatingItemFontInfo.PointerOverPlaceholderGlyphProperty, (object)value);
        }

        public string UnsetGlyph
        {
            get => (string)this.GetValue(RatingItemFontInfo.UnsetGlyphProperty);
            set => this.SetValue(RatingItemFontInfo.UnsetGlyphProperty, (object)value);
        }

        protected override Freezable CreateInstanceCore() => (Freezable)new RatingItemFontInfo();
    }
}
