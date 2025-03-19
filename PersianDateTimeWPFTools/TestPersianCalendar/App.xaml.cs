using System.Configuration;
using System.Data;
using System.Globalization;
using System.Windows;

namespace TestPersianCalendar
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private bool IsUsCuture = true;
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            /*if (IsUsCuture)
            {
                CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-US");
                CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("en-US");
            }
            else
            {
                CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("fa-IR");
                CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("fa-IR");
            }

            Thread.CurrentThread.CurrentCulture = CultureInfo.DefaultThreadCurrentCulture;*/
        }
    }

}
