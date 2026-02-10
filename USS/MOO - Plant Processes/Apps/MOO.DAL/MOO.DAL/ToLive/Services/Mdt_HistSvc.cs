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
    public static class Mdt_HistSvc
    {

        static Mdt_HistSvc()
        {
            Util.RegisterOracle();
        }


        public static Mdt_Hist Get(string mac_addr1,string equipment_id,string device_name)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine($"WHERE mac_addr1 = :mac_addr1");
            sql.AppendLine($"AND equipment_id = :equipment_id");
            sql.AppendLine($"AND device_name = :device_name");

            OracleDataAdapter da = new(sql.ToString(), MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            da.SelectCommand.Parameters.Add("mac_addr1", mac_addr1);
            da.SelectCommand.Parameters.Add("equipment_id", equipment_id);
            da.SelectCommand.Parameters.Add("device_name", device_name);

            da.SelectCommand.BindByName = true;

            DataSet ds = MOO.Data.ExecuteQuery(da);
            if (ds.Tables[0].Rows.Count == 1)
                return DataRowToObject(ds.Tables[0].Rows[0]);
            else
                return null;
        }
        public static List<Mdt_Hist> GetAll(string mac_addr1)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());

            sql.AppendLine($"WHERE mac_addr1 = :mac_addr1");

            OracleDataAdapter da = new(sql.ToString(), MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            da.SelectCommand.Parameters.Add("mac_addr1", mac_addr1);
            
            da.SelectCommand.BindByName = true;

            DataSet ds = MOO.Data.ExecuteQuery(da);
            List<Mdt_Hist> elements = new();
            if (ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    elements.Add(DataRowToObject(dr));
                }
            }
            return elements;
        }

        private static string GetSelect()
        {
            StringBuilder sql = new();
            sql.AppendLine("SELECT *");
            sql.AppendLine("FROM tolive.mdt_hist");
            return sql.ToString();
        }

        public static int Insert(Mdt_Hist obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Insert(obj, conn);
            conn.Close();
            return recsAffected;
        }


        public static int Insert(Mdt_Hist obj, OracleConnection conn)
        {

            StringBuilder sql = new();
            sql.AppendLine("INSERT INTO tolive.Mdt_Hist(");
            sql.AppendLine("mac_addr1, device_name,  equipment_id, plant)");
            sql.AppendLine("VALUES(");
            sql.AppendLine(":mac_addr1,:device_name,:equipment_id, :plant)");

            OracleCommand ins = new(sql.ToString(), conn);
            ins.BindByName = true;
            ins.Parameters.Add("mac_addr1", obj.Mac_Addr1);
            ins.Parameters.Add("device_name", obj.Device_Name);
            ins.Parameters.Add("equipment_id", obj.Equipment_Id);
            ins.Parameters.Add("plant", obj.Plant.ToString());
            int recsAffected = MOO.Data.ExecuteNonQuery(ins);
            return recsAffected;
        }




        internal static Mdt_Hist DataRowToObject(DataRow row)
        {
            Mdt_Hist RetVal = new();
            RetVal.Mac_Addr1 = row.Field<string>($"mac_addr1");
            RetVal.Device_Name = row.Field<string>($"device_name");
            RetVal.Equipment_Id = row.Field<string>($"equipment_id");
            RetVal.Insert_Date = row.Field<DateTime>($"Insert_Date");
            if (!row.IsNull("plant"))
                RetVal.Plant = Enum.Parse<MOO.Plant>(row.Field<string>($"plant"));

            return RetVal;
        }


    }

}
