using PersianDateTimeWPFTools.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersianDateTimeWPFTools.Themes
{
    public abstract class BaseTheme : ICloneable
    {
        public BaseThemeName Theme { get; private set; }

        protected BaseTheme(BaseThemeName theme)
        {
            this.Theme = theme;
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
