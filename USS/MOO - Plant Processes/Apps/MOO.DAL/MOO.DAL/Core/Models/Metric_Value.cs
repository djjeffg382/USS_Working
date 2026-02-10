using System;
using System.Collections.Generic;

namespace MOO.DAL.Core.Models
{
    public class Metric_Value
    {
        public long Metric_Id { get; set; }

        private DateTime _start_Date_No_DST;

        /// <summary>
        /// Date time in Standard time (No Daylight time)
        /// </summary>
        public DateTime Start_Date_No_DST
        {
            get { return _start_Date_No_DST; }
            set
            {
                //the date is a primary key so we need to truncate the milliseconds
                _start_Date_No_DST = new DateTime(value.Year, value.Month, value.Day,
                                      value.Hour, value.Minute, value.Second);
            }
        }

        private DateTime _start_Date;

        /// <summary>
        /// Date time of the value in local time (adjusted for Daylight time if in daylight time)
        /// </summary>
        public DateTime Start_Date
        {
            get { return _start_Date; }
            set
            {
                //truncate the milliseconds
                _start_Date = new DateTime(value.Year, value.Month, value.Day,
                                      value.Hour, value.Minute, value.Second);
            }
        }

        /// <summary>
        /// Shift Number
        /// </summary>
        public byte? Shift { get; set; }

        /// <summary>
        /// Shift Half
        /// </summary>
        public byte? Half { get; set; }

        /// <summary>
        /// Shift hour
        /// </summary>
        public byte? Hour { get; set; }

        /// <summary>
        /// Shift Date
        /// </summary>
        public DateTime? Shift_Date { get; set; }

        /// <summary>
        /// Date value was inserted
        /// </summary>
        public DateTime? Inserted_Date { get; set; } = DateTime.Now;

        /// <summary>
        /// Reference to the approval record if data was approved
        /// </summary>
        public int? Approval_Id { get
            {
                if (Approval != null)
                    return (int)Approval.Approval_Id;
                else
                    return null;
            }
        }


        public Approval Approval { get; set; }

        /// <summary>
        /// Decimal value of the record
        /// </summary>
        public decimal? Value { get; set; }

        public decimal? Quality { get; set; }

        /// <summary>
        /// Last updated date
        /// </summary>
        public DateTime? Update_Date { get; set; } = DateTime.Now;

        /// <summary>
        /// String value if this metric is a string
        /// </summary>
        public string Value_Str { get; set; }

        private List<Metric_Value_Audit> _audit_Records;

        public List<Metric_Value_Audit> Audit_Records
        {
            get
            {
                if (_audit_Records == null)
                    _audit_Records = Services.Metric_Value_AuditSvc.GetAuditsForMetricValue(Metric_Id, Start_Date_No_DST);
                return _audit_Records;
            }
        }

        private void SetShiftVals(MOO.Plant plant, MOO.Area area, DateTime startDate, bool use8HourShift, int metric_id)
        {
            Metric_Id = metric_id;
            Start_Date = startDate;

            if (startDate.IsDaylightSavingTime())
                Start_Date_No_DST = Start_Date.AddHours(-1);
            else
                Start_Date_No_DST = Start_Date;
            if (use8HourShift)
            {
                Shift_Date = MOO.Shifts.Shift8.ShiftDate(startDate, plant);
                Shift = MOO.Shifts.Shift8.ShiftNumber(startDate, plant);
                Half = MOO.Shifts.Shift8.HalfShift(startDate, plant);
                Hour = MOO.Shifts.Shift8.ShiftHour(startDate, plant);
            }
            else
            {
                Shift_Date = MOO.Shifts.Shift.ShiftDay(plant, area, startDate);
                Shift = MOO.Shifts.Shift.ShiftNumber(plant, area, startDate);
                Hour = MOO.Shifts.Shift.ShiftHour(plant, area, startDate);
            }
        }

        public Metric_Value()
        {
        }

        public Metric_Value(MOO.Plant plant, MOO.Area area, DateTime startDate, bool use8HourShift, int metric_id, decimal value)
        {
            SetShiftVals(plant, area, startDate, use8HourShift, metric_id);
            Value = value;
        }

        public Metric_Value(MOO.Plant plant, MOO.Area area, DateTime startDate, bool use8HourShift, int metric_id, string value)
        {
            SetShiftVals(plant, area, startDate, use8HourShift, metric_id);
            Value_Str = value;
        }
    }
}