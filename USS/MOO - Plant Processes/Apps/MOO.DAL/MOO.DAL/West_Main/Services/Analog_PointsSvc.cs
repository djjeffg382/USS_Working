using MOO.DAL.West_Main.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oracle.ManagedDataAccess.Client;

namespace MOO.DAL.West_Main.Services
{
    public static class Analog_PointsSvc
    {
        static Analog_PointsSvc()
        {
            Util.RegisterOracle();
        }


        public static Analog_Points Get(MOO.Plant Plant, string Area, string Tag)
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
                Analog_Points retVal = DataRowToObject(ds.Tables[0].Rows[0]);
                return retVal;
            }

            else
                return null;
        }

        public static List<Analog_Points> SearchByTagName(string TagNameSearch)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine("WHERE UPPER(Tag) LIKE '%' || :Tag || '%'");

            OracleDataAdapter da = new(sql.ToString(), MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            da.SelectCommand.Parameters.Add("Tag", TagNameSearch.ToUpper());

            DataSet ds = MOO.Data.ExecuteQuery(da);
            List<Analog_Points> retVal = new();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                retVal.Add(DataRowToObject(dr));
            }
            return retVal;
        }

        public static List<Analog_Points> SearchByDescription(string DescriptionSearch)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine("WHERE UPPER(Description) LIKE '%' || :Tag || '%'");

            OracleDataAdapter da = new(sql.ToString(), MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            da.SelectCommand.Parameters.Add("Tag", DescriptionSearch.ToUpper());

            DataSet ds = MOO.Data.ExecuteQuery(da);
            List<Analog_Points> retVal = new();
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
            sql.AppendLine("Plant, Area, Tag, Location, Description, UOM, Min, Max, Last_update");
            sql.AppendLine("FROM west_main.analog_points");
            return sql.ToString();
        }



        /// <summary>
        /// Inserts into the analog points table
        /// </summary>
        /// <param name="ap"></param>
        /// <returns></returns>
        public static int Insert(Analog_Points ap)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            return Insert(ap, conn);

        }

        /// <summary>
        /// Inserts the analog points table
        /// </summary>
        /// <param name="ap"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public static int Insert(Analog_Points ap, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine($"INSERT INTO west_main.analog_points (plant, area, ");
            sql.AppendLine("    tag, location, description, uom, min, max, last_update)");
            sql.AppendLine("VALUES(:plant, :area, ");
            sql.AppendLine("    :tag, :location, :description, :uom, :min, :max, SYSDATE)");

            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.Parameters.Add("plant", ap.Plant.ToString());
            cmd.Parameters.Add("area", ap.Area);
            cmd.Parameters.Add("tag", ap.Tag);
            cmd.Parameters.Add("location", ap.Location);
            cmd.Parameters.Add("description", ap.Description);
            cmd.Parameters.Add("uom", ap.UOM);
            cmd.Parameters.Add("min", ap.Min);
            cmd.Parameters.Add("max", ap.Max);
            ap.Last_Update = DateTime.Now;

            return cmd.ExecuteNonQuery();

        }

        /// <summary>
        /// Updates the analog_points table
        /// </summary>
        /// <param name="ap"></param>
        /// <returns></returns>
        public static int Update(Analog_Points ap)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();

            return Update(ap, conn);


        }

        /// <summary>
        /// Update the analog_points table
        /// </summary>
        /// <param name="ap"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public static int Update(Analog_Points ap, OracleConnection conn)
        {
            StringBuilder sql = new();
            //only update the n_count and the hour_total, other fields should not need to be changed after inserting
            sql.AppendLine($"UPDATE west_main.analog_points ");
            sql.AppendLine("    SET location = :location, description = :description,");
            sql.AppendLine("    uom = :uom, min = :min, max = :max, last_update = SYSDATE");
            sql.AppendLine("WHERE plant = :plant AND area = :area AND tag = :Tag");

            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.Parameters.Add("location", ap.Location);
            cmd.Parameters.Add("description", ap.Description);
            cmd.Parameters.Add("uom", ap.UOM);
            cmd.Parameters.Add("min", ap.Min);
            cmd.Parameters.Add("max", ap.Max);

            cmd.Parameters.Add("plant", ap.Plant.ToString());
            cmd.Parameters.Add("area", ap.Area);
            cmd.Parameters.Add("tag", ap.Tag);
            ap.Last_Update = DateTime.Now; 
            return cmd.ExecuteNonQuery();

        }



        /// <summary>
        /// Deletes the record in the analog_points table
        /// </summary>
        /// <param name="ap"></param>
        /// <returns></returns>
        public static int Delete(Analog_Points ap)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            return Delete(ap, conn);

        }

        /// <summary>
        /// Deletes the record in the analog_points table
        /// </summary>
        /// <param name="ap"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public static int Delete(Analog_Points ap, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine($"DELETE FROM west_main.analog_points");
            sql.AppendLine("WHERE plant = :plant AND area = :area AND tag = :Tag");
            OracleCommand cmd = new(sql.ToString(), conn);

            cmd.Parameters.Add("plant", ap.Plant.ToString());
            cmd.Parameters.Add("area", ap.Area);
            cmd.Parameters.Add("tag", ap.Tag);
            return cmd.ExecuteNonQuery();

        }

        private static Analog_Points DataRowToObject(DataRow row)
        {
            Analog_Points RetVal = new();
            RetVal.Plant = Enum.Parse<MOO.Plant>(row.Field<string>("plant"));
            RetVal.Area = row.Field<string>("Area");
            RetVal.Tag = row.Field<string>("Tag");
            RetVal.Location = row.Field<string>("Location");
            RetVal.Description = row.Field<string>("Description");
            RetVal.UOM = row.Field<string>("UOM");


            RetVal.Min = row.Field<decimal?>("Min");
            RetVal.Max = row.Field<decimal?>("Max");
            RetVal.Last_Update = row.Field<DateTime>("Last_Update");
            return RetVal;
        }
    }
}
