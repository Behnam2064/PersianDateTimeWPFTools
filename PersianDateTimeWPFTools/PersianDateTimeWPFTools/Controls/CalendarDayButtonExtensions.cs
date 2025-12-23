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

    }

}
