using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace PersianDateTimeWPFTools.Tools
{
    [GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
    [DebuggerNonUserCode]
    [CompilerGenerated]
    internal class Strings
    {
        private static ResourceManager resourceMan;
        private static CultureInfo resourceCulture;

        internal Strings()
        {
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        internal static ResourceManager ResourceManager
        {
            get
            {
                if (Strings.resourceMan == null)
                    Strings.resourceMan = new ResourceManager("PersianDateTimeWPFTools.Resources.Strings", typeof(Strings).Assembly);
                return Strings.resourceMan;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        internal static CultureInfo Culture
        {
            get => Strings.resourceCulture;
            set => Strings.resourceCulture = value;
        }

        internal static string AppBarMoreButtonClosedToolTip
        {
            get
            {
                return Strings.ResourceManager.GetString(nameof(AppBarMoreButtonClosedToolTip), Strings.resourceCulture);
            }
        }

        internal static string AppBarMoreButtonName
        {
            get
            {
                return Strings.ResourceManager.GetString(nameof(AppBarMoreButtonName), Strings.resourceCulture);
            }
        }

        internal static string AppBarMoreButtonOpenToolTip
        {
            get
            {
                return Strings.ResourceManager.GetString(nameof(AppBarMoreButtonOpenToolTip), Strings.resourceCulture);
            }
        }

        internal static string IgnoreMenuItemLabel
        {
            get
            {
                return Strings.ResourceManager.GetString(nameof(IgnoreMenuItemLabel), Strings.resourceCulture);
            }
        }

        internal static string ToggleSwitchOff
        {
            get => Strings.ResourceManager.GetString(nameof(ToggleSwitchOff), Strings.resourceCulture);
        }

        internal static string ToggleSwitchOn
        {
            get => Strings.ResourceManager.GetString(nameof(ToggleSwitchOn), Strings.resourceCulture);
        }
    }
}
