using MOO.DAL.Drill.Models;
using System.Data.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MOO.DAL.Drill.Imports
{
    /// <summary>
    /// Main import class used for importing all of the drill systems into the Drill Database
    /// </summary>
    public class MainImport
    {
        private readonly List<Equip> FullDrillList;
        private readonly List<Operator> FullOpList;
        private readonly List<Drill_Bit> BitsUsed = new();
        private readonly List<Pattern> PatternsUsed = new();
        private readonly MOO.Plant Plant;

        public MainImport(MOO.Plant Plant)
        {
            this.Plant = Plant;
            FullDrillList = MOO.DAL.Drill.Services.EquipSvc.GetAll(Plant);
            FullOpList = MOO.DAL.Drill.Services.OperatorSvc.GetAll(Plant);
        }

        /// <summary>
        /// Imports hole data for the specified start to end Date
        /// </summary>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <returns>Returns a tuple list that contains Each Import name with a count of imports</returns>
        private List<Tuple<string, int>> RunImport(DateTime StartDate, DateTime EndDate)
        {
            List<Raw_Drilled_Hole> rdh;
            List<Tuple<string, int>> retVal = new();
            if (Plant == MOO.Plant.Minntac)
            {
                
                /*********************      Epiroc Import        ***********************/
                Imports.EpirocImport e = new(Plant);
                rdh = e.GetRawData(StartDate, EndDate);
                InsertIntoDrillDB(rdh, DrillSystem.Epiroc);
                retVal.Add(new Tuple<string, int>("Epiroc", rdh.Count));
                /**********************************************************************/

                /*********************      Terrain Import        ***********************/
                Imports.TerrainImport t = new(Plant);
                rdh = t.GetRawData(StartDate, EndDate);
                InsertIntoDrillDB(rdh, DrillSystem.Terrain);
                retVal.Add(new Tuple<string, int>("Terrain", rdh.Count));
                /**********************************************************************/
            }
            else if(Plant == MOO.Plant.Keetac)
            {
                /*********************      Wenco Import        ***********************/
                Imports.WencoImport w = new(Plant);
                rdh = w.GetRawData(StartDate, EndDate);
                InsertIntoDrillDB(rdh, DrillSystem.Wenco);
                retVal.Add(new Tuple<string, int>("Wenco", rdh.Count));
                /**********************************************************************/

                /*********************      Epiroc Import        ***********************/
                Imports.EpirocImport e = new(Plant);
                rdh = e.GetRawData(StartDate, EndDate);
                InsertIntoDrillDB(rdh, DrillSystem.Epiroc);
                retVal.Add(new Tuple<string, int>("Epiroc", rdh.Count));
                /**********************************************************************/
            }
            return retVal;
        }

        /// <summary>
        /// Deletes the specified shift in the drill database and reruns the import
        /// </summary>
        /// <param name="ShiftDate"></param>
        /// <param name="Shift"></param>
        /// <returns>Returns a tuple list that contains Each Import name with a count of imports</returns>
        public List<Tuple<string, int>> RunImport(DateTime ShiftDate, byte Shift)
        {
            MOO.DAL.Drill.Services.Drilled_HoleSvc.DeleteShift(Plant, ShiftDate, Shift);
            DateTime[] StartEndTime = MOO.Shifts.Shift.ShiftStartEndTime(Plant, MOO.Area.Drilling, ShiftDate, Shift);
            var retVal = RunImport(StartEndTime[0], StartEndTime[1]);
            return retVal;
        }

        public void InsertIntoDrillDB(List<Raw_Drilled_Hole> RdhList, MOO.DAL.Drill.Models.DrillSystem DrillSys)
        {
            foreach (Raw_Drilled_Hole rdh in RdhList)
            {
                InsertIntoDrillDB(rdh, DrillSys);
            }
        }

        public void InsertIntoDrillDB(Raw_Drilled_Hole Rdh, MOO.DAL.Drill.Models.DrillSystem DrillSys)
        {

            MOO.DAL.Drill.Models.Drilled_Hole dh = new()
            {
                Plant = Rdh.Plant,
                Hole_Number = Rdh.Hole_Number,
                Planned_Depth = Math.Min(Rdh.Planned_Depth,999.99M),
                Drilled_Depth = Math.Min(Rdh.Drilled_Depth, 999.99M),
                Start_Date = Rdh.Start_Date,
                End_Date = Rdh.End_Date,
                Design_Northing = Rdh.Design_Northing,
                Actual_Northing = Rdh.Actual_Northing,
                Design_Easting = Rdh.Design_Easting,
                Actual_Easting = Rdh.Actual_Easting,
                Design_Bottom = Rdh.Design_Bottom,
                Actual_Bottom = Rdh.Actual_Bottom,
                Collar = Rdh.Collar,
                Reference_Key = Rdh.Reference_Key,
                ROP_Ft_Per_Min = Rdh.ROP_Ft_Per_Min

            };


            dh.Material = Rdh.Material;
            //determine shift and shift date
            dh.Shift = MOO.Shifts.Shift.ShiftNumber(Plant, MOO.Area.Drilling, dh.End_Date);
            dh.Shift_Date = MOO.Shifts.Shift.ShiftDay(Plant, MOO.Area.Drilling, dh.End_Date);
            dh.Pattern = GetPattern(Rdh);


            //if this is minntac and our design data is missing, then we can pull this from the Blast Data in DMART
            if (dh.Plant == Plant.Minntac &&
                        (!dh.Design_Bottom.HasValue || !dh.Design_Northing.HasValue || !dh.Design_Bottom.HasValue))
            {
                MOO.DAL.Blast.Models.Pattern_Holes ph = MOO.DAL.Blast.Services.Pattern_HolesSvc.Get(dh.Pattern.Pattern_Number, dh.Hole_Number);
                if (ph != null)
                {
                    dh.Design_Bottom = ph.Bottom_Altitude;
                    dh.Design_Easting = ph.Easting;
                    dh.Design_Northing = ph.Northing;
                }
            }


            if (string.IsNullOrEmpty(Rdh.Operator_Number))
            {
                //no operator was given, use op_number -1 with name Unknown
                dh.Operator = GetOperator("-1", "", "Unknown");
            }
            else
                dh.Operator = GetOperator(Rdh.Operator_Number, Rdh.Operator_First_Name, Rdh.Operator_Last_Name);
            dh.Equip = GetDrill(Rdh.Equipment_Number, DrillSys);
            if (!string.IsNullOrEmpty(Rdh.Drill_Bit_Serial))
                dh.Drill_Bit = GetBit(Rdh.Drill_Bit_Serial);

            MOO.DAL.Drill.Services.Drilled_HoleSvc.Insert(dh);
        }


        private Drill_Bit GetBit(string BitSerial)
        {
            //first check if we have the equip already
            Drill_Bit RetVal;

            RetVal = BitsUsed.FirstOrDefault(x => x.Serial_Number == BitSerial);
            if (RetVal != null)
                return RetVal;
            else
            {
                //haven't got it from the db yet, try to get it now.
                RetVal = MOO.DAL.Drill.Services.Drill_BitSvc.Get(Plant, BitSerial);
                if (RetVal == null)
                {
                    //This is a new pattern so we must create it now
                    RetVal = new()
                    {
                        Plant = Plant,
                        Serial_Number = BitSerial
                    };
                    MOO.DAL.Drill.Services.Drill_BitSvc.Insert(RetVal);
                }
                //add this to our drill list so we don't need to check the DB next time
                BitsUsed.Add(RetVal);
                return RetVal;
            }
        }

        private Operator GetOperator(string Employee_Number, string First_Name, string Last_Name)
        {
            //first check if we have the equip already
            MOO.DAL.Drill.Models.Operator RetVal;

            RetVal = FullOpList.FirstOrDefault(x => x.Employee_Number == Employee_Number);
            if (RetVal != null)
                return RetVal;
            else
            {
                //haven't got it from the db yet, try to get it now.
                RetVal = MOO.DAL.Drill.Services.OperatorSvc.Get(Plant, Employee_Number);
                if (RetVal == null)
                {
                    //This is a new pattern so we must create it now
                    RetVal = new()
                    {
                        Plant = Plant,
                        Active = true,
                        First_Name = First_Name,
                        Employee_Number = Employee_Number,
                        Last_Name = Last_Name
                    };
                    MOO.DAL.Drill.Services.OperatorSvc.Insert(RetVal);
                }
                //add this to our drill list so we don't need to check the DB next time
                FullOpList.Add(RetVal);
                return RetVal;
            }
        }


        private Equip GetDrill(string DrillNumber, MOO.DAL.Drill.Models.DrillSystem DrillSys)
        {
            //first check if we have the equip already
            MOO.DAL.Drill.Models.Equip RetVal;

            RetVal = FullDrillList.FirstOrDefault(x => x.Reference_Id == DrillNumber);
            if (RetVal != null)
                return RetVal;
            else
            {
                //haven't got it from the db yet, try to get it now.
                RetVal = MOO.DAL.Drill.Services.EquipSvc.Get(Plant, DrillNumber);
                if (RetVal == null)
                {
                    //This is a new pattern so we must create it now
                    RetVal = new()
                    {
                        Plant = Plant,
                        Equip_Number = DrillNumber,
                        Active = true,
                        Drill_System = DrillSys,
                        Reference_Id = DrillNumber

                    };
                    Services.EquipSvc.Insert(RetVal);
                }
                //add this to our drill list so we don't need to check the DB next time
                FullDrillList.Add(RetVal);
                return RetVal;
            }
        }


        /// <summary>
        /// Gets the Pattern object from the Drill Database
        /// </summary>
        /// <param name="Pattern_Number"></param>
        /// <returns></returns>
        private MOO.DAL.Drill.Models.Pattern GetPattern(Raw_Drilled_Hole rdh)
        {
            //first check if we have the pattern already
            Pattern RetVal;

            RetVal = PatternsUsed.FirstOrDefault(x => x.Pattern_Number == rdh.Pattern_Number.Trim());
            if (RetVal != null)
                return RetVal;
            else
            {
                //haven't got it from the db yet, try to get it now.
                RetVal =Services.PatternSvc.Get(Plant, rdh.Pattern_Number.Trim());
                if (RetVal == null)
                {
                    //This is a new pattern so we must create it now
                    RetVal = new()
                    {
                        Pattern_Number = rdh.Pattern_Number.Trim(),
                        Plant = Plant
                    };

                    if (Plant == MOO.Plant.Minntac)
                    {
                        if (rdh.Actual_Northing.HasValue && rdh.Actual_Easting.HasValue)
                            RetVal.Pit = MOO.DAL.Drill.Services.PatternSvc.GetMTCPit(rdh.Actual_Northing.Value, rdh.Actual_Easting.Value);
                        else
                            RetVal.Pit = MOO.DAL.Drill.Models.Pit.Not_Set;
                    }
                    else
                    {
                        RetVal.Pit = MOO.DAL.Drill.Models.Pit.Keetac;
                    }
                    MOO.DAL.Drill.Services.PatternSvc.Insert(RetVal);
                }
                //add this to our used patterns list so we don't need to check the DB next time
                PatternsUsed.Add(RetVal);
                return RetVal;
            }
        }

    }
}