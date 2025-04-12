using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace PersianDateTimeWPFTools.Tools
{
    public class TimeStringConverter : MarkupExtension, IValueConverter
    {

        public TimeStringInputType StringInputType { get; set; }
        public TimeStringOutputType StringOutputType { get; set; }

        public string ConcatString { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;

            switch (StringInputType)
            {
                case TimeStringInputType.ISO8601_String:
                    DateTime dateTime = DateTime.Parse(value.ToString());
                    switch (StringOutputType)
                    {
                        case TimeStringOutputType.Hour:
                            return dateTime.Hour.ToString() + ConcatString;
                        case TimeStringOutputType.Minute:
                            return dateTime.Minute.ToString() + ConcatString;
                        case TimeStringOutputType.Second:
                            return dateTime.Second.ToString() + ConcatString;
                        default:
                            break;
                    }
                    break;
                case TimeStringInputType.HH_MM_ss_String:
                    string[] times = value.ToString().Split(':');
                    switch (StringOutputType)
                    {
                        case TimeStringOutputType.Hour:
                            return times[0].Trim() + ConcatString;
                        case TimeStringOutputType.Minute:
                            if (times.Length >= 2)
                                return times[1].Trim() + ConcatString;
                            else
                                return "";
                        case TimeStringOutputType.Second:
                            if (times.Length >= 3)
                                return times[2].Trim() + ConcatString;
                            else
                                return "";
                        default:
                            break;
                    }
                    break;
                default:
                    break;
            }
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }

    public enum TimeStringInputType
    {
        ISO8601_String,
        HH_MM_ss_String
    }

    public enum TimeStringOutputType
    {
        Hour,
        Minute,
        Second
    }
}
