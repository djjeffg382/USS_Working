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
    public class Drilled_Hole_StatusSvc
    {

        static Drilled_Hole_StatusSvc()
        {
            Util.RegisterSqlServer();
        }


        public static Drilled_Hole_Status Get(int Drilled_Hole_Status_id)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine($"WHERE Drilled_Hole_Status_Id = {Drilled_Hole_Status_id}");

            DataSet ds = MOO.Data.ExecuteQuery(sql.ToString(), Data.MNODatabase.USSDrillData);
            if (ds.Tables[0].Rows.Count == 1)
                return DataRowToObject(ds.Tables[0].Rows[0]);
            else
                return null;
        }

        public static Drilled_Hole_Status Get(int Drilled_Hole_Id, DateTime StarTime)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine($"WHERE Drilled_Hole_Id = {Drilled_Hole_Id}");
            sql.AppendLine($"AND start_time = @Start_Time");

            SqlDataAdapter da = new(sql.ToString(), MOO.Data.GetConnectionString(Data.MNODatabase.USSDrillData));
            da.SelectCommand.Parameters.AddWithValue("Start_Time", StarTime);          

            DataSet ds = MOO.Data.ExecuteQuery(da);
            if (ds.Tables[0].Rows.Count == 1)
                return DataRowToObject(ds.Tables[0].Rows[0]);
            else
                return null;
        }

        private static string GetSelect()
        {
            StringBuilder sql = new();
            sql.AppendLine("SELECT ");
            sql.AppendLine("Drilled_Hole_Status_Id,Drilled_Hole_Id, Plant, Shift_Date, shift, Start_Time, End_Time,");
            sql.AppendLine("    StatusCode, StatusBucket");
            sql.AppendLine("FROM Drill.dbo.Drilled_Hole_Status ");
            return sql.ToString();
        }



        /// <summary>
        /// Inserts the Drilled_Hole_Status into the Drill database
        /// </summary>
        /// <param name="Dhs">Drilled_Hole_Status To insert</param>
        /// <returns></returns>
        public static int Insert(Drilled_Hole_Status Dhs)
        {
            SqlConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.USSDrillData));
            conn.Open();
            try
            {
                return Insert(Dhs, conn);
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
        /// Inserts the Drilled_Hole_Status into the Drill Database
        /// </summary>
        /// <param name="Dhs">Drilled_Hole_Status To insert</param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public static int Insert(Drilled_Hole_Status Dhs, SqlConnection conn)
        {


            StringBuilder sql = new();
            sql.AppendLine("INSERT INTO Drill.dbo.Drilled_Hole_Status ( Drilled_Hole_Id, Plant, Shift_Date, Shift,");
            sql.AppendLine("    Start_Time, End_Time, StatusCode, StatusBucket)");
            sql.AppendLine("VALUES (@Drilled_Hole_Id, @Plant, @Shift_Date, @Shift,");
            sql.AppendLine("    @Start_Time, @End_Time, @StatusCode, @StatusBucket)");

            SqlCommand ins = new(sql.ToString(), conn);
            //equip_hours_id will increment automatically
            ins.Parameters.AddWithValue("Drilled_Hole_Id", Dhs.Drilled_Hole_Id);
            ins.Parameters.AddWithValue("Plant", Dhs.Plant.ToString());
            ins.Parameters.AddWithValue("Shift_Date", Dhs.Shift_Date);
            ins.Parameters.AddWithValue("Shift", Dhs.Shift);
            ins.Parameters.AddWithValue("Start_Time", Dhs.Start_Time);
            ins.Parameters.AddWithValue("End_Time", Dhs.End_Time);
            ins.Parameters.AddWithValue("StatusCode", Dhs.StatusCode);
            ins.Parameters.AddWithValue("StatusBucket", Dhs.StatusBucket.ToString());

            return ins.ExecuteNonQuery();

        }



        /// <summary>
        /// Updates the Drilled_Hole_Status in the Drill database
        /// </summary>
        /// <param name="Dhs">Drilled_Hole_Status To update</param>
        /// <returns></returns>
        public static int Update(Drilled_Hole_Status Dhs)
        {
            SqlConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.USSDrillData));
            conn.Open();
            try
            {
                return Update(Dhs, conn);
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
        /// Updates the Drilled_Hole_Status in the Drill database
        /// </summary>
        /// <param name="Dhs">Drilled_Hole_Status To update</param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public static int Update(Drilled_Hole_Status Dhs, SqlConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("UPDATE Drill.dbo.Drilled_Hole_Status ");
            sql.AppendLine("    SET Drilled_Hole_Id = @Drilled_Hole_Id, Plant = @Plant, Shift_Date = @Shift_Date, Shift = @Shift, ");
            sql.AppendLine("    Start_Time = @Start_Time, End_Time = @End_Time, StatusCode = @StatusCode, StatusBucket = @StatusBucket");
            sql.AppendLine("WHERE Drilled_Hole_Status_Id = @Drilled_Hole_Status_Id");

            SqlCommand cmd = new(sql.ToString(), conn);
            cmd.Parameters.AddWithValue("Drilled_Hole_Id", Dhs.Drilled_Hole_Id);
            cmd.Parameters.AddWithValue("Plant", Dhs.Plant.ToString());
            cmd.Parameters.AddWithValue("Shift_Date", Dhs.Shift_Date);
            cmd.Parameters.AddWithValue("Shift", Dhs.Shift);
            cmd.Parameters.AddWithValue("Start_Time", Dhs.Start_Time);
            cmd.Parameters.AddWithValue("End_Time", Dhs.End_Time);
            cmd.Parameters.AddWithValue("StatusCode", Dhs.StatusCode);
            cmd.Parameters.AddWithValue("StatusBucket", Dhs.StatusBucket.ToString());
            cmd.Parameters.AddWithValue("Drilled_Hole_Status_Id", Dhs.Drilled_Hole_Status_Id);

            return cmd.ExecuteNonQuery();

        }


        /// <summary>
        /// Deletes the Drilled_Hole_Status in the Drill database
        /// </summary>
        /// <param name="Dhs">Drilled_Hole_Status To delete</param>
        /// <returns></returns>
        public static int Delete(Drilled_Hole_Status Dhs)
        {
            SqlConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.USSDrillData));
            conn.Open();
            try
            {
                return Delete(Dhs, conn);
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
        /// Deletes the Drilled_Hole_Status in the Drill database
        /// </summary>
        /// <param name="Dhs">Drilled_Hole_Status To Delete</param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public static int Delete(Drilled_Hole_Status Dhs, SqlConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("DELETE FROM Drill.dbo.Drilled_Hole_Status ");
            sql.AppendLine("WHERE Drilled_Hole_Status_Id = @Drilled_Hole_Status_Id");

            SqlCommand cmd = new(sql.ToString(), conn);
            cmd.Parameters.AddWithValue("Drilled_Hole_Status_Id", Dhs.Drilled_Hole_Status_Id);

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
            sql.AppendLine("DELETE FROM Drill.dbo.Drilled_Hole_Status ");
            sql.AppendLine($"WHERE Plant = '{Plant}'");
            sql.AppendLine("AND Shift_Date = @ShiftDate");
            sql.AppendLine("AND Shift = @Shift");

            SqlCommand cmd = new(sql.ToString(), conn);
            cmd.Parameters.AddWithValue("ShiftDate", ShiftDate);
            cmd.Parameters.AddWithValue("Shift", Shift);

            return cmd.ExecuteNonQuery();

        }


        internal static Drilled_Hole_Status DataRowToObject(DataRow row, string FieldPrefix = "")
        {
            Drilled_Hole_Status RetVal = new();
            RetVal.Drilled_Hole_Status_Id = row.Field<int>($"{FieldPrefix}Drilled_Hole_Status_Id");
            RetVal.Drilled_Hole_Id = row.Field<int>($"{FieldPrefix}Drilled_Hole_Id");

            RetVal.Shift_Date = row.Field<DateTime>($"{FieldPrefix}shift_Date");
            RetVal.Shift = row.Field<byte>($"{FieldPrefix}shift");
            RetVal.Start_Time = row.Field<DateTime>($"{FieldPrefix}Start_Time");
            RetVal.End_Time = row.Field<DateTime>($"{FieldPrefix}End_Time");

            RetVal.StatusCode = row.Field<string>($"{FieldPrefix}StatusCode");
            RetVal.StatusBucket =Enum.Parse<Drilled_Hole_Status.StatusType>( row.Field<string>($"{FieldPrefix}StatusBucket"));
            RetVal.Plant = Enum.Parse<MOO.Plant>(row.Field<string>($"{FieldPrefix}Plant"));
            return RetVal;
        }
    }
}
