using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Markup;

[assembly: ThemeInfo(
    ResourceDictionaryLocation.None,            //where theme specific resource dictionaries are located
                                                //(used if a resource is not found in the page,
                                                // or application resource dictionaries)
    ResourceDictionaryLocation.SourceAssembly   //where the generic resource dictionary is located
                                                //(used if a resource is not found in the page,
                                                // app, or any theme specific resource dictionaries)
)]


//https://github.dev/Kinnara/PersianDateTimeWPFTools

//[assembly: InternalsVisibleTo("PersianDateTimeWPFTools.Controls")]
//[assembly: InternalsVisibleTo("PersianDateTimeWPFTools.MahApps")]
//[assembly: InternalsVisibleTo("MUXControlsTestApp")]

[assembly: XmlnsPrefix("https://github.com/Behnam2064/PersianDateTimeWPFTools", "pdtt")]
[assembly: XmlnsDefinition("https://github.com/Behnam2064/PersianDateTimeWPFTools", "PersianDateTimeWPFTools")]
[assembly: XmlnsDefinition("https://github.com/Behnam2064/PersianDateTimeWPFTools", "PersianDateTimeWPFTools.Tools")]
[assembly: XmlnsDefinition("https://github.com/Behnam2064/PersianDateTimeWPFTools", "PersianDateTimeWPFTools.Controls")]
[assembly: XmlnsDefinition("https://github.com/Behnam2064/PersianDateTimeWPFTools", "PersianDateTimeWPFTools.Windows.Controls")]
[assembly: XmlnsDefinition("https://github.com/Behnam2064/PersianDateTimeWPFTools", "PersianDateTimeWPFTools.Windows.Controls.Primitives")]