using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PersianDateTimeWPFTools.Controls
{
    public static class CalendarDayButtonExtensions
    {
        #region Tooltip feature
        public static readonly DependencyProperty DayToolTipProperty =
            DependencyProperty.RegisterAttached(
                "DayToolTip",
                typeof(object),
                typeof(CalendarDayButtonExtensions),
                new FrameworkPropertyMetadata(null));

        public static void SetDayToolTip(
            DependencyObject element, object value)
        {
            element.SetValue(DayToolTipProperty, value);
        }

        public static object GetDayToolTip(
            DependencyObject element)
        {
            return element.GetValue(DayToolTipProperty);
        }

        #endregion

        #region Tooltip template feature
        public static readonly DependencyProperty DayToolTipTemplateProperty =
    DependencyProperty.RegisterAttached(
        "DayToolTipTemplate",
        typeof(DataTemplate),
        typeof(CalendarDayButtonExtensions),
        new FrameworkPropertyMetadata(null));

        public static void SetDayToolTipTemplate(
            DependencyObject element, DataTemplate value)
        {
            element.SetValue(DayToolTipTemplateProperty, value);
        }

        public static DataTemplate GetDayToolTipTemplate(
            DependencyObject element)
        {
            return (DataTemplate)element.GetValue(DayToolTipTemplateProperty);
        }
        #endregion

        #region Day Indicators featuer

        public static readonly DependencyProperty HasDayIndicatorProperty =
    DependencyProperty.RegisterAttached(
        "HasDayIndicator",
        typeof(bool),
        typeof(CalendarDayButtonExtensions),
        new FrameworkPropertyMetadata(false));

        public static void SetHasDayIndicator(
            DependencyObject element, bool value)
        {
            element.SetValue(HasDayIndicatorProperty, value);
        }

        public static bool GetHasDayIndicator(
            DependencyObject element)
        {
            return (bool)element.GetValue(HasDayIndicatorProperty);
        }

        #endregion

        #region Day Metadata feature


        public static readonly DependencyProperty HasDayMetadataProperty =
    DependencyProperty.RegisterAttached(
        "HasDayMetadata",
        typeof(bool),
        typeof(CalendarDayButtonExtensions),
        new FrameworkPropertyMetadata(false));

        public static void SetHasDayMetadata(
            DependencyObject element, bool value)
        {
            element.SetValue(HasDayMetadataProperty, value);
        }

        public static bool GetHasDayMetadata(
            DependencyObject element)
        {
            return (bool)element.GetValue(HasDayMetadataProperty);
        }


        public static readonly DependencyProperty DayIndicatorStyleProperty =
DependencyProperty.RegisterAttached(
"DayIndicatorStyle",
typeof(Style),
typeof(CalendarDayButtonExtensions),
new FrameworkPropertyMetadata(null));

        public static void SetDayIndicatorStyle(
            DependencyObject element, Style value)
        {
            element.SetValue(DayIndicatorStyleProperty, value);
        }

        public static Style GetDayIndicatorStyle(
            DependencyObject element)
        {
            return (Style)element.GetValue(DayIndicatorStyleProperty);
        }

        #endregion
    }

}
