using MOO.DAL.West_Main.Models;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Common;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.West_Main.Services
{
    public static class VMS_DigitalSvc
    {
        static VMS_DigitalSvc()
        {
            Util.RegisterOracle();
        }


        public static VMS_Digital Get(WestMainPlants Plant, short Point_Id)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine($"WHERE plant = :plant");
            sql.AppendLine($"AND  point_id = :point_id");


            VMS_Digital retVal = null;
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.Parameters.Add("plant", Plant.ToString());
            cmd.Parameters.Add("Point_Id", Point_Id);
            OracleDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                rdr.Read();
                retVal = DataRowToObject(rdr);
            }
            conn.Close();
            return retVal;
        }

        public static List<VMS_Digital> GetAll()
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());

            List<VMS_Digital> elements = new();
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);
            OracleDataReader rdr = cmd.ExecuteReader();
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





        internal static string GetColumns(string TableAlias = "", string ColPrefix = "")
        {
            string ta = "";
            if (!string.IsNullOrEmpty(TableAlias))
                ta = TableAlias + ".";
            StringBuilder cols = new();
            cols.AppendLine($"{ta}plant {ColPrefix}plant, {ta}point_id {ColPrefix}point_id, {ta}point_name {ColPrefix}point_name, ");
            cols.AppendLine($"{ta}description {ColPrefix}description, {ta}last_update {ColPrefix}last_update, {ta}pi_pnt_type {ColPrefix}pi_pnt_type");
            return cols.ToString();
        }
        private static string GetSelect()
        {
            StringBuilder sql = new();
            sql.AppendLine("SELECT ");
            sql.AppendLine(GetColumns());
            sql.AppendLine("FROM west_main.vms_digital");
            return sql.ToString();
        }


        public static int Insert(VMS_Digital obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Insert(obj, conn);
            conn.Close();
            return recsAffected;
        }

        /// <summary>
        /// Gets the PI Point type for the given Digital point
        /// </summary>
        /// <returns></returns>
        /// <remarks>This is used to determine how to query time on and time off</remarks>
        private static string GetPiPointType(string PiTag)
        {
            MOO.DAL.Pi.Models.PiPointClassic pnt = MOO.DAL.Pi.Services.PiPointClassicSvc.Get(PiTag);
            if (pnt != null)
                return pnt.PointType.Substring(0,1);
            else
                return "";
        }

        public static int Insert(VMS_Digital obj, OracleConnection conn)
        {
            //on insert

            StringBuilder sql = new();
            sql.AppendLine("INSERT INTO west_main.VMS_Digital(");
            sql.AppendLine("plant, point_id, point_name, description, last_update, pi_pnt_type)");
            sql.AppendLine("VALUES(");
            sql.AppendLine(":plant, :point_id, :point_name, :description, SYSDATE, :pi_pnt_type)");
            OracleCommand ins = new(sql.ToString(), conn);
            ins.BindByName = true;
            ins.Parameters.Add("plant", obj.Plant.ToString());
            ins.Parameters.Add("point_id", obj.Point_Id);
            ins.Parameters.Add("point_name", obj.Point_Name);
            ins.Parameters.Add("description", obj.Description);
            ins.Parameters.Add("pi_pnt_type", GetPiPointType(obj.Point_Name));
            int recsAffected = MOO.Data.ExecuteNonQuery(ins);
            return recsAffected;
        }


        public static int Update(VMS_Digital obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Update(obj, conn);
            conn.Close();
            return recsAffected;
        }


        public static int Update(VMS_Digital obj, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("UPDATE west_main.VMS_Digital SET");
            sql.AppendLine("point_name = :point_name, ");
            sql.AppendLine("description = :description, ");
            sql.AppendLine("last_update = SYSDATE,");
            sql.AppendLine("pi_pnt_type = :pi_pnt_type");
            sql.AppendLine("WHERE plant = :plant");
            sql.AppendLine("AND point_id = :point_id");
            OracleCommand upd = new(sql.ToString(), conn);
            upd.BindByName = true;
            upd.Parameters.Add("point_name", obj.Point_Name);
            upd.Parameters.Add("description", obj.Description);
            upd.Parameters.Add("pi_pnt_type", GetPiPointType(obj.Point_Name));

            upd.Parameters.Add("plant", obj.Plant.ToString());
            upd.Parameters.Add("point_id", obj.Point_Id);
            int recsAffected = MOO.Data.ExecuteNonQuery(upd);
            return recsAffected;
        }


        public static int Delete(VMS_Digital obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Delete(obj, conn);
            conn.Close();
            return recsAffected;
        }

        public static int Delete(VMS_Digital obj, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("DELETE FROM west_main.VMS_Digital");
            sql.AppendLine("WHERE plant = :plant");
            sql.AppendLine("AND point_id = :point_id");
            OracleCommand del = new(sql.ToString(), conn);
            del.BindByName = true;
            del.Parameters.Add("plant", obj.Plant.ToString());
            del.Parameters.Add("point_id", obj.Point_Id);
            int recsAffected = MOO.Data.ExecuteNonQuery(del);
            return recsAffected;
        }



        /// <summary>
        /// Deletes all the points for a given plant
        /// </summary>
        /// <param name="Plant"></param>
        /// <returns></returns>
        public static int DeletePlantRecords(WestMainPlants Plant)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = DeletePlantRecords(Plant, conn);
            conn.Close();
            return recsAffected;
        }

        /// <summary>
        /// Deletes all the points for a given plant
        /// </summary>
        /// <param name="Plant"></param>
        /// <returns></returns>
        public static int DeletePlantRecords(WestMainPlants Plant, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("DELETE FROM west_main.VMS_Digital");
            sql.AppendLine("WHERE plant = :plant");
            OracleCommand del = new(sql.ToString(), conn);
            del.BindByName = true;
            del.Parameters.Add("plant", Plant.ToString());
            int recsAffected = MOO.Data.ExecuteNonQuery(del);
            return recsAffected;
        }


        internal static VMS_Digital DataRowToObject(DbDataReader row, string ColPrefix = "")
        {
            VMS_Digital RetVal = new();
            RetVal.Plant = Enum.Parse<WestMainPlants>((string)Util.GetRowVal(row, $"{ColPrefix}plant"));
            RetVal.Point_Id = (short)Util.GetRowVal(row, $"{ColPrefix}point_id");
            RetVal.Point_Name = (string)Util.GetRowVal(row, $"{ColPrefix}point_name");
            RetVal.Description = (string)Util.GetRowVal(row, $"{ColPrefix}description");
            RetVal.Last_Update = (DateTime)Util.GetRowVal(row, $"{ColPrefix}last_update");
            RetVal.Pi_Pnt_Type = (string)Util.GetRowVal(row, $"{ColPrefix}pi_pnt_type");
            return RetVal;
        }

    }
}
