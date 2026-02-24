using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.ToLive.Models
{
    public sealed class Lab_Phys_Analysis
    {

        public Lab_Phys_Analysis() { }

        /// <summary>
        /// Creates a new lab phys analysis record for inserting based off of the Lab Chem Type ID and analysis Date
        /// </summary>
        /// <param name="LabChemTypeId"></param>
        /// <param name="AnalysisDate"></param>
        /// <param name="Plant"></param>
        /// <param name="Area"></param>
        public Lab_Phys_Analysis(int LabPhysTypeId, DateTime AnalysisDate, MOO.Plant Plant, MOO.Area Area)
        {
            Lab_Phys_Type = MOO.DAL.ToLive.Services.Lab_Phys_TypeSvc.Get(LabPhysTypeId);
            Analysis_Date = AnalysisDate;
            CalcShift12(Plant, Area);
            CalcShift8(Plant);
        }

        public int Lab_Phys_Analysis_Id { get; set; }
        public Lab_Phys_Type Lab_Phys_Type { get; set; }
        public int? Line_Nbr { get; set; }
        public DateTime Analysis_Date { get; set; }
        public DateTime Shift_Date8 { get; set; }
        public short Shift_Nbr8 { get; set; }
        public DateTime Shift_Date12 { get; set; }
        public short Shift_Nbr12 { get; set; }
        public DateTime Update_Date { get; set; } = DateTime.Now;
        public string Last_Update_By { get; set; }
        public string? Authorized_By { get; set; }
        public bool Defaults_Used { get; set; }
        public byte? Shift_Half8 { get; set; }
        public double? Start_Wgt { get; set; }
        public double? Inch_1_Wgt { get; set; }
        public double? Inch_1_Pct { get { return Start_Wgt > 0 && Inch_1_Wgt.HasValue ? Math.Round(Inch_1_Wgt.GetValueOrDefault(0) / Start_Wgt.Value * 100, 4, MidpointRounding.AwayFromZero) : null; } }
        public double? Inch_3_4_Wgt { get; set; }
        public double? Inch_3_4_Pct { get { return Start_Wgt > 0 && Inch_3_4_Wgt.HasValue ? Math.Round(Inch_3_4_Wgt.GetValueOrDefault(0) / Start_Wgt.Value * 100, 4, MidpointRounding.AwayFromZero) : null; } }
        public double? Inch_5_8_Wgt { get; set; }
        public double? Inch_5_8_Pct { get { return Start_Wgt > 0 && Inch_5_8_Wgt.HasValue ? Math.Round(Inch_5_8_Wgt.GetValueOrDefault(0) / Start_Wgt.Value * 100, 4, MidpointRounding.AwayFromZero) : null; } }
        public double? Inch_9_16_Wgt { get; set; }
        public double? Inch_9_16_Pct { get { return Start_Wgt > 0 && Inch_9_16_Wgt.HasValue ? Math.Round(Inch_9_16_Wgt.GetValueOrDefault(0) / Start_Wgt.Value * 100, 4, MidpointRounding.AwayFromZero) : null; } }
        public double? Inch_1_2_Wgt { get; set; }
        public double? Inch_1_2_Pct { get { return Start_Wgt > 0 && Inch_1_2_Wgt.HasValue ? Math.Round(Inch_1_2_Wgt.GetValueOrDefault(0) / Start_Wgt.Value * 100, 4, MidpointRounding.AwayFromZero) : null; } }
        public double? Inch_7_16_Wgt { get; set; }
        public double? Inch_7_16_Pct { get { return Start_Wgt > 0 && Inch_7_16_Wgt.HasValue ? Math.Round(Inch_7_16_Wgt.GetValueOrDefault(0) / Start_Wgt.Value * 100, 4, MidpointRounding.AwayFromZero) : null; } }
        public double? Inch_3_8_Wgt { get; set; }
        public double? Inch_3_8_Pct { get { return Start_Wgt > 0 && Inch_3_8_Wgt.HasValue ? Math.Round(Inch_3_8_Wgt.GetValueOrDefault(0) / Start_Wgt.Value * 100, 4, MidpointRounding.AwayFromZero) : null; } }
        public double? Inch_1_4_Wgt { get; set; }
        public double? Inch_1_4_Pct { get { return Start_Wgt > 0 && Inch_1_4_Wgt.HasValue ? Math.Round(Inch_1_4_Wgt.GetValueOrDefault(0) / Start_Wgt.Value * 100, 4, MidpointRounding.AwayFromZero) : null; } }
        public double? Mesh_3_Wgt { get; set; }
        public double? Mesh_3_Pct { get { return Start_Wgt > 0 && Mesh_3_Wgt.HasValue ? Math.Round(Mesh_3_Wgt.GetValueOrDefault(0) / Start_Wgt.Value * 100, 4, MidpointRounding.AwayFromZero) : null; } }
        public double? Mesh_4_Wgt { get; set; }
        public double? Mesh_4_Pct { get { return Start_Wgt > 0 && Mesh_4_Wgt.HasValue ? Math.Round(Mesh_4_Wgt.GetValueOrDefault(0) / Start_Wgt.Value * 100, 4, MidpointRounding.AwayFromZero) : null; } }
        public double? Mesh_6_Wgt { get; set; }
        public double? Mesh_6_Pct { get { return Start_Wgt > 0 && Mesh_6_Wgt.HasValue ? Math.Round(Mesh_6_Wgt.GetValueOrDefault(0) / Start_Wgt.Value * 100, 4, MidpointRounding.AwayFromZero) : null; } }
        public double? Mesh_8_Wgt { get; set; }
        public double? Mesh_8_Pct { get { return Start_Wgt > 0 && Mesh_8_Wgt.HasValue ? Math.Round(Mesh_8_Wgt.GetValueOrDefault(0) / Start_Wgt.Value * 100, 4, MidpointRounding.AwayFromZero) : null; } }
        public double? Mesh_10_12_Wgt { get; set; }
        public double? Mesh_10_12_Pct { get { return Start_Wgt > 0 && Mesh_10_12_Wgt.HasValue ? Math.Round(Mesh_10_12_Wgt.GetValueOrDefault(0) / Start_Wgt.Value * 100, 4, MidpointRounding.AwayFromZero) : null; } }
        public double? Mesh_14_16_Wgt { get; set; }
        public double? Mesh_14_16_Pct { get { return Start_Wgt > 0 && Mesh_14_16_Wgt.HasValue ? Math.Round(Mesh_14_16_Wgt.GetValueOrDefault(0) / Start_Wgt.Value * 100, 4, MidpointRounding.AwayFromZero) : null; } }
        public double? Mesh_20_Wgt { get; set; }
        public double? Mesh_20_Pct { get { return Start_Wgt > 0 && Mesh_20_Wgt.HasValue ? Math.Round(Mesh_20_Wgt.GetValueOrDefault(0) / Start_Wgt.Value * 100, 4, MidpointRounding.AwayFromZero) : null; } }
        public double? Mesh_28_30_Wgt { get; set; }
        public double? Mesh_28_30_Pct { get { return Start_Wgt > 0 && Mesh_28_30_Wgt.HasValue ? Math.Round(Mesh_28_30_Wgt.GetValueOrDefault(0) / Start_Wgt.Value * 100, 4, MidpointRounding.AwayFromZero) : null; } }
        public double? Mesh_35_40_Wgt { get; set; }
        public double? Mesh_35_40_Pct { get { return Start_Wgt > 0 && Mesh_35_40_Wgt.HasValue ? Math.Round(Mesh_35_40_Wgt.GetValueOrDefault(0) / Start_Wgt.Value * 100, 4, MidpointRounding.AwayFromZero) : null; } }
        public double? Mesh_48_50_Wgt { get; set; }
        public double? Mesh_48_50_Pct { get { return Start_Wgt > 0 && Mesh_48_50_Wgt.HasValue ? Math.Round(Mesh_48_50_Wgt.GetValueOrDefault(0) / Start_Wgt.Value * 100, 4, MidpointRounding.AwayFromZero) : null; } }
        public double? Mesh_65_70_Wgt { get; set; }
        public double? Mesh_65_70_Pct { get { return Start_Wgt > 0 && Mesh_65_70_Wgt.HasValue ? Math.Round(Mesh_65_70_Wgt.GetValueOrDefault(0) / Start_Wgt.Value * 100, 4, MidpointRounding.AwayFromZero) : null; } }
        public double? Mesh_100_Wgt { get; set; }
        public double? Mesh_100_Pct { get { return Start_Wgt > 0 && Mesh_100_Wgt.HasValue ? Math.Round(Mesh_100_Wgt.GetValueOrDefault(0) / Start_Wgt.Value * 100, 4, MidpointRounding.AwayFromZero) : null; } }
        public double? Mesh_150_140_Wgt { get; set; }
        public double? Mesh_150_140_Pct { get { return Start_Wgt > 0 && Mesh_150_140_Wgt.HasValue ? Math.Round(Mesh_150_140_Wgt.GetValueOrDefault(0) / Start_Wgt.Value * 100, 4, MidpointRounding.AwayFromZero) : null; } }
        public double? Mesh_200_Wgt { get; set; }
        public double? Mesh_200_Pct { get { return Start_Wgt > 0 && Mesh_200_Wgt.HasValue ? Math.Round(Mesh_200_Wgt.GetValueOrDefault(0) / Start_Wgt.Value * 100, 4, MidpointRounding.AwayFromZero) : null; } }
        public double? Mesh_270_Wgt { get; set; }
        public double? Mesh_270_Pct { get { return Start_Wgt > 0 && Mesh_270_Wgt.HasValue ? Math.Round(Mesh_270_Wgt.GetValueOrDefault(0) / Start_Wgt.Value * 100, 4, MidpointRounding.AwayFromZero) : null; } }
        public double? Mesh_325_Wgt { get; set; }
        public double? Mesh_325_Pct { get { return Start_Wgt > 0 && Mesh_325_Wgt.HasValue ? Math.Round(Mesh_325_Wgt.GetValueOrDefault(0) / Start_Wgt.Value * 100, 4, MidpointRounding.AwayFromZero) : null; } }
        public double? Mesh_400_Wgt { get; set; }
        public double? Mesh_400_Pct { get { return Start_Wgt > 0 && Mesh_400_Wgt.HasValue ? Math.Round(Mesh_400_Wgt.GetValueOrDefault(0) / Start_Wgt.Value * 100, 4, MidpointRounding.AwayFromZero) : null; } }
        public double? Mesh_500_Wgt { get; set; }
        public double? Mesh_500_Pct { get { return Start_Wgt > 0 && Mesh_500_Wgt.HasValue ? Math.Round(Mesh_500_Wgt.GetValueOrDefault(0) / Start_Wgt.Value * 100, 4, MidpointRounding.AwayFromZero) : null; } }

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
