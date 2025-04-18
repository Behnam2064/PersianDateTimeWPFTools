using PersianDateTimeWPFTools.Themes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace PersianDateTimeWPFTools.Tools
{
    public class InitResources : ResourceDictionary
    {

        public static EventHandler<ThemeChangedEventArgs> OnTheme;

        private static BaseTheme _theme = new ThemeDefault();
        public static BaseTheme Theme
        {
            get => _theme;
        }

        public BaseThemeName SelectedTheme
        {
            set
            {
                switch (value)
                {
                    case BaseThemeName.DarkModern1:
                        _theme = new ThemeDarkModern1();
                        break;
                    case BaseThemeName.LightModern1:
                        _theme = new ThemeLightModern1();
                        break;
                    case BaseThemeName.Default:
                    default:
                        _theme = new ThemeDefault();
                        break;
                }
            }
            get
            {
                return Theme.Theme;
            }
        }

        public InitResources()
        {
            // Check for design mode. 
            /*  if ((bool)(DesignerProperties.IsInDesignModeProperty.GetMetadata(typeof(DependencyObject)).DefaultValue))
              {
                  //in Design mode
              }*/
            Init();
            if (CultureInfo.CurrentUICulture.Name == "fa-IR")
            {
                ChangeLanguage("fa");
            }
            else
            {
                // Default value is en-US
                ChangeLanguage("en");
            }
        }

        public InitResources(bool noInit)
        {

        }

        internal static Style FindStyle(string controlName, string styleName)
        {
            return (Style)Application.Current.Resources[controlName + styleName];
        }

        internal static void SetControlStyle(Control control)
        {
            if (Theme.Theme != BaseThemeName.Default)
            {
                var style = FindStyle(control.GetType().Name, Theme.Theme.ToString());
                if (style != null)
                    control.Style = style;
            }
        }

        public static void SetTheme(BaseTheme theme)
        {
            if (theme == null)
                throw new ArgumentNullException(nameof(theme));

            var old = _theme.Clone();
            _theme = theme;
            var args = new ThemeChangedEventArgs(theme, (BaseTheme)old);
            OnTheme?.Invoke(new InitResources(true), args);
        }

        private void Init()
        {
            string res = $"pack://application:,,,/PersianDateTimeWPFTools;component/Resources.xaml";
            var exist = SearchResourceDictionary(res, Application.Current.Resources.MergedDictionaries) != null;
            if (!exist)
            {
                ResourceDictionary resDict = new ResourceDictionary { Source = new Uri(res) };
                Application.Current.Resources.MergedDictionaries.Add(resDict);
            }
        }

        /// <summary>
        /// Change Language
        /// </summary>
        /// <param name="language">fa or en</param>
        /// <param name="lanAddress"></param>
        public void ChangeLanguage(string language, string lanAddress = null)
        {
            string langFile = $"pack://application:,,,/PersianDateTimeWPFTools;component/Resources/Lang/Lang.{language}.xaml";
            if (!string.IsNullOrEmpty(lanAddress))
                langFile = lanAddress;

            ResourceDictionary langDict = new ResourceDictionary { Source = new Uri(langFile) };
            Application.Current.Resources.MergedDictionaries.Remove(SearchResourceDictionary("/Resources/Lang/", Application.Current.Resources.MergedDictionaries));
            Application.Current.Resources.MergedDictionaries.Add(langDict);
        }

        private ResourceDictionary SearchResourceDictionary(string containsString, Collection<ResourceDictionary> mergedDictionaries)
        {
            foreach (var item in mergedDictionaries)
            {
                if (item?.Source != null // In DesignMode will be null
                    && item.Source.AbsoluteUri.Contains(containsString))
                {
                    return item;
                }
                else
                {
                    if (item.MergedDictionaries.Count > 0)
                    {
                        return SearchResourceDictionary(containsString, item.MergedDictionaries);
                    }
                }
            }

            return null;
        }
    }

    public class ThemeChangedEventArgs : EventArgs
    {
        public BaseTheme NewTheme { get; private set; }
        public BaseTheme OldTheme { get; private set; }
        public ThemeChangedEventArgs(BaseTheme newTheme, BaseTheme oldTheme)
        {
            this.NewTheme = newTheme;
            this.OldTheme = OldTheme;
        }
    }

}
