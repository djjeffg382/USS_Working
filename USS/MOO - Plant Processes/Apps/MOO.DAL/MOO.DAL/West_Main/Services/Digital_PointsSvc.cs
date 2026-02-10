using MOO.DAL.West_Main.Models;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.West_Main.Services
{
    public static class Digital_PointsSvc
    {
        static Digital_PointsSvc()
        {
            Util.RegisterOracle();
        }


        public static Digital_Points Get(MOO.Plant Plant, string Area, string Tag)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine("WHERE plant = :Plant");
            sql.AppendLine("AND area = :Area ");
            sql.AppendLine("AND Tag = :Tag");

            OracleDataAdapter da = new(sql.ToString(), MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            da.SelectCommand.Parameters.Add("Plant", Plant);
            da.SelectCommand.Parameters.Add("Area", Area);
            da.SelectCommand.Parameters.Add("Tag", Tag);

            DataSet ds = MOO.Data.ExecuteQuery(da);
            if (ds.Tables[0].Rows.Count == 1)
            {
                Digital_Points retVal = DataRowToObject(ds.Tables[0].Rows[0]);
                return retVal;
            }

            else
                return null;
        }

        public static List<Digital_Points> SearchByTagName(string TagNameSearch)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine("WHERE UPPER(Tag) LIKE '%' || :Tag || '%'");

            OracleDataAdapter da = new(sql.ToString(), MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            da.SelectCommand.Parameters.Add("Tag", TagNameSearch.ToUpper());

            DataSet ds = MOO.Data.ExecuteQuery(da);
            List<Digital_Points> retVal = new();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                retVal.Add(DataRowToObject(dr));
            }
            return retVal;
        }

        public static List<Digital_Points> SearchByDescription(string DescriptionSearch)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine("WHERE UPPER(Description) LIKE '%' || :Tag || '%'");

            OracleDataAdapter da = new(sql.ToString(), MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            da.SelectCommand.Parameters.Add("Tag", DescriptionSearch.ToUpper());

            DataSet ds = MOO.Data.ExecuteQuery(da);
            List<Digital_Points> retVal = new();
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
            sql.AppendLine("Plant, Area, Tag, Location, Description, on_val, off_val, Last_update");
            sql.AppendLine("FROM west_main.digital_points");
            return sql.ToString();
        }



        /// <summary>
        /// Inserts into the digital points table
        /// </summary>
        /// <param name="ap"></param>
        /// <returns></returns>
        public static int Insert(Digital_Points dp)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            return Insert(dp, conn);

        }

        /// <summary>
        /// Inserts the digital points table
        /// </summary>
        /// <param name="dp"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public static int Insert(Digital_Points dp, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine($"INSERT INTO west_main.digital_points (plant, area, ");
            sql.AppendLine("    tag, location, description, on_val, off_val, last_update)");
            sql.AppendLine("VALUES(:plant, :area, ");
            sql.AppendLine("    :tag, :location, :description,  :on_val, :off_val, SYSDATE)");

            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.Parameters.Add("plant", dp.Plant.ToString());
            cmd.Parameters.Add("area", dp.Area);
            cmd.Parameters.Add("tag", dp.Tag);
            cmd.Parameters.Add("location", dp.Location);
            cmd.Parameters.Add("description", dp.Description);
            cmd.Parameters.Add("on_val", dp.On_Val);
            cmd.Parameters.Add("off_val", dp.Off_Val);
            dp.Last_Update = DateTime.Now;

            return cmd.ExecuteNonQuery();

        }

        /// <summary>
        /// Updates the digital_points table
        /// </summary>
        /// <param name="dp"></param>
        /// <returns></returns>
        public static int Update(Digital_Points dp)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();

            return Update(dp, conn);


        }

        /// <summary>
        /// Update the digital_points table
        /// </summary>
        /// <param name="ap"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public static int Update(Digital_Points dp, OracleConnection conn)
        {
            StringBuilder sql = new();
            //only update the n_count and the hour_total, other fields should not need to be changed after inserting
            sql.AppendLine($"UPDATE west_main.digital_points ");
            sql.AppendLine("    SET location = :location, description = :description,");
            sql.AppendLine("    on_val = :on_val, off_val = :off_val, last_update = SYSDATE");
            sql.AppendLine("WHERE plant = :plant AND area = :area AND tag = :Tag");

            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.Parameters.Add("location", dp.Location);
            cmd.Parameters.Add("description", dp.Description);
            cmd.Parameters.Add("on_val", dp.On_Val);
            cmd.Parameters.Add("off_val", dp.Off_Val);

            cmd.Parameters.Add("plant", dp.Plant.ToString());
            cmd.Parameters.Add("area", dp.Area);
            cmd.Parameters.Add("tag", dp.Tag);
            dp.Last_Update = DateTime.Now;
            return cmd.ExecuteNonQuery();

        }



        /// <summary>
        /// Deletes the record in the digital_points table
        /// </summary>
        /// <param name="dp"></param>
        /// <returns></returns>
        public static int Delete(Digital_Points dp)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            return Delete(dp, conn);

        }

        /// <summary>
        /// Deletes the record in the digital_points table
        /// </summary>
        /// <param name="ap"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public static int Delete(Digital_Points dp, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine($"DELETE FROM west_main.digital_points");
            sql.AppendLine("WHERE plant = :plant AND area = :area AND tag = :Tag");
            OracleCommand cmd = new(sql.ToString(), conn);

            cmd.Parameters.Add("plant", dp.Plant.ToString());
            cmd.Parameters.Add("area", dp.Area);
            cmd.Parameters.Add("tag", dp.Tag);
            return cmd.ExecuteNonQuery();

        }

        private static Digital_Points DataRowToObject(DataRow row)
        {
            Digital_Points RetVal = new();
            RetVal.Plant = Enum.Parse<MOO.Plant>(row.Field<string>("plant"));
            RetVal.Area = row.Field<string>("Area");
            RetVal.Tag = row.Field<string>("Tag");
            RetVal.Location = row.Field<string>("Location");
            RetVal.Description = row.Field<string>("Description");
            RetVal.On_Val = row.Field<string>("On_Val");
            RetVal.Off_Val = row.Field<string>("Off_Val");

            RetVal.Last_Update = row.Field<DateTime>("Last_Update");
            return RetVal;
        }
    }
}
