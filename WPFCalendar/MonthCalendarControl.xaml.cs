using System.Globalization;
using System.Windows;
using System.Windows.Media;
using System.Windows.Input;
using System.Collections.Generic;
using System.Windows.Controls;
using System;

namespace MonthCalendar
{
    /// <summary>
    /// Interaction logic for MonthCalendarControl.xaml
    /// </summary>

public partial class MonthCalendarControl : UserControl
{

	internal System.DateTime _DisplayStartDate = System.DateTime.Now.AddDays(-1 * (System.DateTime.Now.Day - 1));
	private int _DisplayMonth;
	private int _DisplayYear;
	private CultureInfo _cultureInfo = new CultureInfo(CultureInfo.CurrentUICulture.LCID);
	private System.Globalization.Calendar sysCal;

	private List<Appointment> _monthAppointments;
	public event MonthChangedEventHandler OnMonthChanged;
	public delegate void MonthChangedEventHandler(MonthChangedEventArgs e);
	public event DayDoubleClickedEventHandler OnDayDoubleClicked;
	public delegate void DayDoubleClickedEventHandler(NewAppointmentEventArgs e);
	public event AppointmentDoubleClickedEventHandler OnAppointmentDoubleClicked;
	public delegate void AppointmentDoubleClickedEventHandler(int Appointment_Id);

    public MonthCalendarControl()
    {
        InitializeComponent();

    	_DisplayMonth = _DisplayStartDate.Month;
	    _DisplayYear = _DisplayStartDate.Year;
	    _cultureInfo = new CultureInfo(CultureInfo.CurrentUICulture.LCID);
	    sysCal = _cultureInfo.Calendar;
    }

	public System.DateTime DisplayStartDate {
		get { return _DisplayStartDate; }
		set {
			_DisplayStartDate = value;
			_DisplayMonth = _DisplayStartDate.Month;
			_DisplayYear = _DisplayStartDate.Year;
		}
	}

	public List<Appointment> MonthAppointments {
		get { return _monthAppointments; }
		set {
			_monthAppointments = value;
			BuildCalendarUI();
		}
	}

	private void MonthView_Loaded(object sender, RoutedEventArgs e)
	{
		//-- Want to have the calendar show up, even if no appoints are assigned 
		//   Note - in my own app, appointments are loaded by a backgroundWorker thread to avoid a laggy UI
		if (_monthAppointments == null)
			BuildCalendarUI();
	}

	private void BuildCalendarUI()
	{
		int iDaysInMonth = sysCal.GetDaysInMonth(_DisplayStartDate.Year, _DisplayStartDate.Month);
		int iOffsetDays = Convert.ToInt32(System.Enum.ToObject(typeof(System.DayOfWeek), _DisplayStartDate.DayOfWeek));
		int iWeekCount = 0;
		WeekOfDaysControl weekRowCtrl = new WeekOfDaysControl();

		MonthViewGrid.Children.Clear();
		AddRowsToMonthGrid(iDaysInMonth, iOffsetDays);
        MonthYearLabel.Content = (new DateTimeFormatInfo()).GetMonthName(_DisplayMonth) + " " + _DisplayYear;
        //CultureInfo.CurrentUICulture.DateTimeFormat.MonthNames[i]

		for (int i = 1; i <= iDaysInMonth; i++) {
			if ((i != 1) && System.Math.IEEERemainder((i + iOffsetDays - 1), 7) == 0) {
				//-- add existing weekrowcontrol to the monthgrid
				Grid.SetRow(weekRowCtrl, iWeekCount);
				MonthViewGrid.Children.Add(weekRowCtrl);
				//-- make a new weekrowcontrol
				weekRowCtrl = new WeekOfDaysControl();
				iWeekCount += 1;
			}

			//-- load each weekrow with a DayBoxControl whose label is set to day number
			DayBoxControl dayBox = new DayBoxControl();
			dayBox.DayNumberLabel.Content = i.ToString();
			dayBox.Tag = i;
			dayBox.MouseDoubleClick += DayBox_DoubleClick;

			//-- customize daybox for today:
			if ((new System.DateTime(_DisplayYear, _DisplayMonth, i)) == System.DateTime.Today) {
				dayBox.DayLabelRowBorder.Background = (Brush)dayBox.TryFindResource("OrangeGradientBrush");
				dayBox.DayAppointmentsStack.Background = Brushes.Wheat;
			}

			//-- for design mode, add appointments to random days for show...
			if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this)) {
				if (Math.Round(1d) < 0.25) {
					DayBoxAppointmentControl apt = new DayBoxAppointmentControl();
					apt.DisplayText.Text = "Apt on " + i + "th";
					dayBox.DayAppointmentsStack.Children.Add(apt);
				}

			} else if (_monthAppointments != null) {
				//-- Compiler warning about unpredictable results if using i (the iterator) in lambda, the 
				//   "hint" suggests declaring another var and set equal to iterator var
				int iday = i;
				List<Appointment> aptInDay = _monthAppointments.FindAll(new System.Predicate<Appointment>((Appointment apt) => Convert.ToDateTime(apt.StartTime).Day == iday));
				foreach (Appointment a in aptInDay) {
					DayBoxAppointmentControl apt = new DayBoxAppointmentControl();
					apt.DisplayText.Text = a.Subject;
					apt.Tag = a.AppointmentID;
					apt.MouseDoubleClick += Appointment_DoubleClick;
					dayBox.DayAppointmentsStack.Children.Add(apt);
				}

			}

			Grid.SetColumn(dayBox, (i - (iWeekCount * 7)) + iOffsetDays);
			weekRowCtrl.WeekRowGrid.Children.Add(dayBox);
		}
		Grid.SetRow(weekRowCtrl, iWeekCount);
		MonthViewGrid.Children.Add(weekRowCtrl);
	}

	private void AddRowsToMonthGrid(int DaysInMonth, int OffSetDays)
	{
		MonthViewGrid.RowDefinitions.Clear();
		System.Windows.GridLength rowHeight = new System.Windows.GridLength(60, System.Windows.GridUnitType.Star);

		int EndOffSetDays = 7 - (Convert.ToInt32(System.Enum.ToObject(typeof(System.DayOfWeek), _DisplayStartDate.AddDays(DaysInMonth - 1).DayOfWeek)) + 1);

		for (int i = 1; i <= Convert.ToInt32((DaysInMonth + OffSetDays + EndOffSetDays) / 7); i++) {
			dynamic rowDef = new RowDefinition();
			rowDef.Height = rowHeight;
			MonthViewGrid.RowDefinitions.Add(rowDef);
		}
	}

	private void UpdateMonth(int MonthsToAdd)
	{
		MonthChangedEventArgs ev = new MonthChangedEventArgs();
		ev.OldDisplayStartDate = _DisplayStartDate;
		this.DisplayStartDate = _DisplayStartDate.AddMonths(MonthsToAdd);
		ev.NewDisplayStartDate = _DisplayStartDate;
		if (OnMonthChanged != null) {
			OnMonthChanged(ev);
		}
	}

	#region " UI Event Handlers "

	private void MonthGoPrev_MouseLeftButtonUp(System.Object sender, MouseButtonEventArgs e)
	{
		UpdateMonth(-1);
	}

	private void MonthGoNext_MouseLeftButtonUp(System.Object sender, MouseButtonEventArgs e)
	{
		UpdateMonth(1);
	}

	private void Appointment_DoubleClick(object sender, MouseButtonEventArgs e)
	{
		if (e.Source is DayBoxAppointmentControl) 
        {
			if (((DayBoxAppointmentControl)e.Source).Tag != null) 
            {
				//-- You could put your own call to your appointment-displaying code or whatever here..
				if (OnAppointmentDoubleClicked != null) 
					OnAppointmentDoubleClicked(Convert.ToInt32(((DayBoxAppointmentControl)e.Source).Tag));
			}
			e.Handled = true;
		}
	}

	private void DayBox_DoubleClick(object sender, MouseButtonEventArgs e)
	{
		//-- call to FindVisualAncestor to make sure they didn't click on existing appointment (in which case,
		//   that appointment window is already opened by handler Appointment_DoubleClick)

		if (e.Source is DayBoxControl && Utilities.FindVisualAncestor(typeof(DayBoxAppointmentControl), (Visual)e.OriginalSource) == null) {
			NewAppointmentEventArgs ev = new NewAppointmentEventArgs();
			if (((DayBoxControl)e.Source).Tag != null) {
				ev.StartDate = new System.DateTime(_DisplayYear, _DisplayMonth, Convert.ToInt32(((DayBoxControl)e.Source).Tag), 10, 0, 0);
				ev.EndDate = Convert.ToDateTime(ev.StartDate).AddHours(2);
			}
			if (OnDayDoubleClicked != null) {
				OnDayDoubleClicked(ev);
			}
			e.Handled = true;
		}
	}

	public void MonthView()
	{
		Loaded += MonthView_Loaded;
	}

	#endregion

}

public struct MonthChangedEventArgs
{
	public System.DateTime OldDisplayStartDate;
	public System.DateTime NewDisplayStartDate;
}

public struct NewAppointmentEventArgs
{
	public System.DateTime? StartDate;
	public System.DateTime? EndDate;
	public int? CandidateId;
	public int? RequirementId;
}

class Utilities
{
	//-- Many thanks to Bea Stollnitz, on whose blog I found the original C# version of below in a drag-drop helper class... 
	public static FrameworkElement FindVisualAncestor(System.Type ancestorType, System.Windows.Media.Visual visual)
	{
		while ((visual != null && !ancestorType.IsInstanceOfType(visual))) 
			visual = (System.Windows.Media.Visual)System.Windows.Media.VisualTreeHelper.GetParent(visual);
		return (FrameworkElement)visual;
	}
}
}
