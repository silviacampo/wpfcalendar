using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MonthCalendar;

namespace MonthCalendarTest
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Loaded += Window1_Loaded;
        }

        private List<Appointment> _myAppointmentsList = new List<Appointment>();

        private void Window1_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            Random rand = new Random(System.DateTime.Now.Second);

            for (int i = 1; i <= 50; i++)
            {
                Appointment apt = new Appointment();
                apt.AppointmentID = i;
                apt.StartTime = new System.DateTime(System.DateTime.Now.Year, rand.Next(1, 12), rand.Next(1, System.DateTime.DaysInMonth(System.DateTime.Now.Year, System.DateTime.Now.Month)));
                apt.EndTime = apt.StartTime;
                apt.Subject = "Random apt, blah blah";
                _myAppointmentsList.Add(apt);
            }

            SetAppointments();
        }

        private void DayDoubleClicked(NewAppointmentEventArgs e)
        {
            MessageBox.Show("You double-clicked on day " + Convert.ToDateTime(e.StartDate).ToShortDateString(), "Calendar Event", MessageBoxButton.OK);
        }

        private void AppointmentDoubleClicked(int Appointment_Id)
        {
            MessageBox.Show("You double-clicked on appointment with ID = " + Appointment_Id, "Calendar Event", MessageBoxButton.OK);
        }

        private void MonthChanged(MonthChangedEventArgs e)
        {
            SetAppointments();
        }

        private void SetAppointments()
        {
            AptCalendar.MonthAppointments = _myAppointmentsList.FindAll(new System.Predicate<Appointment>((Appointment apt) => apt.StartTime != null && Convert.ToDateTime(apt.StartTime).Month == this.AptCalendar.DisplayStartDate.Month && Convert.ToDateTime(apt.StartTime).Year == this.AptCalendar.DisplayStartDate.Year));
        }
    }
}

