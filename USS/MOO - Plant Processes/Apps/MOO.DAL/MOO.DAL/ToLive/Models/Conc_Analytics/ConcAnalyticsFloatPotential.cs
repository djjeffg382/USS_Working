using MOO.DAL.ToLive.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.ToLive.Models
{
    /// <summary>
    /// A wrapper for the conc_analytics_general record.  This still has value1 - value5 but has additional properties that give a better name to those
    /// </summary>
    public sealed class ConcAnalyticsFloatPotential : Conc_Analytics_General
    {
        public ConcAnalyticsFloatPotential()
        {
            Data_Group = ConcAnalyticsGnrlGroup.Floatation_Potential;
            Line_Nbr = 0;
        }
        public ConcAnalyticsFloatPotential(Conc_Analytics_General obj)
        {
            Data_Group = ConcAnalyticsGnrlGroup.Floatation_Potential;
            Line_Nbr = 0;
            Sort_Order = obj.Sort_Order;
            Value1 = obj.Value1;
            Value2 = obj.Value2;
            Value3 = obj.Value3;
            Value4 = obj.Value4;
            Value5 = obj.Value5;
            Value6 = obj.Value6;
            Value7 = obj.Value7;
            Value8 = obj.Value8;
            Value9 = obj.Value9;
        }
        public DateTime UpdateTime
        {
            get
            {
                if (DateTime.TryParse(Value1, out DateTime newVal))
                    return newVal;
                else
                    return DateTime.MinValue;
            }
            set
            {
                Value1 = value.ToString("MM/dd/yyyy HH:mm:ss");
            }
        }

        public string ImpactArea
        {
            get
            {
                return Value2;
            }
            set
            {
                Value2 = value;
            }
        }
        public string SectionName
        {
            get
            {
                return Value3;
            }
            set
            {
                Value3 = value;
            }
        }
        public double PreviousDay
        {
            get
            {
                if (double.TryParse(Value4, out double newVal))
                    return newVal;
                else
                    return 0;
            }
            set
            {
                Value4 = value.ToString();
            }
        }
        public double Previous7Day
        {
            get
            {
                if (double.TryParse(Value5, out double newVal))
                    return newVal;
                else
                    return 0;
            }
            set
            {
                Value5 = value.ToString();
            }
        }

        public double Previous30Day
        {
            get
            {
                if (double.TryParse(Value6, out double newVal))
                    return newVal;
                else
                    return 0;
            }
            set
            {
                Value6 = value.ToString();
            }
        }



        //Set points average

        public double PreviousDaySetpoint
        {
            get
            {
                if (double.TryParse(Value7, out double newVal))
                    return newVal;
                else
                    return 0;
            }
            set
            {
                Value7 = value.ToString();
            }
        }

        public double Previous7DaySetpoint
        {
            get
            {
                if (double.TryParse(Value8, out double newVal))
                    return newVal;
                else
                    return 0;
            }
            set
            {
                Value8 = value.ToString();
            }
        }

        public double Previous30DaySetpoint
        {
            get
            {
                if (double.TryParse(Value9, out double newVal))
                    return newVal;
                else
                    return 0;
            }
            set
            {
                Value9 = value.ToString();
            }
        }


    }
}
