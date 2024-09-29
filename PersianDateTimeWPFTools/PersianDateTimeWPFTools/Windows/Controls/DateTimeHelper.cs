using System;
using System.Globalization;
using System.Windows;
using System.Windows.Markup;
#if NET8_0_OR_GREATER
#nullable disable
#endif
namespace PersianDateTimeWPFTools.Windows.Controls
{
    internal class DateTimeHelper
    {
        private readonly Calendar cal; //= PersianCalendarHelper.GetCurrentCalendar();

        public DateTimeHelper(Calendar calendar)
        {
            this.cal = calendar;
        }

        //public static Calendar Calendar => DateTimeHelper.cal;

        public DateTime? AddDays(DateTime time, int days)
        {
            try
            {
                return new DateTime?(cal.AddDays(time, days));
            }
            catch (ArgumentException ex)
            {
                return new DateTime?();
            }
        }

        public DateTime? AddMonths(DateTime time, int months)
        {
            try
            {
                return new DateTime?(cal.AddMonths(time, months));
            }
            catch (ArgumentException ex)
            {
                return new DateTime?();
            }
        }

        public DateTime? AddYears(DateTime time, int years)
        {
            try
            {
                return new DateTime?(cal.AddYears(time, years));
            }
            catch (ArgumentException ex)
            {
                return new DateTime?();
            }
        }

        public DateTime? SetYear(DateTime date, DateTime yearDate)
        {
            int year1 = cal.GetYear(date);
            int year2 = cal.GetYear(yearDate);
            return AddYears(date, year2 - year1);
        }

        public DateTime? SetYearMonth(DateTime date, DateTime yearMonth)
        {
            DateTime? nullable = SetYear(date, yearMonth);
            if (nullable.HasValue)
            {
                int month1 = cal.GetMonth(date);
                int month2 = cal.GetMonth(yearMonth);
                nullable = AddMonths(nullable.Value, month2 - month1);
            }
            return nullable;
        }

        public static int CompareDays(DateTime dt1, DateTime dt2)
        {
            DateTime? nullable = DateTimeHelper.DiscardTime(new DateTime?(dt1));
            DateTime t1 = nullable.Value;
            nullable = DateTimeHelper.DiscardTime(new DateTime?(dt2));
            DateTime t2 = nullable.Value;
            return DateTime.Compare(t1, t2);
        }

        public int CompareYearMonth(DateTime dt1, DateTime dt2)
        {
            int year1 = cal.GetYear(dt1);
            int year2 = cal.GetYear(dt2);
            int month1 = cal.GetMonth(dt1);
            int month2 = cal.GetMonth(dt2);
            int num = year2;
            return (year1 - num) * 12 + (month1 - month2);
        }

        public DateTime DecadeOfDate(DateTime date)
        {
            int year1 = cal.GetYear(date);
            int year2 = year1 - year1 % 10;
            return cal.ToDateTime(year2, 1, 1, 0, 0, 0, 0);
        }

        public DateTime DiscardDayTime(DateTime d)
        {
            int year = cal.GetYear(d);
            int month = cal.GetMonth(d);
            return cal.ToDateTime(year, month, 1, 0, 0, 0, 0);
        }

        public DateTime GetFirstDayOfMonth(DateTime dt)
        {
            int year = cal.GetYear(dt);
            int month = cal.GetMonth(dt);
            return cal.ToDateTime(year, month, 1, 0, 0, 0, 0);
        }

        public DateTime DiscardMonthDayTime(DateTime d)
        {
            int year = cal.GetYear(d);
            return cal.ToDateTime(year, 1, 1, 0, 0, 0, 0);
        }

        public static DateTime? DiscardTime(DateTime? d)
        {
            return !d.HasValue ? new DateTime?() : new DateTime?(d.Value.Date);
        }

        public DateTime GetLastMonth(DateTime d)
        {
            int year = cal.GetYear(d);
            return cal.ToDateTime(year, 12, 1, 0, 0, 0, 0);
        }

        public DateTime EndOfDecade(DateTime date)
        {
            return cal.AddYears(DecadeOfDate(date), 9);
        }

        public static DateTimeFormatInfo GetCurrentDateFormat()
        {
            return DateTimeHelper.GetDateFormat(CultureInfo.CurrentCulture);
        }

        internal static CultureInfo GetCulture(FrameworkElement element)
        {
            return DependencyPropertyHelper.GetValueSource((DependencyObject)element, FrameworkElement.LanguageProperty).BaseValueSource == BaseValueSource.Default ? CultureInfo.CurrentCulture : DateTimeHelper.GetCultureInfo((DependencyObject)element);
        }

        internal static CultureInfo GetCultureInfo(DependencyObject element)
        {
            XmlLanguage xmlLanguage = (XmlLanguage)element.GetValue(FrameworkElement.LanguageProperty);
            try
            {
                return xmlLanguage.GetSpecificCulture();
            }
            catch (InvalidOperationException ex)
            {
                return CultureInfo.ReadOnly(new CultureInfo("en-us", false));
            }
        }

        internal static DateTimeFormatInfo GetDateFormat(CultureInfo culture)
        {
            //return PersianCalendarHelper.GetDateTimeFormatInfo();
            return new CalendarHelper(culture, true).GetDateTimeFormatInfo();
        }

        public static bool InRange(DateTime date, CalendarDateRange range)
        {
            return DateTimeHelper.InRange(date, range.Start, range.End);
        }

        public static bool InRange(DateTime date, DateTime start, DateTime end)
        {
            return DateTimeHelper.CompareDays(date, start) > -1 && DateTimeHelper.CompareDays(date, end) < 1;
        }

        public string ToDayString(DateTime? date, CultureInfo culture)
        {
            string empty = string.Empty;
            DateTimeFormatInfo dateFormat = DateTimeHelper.GetDateFormat(culture);
            if (date.HasValue && dateFormat != null)
                empty = cal.GetDayOfMonth(date.Value).ToString((IFormatProvider)dateFormat);
            return empty;
        }

        public string ToDecadeRangeString(DateTime decade, CultureInfo culture)
        {
            string decadeRangeString = string.Empty;
            DateTimeFormatInfo dateTimeFormat = culture.DateTimeFormat;
            if (dateTimeFormat != null)
            {
                int year = cal.GetYear(decade);
                int num = year + 9;
                decadeRangeString = year.ToString((IFormatProvider)dateTimeFormat) + "-" + num.ToString((IFormatProvider)dateTimeFormat);
            }
            return decadeRangeString;
        }

        public static string ToYearMonthPatternString(DateTime? date, CultureInfo culture)
        {
            string monthPatternString = string.Empty;
            DateTimeFormatInfo dateFormat = DateTimeHelper.GetDateFormat(culture);
            if (date.HasValue && dateFormat != null)
                monthPatternString = new CalendarHelper(culture, true).ToCurrentCultureString(date.Value, dateFormat.YearMonthPattern, dateFormat); //PersianCalendarHelper.ToCurrentCultureString(date.Value, dateFormat.YearMonthPattern, dateFormat);
            return monthPatternString;
        }

        public string ToYearString(DateTime? date, CultureInfo culture)
        {
            string empty = string.Empty;
            DateTimeFormatInfo dateFormat = DateTimeHelper.GetDateFormat(culture);
            if (date.HasValue && dateFormat != null)
                empty = cal.GetYear(date.Value).ToString((IFormatProvider)dateFormat);
            return empty;
        }

        public static string ToAbbreviatedMonthString(DateTime? date, CultureInfo culture)
        {
            string abbreviatedMonthString = string.Empty;
            DateTimeFormatInfo dateFormat = DateTimeHelper.GetDateFormat(culture);
            if (date.HasValue && dateFormat != null)
                abbreviatedMonthString = new CalendarHelper(culture, true).ToCurrentCultureString(date.Value, "MMM", dateFormat); // PersianCalendarHelper.ToCurrentCultureString(date.Value, "MMM", dateFormat);
            return abbreviatedMonthString;
        }

        public static string ToLongDateString(DateTime? date, CultureInfo culture)
        {
            string longDateString = string.Empty;
            DateTimeFormatInfo dateFormat = DateTimeHelper.GetDateFormat(culture);
            if (date.HasValue && dateFormat != null)
                longDateString = new CalendarHelper(culture, true).ToCurrentCultureString(date.Value.Date, dateFormat.LongDatePattern, dateFormat); // PersianCalendarHelper.ToCurrentCultureString(date.Value.Date, dateFormat.LongDatePattern, dateFormat);
            return longDateString;
        }
    }
}
