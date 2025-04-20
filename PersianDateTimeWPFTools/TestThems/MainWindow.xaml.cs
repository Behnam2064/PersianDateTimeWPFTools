using PersianDateTimeWPFTools.Controls;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TestThems
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {


        public DateTime DisplayDateTime
        {
            get { return (DateTime)GetValue(DisplayDateTimeProperty); }
            set { SetValue(DisplayDateTimeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DisplayDateTime.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DisplayDateTimeProperty =
            DependencyProperty.Register("DisplayDateTime", typeof(DateTime), typeof(MainWindow), new PropertyMetadata(DateTime.Now));


        public MainWindow()
        {
            InitializeComponent();
        }

        private void cbIsStretch_Checked(object sender, RoutedEventArgs e)
        {
            VerticalAlignment = VerticalAlignment.Stretch;
            HorizontalAlignment = HorizontalAlignment.Stretch;
            if (cbIsStretch.IsChecked == false)
            {
                HorizontalAlignment = HorizontalAlignment.Center;
                VerticalAlignment = VerticalAlignment.Center;
            }
            

            foreach (var item in FindVisualChildren<PersianCalendar>(this))
            {
                item.VerticalAlignment = VerticalAlignment;
                item.HorizontalAlignment = HorizontalAlignment; 
            }
        }

        public static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj == null) yield return (T)Enumerable.Empty<T>();
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
            {
                DependencyObject ithChild = VisualTreeHelper.GetChild(depObj, i);
                if (ithChild == null) continue;
                if (ithChild is T t) yield return t;
                foreach (T childOfChild in FindVisualChildren<T>(ithChild)) yield return childOfChild;
            }
        }
    }
}