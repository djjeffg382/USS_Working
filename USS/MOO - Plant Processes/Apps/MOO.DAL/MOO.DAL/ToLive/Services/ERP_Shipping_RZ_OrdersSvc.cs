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
    public static class ERP_Shipping_RZ_OrdersSvc
    {
        static ERP_Shipping_RZ_OrdersSvc()
        {
            Util.RegisterOracle();
        }


        public static ERP_Shipping_RZ_Orders Get(int works, string orderNbr, int dateEnt)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine("WHERE works = :works");
            sql.AppendLine("AND order_num = :orderNbr");
            sql.AppendLine("AND date_ent = :dateEnt");


            ERP_Shipping_RZ_Orders retVal = null;
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.BindByName = true;
            cmd.Parameters.Add("works", works);
            cmd.Parameters.Add("orderNbr", orderNbr);
            cmd.Parameters.Add("dateEnt", dateEnt);
            OracleDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                rdr.Read();
                retVal = DataRowToObject(rdr);
            }
            conn.Close();
            return retVal;
        }

        public static List<ERP_Shipping_RZ_Orders> GetAll(bool showActiveOnly = true)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            if (showActiveOnly)
                sql.AppendLine("WHERE active = 'A'");

            List<ERP_Shipping_RZ_Orders> elements = [];
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


        internal static string GetColumns(string TableAlias = "", string ColPrefix = "")
        {
            string ta = "";
            if (!string.IsNullOrEmpty(TableAlias))
                ta = TableAlias + ".";
            StringBuilder cols = new();
            cols.AppendLine($"{ta}works {ColPrefix}works, {ta}order_num {ColPrefix}order_num, {ta}date_ent {ColPrefix}date_ent, ");
            cols.AppendLine($"{ta}time_ent {ColPrefix}time_ent, {ta}unit_type {ColPrefix}unit_type, {ta}active {ColPrefix}active, ");
            cols.AppendLine($"{ta}frt_terms {ColPrefix}frt_terms, {ta}expire_date {ColPrefix}expire_date, ");
            cols.AppendLine($"{ta}customer {ColPrefix}customer, {ta}location {ColPrefix}location, ");
            cols.AppendLine($"{ta}consignee {ColPrefix}consignee, {ta}destination {ColPrefix}destination, ");
            cols.AppendLine($"{ta}state {ColPrefix}state, {ta}zip {ColPrefix}zip, {ta}country {ColPrefix}country, ");
            cols.AppendLine($"{ta}description {ColPrefix}description, {ta}total_order_wgt {ColPrefix}total_order_wgt, ");
            cols.AppendLine($"{ta}wgt_remaining {ColPrefix}wgt_remaining, {ta}forced_referral {ColPrefix}forced_referral, ");
            cols.AppendLine($"{ta}ref_english {ColPrefix}ref_english, {ta}ref_date {ColPrefix}ref_date, ");
            cols.AppendLine($"{ta}ebs_order {ColPrefix}ebs_order, {ta}ebs_line {ColPrefix}ebs_line, ");
            cols.AppendLine($"{ta}descrete_org {ColPrefix}descrete_org, {ta}ebs_item {ColPrefix}ebs_item, ");
            cols.AppendLine($"{ta}non_ebs {ColPrefix}non_ebs");
            return cols.ToString();
        }
        private static string GetSelect()
        {
            StringBuilder sql = new();
            sql.AppendLine("SELECT ");
            sql.AppendLine(GetColumns());
            sql.AppendLine("FROM tolive.erp_shipping_rz_orders");
            return sql.ToString();
        }


        public static int Insert(ERP_Shipping_RZ_Orders obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Insert(obj, conn);
            conn.Close();
            return recsAffected;
        }


        public static int Insert(ERP_Shipping_RZ_Orders obj, OracleConnection conn)
        {
            
            StringBuilder sql = new();
            sql.AppendLine("INSERT INTO tolive.ERP_Shipping_RZ_Orders(");
            sql.AppendLine("works, order_num, date_ent, time_ent, unit_type, active, frt_terms, expire_date, ");
            sql.AppendLine("customer, location, consignee, destination, state, zip, country, description, ");
            sql.AppendLine("total_order_wgt, wgt_remaining, forced_referral, ref_english, ref_date, ebs_order, ");
            sql.AppendLine("ebs_line, descrete_org, ebs_item, non_ebs)");
            sql.AppendLine("VALUES(");
            sql.AppendLine(":works, :order_num, :date_ent, :time_ent, :unit_type, :active, :frt_terms, ");
            sql.AppendLine(":expire_date, :customer, :location, :consignee, :destination, :state, :zip, :country, ");
            sql.AppendLine(":description, :total_order_wgt, :wgt_remaining, :forced_referral, :ref_english, ");
            sql.AppendLine(":ref_date, :ebs_order, :ebs_line, :descrete_org, :ebs_item, :non_ebs)");
            OracleCommand ins = new(sql.ToString(), conn);
            ins.BindByName = true;
            ins.Parameters.Add("works", obj.Works);
            ins.Parameters.Add("order_num", obj.Order_Num);
            ins.Parameters.Add("date_ent", obj.Date_Ent);
            ins.Parameters.Add("time_ent", obj.Time_Ent);
            ins.Parameters.Add("unit_type", obj.Unit_Type);
            ins.Parameters.Add("active", obj.Active ? "A" : "I");
            ins.Parameters.Add("frt_terms", obj.Frt_Terms);
            ins.Parameters.Add("expire_date", obj.Expire_Date);
            ins.Parameters.Add("customer", obj.Customer);
            ins.Parameters.Add("location", obj.Location);
            ins.Parameters.Add("consignee", obj.Consignee);
            ins.Parameters.Add("destination", obj.Destination);
            ins.Parameters.Add("state", obj.State);
            ins.Parameters.Add("zip", obj.Zip);
            ins.Parameters.Add("country", obj.Country);
            ins.Parameters.Add("description", obj.Description);
            ins.Parameters.Add("total_order_wgt", obj.Total_Order_Wgt);
            ins.Parameters.Add("wgt_remaining", obj.Wgt_Remaining);
            ins.Parameters.Add("forced_referral", obj.Forced_Referral);
            ins.Parameters.Add("ref_english", obj.Ref_English);
            ins.Parameters.Add("ref_date", obj.Ref_Date);
            ins.Parameters.Add("ebs_order", obj.Ebs_Order);
            ins.Parameters.Add("ebs_line", obj.Ebs_Line);
            ins.Parameters.Add("descrete_org", obj.Descrete_Org);
            ins.Parameters.Add("ebs_item", obj.Ebs_Item);
            ins.Parameters.Add("non_ebs", obj.Non_Ebs);
            int recsAffected = MOO.Data.ExecuteNonQuery(ins);
            return recsAffected;
        }


        public static int Update(ERP_Shipping_RZ_Orders obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Update(obj, conn);
            conn.Close();
            return recsAffected;
        }


        public static int Update(ERP_Shipping_RZ_Orders obj, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("UPDATE tolive.ERP_Shipping_RZ_Orders SET");
            sql.AppendLine("time_ent = :time_ent, ");
            sql.AppendLine("unit_type = :unit_type, ");
            sql.AppendLine("active = :active, ");
            sql.AppendLine("frt_terms = :frt_terms, ");
            sql.AppendLine("expire_date = :expire_date, ");
            sql.AppendLine("customer = :customer, ");
            sql.AppendLine("location = :location, ");
            sql.AppendLine("consignee = :consignee, ");
            sql.AppendLine("destination = :destination, ");
            sql.AppendLine("state = :state, ");
            sql.AppendLine("zip = :zip, ");
            sql.AppendLine("country = :country, ");
            sql.AppendLine("description = :description, ");
            sql.AppendLine("total_order_wgt = :total_order_wgt, ");
            sql.AppendLine("wgt_remaining = :wgt_remaining, ");
            sql.AppendLine("forced_referral = :forced_referral, ");
            sql.AppendLine("ref_english = :ref_english, ");
            sql.AppendLine("ref_date = :ref_date, ");
            sql.AppendLine("ebs_order = :ebs_order, ");
            sql.AppendLine("ebs_line = :ebs_line, ");
            sql.AppendLine("descrete_org = :descrete_org, ");
            sql.AppendLine("ebs_item = :ebs_item, ");
            sql.AppendLine("non_ebs = :non_ebs");
            sql.AppendLine("WHERE works = :works");
            sql.AppendLine("AND order_num = :order_num ");
            sql.AppendLine("AND date_ent = :date_ent ");
            OracleCommand upd = new(sql.ToString(), conn);
            upd.BindByName = true;
            upd.Parameters.Add("time_ent", obj.Time_Ent);
            upd.Parameters.Add("unit_type", obj.Unit_Type);
            upd.Parameters.Add("active", obj.Active ? "A" : "I");
            upd.Parameters.Add("frt_terms", obj.Frt_Terms);
            upd.Parameters.Add("expire_date", obj.Expire_Date);
            upd.Parameters.Add("customer", obj.Customer);
            upd.Parameters.Add("location", obj.Location);
            upd.Parameters.Add("consignee", obj.Consignee);
            upd.Parameters.Add("destination", obj.Destination);
            upd.Parameters.Add("state", obj.State);
            upd.Parameters.Add("zip", obj.Zip);
            upd.Parameters.Add("country", obj.Country);
            upd.Parameters.Add("description", obj.Description);
            upd.Parameters.Add("total_order_wgt", obj.Total_Order_Wgt);
            upd.Parameters.Add("wgt_remaining", obj.Wgt_Remaining);
            upd.Parameters.Add("forced_referral", obj.Forced_Referral);
            upd.Parameters.Add("ref_english", obj.Ref_English);
            upd.Parameters.Add("ref_date", obj.Ref_Date);
            upd.Parameters.Add("ebs_order", obj.Ebs_Order);
            upd.Parameters.Add("ebs_line", obj.Ebs_Line);
            upd.Parameters.Add("descrete_org", obj.Descrete_Org);
            upd.Parameters.Add("ebs_item", obj.Ebs_Item);
            upd.Parameters.Add("non_ebs", obj.Non_Ebs);

            upd.Parameters.Add("works", obj.Works);
            upd.Parameters.Add("order_num", obj.Order_Num);
            upd.Parameters.Add("date_ent", obj.Date_Ent);
            int recsAffected = MOO.Data.ExecuteNonQuery(upd);
            return recsAffected;
        }


        public static int Delete(ERP_Shipping_RZ_Orders obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Delete(obj, conn);
            conn.Close();
            return recsAffected;
        }

        public static int Delete(ERP_Shipping_RZ_Orders obj, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("DELETE FROM tolive.ERP_Shipping_RZ_Orders");
            sql.AppendLine("WHERE works = :works");
            sql.AppendLine("AND order_num = :order_num ");
            sql.AppendLine("AND date_ent = :date_ent ");
            OracleCommand del = new(sql.ToString(), conn);
            del.BindByName = true;
            del.Parameters.Add("works", obj.Works);
            del.Parameters.Add("order_num", obj.Order_Num);
            del.Parameters.Add("date_ent", obj.Date_Ent);
            int recsAffected = MOO.Data.ExecuteNonQuery(del);
            return recsAffected;
        }


        internal static ERP_Shipping_RZ_Orders DataRowToObject(DbDataReader row, string ColPrefix = "")
        {
            ERP_Shipping_RZ_Orders RetVal = new();
            RetVal.Works = (short)Util.GetRowVal(row, $"{ColPrefix}works");
            RetVal.Order_Num = (string)Util.GetRowVal(row, $"{ColPrefix}order_num");
            RetVal.Date_Ent = (int)Util.GetRowVal(row, $"{ColPrefix}date_ent");
            RetVal.Time_Ent = (short)Util.GetRowVal(row, $"{ColPrefix}time_ent");
            RetVal.Unit_Type = (string)Util.GetRowVal(row, $"{ColPrefix}unit_type");
            RetVal.Active = (string)Util.GetRowVal(row, $"{ColPrefix}active") == "A";
            RetVal.Frt_Terms = (string)Util.GetRowVal(row, $"{ColPrefix}frt_terms");
            RetVal.Expire_Date = (short?)Util.GetRowVal(row, $"{ColPrefix}expire_date");
            RetVal.Customer = (int?)Util.GetRowVal(row, $"{ColPrefix}customer");
            RetVal.Location = (short?)Util.GetRowVal(row, $"{ColPrefix}location");
            RetVal.Consignee = (string)Util.GetRowVal(row, $"{ColPrefix}consignee");
            RetVal.Destination = (string)Util.GetRowVal(row, $"{ColPrefix}destination");
            RetVal.State = (string)Util.GetRowVal(row, $"{ColPrefix}state");
            RetVal.Zip = (string)Util.GetRowVal(row, $"{ColPrefix}zip");
            RetVal.Country = (string)Util.GetRowVal(row, $"{ColPrefix}country");
            RetVal.Description = (string)Util.GetRowVal(row, $"{ColPrefix}description");
            RetVal.Total_Order_Wgt = (long?)Util.GetRowVal(row, $"{ColPrefix}total_order_wgt");
            RetVal.Wgt_Remaining = (long?)Util.GetRowVal(row, $"{ColPrefix}wgt_remaining");
            RetVal.Forced_Referral = (string)Util.GetRowVal(row, $"{ColPrefix}forced_referral");
            RetVal.Ref_English = (string)Util.GetRowVal(row, $"{ColPrefix}ref_english");
            RetVal.Ref_Date = (string)Util.GetRowVal(row, $"{ColPrefix}ref_date");
            RetVal.Ebs_Order = (long?)Util.GetRowVal(row, $"{ColPrefix}ebs_order");
            RetVal.Ebs_Line = (string)Util.GetRowVal(row, $"{ColPrefix}ebs_line");
            RetVal.Descrete_Org = (string)Util.GetRowVal(row, $"{ColPrefix}descrete_org");
            RetVal.Ebs_Item = (string)Util.GetRowVal(row, $"{ColPrefix}ebs_item");
            RetVal.Non_Ebs = (string)Util.GetRowVal(row, $"{ColPrefix}non_ebs");
            return RetVal;
        }

    }
}
