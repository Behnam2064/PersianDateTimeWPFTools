﻿using System;
using System.Globalization;
#if NET8_0_OR_GREATER
#nullable disable
#endif
namespace PersianDateTimeWPFTools.Windows.Controls
{
    internal static class PersianCalendarHelper
    {
        public static Calendar GetCurrentCalendar() => (Calendar)new PersianCalendar(); // Or new GregorianCalendar();

        public static string ToCurrentCultureString(
          DateTime dt,
          string format,
          DateTimeFormatInfo formatProvider)
        {
            string[] strArray = new string[22]
            {
        "fffffff",
        "ffffff",
        "fffff",
        "ffff",
        "fff",
        "ff",
        "f",
        "FFFFFFF",
        "FFFFFF",
        "FFFFF",
        "FFFF",
        "FFF",
        "FF",
        "F",
        "gg",
        "g",
        "hh",
        "HH",
        "mm",
        "ss",
        "tt",
        "t"
            };
            Calendar currentCalendar = PersianCalendarHelper.GetCurrentCalendar();
            int year = currentCalendar.GetYear(dt);
            int month = currentCalendar.GetMonth(dt);
            currentCalendar.GetDayOfMonth(dt);
            DayOfWeek dayOfWeek = currentCalendar.GetDayOfWeek(dt);
            foreach (string str in strArray)
                format = format.Replace(str, dt.ToString(str, (IFormatProvider)formatProvider));
            format = format.Replace("dddd", formatProvider.GetDayName(dayOfWeek));
            format = format.Replace("ddd", formatProvider.GetAbbreviatedDayName(dayOfWeek));
            format = format.Replace("dd", ((int)dayOfWeek).ToString("00"));
            format = format.Replace("dd", dayOfWeek.ToString());
            format = format.Replace("MMMM", formatProvider.GetMonthName(month));
            format = format.Replace("MMM", formatProvider.GetAbbreviatedMonthName(month));
            format = format.Replace("MM", month.ToString("00"));
            format = format.Replace("M", month.ToString());
            format = format.Replace("yyyy", year.ToString("0000"));
            format = format.Replace("yyy", year.ToString("000"));
            string str1 = format;
            int num = year % 100;
            string newValue1 = num.ToString("00");
            format = str1.Replace("yy", newValue1);
            string str2 = format;
            num = year % 100;
            string newValue2 = num.ToString();
            format = str2.Replace("y", newValue2);
            
            return format;
        }

        public static DateTimeFormatInfo GetDateTimeFormatInfo()
        {
            return new DateTimeFormatInfo()
            {
                AbbreviatedDayNames = new string[7]
              {
          "ی",
          "د",
          "س",
          "چ",
          "پ",
          "ج",
          "ش"
              },
                AbbreviatedMonthGenitiveNames = new string[13]
              {
          "فروردین",
          "اردیبهشت",
          "خرداد",
          "تیر",
          "مرداد",
          "شهریور",
          "مهر",
          "آبان",
          "آذر",
          "دی",
          "بهمن",
          "اسفند",
          ""
              },
                AbbreviatedMonthNames = new string[13]
              {
          "فروردین",
          "اردیبهشت",
          "خرداد",
          "تیر",
          "مرداد",
          "شهریور",
          "مهر",
          "آبان",
          "آذر",
          "دی",
          "بهمن",
          "اسفند",
          ""
              },
                AMDesignator = "صبح",
                CalendarWeekRule = CalendarWeekRule.FirstDay,
                DateSeparator = "/",
                DayNames = new string[7]
              {
          "یکشنبه",
          "دوشنبه",
          "سه\u200Cشنبه",
          "چهار\u200Cشنبه",
          "پنجشنبه",
          "جمعه",
          "شنبه"
              },
                FirstDayOfWeek = DayOfWeek.Saturday,
                FullDateTimePattern = "dddd dd MMMM yyyy",
                LongDatePattern = "dd MMMM yyyy",
                LongTimePattern = "hh:mm:ss TT",
                MonthDayPattern = "dd MMMM",
                MonthGenitiveNames = new string[13]
              {
          "فروردین",
          "اردیبهشت",
          "خرداد",
          "تیر",
          "مرداد",
          "شهریور",
          "مهر",
          "آبان",
          "آذر",
          "دی",
          "بهمن",
          "اسفند",
          ""
              },
                MonthNames = new string[13]
              {
          "فروردین",
          "اردیبهشت",
          "خرداد",
          "تیر",
          "مرداد",
          "شهریور",
          "مهر",
          "آبان",
          "آذر",
          "دی",
          "بهمن",
          "اسفند",
          ""
              },
                PMDesignator = "عصر",
                ShortDatePattern = "dd/MM/yy",
                ShortestDayNames = new string[7]
              {
          "ی",
          "د",
          "س",
          "چ",
          "پ",
          "ج",
          "ش"
              },
                ShortTimePattern = "HH:mm",
                TimeSeparator = ":",
                YearMonthPattern = "MMMM yyyy"
            };
        }
    }
}
