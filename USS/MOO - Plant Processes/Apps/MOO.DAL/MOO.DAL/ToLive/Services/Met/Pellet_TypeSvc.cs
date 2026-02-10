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
    public static class Pellet_TypeSvc
    {
        static Pellet_TypeSvc()
        {
            Util.RegisterOracle();
        }

        /// <summary>
        /// gets the pellet type by code name (060 or 054)
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static Pellet_Type Get(string code)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine($"WHERE code = :code");

            OracleDataAdapter da = new(sql.ToString(), MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            da.SelectCommand.BindByName = true;
            da.SelectCommand.Parameters.Add("code", code);

            DataSet ds = MOO.Data.ExecuteQuery(da);
            if (ds.Tables[0].Rows.Count == 1)
                return DataRowToObject(ds.Tables[0].Rows[0]);
            else
                return null;
        }

        public static Pellet_Type Get(int PelletTypeId)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine($"WHERE ptype = :ptype");

            OracleDataAdapter da = new(sql.ToString(), MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            da.SelectCommand.BindByName = true;
            da.SelectCommand.Parameters.Add("ptype", PelletTypeId);

            DataSet ds = MOO.Data.ExecuteQuery(da);
            if (ds.Tables[0].Rows.Count == 1)
                return DataRowToObject(ds.Tables[0].Rows[0]);
            else
                return null;
        }

        public static List<Pellet_Type> GetAll()
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());

            OracleDataAdapter da = new(sql.ToString(), MOO.Data.GetConnectionString(Data.MNODatabase.DMART));

            DataSet ds = MOO.Data.ExecuteQuery(da);
            List<Pellet_Type> elements = new();
            if (ds.Tables[0].Rows.Count > 1)
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
            sql.AppendLine("code, material, ptype, marketing_name");
            sql.AppendLine("FROM tolive.pellet_type");
            return sql.ToString();
        }


        internal static Pellet_Type DataRowToObject(DataRow row, string ColPrefix = "")
        {
            Pellet_Type RetVal = new();
            RetVal.Code = row.Field<string>($"{ColPrefix}code");
            RetVal.Material = row.Field<string>($"{ColPrefix}material");
            RetVal.Ptype = row.Field<short>($"{ColPrefix}ptype");
            RetVal.Marketing_Name = row.Field<string>($"{ColPrefix}marketing_name");
            return RetVal;
        }

    }
}
