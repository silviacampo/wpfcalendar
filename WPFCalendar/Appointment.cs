using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MonthCalendar
{
/// <summary>
/// This class is actually a stripped-down version of the actual Appointment class, which was generated using the 
/// Linq-to-SQL Designer (essentially a Linq ORM to the Appointments table in the db)
/// </summary>
/// <remarks>Obviously, you should use your own appointment object/classes, and change the code-behind in MonthView.xaml.vb to
/// support a List(Of T) where T is whatever the System.Type is for your appointment class.
/// </remarks>
/// <author>Kirk Davis, February 2009 (in like, 4 hours, and it shows!)</author>
    public class Appointment
    {

        private int _AppointmentID;
        private string _Subject;
        private string _Location;
        private string _Details;
        private System.Nullable<System.DateTime> _StartTime;
        private System.Nullable<System.DateTime> _EndTime;

        private System.DateTime _reccreatedDate;

        public Appointment()
            : base()
        {
        }

        public int AppointmentID
        {
            get { return this._AppointmentID; }
            set
            {
                if (((this._AppointmentID == value) == false))
                {
                    this._AppointmentID = value;
                }
            }
        }

        public string Subject
        {
            get { return this._Subject; }
            set
            {
                if ((string.Equals(this._Subject, value) == false))
                {
                    this._Subject = value;
                }
            }
        }

        public string Location
        {
            get { return this._Location; }
            set
            {
                if ((string.Equals(this._Location, value) == false))
                {
                    this._Location = value;
                }
            }
        }

        public string Details
        {
            get { return this._Details; }
            set
            {
                if ((string.Equals(this._Details, value) == false))
                {
                    this._Details = value;
                }
            }
        }

        public System.Nullable<System.DateTime> StartTime
        {
            get { return this._StartTime; }
            set
            {
                if ((this._StartTime.Equals(value) == false))
                {
                    this._StartTime = value;
                }
            }
        }

        public System.Nullable<System.DateTime> EndTime
        {
            get { return this._EndTime; }
            set
            {
                if ((this._EndTime.Equals(value) == false))
                {
                    this._EndTime = value;
                }
            }
        }

        public System.DateTime reccreatedDate
        {
            get { return this._reccreatedDate; }
            set
            {
                if (((this._reccreatedDate == value) == false))
                {
                    this._reccreatedDate = value;
                }
            }
        }
    }
}
