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
    public static class Lab_Chem_TypeSvc
    {
        static Lab_Chem_TypeSvc()
        {
            Util.RegisterOracle();
        }


        public static Lab_Chem_Type Get(int lab_chem_type_id)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine("WHERE lab_chem_type_id = :lab_chem_type_id");


            Lab_Chem_Type retVal = null;
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.BindByName = true;
            cmd.Parameters.Add("lab_chem_type_id", lab_chem_type_id);
            OracleDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                rdr.Read();
                retVal = DataRowToObject(rdr);
            }
            conn.Close();
            return retVal;
        }

        public static List<Lab_Chem_Type> GetAll()
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());

            List<Lab_Chem_Type> elements = [];
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
            cols.AppendLine($"{ta}lab_chem_type_id {ColPrefix}lab_chem_type_id, {ta}plant {ColPrefix}plant, ");
            cols.AppendLine($"{ta}name {ColPrefix}name, {ta}description {ColPrefix}description, {ta}forwarded_to {ColPrefix}forwarded_to");
            return cols.ToString();
        }
        private static string GetSelect()
        {
            StringBuilder sql = new();
            sql.AppendLine("SELECT ");
            sql.AppendLine(GetColumns());
            sql.AppendLine("FROM tolive.lab_chem_type");
            return sql.ToString();
        }
        //we won't do an insert/update/delete on this.  Just add the rows manually to the database as needed

        internal static Lab_Chem_Type DataRowToObject(DbDataReader row, string ColPrefix = "")
        {
            Lab_Chem_Type RetVal = new();
            RetVal.Lab_Chem_Type_Id = (short)Util.GetRowVal(row, $"{ColPrefix}lab_chem_type_id");
            RetVal.Plant = Enum.Parse<MOO.Plant>((string)Util.GetRowVal(row, $"{ColPrefix}plant"));
            RetVal.Name = (string)Util.GetRowVal(row, $"{ColPrefix}name");
            RetVal.Description = (string)Util.GetRowVal(row, $"{ColPrefix}description");
            RetVal.Forwarded_To = (string)Util.GetRowVal(row, $"{ColPrefix}forwarded_to");
            return RetVal;
        }

    }
}
