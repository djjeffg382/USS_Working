using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MOO.DAL.Drill.Models;
using Microsoft.Data.SqlClient;

namespace MOO.DAL.Drill.Services
{
    public class Equip_HoursSvc
    {

        static Equip_HoursSvc()
        {
            Util.RegisterSqlServer();
        }


        public static Equip_Hours Get(int Equip_Id)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine($"WHERE eh.Equip_Hours_Id = {Equip_Id}");

            DataSet ds = MOO.Data.ExecuteQuery(sql.ToString(), Data.MNODatabase.USSDrillData);
            if (ds.Tables[0].Rows.Count == 1)
                return DataRowToObject(ds.Tables[0].Rows[0]);
            else
                return null;
        }


        public static Equip_Hours Get(Equip Eq, DateTime ShiftDate, byte Shift)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine($"WHERE eh.Equip_Id = @Equip_Id");
            sql.AppendLine($"AND eh.ShiftDate = @ShiftDate");
            sql.AppendLine($"AND eh.Shift = @Shift");

            SqlDataAdapter da = new(sql.ToString(), MOO.Data.GetConnectionString(Data.MNODatabase.USSDrillData));
            da.SelectCommand.Parameters.AddWithValue("Equip_Id", Eq.Equip_Id);
            da.SelectCommand.Parameters.AddWithValue("ShiftDate", ShiftDate);
            da.SelectCommand.Parameters.AddWithValue("Shift", Shift);

            DataSet ds = MOO.Data.ExecuteQuery(da);
            if (ds.Tables[0].Rows.Count == 1)
                return DataRowToObject(ds.Tables[0].Rows[0]);
            else
                return null;
        }


        public static List<Equip_Hours> GetByDateRange(MOO.Plant Plant, DateTime StartDate, DateTime EndDate)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());

            sql.AppendLine($"WHERE eq.Plant = '{Plant}'");
            sql.AppendLine($"AND ShiftDate BETWEEN @StartDate AND @EndDate");

            SqlDataAdapter da = new(sql.ToString(), MOO.Data.GetConnectionString(Data.MNODatabase.USSDrillData));
            da.SelectCommand.Parameters.AddWithValue("StartDate", StartDate);
            da.SelectCommand.Parameters.AddWithValue("EndDate", EndDate);


            DataSet ds = MOO.Data.ExecuteQuery(da);

            List<Equip_Hours> retVal = new();
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
            sql.AppendLine("eh.equip_hours_id, eh.Equip_Id, eh.ShiftDate, eh.Shift, eh.Oper, eh.Maint, eh.Standby,");
            sql.AppendLine("    eh.Sched, eh.OperDelay, eh.MgmtDec,");
            //Equip fields
            sql.AppendLine("eq.Equip_Id eq_Equip_Id, eq.Plant eq_Plant, eq.Equip_Number eq_Equip_Number,");
            sql.AppendLine("eq.Drill_System eq_Drill_System, eq.Active eq_Active, eq.Reference_Id eq_Reference_Id");
            sql.AppendLine("FROM Drill.dbo.Equip_Hours eh");
            sql.AppendLine("INNER JOIN Drill.dbo.Equip eq ON eq.Equip_Id = eh.Equip_Id");
            return sql.ToString();
        }



        /// <summary>
        /// Inserts the Equip_Hours into the Drill database
        /// </summary>
        /// <param name="EqHr">Equip_Hours To insert</param>
        /// <returns></returns>
        public static int Insert(Equip_Hours EqHr)
        {
            SqlConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.USSDrillData));
            conn.Open();
            try
            {
                return Insert(EqHr, conn);
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
        /// Inserts the Equip_Hours into the Drill Database
        /// </summary>
        /// <param name="EqHr">Equip_Hours To insert</param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public static int Insert(Equip_Hours EqHr, SqlConnection conn)
        {


            StringBuilder sql = new();
            sql.AppendLine("INSERT INTO Drill.dbo.Equip_Hours ( Equip_Id, ShiftDate, Shift,");
            sql.AppendLine("    Oper, Maint, Standby, Sched, OperDelay, MgmtDec)");
            sql.AppendLine("VALUES (@Equip_Id, @ShiftDate, @Shift,");
            sql.AppendLine("    @Oper, @Maint, @Standby, @Sched, @OperDelay, @MgmtDec)");

            SqlCommand ins = new(sql.ToString(), conn);
            //equip_hours_id will increment automatically
            ins.Parameters.AddWithValue("Equip_Id", EqHr.Equip.Equip_Id);
            ins.Parameters.AddWithValue("ShiftDate", EqHr.ShiftDate);
            ins.Parameters.AddWithValue("Shift", EqHr.Shift);
            ins.Parameters.AddWithValue("Oper", EqHr.Oper);
            ins.Parameters.AddWithValue("Maint", EqHr.Maint);
            ins.Parameters.AddWithValue("Standby", EqHr.Standby);
            ins.Parameters.AddWithValue("Sched", EqHr.Sched);
            ins.Parameters.AddWithValue("OperDelay", EqHr.OperDelay);
            ins.Parameters.AddWithValue("MgmtDec", EqHr.MgmtDec);

            return ins.ExecuteNonQuery();

        }



        /// <summary>
        /// Updates the Equip_Hours in the Drill database
        /// </summary>
        /// <param name="EqHr">Equip_Hours To update</param>
        /// <returns></returns>
        public static int Update(Equip_Hours EqHr)
        {
            SqlConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.USSDrillData));
            conn.Open();
            try
            {
                return Update(EqHr, conn);
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
        /// Updates the Equip_Hours in the Drill database
        /// </summary>
        /// <param name="EqHr">Equip_Hours To update</param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public static int Update(Equip_Hours EqHr, SqlConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("UPDATE Drill.dbo.Equip_Hours ");
            sql.AppendLine("    SET Equip_Id = @Equip_Id, ShiftDate = @ShiftDate, Shift = @Shift,");
            sql.AppendLine("    Oper = @Oper, Maint = @Maint, Standby = @Standby, Sched = @Sched, OperDelay = @OperDelay");
            sql.AppendLine("WHERE Equip_Hours_Id = @Equip_Hours_Id");

            SqlCommand cmd = new(sql.ToString(), conn);
            cmd.Parameters.AddWithValue("Equip_Id", EqHr.Equip.Equip_Id);
            cmd.Parameters.AddWithValue("ShiftDate", EqHr.ShiftDate);
            cmd.Parameters.AddWithValue("Shift", EqHr.Shift);
            cmd.Parameters.AddWithValue("Oper", EqHr.Oper);
            cmd.Parameters.AddWithValue("Maint", EqHr.Maint);
            cmd.Parameters.AddWithValue("Standby", EqHr.Standby);
            cmd.Parameters.AddWithValue("Sched", EqHr.Sched);
            cmd.Parameters.AddWithValue("OperDelay", EqHr.OperDelay);
            cmd.Parameters.AddWithValue("MgmtDec", EqHr.MgmtDec);
            cmd.Parameters.AddWithValue("Equip_Hours_Id", EqHr.Equip_Hours_Id);

            return cmd.ExecuteNonQuery();

        }


        /// <summary>
        /// Deletes the Equip_Hours in the Drill database
        /// </summary>
        /// <param name="EqHr">Equip_Hours To delete</param>
        /// <returns></returns>
        public static int Delete(Equip_Hours EqHr)
        {
            SqlConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.USSDrillData));
            conn.Open();
            try
            {
                return Delete(EqHr, conn);
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
        /// Deletes the Equip_Hours in the Drill database
        /// </summary>
        /// <param name="EqHr">Equip_Hours To Delete</param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public static int Delete(Equip_Hours EqHr, SqlConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("DELETE FROM Drill.dbo.Equip_Hours ");
            sql.AppendLine("WHERE Equip_Hours_Id = @Equip_Hours_Id");

            SqlCommand cmd = new(sql.ToString(), conn);
            cmd.Parameters.AddWithValue("Equip_Hours_Id", EqHr.Equip_Hours_Id);

            return cmd.ExecuteNonQuery();

        }

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

        public static int DeleteShift(MOO.Plant Plant, DateTime ShiftDate, byte Shift, SqlConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("DELETE FROM Drill.dbo.Equip_Hours ");
            sql.AppendLine($"WHERE Equip_id IN (SELECT equip_id FROM Drill.dbo.Equip WHERE plant = '{Plant}')");
            sql.AppendLine("AND ShiftDate = @ShiftDate");
            sql.AppendLine("AND Shift = @Shift");

            SqlCommand cmd = new(sql.ToString(), conn);
            cmd.Parameters.AddWithValue("ShiftDate", ShiftDate);
            cmd.Parameters.AddWithValue("Shift", Shift);

            return cmd.ExecuteNonQuery();

        }


        internal static Equip_Hours DataRowToObject(DataRow row, string FieldPrefix = "")
        {
            Equip_Hours RetVal = new();
            RetVal.Equip_Hours_Id = row.Field<int>($"{FieldPrefix}equip_hours_id");
            RetVal.Equip = EquipSvc.DataRowToObject(row, "eq_");

            RetVal.ShiftDate = row.Field<DateTime>($"{FieldPrefix}shiftDate");
            RetVal.Shift = row.Field<byte>($"{FieldPrefix}shift");
            RetVal.Oper = row.Field<decimal>($"{FieldPrefix}Oper");
            RetVal.Maint = row.Field<decimal>($"{FieldPrefix}Maint");
            RetVal.Standby = row.Field<decimal>($"{FieldPrefix}Standby");
            RetVal.Sched = row.Field<decimal>($"{FieldPrefix}Sched");
            RetVal.OperDelay = row.Field<decimal>($"{FieldPrefix}OperDelay");
            RetVal.MgmtDec = row.Field<decimal>($"{FieldPrefix}MgmtDec");
            return RetVal;
        }
    }
}
