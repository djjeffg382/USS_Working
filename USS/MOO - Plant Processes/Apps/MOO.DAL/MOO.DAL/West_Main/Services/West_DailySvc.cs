using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MOO.DAL.West_Main.Models;
using System.Data;
using Oracle.ManagedDataAccess.Client;

namespace MOO.DAL.West_Main.Services
{
    public class West_DailySvc
    {

        static West_DailySvc()
        {
            Util.RegisterOracle();
        }


        public static West_Daily Get(WestMainPlants Plant, int Id, int Ymd)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect(Plant));
            sql.AppendLine($"WHERE id = {Id}");
            sql.AppendLine($"AND ymd = '{Ymd}' ");

            DataSet ds = MOO.Data.ExecuteQuery(sql.ToString(), Data.MNODatabase.DMART);
            if (ds.Tables[0].Rows.Count == 1)
            {
                West_Daily retVal = DataRowToObject(ds.Tables[0].Rows[0]);
                retVal.Plant = Plant;
                return retVal;
            }
                
            else
                return null;
        }

        public static List<West_Daily> GetYMDRange(WestMainPlants Plant, int Id, int YMDStart, int YMDEnd)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect(Plant));
            sql.AppendLine($"WHERE id = {Id}");
            sql.AppendLine($"AND ymd BETWEEN '{YMDStart}' AND '{YMDEnd}'");

            DataSet ds = MOO.Data.ExecuteQuery(sql.ToString(), Data.MNODatabase.DMART);

            List<West_Daily> retVal = new();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                West_Daily newObj = DataRowToObject(dr);
                newObj.Plant = Plant;
                retVal.Add(newObj);
            }
            return retVal;
        }

        private static string GetSelect(WestMainPlants Plant)
        {
            StringBuilder sql = new();
            sql.AppendLine("SELECT ");
            sql.AppendLine("id, timestamp, ymd, ROUND(day_total,8) day_total, n_count");            
            sql.AppendLine("FROM west_main.west_daily_" + Plant.ToString());
            return sql.ToString();
        }



        /// <summary>
        /// Inserts into the west_shift table
        /// </summary>
        /// <param name="wd"></param>
        /// <returns></returns>
        public static int Insert(West_Daily wd)
        {
            OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            try
            {
                return Insert(wd, conn);
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
        /// Inserts the west_daily table
        /// </summary>
        /// <param name="wd"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public static int Insert(West_Daily wd, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine($"INSERT INTO west_main.west_daily_{wd.Plant} (id, timestamp, ");
            sql.AppendLine("    ymd, day_total, n_count)");
            sql.AppendLine("VALUES(:id, :timestamp,");
            sql.AppendLine("    :ymd, :day_total, :n_count)");

            OracleCommand ins = new(sql.ToString(), conn);
            ins.Parameters.Add("id", wd.Id);
            ins.Parameters.Add("timestamp",wd.TimeStamp);
            ins.Parameters.Add("ymd", wd.Ymd.ToString());
            ins.Parameters.Add("day_total", wd.Day_Total);
            ins.Parameters.Add("n_count", wd.N_Count);

            return ins.ExecuteNonQuery();

        }

        /// <summary>
        /// Updates the hour_total and N_Cont in the west_daily table
        /// </summary>
        /// <param name="wd"></param>
        /// <returns></returns>
        public static int Update(West_Daily wd)
        {
            OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            try
            {
                return Update(wd, conn);
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
        /// <param name="wd"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public static int Update(West_Daily wd, OracleConnection conn)
        {
            StringBuilder sql = new();
            //only update the n_count and the hour_total, other fields should not need to be changed after inserting
            sql.AppendLine($"UPDATE west_main.west_daily_{wd.Plant} ");
            sql.AppendLine("    SET day_total = :day_total,");
            sql.AppendLine("    n_count = :n_count");
            sql.AppendLine("WHERE id = :id");
            sql.AppendLine($"    AND timestamp = :timestamp");

            OracleCommand cmd = new(sql.ToString(), conn);

            cmd.Parameters.Add("day_total", wd.Day_Total);
            cmd.Parameters.Add("n_count", wd.N_Count);
            cmd.Parameters.Add("id", wd.Id);
            cmd.Parameters.Add("timestamp", wd.TimeStamp);
            return cmd.ExecuteNonQuery();

        }



        /// <summary>
        /// Deletes the record in the west_daily table
        /// </summary>
        /// <param name="wd"></param>
        /// <returns></returns>
        public static int Delete(West_Daily wd)
        {
            OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            try
            {
                return Delete(wd, conn);
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
        /// Deletes the record in the west_daily table
        /// </summary>
        /// <param name="wd"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public static int Delete(West_Daily wd, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine($"DELETE FROM west_main.west_daily_{wd.Plant}");
            sql.AppendLine("WHERE id = :id");
            sql.AppendLine($"    AND timestamp = :timestamp");
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.Parameters.Add("id", wd.Id);
            cmd.Parameters.Add("timestamp", wd.TimeStamp);
            return cmd.ExecuteNonQuery();

        }

        private static West_Daily DataRowToObject(DataRow row)
        {
            West_Daily RetVal = new();
            RetVal.Id = row.Field<Int64>("id");
            RetVal.TimeStamp = row.Field<DateTime>("timestamp");
            RetVal.Ymd = Convert.ToInt32(row.Field<string>("ymd"));
            RetVal.Day_Total = row.Field<decimal>("day_total");
            RetVal.N_Count = Convert.ToInt32(row.Field<decimal>("n_count"));
            return RetVal;
        }
    }
}
