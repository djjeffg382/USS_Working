using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MOO.DAL.West_Main.Models;
using System.Data;
using Oracle.ManagedDataAccess.Client;

namespace MOO.DAL.West_Main.Services
{
    public class West_HourlySvc
    {

        static West_HourlySvc()
        {
            Util.RegisterOracle();
        }


        public static West_Hourly Get(WestMainPlants Plant, int Id, int Ymd, int Hour)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect(Plant));
            sql.AppendLine($"WHERE id = {Id}");
            sql.AppendLine($"AND ymd = '{Ymd}' ");
            sql.AppendLine($"AND hour = '{Hour.ToString().PadLeft(2,'0')}' ");

            DataSet ds = MOO.Data.ExecuteQuery(sql.ToString(), Data.MNODatabase.DMART);
            if (ds.Tables[0].Rows.Count == 1)
            {
                West_Hourly retVal = DataRowToObject(ds.Tables[0].Rows[0]);
                retVal.Plant = Plant;
                return retVal;
            }
                
            else
                return null;
        }

        public static List<West_Hourly> GetYMDRange(WestMainPlants Plant, int Id, int YMDStart, int YMDEnd)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect(Plant));
            sql.AppendLine($"WHERE id = {Id}");
            sql.AppendLine($"AND ymd BETWEEN '{YMDStart}' AND '{YMDEnd}'");

            DataSet ds = MOO.Data.ExecuteQuery(sql.ToString(), Data.MNODatabase.DMART);

            List<West_Hourly> retVal = new();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                West_Hourly newObj = DataRowToObject(dr);
                newObj.Plant = Plant;
                retVal.Add(newObj);
            }
            return retVal;
        }

        private static string GetSelect(WestMainPlants Plant)
        {
            StringBuilder sql = new();
            sql.AppendLine("SELECT ");
            sql.AppendLine("id, timestamp, ymd, shift, half, hour, ROUND(hour_total,8) hour_total, n_count");            
            sql.AppendLine("FROM west_main.west_hourly_" + Plant.ToString());
            return sql.ToString();
        }



        /// <summary>
        /// Inserts into the west_hourly table
        /// </summary>
        /// <param name="wh"></param>
        /// <returns></returns>
        public static int Insert(West_Hourly wh)
        {
            OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            try
            {
                return Insert(wh, conn);
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
        /// Inserts the west_hourly table
        /// </summary>
        /// <param name="wh"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public static int Insert(West_Hourly wh, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine($"INSERT INTO west_main.west_hourly_{wh.Plant} (id, timestamp, ");
            sql.AppendLine("    ymd, shift,half,hour,");
            sql.AppendLine("    hour_total, n_count)");
            sql.AppendLine("VALUES(:id, :timestamp,");
            sql.AppendLine("    :ymd, :shift,:half, :hour,");
            sql.AppendLine("    :hour_total, :n_count)");

            OracleCommand ins = new(sql.ToString(), conn);
            ins.Parameters.Add("id", wh.Id);
            ins.Parameters.Add("timestamp",wh.TimeStamp);
            ins.Parameters.Add("ymd", wh.Ymd.ToString());
            ins.Parameters.Add("shift", wh.Shift.ToString());
            ins.Parameters.Add("half", wh.Half.ToString());
            ins.Parameters.Add("hour", wh.Hour.ToString().PadLeft(2,'0'));
            ins.Parameters.Add("hour_total", wh.Hour_Total);
            ins.Parameters.Add("n_count", wh.N_Count);

            return ins.ExecuteNonQuery();

        }

        /// <summary>
        /// Updates the hour_total and N_Cont in the west_hourly table
        /// </summary>
        /// <param name="wh"></param>
        /// <returns></returns>
        public static int Update(West_Hourly wh)
        {
            OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            try
            {
                return Update(wh, conn);
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
        /// Update the hour_total and N_Cont in the west_hourly table
        /// </summary>
        /// <param name="wh"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public static int Update(West_Hourly wh, OracleConnection conn)
        {
            StringBuilder sql = new();
            //only update the n_count and the hour_total, other fields should not need to be changed after inserting
            sql.AppendLine($"UPDATE west_main.west_hourly_{wh.Plant} ");
            sql.AppendLine("    SET hour_total = :hour_total,");
            sql.AppendLine("    n_count = :n_count");
            sql.AppendLine("WHERE id = :id");
            sql.AppendLine($"    AND timestamp = :timestamp");

            OracleCommand cmd = new(sql.ToString(), conn);

            cmd.Parameters.Add("hour_total", wh.Hour_Total);
            cmd.Parameters.Add("n_count", wh.N_Count);
            cmd.Parameters.Add("id", wh.Id);
            cmd.Parameters.Add("timestamp", wh.TimeStamp);
            return cmd.ExecuteNonQuery();

        }



        /// <summary>
        /// Deletes the record in the west_hourly table
        /// </summary>
        /// <param name="wh"></param>
        /// <returns></returns>
        public static int Delete(West_Hourly wh)
        {
            OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            try
            {
                return Delete(wh, conn);
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
        /// Deletes the record in the west_hourly table
        /// </summary>
        /// <param name="wh"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public static int Delete(West_Hourly wh, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine($"DELETE FROM west_main.west_hourly_{wh.Plant}");
            sql.AppendLine("WHERE id = :id");
            sql.AppendLine($"    AND timestamp = :timestamp");
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.Parameters.Add("id", wh.Id);
            cmd.Parameters.Add("timestamp", wh.TimeStamp);
            return cmd.ExecuteNonQuery();

        }

        private static West_Hourly DataRowToObject(DataRow row)
        {
            West_Hourly RetVal = new();
            RetVal.Id = row.Field<Int64>("id");
            RetVal.TimeStamp = row.Field<DateTime>("timestamp");
            RetVal.Ymd = Convert.ToInt32(row.Field<string>("ymd"));
            RetVal.Shift = Convert.ToInt16(row.Field<string>("shift"));
            RetVal.Hour = Convert.ToInt32(row.Field<string>("hour"));
            RetVal.Half = Convert.ToInt32(row.Field<string>("hour"));
            RetVal.Hour_Total = row.Field<decimal>("hour_total");
            RetVal.N_Count = Convert.ToInt32(row.Field<decimal>("n_count"));
            return RetVal;
        }
    }
}
