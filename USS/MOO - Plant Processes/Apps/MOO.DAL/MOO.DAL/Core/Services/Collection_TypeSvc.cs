using MOO.DAL.Core.Models;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.Core.Services
{
    public static class Collection_TypeSvc
    {
        static Collection_TypeSvc()
        {
            Util.RegisterOracle();
        }


        public static Collection_Type Get(int Coll_Type_Id)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine($"WHERE coll_type_id = :coll_type_id");


            Collection_Type retVal = null;
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.Parameters.Add("coll_type_id", Coll_Type_Id);
            OracleDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                rdr.Read();
                retVal = DataRowToObject(rdr);
            }
            conn.Close();
            return retVal;
        }

        public static List<Collection_Type> GetAll()
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());

            List<Collection_Type> elements = new();
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
            cols.AppendLine($"{ta}coll_type_id {ColPrefix}coll_type_id, {ta}coll_type_name {ColPrefix}coll_type_name, ");
            cols.AppendLine($"{ta}description {ColPrefix}description, {ta}manually_entered {ColPrefix}manually_entered");
            return cols.ToString();
        }
        private static string GetSelect()
        {
            StringBuilder sql = new();
            sql.AppendLine("SELECT ");
            sql.AppendLine(GetColumns());
            sql.AppendLine("FROM core.collection_type");
            return sql.ToString();
        }


        internal static Collection_Type DataRowToObject(DbDataReader row, string ColPrefix = "")
        {
            Collection_Type RetVal = new();
            RetVal.Coll_Type_Id = (int)(decimal)Util.GetRowVal(row, $"{ColPrefix}coll_type_id");
            RetVal.Coll_Type_Name = (string)Util.GetRowVal(row, $"{ColPrefix}coll_type_name");
            RetVal.Description = (string)Util.GetRowVal(row, $"{ColPrefix}description");
            RetVal.Manually_Entered = (decimal)Util.GetRowVal(row, $"{ColPrefix}manually_entered") == 1;
            return RetVal;
        }

    }
}
