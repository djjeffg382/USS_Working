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
    public class Auto_Rpt_RcptSvc
    {
        static Auto_Rpt_RcptSvc()
        {
            Util.RegisterOracle();
        }


        public static Auto_Rpt_Rcpt Get(int recipient_id)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine($"WHERE arr.Rcpt_Id = :recipient_id");

            OracleDataAdapter da = new(sql.ToString(), MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            da.SelectCommand.Parameters.Add("recipient_id", recipient_id);
            da.SelectCommand.BindByName = true;

            DataSet ds = MOO.Data.ExecuteQuery(da);
            if (ds.Tables[0].Rows.Count == 1)
                return DataRowToObject(ds.Tables[0].Rows[0], "arr_");
            else
                return null;
        }

        /// <summary>
        /// Gets the recipient record based on the Report Item ID and the Report AD Account name
        /// </summary>
        /// <param name="ReportItemID">Auto Rpt Item ID</param>
        /// <param name="ReportRecipient">Recipient AD Account (example:mno\abc1234)</param>
        /// <returns></returns>
        public static Auto_Rpt_Rcpt Get(long ReportItemID, string ReportRecipient)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine("WHERE arr.Item_Id = :ReportItemID");
            sql.AppendLine("AND lower(arr.recipient) = :ReportRecipient");
            sql.AppendLine("AND arr.is_printer = 0");

            OracleDataAdapter da = new(sql.ToString(), MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            da.SelectCommand.Parameters.Add("ReportItemID", ReportItemID);
            da.SelectCommand.Parameters.Add("ReportRecipient", ReportRecipient.ToLower());
            da.SelectCommand.BindByName = true;

            DataSet ds = MOO.Data.ExecuteQuery(da);
            if (ds.Tables[0].Rows.Count == 1)
                return DataRowToObject(ds.Tables[0].Rows[0], "arr_");
            else
                return null;
        }


        /// <summary>
        /// Gets the printer recipient record based on the Report Item ID and the Report AD Account name
        /// </summary>
        /// <param name="ReportItemID">Auto Rpt Item ID</param>
        /// <param name="Printer">Recipient AD Account (example:mno\abc1234)</param>
        /// <returns></returns>
        public static Auto_Rpt_Rcpt GetPrinterRecipient(long ReportItemID, string Printer)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine("WHERE arr.Item_Id = :ReportItemID");
            sql.AppendLine("AND lower(arr.recipient) = :Printer");
            sql.AppendLine("AND arr.is_printer = 1");

            OracleDataAdapter da = new(sql.ToString(), MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            da.SelectCommand.Parameters.Add("ReportItemID", ReportItemID);
            da.SelectCommand.Parameters.Add("Printer", Printer.ToLower());
            da.SelectCommand.BindByName = true;

            DataSet ds = MOO.Data.ExecuteQuery(da);
            if (ds.Tables[0].Rows.Count == 1)
                return DataRowToObject(ds.Tables[0].Rows[0], "arr_");
            else
                return null;
        }


        /// <summary>
        /// Gets the recipient that is subscribed to by email and not by AD Account
        /// </summary>
        /// <param name="ReportItemID">Auto Rpt Item ID</param>
        /// <param name="EmailAddress">Recipient AD Account (example:mno\abc1234)</param>
        /// <returns></returns>
        public static Auto_Rpt_Rcpt GetEmailRecipient(long ReportItemID, string EmailAddress)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine("WHERE arr.Item_Id = :ReportItemID");
            sql.AppendLine("AND arr.lower(email) = :EmailAddress");

            OracleDataAdapter da = new(sql.ToString(), MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            da.SelectCommand.Parameters.Add("ReportItemID", ReportItemID);
            da.SelectCommand.Parameters.Add("EmailAddress", EmailAddress.ToLower());
            da.SelectCommand.BindByName = true;

            DataSet ds = MOO.Data.ExecuteQuery(da);
            if (ds.Tables[0].Rows.Count == 1)
                return DataRowToObject(ds.Tables[0].Rows[0], "arr_");
            else
                return null;
        }

        /// <summary>
        /// Gets Recipients of a report
        /// </summary>
        /// <param name="ReportItem"></param>
        /// <returns></returns>
        public static List<Auto_Rpt_Rcpt> GetRecipientsOfReport(long ReportItem)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine($"WHERE arr.Item_ID = :Item_ID");

            OracleDataAdapter da = new(sql.ToString(), MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            da.SelectCommand.Parameters.Add("Item_ID", ReportItem);
            da.SelectCommand.BindByName = true;

            DataSet ds = MOO.Data.ExecuteQuery(da);
            List<Auto_Rpt_Rcpt> elements = new();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                elements.Add(DataRowToObject(dr, "arr_"));
            }
            return elements;
        }



        /// <summary>
        /// Gets all Auto Print printer recipients
        /// </summary>
        /// <returns></returns>
        public static List<Auto_Rpt_Rcpt> GetAllAutoPrints()
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine("WHERE is_printer = 1");

            OracleDataAdapter da = new(sql.ToString(), MOO.Data.GetConnectionString(Data.MNODatabase.DMART));

            DataSet ds = MOO.Data.ExecuteQuery(da);
            List<Auto_Rpt_Rcpt> elements = new();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                elements.Add(DataRowToObject(dr, "arr_"));
            }
            return elements;
        }



        /// <summary>
        /// Gets all subscriptions that are non domain account (self subscribe)
        /// </summary>
        /// <returns></returns>
        public static List<Auto_Rpt_Rcpt> GetAllNonDomainSubscriptions()
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine("WHERE email IS NOT NULL");

            OracleDataAdapter da = new(sql.ToString(), MOO.Data.GetConnectionString(Data.MNODatabase.DMART));

            DataSet ds = MOO.Data.ExecuteQuery(da);
            List<Auto_Rpt_Rcpt> elements = new();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                elements.Add(DataRowToObject(dr, "arr_"));
            }
            return elements;
        }

        internal static string GetColumns(string TableAlias = "", string ColPrefix = "")
        {
            string ta = "";
            if (!string.IsNullOrEmpty(TableAlias))
                ta = TableAlias + ".";
            StringBuilder cols = new();
            cols.AppendLine($"{ta}Rcpt_Id {ColPrefix}Rcpt_Id, {ta}recipient {ColPrefix}recipient, ");
            cols.AppendLine($"{ta}item_id {ColPrefix}item_id, {ta}is_printer {ColPrefix}is_printer, {ta}email {ColPrefix}email");
            return cols.ToString();
        }
        private static string GetSelect()
        {
            StringBuilder sql = new();
            sql.AppendLine("SELECT ");
            sql.AppendLine(GetColumns("arr","arr_") + ",");
            sql.AppendLine(Auto_Rpt_ItemSvc.GetColumns("ari", "ari_")+ ",");  //add in the auto_rpt_item table
            sql.AppendLine(Auto_Rpt_TypeSvc.GetColumns("rt", "rt_"));  //add in the auto_rpt_type 
            sql.AppendLine("FROM tolive.auto_rpt_rcpt arr");
            sql.AppendLine("JOIN tolive.auto_rpt_item ari ON arr.item_id = ari.item_id");
            sql.AppendLine("JOIN tolive.auto_rpt_type rt ON ari.type_id = rt.type_id");
            return sql.ToString();
        }


        public static int Insert(Auto_Rpt_Rcpt obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Insert(obj, conn);
            conn.Close();
            return recsAffected;
        }


        public static int Insert(Auto_Rpt_Rcpt obj, OracleConnection conn)
        {
            //new id is the max id + 1 we don't have a sequence set up for this
            if (obj.Rcpt_Id <= 0)
                obj.Rcpt_Id = (long)MOO.Data.ExecuteScalar("SELECT CAST(NVL(MAX(Rcpt_Id),0) as NUMBER(10,0)) FROM tolive.AUTO_RPT_RCPT", Data.MNODatabase.DMART, 0) + 1;

            StringBuilder sql = new();
            sql.AppendLine("INSERT INTO tolive.AUTO_RPT_RCPT(");
            sql.AppendLine("Rcpt_Id, recipient, item_id, is_printer, email)");
            sql.AppendLine("VALUES(");
            sql.AppendLine(":Rcpt_Id, :recipient, :item_id, :is_printer, :email)");
            OracleCommand ins = new(sql.ToString(), conn);
            ins.BindByName = true;
            ins.Parameters.Add("Rcpt_Id", obj.Rcpt_Id);
            ins.Parameters.Add("recipient", obj.Recipient);
            ins.Parameters.Add("item_id", obj.Report.Item_Id);
            ins.Parameters.Add("is_printer", obj.Is_Printer ? 1 : 0);
            ins.Parameters.Add("email", obj.Email);
            int recsAffected = MOO.Data.ExecuteNonQuery(ins);
            return recsAffected;
        }


        public static int Update(Auto_Rpt_Rcpt obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Update(obj, conn);
            conn.Close();
            return recsAffected;
        }


        public static int Update(Auto_Rpt_Rcpt obj, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("UPDATE tolive.AUTO_RPT_RCPT SET");
            sql.AppendLine("recipient = :recipient, ");
            sql.AppendLine("item_id = :item_id, ");
            sql.AppendLine("is_printer = :is_printer, ");
            sql.AppendLine("email = :email");
            sql.AppendLine("WHERE Rcpt_Id = :recipient_id");
            OracleCommand upd = new(sql.ToString(), conn);
            upd.BindByName = true;
            upd.Parameters.Add("recipient", obj.Recipient);
            upd.Parameters.Add("item_id", obj.Report.Item_Id);
            upd.Parameters.Add("is_printer", obj.Is_Printer ? 1 : 0);
            upd.Parameters.Add("email", obj.Email);
            upd.Parameters.Add("recipient_id", obj.Rcpt_Id);
            int recsAffected = MOO.Data.ExecuteNonQuery(upd);
            return recsAffected;
        }


        public static int Delete(Auto_Rpt_Rcpt obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Delete(obj, conn);
            conn.Close();
            return recsAffected;
        }

        public static int Delete(Auto_Rpt_Rcpt obj, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("DELETE FROM tolive.AUTO_RPT_RCPT");
            sql.AppendLine("WHERE Rcpt_Id = :recipient_id");
            OracleCommand del = new(sql.ToString(), conn);
            del.BindByName = true;
            del.Parameters.Add("recipient_id", obj.Rcpt_Id);
            int recsAffected = MOO.Data.ExecuteNonQuery(del);
            return recsAffected;
        }


        /// <summary>
        /// changes the domain on the user from mno to hdq.  Used for changeover of users from mno -> hdq
        /// </summary>
        /// <param name="Username">the username without the domain</param>
        ///<returns>Returns number of rows updated</returns>
        public static int MoveDomainMnoToHdq(string Username)
        {
            if (Username.Contains("\\"))
                throw new Exception("Provide the username without the domain");
            var sql = @"UPDATE tolive.auto_rpt_rcpt
                        SET recipient = :newUser
                        WHERE LOWER(recipient) = :oldUser";
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand upd = new(sql, conn);
            upd.Parameters.Add("newUser", $"hdq\\{Username.ToLower()}");
            upd.Parameters.Add("oldUser", $"mno\\{Username.ToLower()}");
            int recsAffected = upd.ExecuteNonQuery();
            conn.Close();
            return recsAffected;
        }

        internal static Auto_Rpt_Rcpt DataRowToObject(DataRow row, string ColPrefix = "")
        {
            Auto_Rpt_Rcpt RetVal = new();
            RetVal.Rcpt_Id = row.Field<long>($"{ColPrefix}Rcpt_Id");
            RetVal.Recipient = row.Field<string>($"{ColPrefix}recipient");
            RetVal.Is_Printer = row.Field<short>($"{ColPrefix}is_printer") == 1;
            RetVal.Email = row.Field<string>($"{ColPrefix}email");
            RetVal.Report = Auto_Rpt_ItemSvc.DataRowToObject(row, "ari_");
            return RetVal;
        }

    }
}
