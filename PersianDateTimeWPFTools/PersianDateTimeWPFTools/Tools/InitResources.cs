using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PersianDateTimeWPFTools.Tools
{
    public class InitResources : ResourceDictionary
    {
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
            }
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
}
