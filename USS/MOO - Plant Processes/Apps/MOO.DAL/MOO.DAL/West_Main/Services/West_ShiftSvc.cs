using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MOO.DAL.West_Main.Models;
using System.Data;
using Oracle.ManagedDataAccess.Client;

namespace MOO.DAL.West_Main.Services
{
    public class West_ShiftSvc
    {

        static West_ShiftSvc()
        {
            Util.RegisterOracle();
        }


        public static West_Shift Get(WestMainPlants Plant, int Id, int Ymd, int Shift)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect(Plant));
            sql.AppendLine($"WHERE id = {Id}");
            sql.AppendLine($"AND ymd = '{Ymd}' ");
            sql.AppendLine($"AND shift = {Shift} ");

            DataSet ds = MOO.Data.ExecuteQuery(sql.ToString(), Data.MNODatabase.DMART);
            if (ds.Tables[0].Rows.Count == 1)
            {
                West_Shift retVal = DataRowToObject(ds.Tables[0].Rows[0]);
                retVal.Plant = Plant;
                return retVal;
            }
                
            else
                return null;
        }

        public static List<West_Shift> GetYMDRange(WestMainPlants Plant, int Id, int YMDStart, int YMDEnd)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect(Plant));
            sql.AppendLine($"WHERE id = {Id}");
            sql.AppendLine($"AND ymd BETWEEN '{YMDStart}' AND '{YMDEnd}'");

            DataSet ds = MOO.Data.ExecuteQuery(sql.ToString(), Data.MNODatabase.DMART);

            List<West_Shift> retVal = new();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                West_Shift newObj = DataRowToObject(dr);
                newObj.Plant = Plant;
                retVal.Add(newObj);
            }
            return retVal;
        }

        private static string GetSelect(WestMainPlants Plant)
        {
            StringBuilder sql = new();
            sql.AppendLine("SELECT ");
            sql.AppendLine("id, timestamp, ymd, shift, ROUND(shift_total,8) shift_total, n_count");            
            sql.AppendLine("FROM west_main.west_shift_" + Plant.ToString());
            return sql.ToString();
        }



        /// <summary>
        /// Inserts into the west_shift table
        /// </summary>
        /// <param name="ws"></param>
        /// <returns></returns>
        public static int Insert(West_Shift ws)
        {
            OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            try
            {
                return Insert(ws, conn);
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
        /// Inserts the west_shift table
        /// </summary>
        /// <param name="ws"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public static int Insert(West_Shift ws, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine($"INSERT INTO west_main.west_shift_{ws.Plant} (id, timestamp, ");
            sql.AppendLine("    ymd, shift,");
            sql.AppendLine("    shift_total, n_count)");
            sql.AppendLine("VALUES(:id, :timestamp,");
            sql.AppendLine("    :ymd, :shift,");
            sql.AppendLine("    :shift_total, :n_count)");

            OracleCommand ins = new(sql.ToString(), conn);
            ins.Parameters.Add("id", ws.Id);
            ins.Parameters.Add("timestamp",ws.TimeStamp);
            ins.Parameters.Add("ymd", ws.Ymd.ToString());
            ins.Parameters.Add("shift", ws.Shift);
            ins.Parameters.Add("shift_total", ws.Shift_Total);
            ins.Parameters.Add("n_count", ws.N_Count);

            return ins.ExecuteNonQuery();

        }

        /// <summary>
        /// Updates the hour_total and N_Cont in the west_shift table
        /// </summary>
        /// <param name="ws"></param>
        /// <returns></returns>
        public static int Update(West_Shift ws)
        {
            OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            try
            {
                return Update(ws, conn);
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
        /// Update the hour_total and N_Cont in the west_shift table
        /// </summary>
        /// <param name="ws"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public static int Update(West_Shift ws, OracleConnection conn)
        {
            StringBuilder sql = new();
            //only update the n_count and the hour_total, other fields should not need to be changed after inserting
            sql.AppendLine($"UPDATE west_main.west_shift_{ws.Plant} ");
            sql.AppendLine("    SET shift_total = :shift_total,");
            sql.AppendLine("    n_count = :n_count");
            sql.AppendLine("WHERE id = :id");
            sql.AppendLine($"    AND timestamp = :timestamp");

            OracleCommand cmd = new(sql.ToString(), conn);

            cmd.Parameters.Add("shift_total", ws.Shift_Total);
            cmd.Parameters.Add("n_count", ws.N_Count);
            cmd.Parameters.Add("id", ws.Id);
            cmd.Parameters.Add("timestamp", ws.TimeStamp);
            return cmd.ExecuteNonQuery();

        }



        /// <summary>
        /// Deletes the record in the west_shift table
        /// </summary>
        /// <param name="ws"></param>
        /// <returns></returns>
        public static int Delete(West_Shift ws)
        {
            OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            try
            {
                return Delete(ws, conn);
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
        /// Deletes the record in the west_shift table
        /// </summary>
        /// <param name="ws"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public static int Delete(West_Shift ws, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine($"DELETE FROM west_main.west_shift_{ws.Plant}");
            sql.AppendLine("WHERE id = :id");
            sql.AppendLine($"    AND timestamp = :timestamp");
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.Parameters.Add("id", ws.Id);
            cmd.Parameters.Add("timestamp", ws.TimeStamp);
            return cmd.ExecuteNonQuery();

        }

        private static West_Shift DataRowToObject(DataRow row)
        {
            West_Shift RetVal = new();
            RetVal.Id = row.Field<Int64>("id");
            RetVal.TimeStamp = row.Field<DateTime>("timestamp");
            RetVal.Ymd = Convert.ToInt32(row.Field<string>("ymd"));
            RetVal.Shift =row.Field<Int16>("shift");
            RetVal.Shift_Total = row.Field<decimal>("shift_total");
            RetVal.N_Count = Convert.ToInt32(row.Field<decimal>("n_count"));
            return RetVal;
        }
    }
}
