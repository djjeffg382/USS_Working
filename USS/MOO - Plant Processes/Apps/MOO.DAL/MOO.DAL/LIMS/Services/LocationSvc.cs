using MOO.DAL.ToLive.Models;
using MOO.DAL.ToLive.Services;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MOO.DAL.LIMS.Models;
using Microsoft.Data.SqlClient;
using System.Security.Principal;

namespace MOO.DAL.LIMS.Services
{
    public static class LocationSvc
    {

        static LocationSvc()
        {
            Util.RegisterSqlServer();
        }


        public static Location Get(string Identity)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine($"WHERE l.[IDENTITY] = @Identity");
            sql.AppendLine("AND l.removeflag = 'F'");


            Location retVal = null;
            using SqlConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.LIMS_Read));
            conn.Open();
            SqlCommand cmd = new(sql.ToString(), conn);
            cmd.Parameters.AddWithValue("Identity", Identity);
            SqlDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                rdr.Read();
                retVal = DataRowToObject(rdr, "l_", true);
            }
            conn.Close();
            return retVal;
        }


        /// <summary>
        /// Gets list of all locations
        /// </summary>
        /// <param name="ParentLocationId">Optional parameter to filter by parent id</param>
        /// <returns></returns>
        public static List<Location> GetAll(string ParentLocationId = "")
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine("WHERE l.removeflag = 'F'");
            if (!string.IsNullOrEmpty(ParentLocationId))
                sql.AppendLine($"AND l2.[IDENTITY] = @Parent");

            List<Location> elements = new();
            using SqlConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.LIMS_Read));
            conn.Open();
            SqlCommand cmd = new(sql.ToString(), conn);
            if (!string.IsNullOrEmpty(ParentLocationId))
                cmd.Parameters.AddWithValue("Parent", ParentLocationId);

            SqlDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                while (rdr.Read())
                {
                    elements.Add(DataRowToObject(rdr, "l_",true));
                }
            }
            conn.Close();
            return elements;
        }




        internal static string GetColumns(string TableAlias = "", string ColPrefix = "")
        {
            string ta = "";
            if (!string.IsNullOrEmpty(TableAlias))
                ta = TableAlias + ".";
            StringBuilder cols = new();
            cols.AppendLine($"{ta}[IDENTITY] {ColPrefix}IDENTITY, {ta}Description {ColPrefix}Description, {ta}Name {ColPrefix}Name, ");
            cols.AppendLine($"{ta}Location_Type {ColPrefix}Location_Type");
            return cols.ToString();
        }
        private static string GetSelect()
        {
            string db = MOO.Settings.LIMS_DB_Name;

            //we will assume 3 levels deep for parent location
            StringBuilder sql = new();
            sql.AppendLine("SELECT ");
            sql.AppendLine(GetColumns("l", "l_") + ",");
            sql.AppendLine(GetColumns("l2", "l2_") + ",");
            sql.AppendLine(GetColumns("l3", "l3_"));
            sql.AppendLine($"FROM {db}.dbo.location l");
            sql.AppendLine($"LEFT JOIN {db}.dbo.location l2 ON l.parent_location = l2.[IDENTITY]");
            sql.AppendLine($"LEFT JOIN {db}.dbo.location l3 ON l2.parent_location = l3.[IDENTITY]");
            return sql.ToString();
        }


        internal static Location DataRowToObject(DbDataReader row, string ColPrefix = "", bool FillParentLoc = false,
                                                        string ParentPrefix = "l2_", string GrandParentPrefix = "l3_")
        {
            Location RetVal = new();
            RetVal.Identity = (string)Util.GetRowVal(row, $"{ColPrefix}Identity");
            RetVal.Name = (string)Util.GetRowVal(row, $"{ColPrefix}Name");
            RetVal.Description = (string)Util.GetRowVal(row, $"{ColPrefix}Description");
            RetVal.Location_Type = (string)Util.GetRowVal(row, $"{ColPrefix}Location_Type");

            if (FillParentLoc && Util.GetRowVal(row, $"{ParentPrefix}identity") !=null)
            {
                RetVal.Parent_Location = DataRowToObject(row, ParentPrefix, false);
                if(RetVal.Parent_Location !=null && Util.GetRowVal(row, $"{GrandParentPrefix}identity") != null)
                    RetVal.Parent_Location.Parent_Location = DataRowToObject(row, GrandParentPrefix, false);
            }


            return RetVal;
        }


    }
}
