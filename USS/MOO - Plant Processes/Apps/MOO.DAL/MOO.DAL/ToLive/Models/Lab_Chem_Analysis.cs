using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MOO.DAL.ToLive.Models.DVFE_Compliance;

namespace MOO.DAL.ToLive.Models
{
    public sealed class Lab_Chem_Analysis
    {
        public Lab_Chem_Analysis()
        {

        }

        /// <summary>
        /// Creates a new lab chem analysis record for inserting based off of the Lab Chem Type ID and analysis Date
        /// </summary>
        /// <param name="LabChemTypeId"></param>
        /// <param name="AnalysisDate"></param>
        /// <param name="Plant"></param>
        /// <param name="Area"></param>
        public Lab_Chem_Analysis(int LabChemTypeId, DateTime AnalysisDate, MOO.Plant Plant, MOO.Area Area)
        {
            Lab_Chem_Type = MOO.DAL.ToLive.Services.Lab_Chem_TypeSvc.Get(LabChemTypeId);
            Analysis_Date = AnalysisDate;
            CalcShift12(Plant, Area);
            CalcShift8(Plant);
        }

        public int Lab_Chem_Analysis_Id { get; set; }
        public Lab_Chem_Type Lab_Chem_Type { get; set; }
        public short? Line_Nbr { get; set; }
        public DateTime Analysis_Date { get; set; }
        public string Approved_By { get; set; }
        public DateTime? Approval_Date { get; set; }
        /// <summary>
        /// Sample Manager Authorized Date
        /// </summary>
        public DateTime? SM_Authorized_Date { get; set; }
        public int? SampleMgr_Id { get; set; }
        public DateTime Shift_Date8 { get; set; }
        public short Shift_Nbr8 { get; set; }
        public DateTime Shift_Date12 { get; set; }
        public short Shift_Nbr12 { get; set; }
        public DateTime Update_Date { get; set; } = DateTime.Now;
        public string Last_Update_By { get; set; }
        public double? Fe { get; set; }
        public double? SiO2 { get; set; }
        public double? CaO { get; set; }
        public double? Al2O3 { get; set; }
        public double? Mn { get; set; }
        public double? MgO { get; set; }
        public double? P2O5 { get; set; }
        public double? TiO2 { get; set; }
        public double? Recovery { get; set; }
        public string Custom1 { get; set; } = "";
        public string Custom2 { get; set; } = "";

        /// <summary>
        /// Calculates the shift12 date and shift number based on analysis date
        /// </summary>
        public void CalcShift12(MOO.Plant plant, MOO.Area area)
        {
            Shift_Date12 = MOO.Shifts.Shift.ShiftDay(plant, area, Analysis_Date);
            Shift_Nbr12 = MOO.Shifts.Shift.ShiftNumber(plant, area, Analysis_Date);
        }


        /// <summary>
        /// Calculates the shift8 date and shift number based on analysis date
        /// </summary>
        public void CalcShift8(MOO.Plant plant)
        {
            Shift_Date8 = MOO.Shifts.Shift8.ShiftDate(Analysis_Date, plant);
            Shift_Nbr8 = MOO.Shifts.Shift8.ShiftNumber(Analysis_Date, plant);
        }

    }
}
