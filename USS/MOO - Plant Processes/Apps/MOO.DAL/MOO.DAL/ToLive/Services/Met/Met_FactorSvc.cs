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
    public static class Met_FactorSvc
    {
        static Met_FactorSvc()
        {
            Util.RegisterOracle();
        }


        public static Met_Factor Get(int Factor_Id)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine($"WHERE factor_id = :factor_id");

            OracleDataAdapter da = new(sql.ToString(), MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            da.SelectCommand.Parameters.Add("factor_id", Factor_Id);

            DataSet ds = MOO.Data.ExecuteQuery(da);
            if (ds.Tables[0].Rows.Count == 1)
                return DataRowToObject(ds.Tables[0].Rows[0]);
            else
                return null;
        }


        public static List<Met_Factor> GetAll()
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());

            OracleDataAdapter da = new(sql.ToString(), MOO.Data.GetConnectionString(Data.MNODatabase.DMART));

            DataSet ds = MOO.Data.ExecuteQuery(da);
            List<Met_Factor> elements = new();
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
            sql.AppendLine("SELECT ");
            sql.AppendLine("factor_id, name, description");
            sql.AppendLine("FROM tolive.met_factor");
            return sql.ToString();
        }


        internal static Met_Factor DataRowToObject(DataRow row, string ColumnPrefix = "")
        {
            Met_Factor RetVal = new();
            RetVal.Factor_Id = (int)row.Field<decimal>($"{ColumnPrefix}factor_id");
            RetVal.Name = row.Field<string>($"{ColumnPrefix}name");
            RetVal.Description = row.Field<string>($"{ColumnPrefix}description");
            return RetVal;
        }

    }
}
