﻿using System.Globalization;
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
            pcwc1.BlackoutDates.Add(new PersianDateTimeWPFTools.Windows.Controls.CalendarDateRange(DateTime.Now.AddDays(2), DateTime.Now.AddDays(2 + 2)));
            pc1.BlackoutDates.Add(pcwc1.BlackoutDates[0]);
            pdp1.BlackoutDates.Add(pcwc1.BlackoutDates[0]);
            pdtp1.BlackoutDates.Add(pcwc1.BlackoutDates[0]);
        }

        private void ToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            var isChecked = (sender as ToggleButton)?.IsChecked ?? true;
            CultureInfo? culture = null;
            if (isChecked)
            {
                culture = CultureInfo.CreateSpecificCulture("en-US");
            }
            else
            {

                culture = CultureInfo.CreateSpecificCulture("fa-IR");
            }

            pcwc1.CustomCulture = culture;
            pc1.CustomCulture = culture;
            pdp1.CustomCulture = culture;
            pdtp1.CustomCulture = culture;

        }
    }
}