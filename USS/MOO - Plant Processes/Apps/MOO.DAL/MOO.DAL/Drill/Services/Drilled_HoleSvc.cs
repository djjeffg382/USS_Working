using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MOO.DAL.Drill.Models;
using Microsoft.Data.SqlClient;
using System.Net;

namespace MOO.DAL.Drill.Services
{
    public class Drilled_HoleSvc
    {


        static Drilled_HoleSvc()
        {
            Util.RegisterSqlServer();
        }


        public static Drilled_Hole Get(int Drilled_Hole_Id)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine($"WHERE Drilled_Hole_Id = {Drilled_Hole_Id}");

            DataSet ds = MOO.Data.ExecuteQuery(sql.ToString(), Data.MNODatabase.USSDrillData);
            if (ds.Tables[0].Rows.Count == 1)
                return DataRowToObject(ds.Tables[0].Rows[0]);
            else
                return null;
        }


        public static List<Drilled_Hole> GetByShift(MOO.Plant Plant, DateTime ShiftDate, byte Shift)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine($"WHERE dh.Plant = '{Plant}'");
            sql.AppendLine($"AND dh.Shift_Date = '{ShiftDate:MM/dd/yyyy}'");
            sql.AppendLine($"AND dh.Shift = {Shift}");
            sql.AppendLine("ORDER BY eq.equip_number, dh.End_Date");


            DataSet ds = MOO.Data.ExecuteQuery(sql.ToString(), Data.MNODatabase.USSDrillData);

            List<Drilled_Hole> retVal = new();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                retVal.Add(DataRowToObject(dr));
            }

            return retVal;
        }

        /// <summary>
        /// Gets the holes by the specified Pattern_id
        /// </summary>
        /// <param name="PatternId"></param>
        /// <returns></returns>
        public static List<Drilled_Hole> GetByPattern(int PatternId)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine($"WHERE dh.Pattern_Id = '{PatternId}'");
            sql.AppendLine("ORDER BY eq.equip_number, dh.End_Date");


            DataSet ds = MOO.Data.ExecuteQuery(sql.ToString(), Data.MNODatabase.USSDrillData);

            List<Drilled_Hole> retVal = new();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                retVal.Add(DataRowToObject(dr));
            }

            return retVal;
        }

        /// <summary>
        /// Gets the holes for a pattern given the pattern number
        /// </summary>
        /// <param name="PatternId"></param>
        /// <returns></returns>
        public static List<Drilled_Hole> GetByPattern(MOO.Plant Plant, string PatternNumber)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine($"WHERE dh.Pattern_Id = (SELECT Pattern_ID FROM Drill.dbo.Pattern WHERE plant = @plant AND Pattern_Number = @PatternNumber)");
            sql.AppendLine("ORDER BY eq.equip_number, dh.End_Date");

            SqlDataAdapter da = new(sql.ToString(),MOO.Data.GetConnectionString(Data.MNODatabase.USSDrillData));
            da.SelectCommand.Parameters.AddWithValue("plant", Plant.ToString());
            da.SelectCommand.Parameters.AddWithValue("PatternNumber", PatternNumber);


            DataSet ds = MOO.Data.ExecuteQuery(da);

            List<Drilled_Hole> retVal = new();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                retVal.Add(DataRowToObject(dr));
            }

            return retVal;
        }



        private static string GetSelect()
        {
            StringBuilder sql = new();
            sql.AppendLine("SELECT ");
            sql.AppendLine("    dh.Drilled_Hole_Id, dh.Plant, dh.Material, dh.Hole_Number, dh.Planned_Depth, dh.Drilled_Depth, dh.Start_Date, dh.End_Date, ");
            sql.AppendLine("    dh.Shift_Date, dh.Shift, dh.Design_Northing, dh.Actual_Northing, dh.Design_Easting, dh.Actual_Easting,");
            sql.AppendLine("    dh.Design_Bottom, dh.Actual_Bottom, dh.Collar, dh.Reference_Key, dh.Redrill_Type, dh.ROP_Ft_Per_Min,");
            sql.AppendLine("    dh.Measured_Depth, dh.Deep_Hole,");
            //Equip Table columns
            sql.AppendLine("    eq.Equip_Id eq_Equip_Id, eq.Plant eq_Plant, eq.equip_number eq_Equip_Number,");
            sql.AppendLine("    eq.Drill_System eq_Drill_System, eq.Active eq_Active, eq.Reference_Id eq_reference_id,");
            //Pattern Table
            sql.AppendLine("    ptn.pattern_id ptn_pattern_id, ptn.plant ptn_plant, ptn.Pattern_Number ptn_Pattern_Number, ptn.Pit ptn_pit,");
            //Drill Bit
            sql.AppendLine("    db.Drill_Bit_Id db_Drill_Bit_Id, db.Plant db_Plant, db.Serial_Number db_Serial_Number, db.manufacturer db_manufacturer,");
            //Operator
            sql.AppendLine("    op.Operator_Id op_Operator_Id, op.Plant op_Plant, op.Employee_Number op_Employee_Number,");
            sql.AppendLine("    op.First_Name op_First_Name, op.Last_Name op_Last_Name, op.Active op_Active,");
            //Pattern size
            sql.AppendLine("ps.pattern_size_id ps_pattern_size_id, ps.description ps_description, ");
            sql.AppendLine("ps.holes_length ps_holes_length, ps.holes_width ps_holes_width, ps.ltons_per_foot ps_ltons_per_foot, ps.plant ps_plant,");
            //Drilled Hole Notes
            sql.AppendLine(Drilled_Hole_NotesSvc.GetColumns("dhn","dhn_"));


            sql.AppendLine("FROM Drill.dbo.Drilled_Hole dh");
            sql.AppendLine("INNER JOIN Drill.dbo.Equip eq ON dh.Equip_id = eq.Equip_id");
            sql.AppendLine("INNER JOIN Drill.dbo.Pattern ptn ON dh.Pattern_Id = ptn.Pattern_Id");
            sql.AppendLine("LEFT OUTER JOIN Drill.dbo.Drill_Bit db ON dh.Drill_Bit_Id = db.Drill_Bit_Id");
            sql.AppendLine("INNER JOIN Drill.dbo.Operator op ON dh.Operator_Id = op.Operator_Id");
            sql.AppendLine("LEFT OUTER JOIN Drill.dbo.Pattern_Size ps ON ptn.Pattern_Size_Id = ps.pattern_size_id");
            sql.AppendLine("LEFT OUTER JOIN Drill.dbo.Drilled_Hole_Notes dhn ON dh.Drilled_Hole_Notes_Id = dhn.drilled_hole_notes_id");
            return sql.ToString();
        }



        /// <summary>
        /// Inserts the Drilled_Hole into the Drill database
        /// </summary>
        /// <param name="Dh">Drilled_Hole To insert</param>
        /// <returns></returns>
        public static int Insert(Drilled_Hole Dh)
        {
            SqlConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.USSDrillData));
            conn.Open();
            try
            {
                return Insert(Dh, conn);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                conn.Close();
            }
        }

        /// <summary>
        /// Inserts the Drilled_Hole into the Drill Database
        /// </summary>
        /// <param name="Dh">Drilled_Hole To insert</param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public static int Insert(Drilled_Hole Dh, SqlConnection conn)
        {
            //Note: Redrill type will be calculated on Insert Trigger and will not be updated in this statement
            if (Dh.Drilled_Hole_Id <= 0)
                Dh.Drilled_Hole_Id = Convert.ToInt32(MOO.Data.GetNextSequence("Drill.dbo.seq_Drilled_Hole", MOO.Data.MNODatabase.USSDrillData));

            StringBuilder sql = new();
            sql.AppendLine("INSERT INTO Drill.dbo.Drilled_Hole (Drilled_Hole_Id, Plant, Equip_Id, Pattern_Id, Drill_Bit_Id,");
            sql.AppendLine("    Operator_Id, Material, Hole_Number, Planned_Depth, Drilled_Depth, Start_Date, End_Date, Shift_Date, Shift,");
            sql.AppendLine("    Design_Northing, Actual_Northing, Design_Easting, Actual_Easting, ");
            sql.AppendLine("    Design_Bottom, Actual_Bottom, Collar, Reference_Key, ROP_Ft_Per_Min,");
            sql.AppendLine("    Measured_Depth, Deep_Hole, Drilled_Hole_Notes_Id)");
            sql.AppendLine("VALUES(@Drilled_Hole_Id, @Plant, @Equip_Id, @Pattern_Id, @Drill_Bit_Id,");
            sql.AppendLine("    @Operator_Id, @Material, @Hole_Number, @Planned_Depth, @Drilled_Depth, @Start_Date, @End_Date, @Shift_Date, @Shift,");
            sql.AppendLine("    @Design_Northing, @Actual_Northing, @Design_Easting, @Actual_Easting, ");
            sql.AppendLine("    @Design_Bottom, @Actual_Bottom, @Collar, @Reference_Key, @ROP_Ft_Per_Min,");
            sql.AppendLine("    @Measured_Depth, @Deep_Hole, @Drilled_Hole_Notes_Id)");

            SqlCommand ins = new(sql.ToString(), conn);
            ins.Parameters.AddWithValue("Drilled_Hole_Id", Dh.Drilled_Hole_Id);
            ins.Parameters.AddWithValue("Plant", Dh.Plant.ToString());
            ins.Parameters.AddWithValue("Equip_Id", Dh.Equip.Equip_Id);
            ins.Parameters.AddWithValue("Pattern_Id", Dh.Pattern.Pattern_Id);
            if(Dh.Drill_Bit != null)
                ins.Parameters.AddWithValue("Drill_Bit_Id", Dh.Drill_Bit.Drill_Bit_Id);
            else
                ins.Parameters.AddWithValue("Drill_Bit_Id", DBNull.Value);

            ins.Parameters.AddWithValue("Operator_Id", Dh.Operator.Operator_Id);
            ins.Parameters.AddWithValue("Material", Dh.Material.ToString());
            ins.Parameters.AddWithValue("Hole_Number", Dh.Hole_Number);
            ins.Parameters.AddWithValue("Planned_Depth", Dh.Planned_Depth);
            ins.Parameters.AddWithValue("Drilled_Depth", Dh.Drilled_Depth);
            ins.Parameters.AddWithValue("Start_Date", Dh.Start_Date);
            ins.Parameters.AddWithValue("End_Date", Dh.End_Date);
            ins.Parameters.AddWithValue("Shift_Date", Dh.Shift_Date);
            ins.Parameters.AddWithValue("Shift", Dh.Shift);
            ins.Parameters.AddWithValue("Design_Northing",  (Dh.Design_Northing==null) ? DBNull.Value:Dh.Design_Northing);
            ins.Parameters.AddWithValue("Actual_Northing", (Dh.Actual_Northing == null) ? DBNull.Value : Dh.Actual_Northing);
            ins.Parameters.AddWithValue("Design_Easting", (Dh.Design_Easting == null) ? DBNull.Value : Dh.Design_Easting);
            ins.Parameters.AddWithValue("Actual_Easting", (Dh.Actual_Easting == null) ? DBNull.Value : Dh.Actual_Easting);
            ins.Parameters.AddWithValue("Design_Bottom", (Dh.Design_Bottom == null) ? DBNull.Value : Dh.Design_Bottom);
            ins.Parameters.AddWithValue("Actual_Bottom", (Dh.Actual_Bottom == null) ? DBNull.Value : Dh.Actual_Bottom);
            ins.Parameters.AddWithValue("Collar", (Dh.Collar == null) ? DBNull.Value : Dh.Collar);
            ins.Parameters.AddWithValue("Reference_Key", (Dh.Reference_Key == null) ? DBNull.Value : Dh.Reference_Key);
            ins.Parameters.AddWithValue("ROP_Ft_Per_Min", (Dh.ROP_Ft_Per_Min == null) ? DBNull.Value : Dh.ROP_Ft_Per_Min);
            ins.Parameters.AddWithValue("Measured_Depth", (Dh.Measured_Depth == null) ? DBNull.Value : Dh.Measured_Depth);
            ins.Parameters.AddWithValue("Deep_Hole", (Dh.Deep_Hole == null) ? DBNull.Value : Dh.Deep_Hole);
            ins.Parameters.AddWithValue("Drilled_Hole_Notes_Id", (Dh.Hole_Notes == null) ? DBNull.Value : Dh.Hole_Notes.Drilled_Hole_Notes_Id);




            return ins.ExecuteNonQuery();

        }



        /// <summary>
        /// Updates the Drilled_Hole in the Drill database
        /// </summary>
        /// <param name="Dh">Drilled_Hole To update</param>
        /// <returns></returns>
        public static int Update(Drilled_Hole Dh)
        {
            SqlConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.USSDrillData));
            conn.Open();
            try
            {
                return Update(Dh, conn);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                conn.Close();
            }
        }

        /// <summary>
        /// Updates the Drilled_Hole in the Drill database
        /// </summary>
        /// <param name="Dh">Drilled_Hole To update</param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public static int Update(Drilled_Hole Dh, SqlConnection conn)
        {
            //Note: Redrill type will be calculated on Update Trigger and will not be updated in this statement
            StringBuilder sql = new();
            sql.AppendLine("UPDATE Drill.dbo.Drilled_Hole ");
            sql.AppendLine("    SET Plant = @Plant, Equip_Id = @Equip_Id, Pattern_Id = @Pattern_id, Drill_Bit_Id = @Drill_Bit_Id,");
            sql.AppendLine("    Operator_Id = @Operator_Id, Material = @Material, Hole_Number = @Hole_Number, Planned_Depth = @Planned_Depth,");
            sql.AppendLine("    Drilled_Depth = @Drilled_Depth, Start_Date = @Start_Date, End_Date = @End_Date, Shift = @Shift,");
            sql.AppendLine("    Design_Northing = @Design_Northing, Actual_Northing = @Actual_Northing,");
            sql.AppendLine("    Design_Easting = @Design_Easting, Actual_Easting = @Actual_Easting, ");
            sql.AppendLine("    Design_Bottom = @Design_Bottom, Actual_Bottom = @Actual_Bottom, ");
            sql.AppendLine("    Collar = @Collar, Reference_Key = @Reference_Key, ROP_Ft_Per_Min = @ROP_Ft_Per_Min,");
            sql.AppendLine("    Measured_Depth = @Measured_Depth, Deep_Hole = @Deep_Hole, Drilled_Hole_Notes_Id = @Drilled_Hole_Notes_Id");
            sql.AppendLine("    ");
            sql.AppendLine("WHERE Drilled_Hole_Id = @Drilled_Hole_Id");

            SqlCommand cmd = new(sql.ToString(), conn);
            cmd.Parameters.AddWithValue("Plant", Dh.Plant.ToString());
            cmd.Parameters.AddWithValue("Equip_Id", Dh.Equip.Equip_Id);
            cmd.Parameters.AddWithValue("Pattern_Id", Dh.Pattern.Pattern_Id);
            if (Dh.Drill_Bit != null)
                cmd.Parameters.AddWithValue("Drill_Bit_Id", Dh.Drill_Bit.Drill_Bit_Id);
            else
                cmd.Parameters.AddWithValue("Drill_Bit_Id", DBNull.Value);

            cmd.Parameters.AddWithValue("Operator_Id", Dh.Operator.Operator_Id);
            cmd.Parameters.AddWithValue("Material", Dh.Material.ToString());
            cmd.Parameters.AddWithValue("Hole_Number", Dh.Hole_Number);
            cmd.Parameters.AddWithValue("Planned_Depth", Dh.Planned_Depth);
            cmd.Parameters.AddWithValue("Drilled_Depth", Dh.Drilled_Depth);
            cmd.Parameters.AddWithValue("Start_Date", Dh.Start_Date);
            cmd.Parameters.AddWithValue("End_Date", Dh.End_Date);
            cmd.Parameters.AddWithValue("Shift_Date", Dh.Shift_Date);
            cmd.Parameters.AddWithValue("Shift", Dh.Shift);
            cmd.Parameters.AddWithValue("Design_Northing", (Dh.Design_Northing == null) ? DBNull.Value : Dh.Design_Northing);
            cmd.Parameters.AddWithValue("Actual_Northing", (Dh.Actual_Northing == null) ? DBNull.Value : Dh.Actual_Northing);
            cmd.Parameters.AddWithValue("Design_Easting", (Dh.Design_Easting == null) ? DBNull.Value : Dh.Design_Easting);
            cmd.Parameters.AddWithValue("Actual_Easting", (Dh.Actual_Easting == null) ? DBNull.Value : Dh.Actual_Easting);
            cmd.Parameters.AddWithValue("Design_Bottom", (Dh.Design_Bottom == null) ? DBNull.Value : Dh.Design_Bottom);
            cmd.Parameters.AddWithValue("Actual_Bottom", (Dh.Actual_Bottom == null) ? DBNull.Value : Dh.Actual_Bottom);
            cmd.Parameters.AddWithValue("Collar", (Dh.Collar == null) ? DBNull.Value : Dh.Collar);
            cmd.Parameters.AddWithValue("Reference_Key", (Dh.Reference_Key == null) ? DBNull.Value : Dh.Reference_Key);
            cmd.Parameters.AddWithValue("ROP_Ft_Per_Min", (Dh.ROP_Ft_Per_Min == null) ? DBNull.Value : Dh.ROP_Ft_Per_Min);
            cmd.Parameters.AddWithValue("Measured_Depth", (Dh.Measured_Depth == null) ? DBNull.Value : Dh.Measured_Depth);
            cmd.Parameters.AddWithValue("Deep_Hole", (Dh.Deep_Hole == null) ? DBNull.Value : Dh.Deep_Hole);
            cmd.Parameters.AddWithValue("Drilled_Hole_Notes_Id", (Dh.Hole_Notes == null) ? DBNull.Value : Dh.Hole_Notes.Drilled_Hole_Notes_Id);


            cmd.Parameters.AddWithValue("Drilled_Hole_Id", Dh.Drilled_Hole_Id);
            return cmd.ExecuteNonQuery();

        }


        /// <summary>
        /// Deletes the Drilled_Hole in the Drill database
        /// </summary>
        /// <param name="Dh">Drilled_Hole To delete</param>
        /// <returns></returns>
        public static int Delete(Drilled_Hole Dh)
        {
            SqlConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.USSDrillData));
            conn.Open();
            try
            {
                return Delete(Dh, conn);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                conn.Close();
            }
        }

        /// <summary>
        /// Deletes the Drilled_Hole in the Drill database
        /// </summary>
        /// <param name="Dh">Drilled_Hole To Delete</param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public static int Delete(Drilled_Hole Dh, SqlConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("DELETE FROM Drill.dbo.Drilled_Hole ");
            sql.AppendLine("WHERE Drilled_Hole_Id = @Drilled_Hole_Id");

            SqlCommand cmd = new(sql.ToString(), conn);
            cmd.Parameters.AddWithValue("Drilled_Hole_Id", Dh.Drilled_Hole_Id);

            return cmd.ExecuteNonQuery();

        }





        /// <summary>
        /// Deletes all drilled hole records for specified Shift Date and Shift (used this before re-importing)
        /// </summary>
        /// <param name="Plant"></param>
        /// <param name="ShiftDate"></param>
        /// <param name="Shift"></param>
        /// <returns></returns>
        public static int DeleteShift(MOO.Plant Plant, DateTime ShiftDate, byte Shift)
        {
            SqlConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.USSDrillData));
            conn.Open();
            try
            {
                return DeleteShift(Plant, ShiftDate, Shift, conn);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                conn.Close();
            }
        }

        /// <summary>
        /// Deletes all drilled hole records for specified Shift Date and Shift (used this before re-importing)
        /// </summary>
        /// <param name="Plant"></param>
        /// <param name="ShiftDate"></param>
        /// <param name="Shift"></param>
        /// <returns></returns>
        public static int DeleteShift(MOO.Plant Plant, DateTime ShiftDate, byte Shift, SqlConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("DELETE FROM Drill.dbo.Drilled_Hole ");
            sql.AppendLine("WHERE Plant = @Plant");
            sql.AppendLine("AND Shift_Date = @Shift_Date");
            sql.AppendLine("AND Shift = @Shift");

            SqlCommand cmd = new(sql.ToString(), conn);
            cmd.Parameters.AddWithValue("Plant", Plant.ToString());
            cmd.Parameters.AddWithValue("Shift_Date", ShiftDate);
            cmd.Parameters.AddWithValue("Shift", Shift);

            return cmd.ExecuteNonQuery();

        }




        internal static Drilled_Hole DataRowToObject(DataRow row, string FieldPrefix = "")
        {
            Drilled_Hole RetVal = new();
            RetVal.Plant = Enum.Parse<MOO.Plant>(row.Field<string>($"{FieldPrefix}plant"));
            RetVal.Drilled_Hole_Id = row.Field<int>($"{FieldPrefix}drilled_hole_id");
            RetVal.Material = Enum.Parse<Material>(row.Field<string>($"{FieldPrefix}material"));
            RetVal.Hole_Number = row.Field<string>($"{FieldPrefix}hole_number");
            RetVal.Planned_Depth = row.Field<decimal>($"{FieldPrefix}planned_depth");
            RetVal.Drilled_Depth = row.Field<decimal>($"{FieldPrefix}drilled_depth");
            RetVal.Start_Date = row.Field<DateTime>($"{FieldPrefix}start_date");
            RetVal.End_Date = row.Field<DateTime>($"{FieldPrefix}end_date");
            RetVal.Shift_Date = row.Field<DateTime>($"{FieldPrefix}shift_date");
            RetVal.Shift = row.Field<byte>($"{FieldPrefix}shift");
            RetVal.Design_Northing = row.Field<decimal?>($"{FieldPrefix}design_northing");
            RetVal.Actual_Northing = row.Field<decimal?>($"{FieldPrefix}actual_northing");
            RetVal.Design_Easting = row.Field<decimal?>($"{FieldPrefix}design_easting");
            RetVal.Actual_Easting = row.Field<decimal?>($"{FieldPrefix}actual_easting");
            RetVal.Design_Bottom = row.Field<decimal?>($"{FieldPrefix}design_bottom");
            RetVal.Actual_Bottom = row.Field<decimal?>($"{FieldPrefix}actual_bottom");
            RetVal.Collar = row.Field<decimal?>($"{FieldPrefix}collar");
            RetVal.Reference_Key = row.Field<string>($"{FieldPrefix}reference_key");
            RetVal.Redrill_Type = (Models.Redrill_Type)row.Field<int>($"{FieldPrefix}redrill_type");
            RetVal.ROP_Ft_Per_Min = row.Field<decimal?>($"{FieldPrefix}ROP_Ft_Per_Min");
            RetVal.Measured_Depth = row.Field<decimal?>($"{FieldPrefix}Measured_Depth");
            RetVal.Deep_Hole = row.Field<bool>($"{FieldPrefix}Deep_Hole");

            RetVal.Equip = EquipSvc.DataRowToObject(row, "eq_");
            RetVal.Pattern = PatternSvc.DataRowToObject(row, "ptn_");
            RetVal.Operator = OperatorSvc.DataRowToObject(row, "op_");
            if (!row.IsNull("dhn_Drilled_Hole_Notes_Id"))
                RetVal.Hole_Notes = Drilled_Hole_NotesSvc.DataRowToObject(row, "dhn_");
            
            if (row.IsNull("ps_pattern_size_id"))
                RetVal.Pattern.Pattern_Size = null;
            else
                RetVal.Pattern.Pattern_Size = Pattern_SizeSvc.DataRowToObject(row, "ps_");

            if (row.IsNull("db_Drill_Bit_Id"))
                RetVal.Drill_Bit = null;
            else
                RetVal.Drill_Bit = Drill_BitSvc.DataRowToObject(row, "db_");


            return RetVal;
        }
    }
}
