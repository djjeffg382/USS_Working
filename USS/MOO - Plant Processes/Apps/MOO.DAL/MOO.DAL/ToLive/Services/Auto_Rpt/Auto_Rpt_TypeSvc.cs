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
    public static class Auto_Rpt_TypeSvc
    {
        static Auto_Rpt_TypeSvc()
        {
            Util.RegisterOracle();
        }


        public static Auto_Rpt_Type Get(long type_id)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine($"WHERE type_id = :type_id");

            OracleDataAdapter da = new(sql.ToString(), MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            da.SelectCommand.Parameters.Add("type_id", type_id);
            da.SelectCommand.BindByName = true;

            DataSet ds = MOO.Data.ExecuteQuery(da);
            if (ds.Tables[0].Rows.Count == 1)
                return DataRowToObject(ds.Tables[0].Rows[0]);
            else
                return null;
        }

        public static List<Auto_Rpt_Type> GetAll()
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());

            OracleDataAdapter da = new(sql.ToString(), MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            da.SelectCommand.BindByName = true;

            DataSet ds = MOO.Data.ExecuteQuery(da);
            List<Auto_Rpt_Type> elements = new();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                elements.Add(DataRowToObject(dr));
            }
            return elements;
        }


        internal static string GetColumns(string TableAlias = "", string ColPrefix = "")
        {
            string ta = "";
            if (!string.IsNullOrEmpty(TableAlias))
                ta = TableAlias + ".";
            StringBuilder cols = new();
            cols.AppendLine($"{ta}type_id {ColPrefix}type_id, {ta}type_name {ColPrefix}type_name");
            return cols.ToString();
        }
        private static string GetSelect()
        {
            StringBuilder sql = new();
            sql.AppendLine("SELECT ");
            sql.AppendLine(GetColumns());
            sql.AppendLine("FROM tolive.auto_rpt_type");
            return sql.ToString();
        }



        internal static Auto_Rpt_Type DataRowToObject(DataRow row, string ColPrefix = "")
        {
            Auto_Rpt_Type RetVal = new();
            RetVal.Type_Id = row.Field<long>($"{ColPrefix}type_id");
            RetVal.Type_Name = row.Field<string>($"{ColPrefix}type_name");
            return RetVal;
        }

    }
}
