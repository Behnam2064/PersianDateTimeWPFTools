using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TestPersianCalendar
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            pdp.SelectedDate = DateTime.Now;
            //new Test01(pc1);

            //pc1.BlackoutDates.Add(new PersianDateTimeWPFTools.Windows.Controls.CalendarDateRange(DateTime.Now.AddDays(+5), DateTime.Now.AddDays(+10)));
            //pc1.BlackoutDates.Add(new PersianDateTimeWPFTools.Windows.Controls.CalendarDateRange(DateTime.Now));

        }
    }
}