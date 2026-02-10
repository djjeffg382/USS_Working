using MOO.DAL.ToLive.Enums;
using MOO.DAL.ToLive.Models;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.ToLive.Services
{
    public static class Tire_Maint_TireSvc
    {
        static Tire_Maint_TireSvc()
        {
            Util.RegisterOracle();
        }


        public static Tire_Maint_Tire Get(int TireMaintTireId)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine("WHERE tire_maint_tire_id = :tire_maint_tire_id");


            Tire_Maint_Tire retVal = null;
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.Parameters.Add("tire_maint_tire_id", TireMaintTireId);
            cmd.BindByName = true;
            OracleDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                rdr.Read();
                retVal = DataRowToObject(rdr);
            }
            conn.Close();
            return retVal;
        }


        public static List<Tire_Maint_Tire> GetByTireMaintId(int TireMaintId)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine("WHERE tire_maint_id = :tire_maint_id");

            List<Tire_Maint_Tire> elements = [];
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.Parameters.Add("tire_maint_id", TireMaintId);
            cmd.BindByName = true;
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

        public static List<Tire_Maint_Tire> GetAll()
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());

            List<Tire_Maint_Tire> elements = [];
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.BindByName = true;
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


        internal static string GetColumns(string TableAlias = "tmt", string ColPrefix = "tmt")
        {
            string ta = "";
            if (!string.IsNullOrEmpty(TableAlias))
                ta = TableAlias + ".";
            StringBuilder cols = new();
            cols.AppendLine($"{ta}tire_maint_tire_id {ColPrefix}tire_maint_tire_id, {ta}tire_maint_id {ColPrefix}tire_maint_id, ");
            cols.AppendLine($"{ta}tire_position {ColPrefix}tire_position, {ta}brand_id {ColPrefix}brand_id, ");
            cols.AppendLine($"{ta}serial_nbr {ColPrefix}serial_nbr, {ta}tread_depth_in {ColPrefix}tread_depth_in, ");
            cols.AppendLine($"{ta}tread_depth_out {ColPrefix}tread_depth_out, {ta}removal_reason1 {ColPrefix}removal_reason1, ");
            cols.AppendLine($"{ta}removal_reason2 {ColPrefix}removal_reason2, {ta}disposition {ColPrefix}disposition, ");
            cols.AppendLine($"{ta}remove_install {ColPrefix}remove_install, {ta}nuts_torqued {ColPrefix}nuts_torqued, ");
            cols.AppendLine($"{ta}tire_shop_date {ColPrefix}tire_shop_date, {ta}tire_shop_status {ColPrefix}tire_shop_status");
            return cols.ToString();
        }
        private static string GetSelect()
        {
            StringBuilder sql = new();
            sql.AppendLine("SELECT ");
            sql.AppendLine(GetColumns() + ", ");
            sql.AppendLine(Tire_Maint_BrandSvc.GetColumns("tmb", "tmb"));
            sql.AppendLine("FROM tolive.tire_maint_tire tmt");
            sql.AppendLine("JOIN tolive.tire_maint_brand tmb");
            sql.AppendLine("    ON tmt.brand_id = tmb.tire_maint_brand_id");
            return sql.ToString();
        }


        public static int Insert(Tire_Maint_Tire obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Insert(obj, conn);
            conn.Close();
            return recsAffected;
        }


        public static int Insert(Tire_Maint_Tire obj, OracleConnection conn)
        {
            if (obj.Tire_Maint_Tire_Id <= 0)
                obj.Tire_Maint_Tire_Id = Convert.ToInt32(MOO.Data.GetNextSequence("tolive.seq_tire_maint"));

            StringBuilder sql = new();
            sql.AppendLine("INSERT INTO tolive.tire_maint_tire(");
            sql.AppendLine("tire_maint_tire_id, tire_maint_id, tire_position, brand_id, serial_nbr, ");
            sql.AppendLine("tread_depth_in, tread_depth_out, removal_reason1, removal_reason2, disposition, ");
            sql.AppendLine("remove_install, nuts_torqued, tire_shop_date, tire_shop_status)");
            sql.AppendLine("VALUES(");
            sql.AppendLine(":tire_maint_tire_id, :tire_maint_id, :tire_position, :brand_id, :serial_nbr, ");
            sql.AppendLine(":tread_depth_in, :tread_depth_out, :removal_reason1, :removal_reason2, :disposition, ");
            sql.AppendLine(":remove_install, :nuts_torqued, :tire_shop_date, :tire_shop_status)");
            OracleCommand ins = new(sql.ToString(), conn);
            ins.BindByName = true;
            ins.Parameters.Add("tire_maint_tire_id", obj.Tire_Maint_Tire_Id);
            ins.Parameters.Add("tire_maint_id", obj.Tire_Maint_Id);
            ins.Parameters.Add("tire_position", obj.Tire_Position);
            ins.Parameters.Add("brand_id", obj.Brand.Tire_Maint_Brand_Id);
            ins.Parameters.Add("serial_nbr", obj.Serial_Nbr);
            ins.Parameters.Add("tread_depth_in", obj.Tread_Depth_In);
            ins.Parameters.Add("tread_depth_out", obj.Tread_Depth_Out);
            ins.Parameters.Add("removal_reason1", (int?)obj.Removal_Reason1);
            ins.Parameters.Add("removal_reason2", (int?)obj.Removal_Reason2);
            ins.Parameters.Add("disposition", obj.Disposition.ToString());
            ins.Parameters.Add("remove_install", obj.Remove_Install == RemoveInstall.Install ? 1 : 0);
            ins.Parameters.Add("nuts_torqued", obj.Nuts_Torqued == null ? null : (bool)obj.Nuts_Torqued ? 1 : 0);
            ins.Parameters.Add("tire_shop_date", obj.Tire_Shop_Date);
            ins.Parameters.Add("tire_shop_status", obj.Tire_Shop_Status.ToString());
            int recsAffected = MOO.Data.ExecuteNonQuery(ins);
            return recsAffected;
        }


        public static int Update(Tire_Maint_Tire obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Update(obj, conn);
            conn.Close();
            return recsAffected;
        }


        public static int Update(Tire_Maint_Tire obj, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("UPDATE tolive.tire_maint_tire SET");
            sql.AppendLine("tire_maint_id = :tire_maint_id, ");
            sql.AppendLine("tire_position = :tire_position, ");
            sql.AppendLine("brand_id = :brand_id, ");
            sql.AppendLine("serial_nbr = :serial_nbr, ");
            sql.AppendLine("tread_depth_in = :tread_depth_in, ");
            sql.AppendLine("tread_depth_out = :tread_depth_out, ");
            sql.AppendLine("removal_reason1 = :removal_reason1, ");
            sql.AppendLine("removal_reason2 = :removal_reason2, ");
            sql.AppendLine("disposition = :disposition, ");
            sql.AppendLine("remove_install = :remove_install, ");
            sql.AppendLine("nuts_torqued = :nuts_torqued, ");
            sql.AppendLine("tire_shop_date = :tire_shop_date, ");
            sql.AppendLine("tire_shop_status = :tire_shop_status");
            sql.AppendLine("WHERE tire_maint_tire_id = :tire_maint_tire_id");
            OracleCommand upd = new(sql.ToString(), conn);
            upd.BindByName = true;
            upd.Parameters.Add("tire_maint_id", obj.Tire_Maint_Id);
            upd.Parameters.Add("tire_position", obj.Tire_Position);
            upd.Parameters.Add("brand_id", obj.Brand.Tire_Maint_Brand_Id);
            upd.Parameters.Add("serial_nbr", obj.Serial_Nbr);
            upd.Parameters.Add("tread_depth_in", obj.Tread_Depth_In);
            upd.Parameters.Add("tread_depth_out", obj.Tread_Depth_Out);
            upd.Parameters.Add("removal_reason1", (int?)obj.Removal_Reason1);
            upd.Parameters.Add("removal_reason2", (int?)obj.Removal_Reason2);
            upd.Parameters.Add("disposition", obj.Disposition.ToString());
            upd.Parameters.Add("remove_install", obj.Remove_Install == RemoveInstall.Install ? 1 : 0);
            upd.Parameters.Add("nuts_torqued", obj.Nuts_Torqued == null ? null : (bool)obj.Nuts_Torqued ? 1 : 0);
            upd.Parameters.Add("tire_shop_date", obj.Tire_Shop_Date);
            upd.Parameters.Add("tire_shop_status", obj.Tire_Shop_Status.ToString());
            upd.Parameters.Add("tire_maint_tire_id", obj.Tire_Maint_Tire_Id);
            int recsAffected = MOO.Data.ExecuteNonQuery(upd);
            return recsAffected;
        }


        public static int Delete(Tire_Maint_Tire obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Delete(obj, conn);
            conn.Close();
            return recsAffected;
        }

        public static int Delete(Tire_Maint_Tire obj, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("DELETE FROM tolive.tire_maint_tire");
            sql.AppendLine("WHERE tire_maint_tire_id = :tire_maint_tire_id");
            OracleCommand del = new(sql.ToString(), conn);
            del.BindByName = true;
            del.Parameters.Add("tire_maint_tire_id", obj.Tire_Maint_Tire_Id);
            int recsAffected = MOO.Data.ExecuteNonQuery(del);
            return recsAffected;
        }


        internal static Tire_Maint_Tire DataRowToObject(DbDataReader row, string ColPrefix = "tmt")
        {
            Tire_Maint_Tire RetVal = new();
            RetVal.Tire_Maint_Tire_Id = (long)Util.GetRowVal(row, $"{ColPrefix}tire_maint_tire_id");
            RetVal.Tire_Maint_Id = (long)Util.GetRowVal(row, $"{ColPrefix}tire_maint_id");
            RetVal.Tire_Position = (short)Util.GetRowVal(row, $"{ColPrefix}tire_position");
            RetVal.Brand = Tire_Maint_BrandSvc.DataRowToObject(row,"tmb");
            RetVal.Serial_Nbr = (string)Util.GetRowVal(row, $"{ColPrefix}serial_nbr");
            RetVal.Tread_Depth_In = (string)Util.GetRowVal(row, $"{ColPrefix}tread_depth_in");
            RetVal.Tread_Depth_Out = (string)Util.GetRowVal(row, $"{ColPrefix}tread_depth_out");
            RetVal.Removal_Reason1 = (TireMaintReason?)((short?)Util.GetRowVal(row, $"{ColPrefix}removal_reason1")??0);
            RetVal.Removal_Reason2 = (TireMaintReason?)((short?)Util.GetRowVal(row, $"{ColPrefix}removal_reason2")??0);
            RetVal.Disposition = (Disposition)Enum.Parse(typeof(Disposition),(string)Util.GetRowVal(row, $"{ColPrefix}disposition"));
            RetVal.Remove_Install = (short)Util.GetRowVal(row, $"{ColPrefix}remove_install") == 1 ? RemoveInstall.Install : RemoveInstall.Remove;
            RetVal.Nuts_Torqued = (short?)Util.GetRowVal(row, $"{ColPrefix}nuts_torqued") == 1;
            RetVal.Tire_Shop_Date = (DateTime?)Util.GetRowVal(row, $"{ColPrefix}tire_shop_date");
            RetVal.Tire_Shop_Status = (TireShopStatus)Enum.Parse(typeof(TireShopStatus),(string)Util.GetRowVal(row, $"{ColPrefix}tire_shop_status"));
            return RetVal;
        }

    }
}
