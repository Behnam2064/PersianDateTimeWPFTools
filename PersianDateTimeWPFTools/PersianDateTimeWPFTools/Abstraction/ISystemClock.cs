using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersianDateTimeWPFTools.Abstraction
{
    public interface ISystemClock
    {
#if NET5_0_OR_GREATER
        public
#endif 
            DateTime Now { get; }
#if NET5_0_OR_GREATER
        public
#endif
        DateTime Today { get; }
#if NET5_0_OR_GREATER
        public
#endif
        DateTime MinValue { get; }
#if NET5_0_OR_GREATER
        public
#endif
        DateTime MaxValue { get; }
#if NET5_0_OR_GREATER
        public
#endif
        int Compare(DateTime t1, DateTime t2);
    }
}
