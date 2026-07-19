using PersianDateTimeWPFTools.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersianDateTimeWPFTools.Time
{
    public static class ClockProvider
    {
        public static PersianDateTimeWPFTools.Abstraction.ISystemClock Current { get; set; } = new Services.DateTimeServices.SystemClock();
    }
}
