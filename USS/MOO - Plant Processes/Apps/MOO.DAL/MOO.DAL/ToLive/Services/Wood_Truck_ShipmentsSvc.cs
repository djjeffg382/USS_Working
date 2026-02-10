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
    public static class Wood_Truck_ShipmentsSvc
    {
        static Wood_Truck_ShipmentsSvc()
        {
            Util.RegisterOracle();
        }


        public static Wood_Truck_Shipments Get(string invoice_nbr)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine($"WHERE invoice_nbr = :invoice_nbr");

            OracleDataAdapter da = new(sql.ToString(), MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            da.SelectCommand.Parameters.Add("invoice_nbr", invoice_nbr);
            da.SelectCommand.BindByName = true;

            DataSet ds = MOO.Data.ExecuteQuery(da);
            if (ds.Tables[0].Rows.Count == 1)
                return DataRowToObject(ds.Tables[0].Rows[0]);
            else
                return null;
        }

        /// <summary>
        /// returns a paged dataset data 
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="totalItems"></param>
        /// <param name="orderBy"></param>
        /// <param name="orderDirection"></param>
        /// <param name="InvoiceNbr"></param>
        /// <param name="DeliveryDateStart"></param>
        /// <param name="DeliveryDateEnd"></param>
        /// <returns></returns>
        /// <remarks>We have to use paged data set as this table is quite large</remarks>
        public static PagedData<List<Wood_Truck_Shipments>> GetPagedData(int offset, int totalItems, string orderBy = "delivery_date", string orderDirection = "desc",
                        string InvoiceNbr = "", DateTime? DeliveryDateStart = null, DateTime? DeliveryDateEnd = null)
        {
            PagedData<List<Wood_Truck_Shipments>> retObj = new();
            StringBuilder sql = new();
            OracleDataAdapter da = new("", MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            string filter = AddFilters(InvoiceNbr, DeliveryDateStart, DeliveryDateEnd, da.SelectCommand);
            string ord;
            if (!string.IsNullOrEmpty(orderBy))
            {
                ord = orderBy;
                if (!string.IsNullOrEmpty(orderDirection))
                    ord = $"{ord} {orderDirection}";
            }
            else
            {
                ord = "delivery_date desc";
            }

            sql.AppendLine("SELECT * FROM (");
            sql.AppendLine(GetSelect($"ROW_NUMBER() OVER(ORDER BY {ord}) rn"));
            sql.AppendLine(filter);
            sql.AppendLine(") tbl");
            sql.AppendLine($"WHERE rn BETWEEN {offset} AND {offset + totalItems}");
            da.SelectCommand.CommandText = sql.ToString();
            DataSet ds = MOO.Data.ExecuteQuery(da);

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                retObj.Data.Add(DataRowToObject(row));
            }
            retObj.TotalRows = GetRecCount(InvoiceNbr, DeliveryDateStart, DeliveryDateEnd);

            return retObj;

        }



        private static int GetRecCount(string InvoiceNbr, DateTime? DeliveryDateStart, DateTime? DeliveryDateEnd )
        {
            StringBuilder sql = new();
            OracleDataAdapter da = new("", MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));

            string filter = AddFilters(InvoiceNbr, DeliveryDateStart, DeliveryDateEnd, da.SelectCommand);

            sql.AppendLine("SELECT COUNT(*) FROM tolive.wood_truck_shipments");
            sql.AppendLine(filter);
            da.SelectCommand.CommandText = sql.ToString();
            DataSet ds = MOO.Data.ExecuteQuery(da);
            if (ds.Tables[0].Rows.Count > 0)
            {
                return Convert.ToInt32(ds.Tables[0].Rows[0][0]);
            }
            else
                return 0;
        }


        private static string AddFilters(string InvoiceNbr, DateTime? DeliveryDateStart, DateTime? DeliveryDateEnd, OracleCommand cmd)
        {
            StringBuilder filter = new();

            if (!string.IsNullOrEmpty(InvoiceNbr))
            {
                filter.AppendLine("WHERE invoice_nbr = :invoiceNbr");
                cmd.Parameters.Add("invoiceNbr", InvoiceNbr);
            }

            /*****************Delivery Date Parameter*************/
            if (DeliveryDateStart.HasValue)
            {
                if (filter.Length == 0)
                    filter.Append("WHERE ");
                else
                    filter.Append("AND ");

                filter.AppendLine("Delivery_Date >= :DeliveryDateStart");
                cmd.Parameters.Add("DeliveryDateStart", DeliveryDateStart.Value);
            }
            if (DeliveryDateEnd.HasValue)
            {
                if (filter.Length == 0)
                    filter.Append("WHERE ");
                else
                    filter.Append("AND ");

                filter.AppendLine("Delivery_Date <= :DeliveryDateEnd");
                cmd.Parameters.Add("DeliveryDateEnd", DeliveryDateEnd.Value);
            }


            return filter.ToString();
        }



        internal static string GetColumns(string TableAlias = "", string ColPrefix = "")
        {
            string ta = "";
            if (!string.IsNullOrEmpty(TableAlias))
                ta = TableAlias + ".";
            StringBuilder cols = new();
            cols.AppendLine($"{ta}invoice_nbr {ColPrefix}invoice_nbr, {ta}delivery_date {ColPrefix}delivery_date, ");
            cols.AppendLine($"{ta}inbound_wt {ColPrefix}inbound_wt, {ta}outbound_wt {ColPrefix}outbound_wt, ");
            cols.AppendLine($"{ta}avg_perc_moist {ColPrefix}avg_perc_moist, {ta}btu_lb {ColPrefix}btu_lb, ");
            cols.AppendLine($"{ta}scale_ticket_number {ColPrefix}scale_ticket_number");
            return cols.ToString();
        }
        private static string GetSelect(string addColumns = "")
        {
            StringBuilder sql = new();
            sql.AppendLine("SELECT ");
            sql.AppendLine(GetColumns());
            if (!string.IsNullOrEmpty(addColumns))
            {
                sql.Append(", ");
                sql.AppendLine(addColumns);
            }
            sql.AppendLine("FROM tolive.wood_truck_shipments");
            return sql.ToString();
        }


        public static int Insert(Wood_Truck_Shipments obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Insert(obj, conn);
            conn.Close();
            return recsAffected;
        }


        public static int Insert(Wood_Truck_Shipments obj, OracleConnection conn)
        {
            
            StringBuilder sql = new();
            sql.AppendLine("INSERT INTO tolive.Wood_Truck_Shipments(");
            sql.AppendLine("invoice_nbr, delivery_date, inbound_wt, outbound_wt, avg_perc_moist, btu_lb, ");
            sql.AppendLine("scale_ticket_number)");
            sql.AppendLine("VALUES(");
            sql.AppendLine(":invoice_nbr, :delivery_date, :inbound_wt, :outbound_wt, :avg_perc_moist, :btu_lb, ");
            sql.AppendLine(":scale_ticket_number)");
            OracleCommand ins = new(sql.ToString(), conn);
            ins.BindByName = true;
            ins.Parameters.Add("invoice_nbr", obj.Invoice_Nbr);
            ins.Parameters.Add("delivery_date", obj.Delivery_Date);
            ins.Parameters.Add("inbound_wt", obj.Inbound_Wt);
            ins.Parameters.Add("outbound_wt", obj.Outbound_Wt);
            ins.Parameters.Add("avg_perc_moist", obj.Avg_Perc_Moist);
            ins.Parameters.Add("btu_lb", obj.Btu_Lb);
            ins.Parameters.Add("scale_ticket_number", obj.Scale_Ticket_Number);
            int recsAffected = MOO.Data.ExecuteNonQuery(ins);
            return recsAffected;
        }


        public static int Update(Wood_Truck_Shipments obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Update(obj, conn);
            conn.Close();
            return recsAffected;
        }


        public static int Update(Wood_Truck_Shipments obj, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("UPDATE tolive.Wood_Truck_Shipments SET");
            sql.AppendLine("invoice_nbr = :new_invoice_nbr, ");
            sql.AppendLine("delivery_date = :delivery_date, ");
            sql.AppendLine("inbound_wt = :inbound_wt, ");
            sql.AppendLine("outbound_wt = :outbound_wt, ");
            sql.AppendLine("avg_perc_moist = :avg_perc_moist, ");
            sql.AppendLine("btu_lb = :btu_lb, ");
            sql.AppendLine("scale_ticket_number = :scale_ticket_number");
            sql.AppendLine("WHERE invoice_nbr = :invoice_nbr");
            OracleCommand upd = new(sql.ToString(), conn);
            upd.BindByName = true;
            upd.Parameters.Add("new_invoice_nbr", obj.Invoice_Nbr);
            upd.Parameters.Add("delivery_date", obj.Delivery_Date);
            upd.Parameters.Add("inbound_wt", obj.Inbound_Wt);
            upd.Parameters.Add("outbound_wt", obj.Outbound_Wt);
            upd.Parameters.Add("avg_perc_moist", obj.Avg_Perc_Moist);
            upd.Parameters.Add("btu_lb", obj.Btu_Lb);
            upd.Parameters.Add("scale_ticket_number", obj.Scale_Ticket_Number);
            upd.Parameters.Add("invoice_nbr", obj.Old_Invoice_Nbr);
            int recsAffected = MOO.Data.ExecuteNonQuery(upd);
            //after update, change the old invoice number so if we update again we update the correct record
            obj.Old_Invoice_Nbr = obj.Invoice_Nbr;
            return recsAffected;
        }



        /// <summary>
        /// Adjusts the BTU Value for all wood shipments between the specified dates
        /// </summary>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <param name="BTUVal"></param>
        /// <returns></returns>
        public static int AdjustBTU(DateTime StartDate, DateTime EndDate, decimal BTUVal)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = AdjustBTU(StartDate, EndDate, BTUVal, conn);
            conn.Close();
            return recsAffected;
        }

        /// <summary>
        /// Adjusts the BTU Value for all wood shipments between the specified dates
        /// </summary>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <param name="BTUVal"></param>
        /// <returns></returns>
        private static int AdjustBTU(DateTime StartDate, DateTime EndDate, decimal BTUVal, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("UPDATE tolive.Wood_Truck_Shipments SET");
            sql.AppendLine("btu_lb = :btu_lb");
            sql.AppendLine("WHERE delivery_date BETWEEN :StartDate AND :EndDate");
            OracleCommand upd = new(sql.ToString(), conn);
            upd.BindByName = true;
            upd.Parameters.Add("btu_lb", BTUVal);
            upd.Parameters.Add("StartDate", StartDate);
            upd.Parameters.Add("EndDate", EndDate);
            int recsAffected = MOO.Data.ExecuteNonQuery(upd);
            return recsAffected;
        }


        public static int Delete(Wood_Truck_Shipments obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Delete(obj, conn);
            conn.Close();
            return recsAffected;
        }

        public static int Delete(Wood_Truck_Shipments obj, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("DELETE FROM tolive.Wood_Truck_Shipments");
            sql.AppendLine("WHERE invoice_nbr = :invoice_nbr");
            OracleCommand del = new(sql.ToString(), conn);
            del.BindByName = true;
            del.Parameters.Add("invoice_nbr", obj.Invoice_Nbr);
            int recsAffected = MOO.Data.ExecuteNonQuery(del);
            return recsAffected;
        }


        internal static Wood_Truck_Shipments DataRowToObject(DataRow row, string ColPrefix = "")
        {
            Wood_Truck_Shipments RetVal = new();
            RetVal.Invoice_Nbr = row.Field<string>($"{ColPrefix}invoice_nbr");
            RetVal.Old_Invoice_Nbr = row.Field<string>($"{ColPrefix}invoice_nbr");
            RetVal.Delivery_Date = row.Field<DateTime?>($"{ColPrefix}delivery_date");
            RetVal.Inbound_Wt = row.Field<decimal?>($"{ColPrefix}inbound_wt");
            RetVal.Outbound_Wt = row.Field<decimal?>($"{ColPrefix}outbound_wt");
            RetVal.Avg_Perc_Moist = row.Field<decimal?>($"{ColPrefix}avg_perc_moist");
            RetVal.Btu_Lb = row.Field<decimal?>($"{ColPrefix}btu_lb");
            RetVal.Scale_Ticket_Number = row.Field<decimal?>($"{ColPrefix}scale_ticket_number");
            return RetVal;
        }

    }
}
