using MOO.DAL.ToLive.Models;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.ToLive.Services
{
    public static class Mdt_DeviceSvc
    {
        static Mdt_DeviceSvc()
        {
            Util.RegisterOracle();
        }


        public static Mdt_Device Get(string mac_addr1)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine($"WHERE mac_addr1 = :mac_addr1");

            OracleDataAdapter da = new(sql.ToString(), MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            da.SelectCommand.Parameters.Add("mac_addr1", mac_addr1);
            da.SelectCommand.BindByName = true;

            DataSet ds = MOO.Data.ExecuteQuery(da);
            if (ds.Tables[0].Rows.Count == 1)
                return DataRowToObject(ds.Tables[0].Rows[0]);
            else
                return null;
        }

        public static List<Mdt_Device> GetAll()
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());

            OracleDataAdapter da = new(sql.ToString(), MOO.Data.GetConnectionString(Data.MNODatabase.DMART));

            DataSet ds = MOO.Data.ExecuteQuery(da);
            List<Mdt_Device> elements = new();
            if (ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    elements.Add(DataRowToObject(dr));
                }
            }
            return elements;
        }

        public static List<Mdt_Device> GetAll(MOO.Plant Plant)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine($"WHERE plant = :Plant");

            OracleDataAdapter da = new(sql.ToString(), MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            da.SelectCommand.Parameters.Add("Plant", Plant);
            da.SelectCommand.BindByName = true;

            DataSet ds = MOO.Data.ExecuteQuery(da);
            List<Mdt_Device> elements = new();
            if (ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    elements.Add(DataRowToObject(dr));
                }
            }
            return elements;
        }


        internal static string GetColumns(string TableAlias = "", string ColPrefix = "")
        {
            string ta = "";
            if (!string.IsNullOrEmpty(TableAlias))
                ta = TableAlias + ".";
            StringBuilder cols = new();
            cols.AppendLine($"{ta}mac_addr1 {ColPrefix}mac_addr1, {ta}mac_addr2 {ColPrefix}mac_addr2, ");
            cols.AppendLine($"{ta}mesh_mac_addr {ColPrefix}mesh_mac_addr, {ta}device_name {ColPrefix}device_name, ");
            cols.AppendLine($"{ta}ip_addr {ColPrefix}ip_addr, {ta}software {ColPrefix}software, ");
            cols.AppendLine($"{ta}last_update {ColPrefix}last_update, {ta}model {ColPrefix}model, ");
            cols.AppendLine($"{ta}os_version {ColPrefix}os_version, {ta}disk_size_mb {ColPrefix}disk_size_mb, ");
            cols.AppendLine($"{ta}equipment_id {ColPrefix}equipment_id, {ta}insert_date {ColPrefix}insert_date, ");
            cols.AppendLine($"{ta}plant {ColPrefix}plant");
            return cols.ToString();
        }
        private static string GetSelect()
        {
            StringBuilder sql = new();
            sql.AppendLine("SELECT ");
            sql.AppendLine(GetColumns());
            sql.AppendLine("FROM tolive.mdt_device");
            return sql.ToString();
        }


        public static int Insert(Mdt_Device obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Insert(obj, conn);
            conn.Close();
            return recsAffected;
        }


        public static int Insert(Mdt_Device obj, OracleConnection conn)
        {
            
            StringBuilder sql = new();
            sql.AppendLine("INSERT INTO tolive.Mdt_Device(");
            sql.AppendLine("mac_addr1, mac_addr2, mesh_mac_addr, device_name, ip_addr, software, last_update, ");
            sql.AppendLine("model, os_version, disk_size_mb, equipment_id, insert_date, plant)");
            sql.AppendLine("VALUES(");
            sql.AppendLine(":mac_addr1, :mac_addr2, :mesh_mac_addr, :device_name, :ip_addr, :software, ");
            sql.AppendLine(":last_update, :model, :os_version, :disk_size_mb, :equipment_id, :insert_date, :plant)");
            OracleCommand ins = new(sql.ToString(), conn);
            ins.BindByName = true;
            ins.Parameters.Add("mac_addr1", obj.Mac_Addr1);
            ins.Parameters.Add("mac_addr2", obj.Mac_Addr2);
            ins.Parameters.Add("mesh_mac_addr", obj.Mesh_Mac_Addr);
            ins.Parameters.Add("device_name", obj.Device_Name);
            ins.Parameters.Add("ip_addr", obj.Ip_Addr);
            ins.Parameters.Add("software", obj.Software);
            ins.Parameters.Add("last_update", obj.Last_Update);
            ins.Parameters.Add("model", obj.Model);
            ins.Parameters.Add("os_version", obj.Os_Version);
            ins.Parameters.Add("disk_size_mb", obj.Disk_Size_Mb);
            ins.Parameters.Add("equipment_id", obj.Equipment_Id);
            ins.Parameters.Add("insert_date", obj.Insert_Date);
            ins.Parameters.Add("plant", obj.Plant.ToString());
            int recsAffected = MOO.Data.ExecuteNonQuery(ins);
            return recsAffected;
        }


        public static int Update(Mdt_Device obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Update(obj, conn);
            conn.Close();
            return recsAffected;
        }


        public static int Update(Mdt_Device obj, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("UPDATE tolive.Mdt_Device SET");
            sql.AppendLine("mac_addr2 = :mac_addr2, ");
            sql.AppendLine("mesh_mac_addr = :mesh_mac_addr, ");
            sql.AppendLine("device_name = :device_name, ");
            sql.AppendLine("ip_addr = :ip_addr, ");
            sql.AppendLine("software = :software, ");
            sql.AppendLine("last_update = :last_update, ");
            sql.AppendLine("model = :model, ");
            sql.AppendLine("os_version = :os_version, ");
            sql.AppendLine("disk_size_mb = :disk_size_mb, ");
            sql.AppendLine("equipment_id = :equipment_id, ");
            sql.AppendLine("plant = :plant");
            sql.AppendLine("WHERE mac_addr1 = :mac_addr1");
            OracleCommand upd = new(sql.ToString(), conn);
            upd.BindByName = true;
            upd.Parameters.Add("mac_addr2", obj.Mac_Addr2);
            upd.Parameters.Add("mesh_mac_addr", obj.Mesh_Mac_Addr);
            upd.Parameters.Add("device_name", obj.Device_Name);
            upd.Parameters.Add("ip_addr", obj.Ip_Addr);
            upd.Parameters.Add("software", obj.Software);
            upd.Parameters.Add("last_update", obj.Last_Update);
            upd.Parameters.Add("model", obj.Model);
            upd.Parameters.Add("os_version", obj.Os_Version);
            upd.Parameters.Add("disk_size_mb", obj.Disk_Size_Mb);
            upd.Parameters.Add("equipment_id", obj.Equipment_Id);
            upd.Parameters.Add("plant", obj.Plant.ToString());
            upd.Parameters.Add("mac_addr1", obj.Mac_Addr1);
            int recsAffected = MOO.Data.ExecuteNonQuery(upd);
            return recsAffected;
        }


        public static int Delete(Mdt_Device obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Delete(obj, conn);
            conn.Close();
            return recsAffected;
        }

        public static int Delete(Mdt_Device obj, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("DELETE FROM tolive.Mdt_Device");
            sql.AppendLine("WHERE mac_addr1 = :mac_addr1");
            OracleCommand del = new(sql.ToString(), conn);
            del.BindByName = true;
            del.Parameters.Add("mac_addr1", obj.Mac_Addr1);
            int recsAffected = MOO.Data.ExecuteNonQuery(del);
            return recsAffected;
        }


        internal static Mdt_Device DataRowToObject(DataRow row, string ColPrefix = "")
        {
            Mdt_Device RetVal = new();
            RetVal.Mac_Addr1 = row.Field<string>($"{ColPrefix}mac_addr1");
            RetVal.Mac_Addr2 = row.Field<string>($"{ColPrefix}mac_addr2");
            RetVal.Mesh_Mac_Addr = row.Field<string>($"{ColPrefix}mesh_mac_addr");
            RetVal.Device_Name = row.Field<string>($"{ColPrefix}device_name");
            RetVal.Ip_Addr = row.Field<string>($"{ColPrefix}ip_addr");
            RetVal.Software = row.Field<string>($"{ColPrefix}software");
            RetVal.Last_Update = row.Field<DateTime>($"{ColPrefix}last_update");
            RetVal.Model = row.Field<string>($"{ColPrefix}model");
            RetVal.Os_Version = row.Field<string>($"{ColPrefix}os_version");
            RetVal.Disk_Size_Mb = row.Field<int?>($"{ColPrefix}disk_size_mb");
            RetVal.Equipment_Id = row.Field<string>($"{ColPrefix}equipment_id");
            RetVal.Insert_Date = row.Field<DateTime>($"{ColPrefix}insert_date");
            if(!row.IsNull("plant"))
                RetVal.Plant = Enum.Parse<MOO.Plant>( row.Field<string>($"{ColPrefix}plant"));

            return RetVal;
        }

    }
}
