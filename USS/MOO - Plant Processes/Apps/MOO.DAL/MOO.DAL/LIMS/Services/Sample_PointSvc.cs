using Microsoft.Data.SqlClient;
using MOO.DAL.LIMS.Models;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.LIMS.Services
{
    public static class Sample_PointSvc
    {
        static Sample_PointSvc()
        {
            Util.RegisterSqlServer();
        }


        public static Sample_Point Get(string SampPointIdentity)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine($"WHERE sp.[IDENTITY] = @SampPointIdentity");
            sql.AppendLine("AND sp.removeflag = 'F'");


            Sample_Point retVal = null;
            using SqlConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.LIMS_Read));
            conn.Open();
            SqlCommand cmd = new(sql.ToString(), conn);
            cmd.Parameters.AddWithValue("SampPointIdentity", SampPointIdentity);
            SqlDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                rdr.Read();
                retVal = DataRowToObject(rdr);
            }
            conn.Close();
            return retVal;
        }



        public static List<Sample_Point> GetAll()
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine("WHERE sp.removeflag = 'F'");

            List<Sample_Point> elements = new();
            using SqlConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.LIMS_Read));
            conn.Open();
            SqlCommand cmd = new(sql.ToString(), conn);
            SqlDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                while (rdr.Read())
                {
                    elements.Add(DataRowToObject(rdr));
                }
            }
            conn.Close();
            return elements;
        }
        /// <summary>
        /// gets sample points by location
        /// </summary>
        /// <param name="Location"></param>
        /// <returns></returns>
        public static List<Sample_Point> GetByLocation(string Location)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine("WHERE sp.removeflag = 'F'");

            List<Sample_Point> elements = new();
            using SqlConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.LIMS_Read));
            conn.Open();
            SqlCommand cmd = new(sql.ToString(), conn);
            SqlDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                while (rdr.Read())
                {
                    if((string)Util.GetRowVal(rdr,"l_identity") == Location)
                        elements.Add(DataRowToObject(rdr));
                }
            }
            conn.Close();
            return elements;
        }


        internal static string GetColumns(string TableAlias = "sp", string ColPrefix = "sp_")
        {
            string ta = "";
            if (!string.IsNullOrEmpty(TableAlias))
                ta = TableAlias + ".";
            StringBuilder cols = new();
            cols.AppendLine($"{ta}[IDENTITY] {ColPrefix}identity, {ta}description {ColPrefix}description, {ta}name {ColPrefix}name");
            return cols.ToString();
        }
        private static string GetSelect()
        {
            string db = MOO.Settings.LIMS_DB_Name;

            //we will assume 3 levels deep for parent location
            StringBuilder sql = new();
            sql.AppendLine("SELECT ");
            sql.AppendLine(GetColumns("sp") + ",");
            sql.AppendLine(PhraseSvc.GetColumns("oil", "oil_")+ ",");
            sql.AppendLine(PhraseSvc.GetColumns("lube", "lube_") + "," );
            sql.AppendLine(LocationSvc.GetColumns("l", "l_") + ",");
            sql.AppendLine(LocationSvc.GetColumns("l2", "l2_") + ",");
            sql.AppendLine(LocationSvc.GetColumns("l3", "l3_"));
            sql.AppendLine($"FROM {db}.dbo.Sample_Point sp");
            sql.AppendLine($"LEFT JOIN {db}.dbo.phrase oil ON oil.phrase_type = 'OIL_TYPE' AND oil.phrase_id = sp.USS_Oil_Type");
            sql.AppendLine($"LEFT JOIN {db}.dbo.phrase lube ON lube.phrase_type = 'LBP_TYPE' AND lube.phrase_id = sp.USS_Lube_Point_Type");
            sql.AppendLine($"LEFT JOIN {db}.dbo.location l ON sp.point_location = l.[IDENTITY]");
            sql.AppendLine($"LEFT JOIN {db}.dbo.location l2 ON l.parent_location = l2.[IDENTITY]");
            sql.AppendLine($"LEFT JOIN {db}.dbo.location l3 ON l2.parent_location = l3.[IDENTITY]");
            return sql.ToString();
        }


        internal static Sample_Point DataRowToObject(DbDataReader row, string ColPrefix = "sp_",
                                                       string OilPrefix = "oil_", string LubePrefix ="lube_",
                                                       string LocPrefix = "l_", string LocParentPrefix = "l2_",
                                                       string LocGrandParentPrefix = "l3_")
        {
            Sample_Point RetVal = new();

            RetVal.Identity = (string)Util.GetRowVal(row, $"{ColPrefix}Identity");
            RetVal.Description = (string)Util.GetRowVal(row, $"{ColPrefix}Description");
            RetVal.Name = (string)Util.GetRowVal(row, $"{ColPrefix}Name");

            if(!row.IsDBNull(row.GetOrdinal($"{OilPrefix}phrase_id")))
            {
                RetVal.Oil_Type = PhraseSvc.DataRowToObject(row, OilPrefix);
            }
            if (!row.IsDBNull(row.GetOrdinal($"{LubePrefix}phrase_id")))
            {
                RetVal.Lube_Point_Type = PhraseSvc.DataRowToObject(row, LubePrefix);
            }

            if (!row.IsDBNull(row.GetOrdinal($"{LocPrefix}identity")))
            {
                RetVal.Point_Location = LocationSvc.DataRowToObject(row, LocPrefix,true,LocParentPrefix, LocGrandParentPrefix);
            }

            return RetVal;
        }
    }
}
