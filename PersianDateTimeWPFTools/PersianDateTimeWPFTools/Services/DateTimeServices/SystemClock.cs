using PersianDateTimeWPFTools.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersianDateTimeWPFTools.Services.DateTimeServices
{
    public class SystemClock : ISystemClock
    {
        public DateTime Now => DateTime.Now;
        public DateTime Today => DateTime.Today;
        public DateTime MinValue => DateTime.MinValue;
        public DateTime MaxValue => DateTime.MaxValue;

        public int Compare(DateTime t1, DateTime t2) => DateTime.Compare(t1, t2);
        
    }

}
