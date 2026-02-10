using Microsoft.Data.SqlClient;
using MOO.DAL.LIMS.Models;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.LIMS.Services
{
    public static class PhraseSvc
    {
        static PhraseSvc()
        {
            Util.RegisterSqlServer();
        }


        public static Phrase Get(string Phrase_Type, string Phrase_Id)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine($"WHERE phrase_type = @Phrase_Type");
            sql.AppendLine($"AND phrase_id = @Phrase_Id");


            Phrase retVal = null;
            using SqlConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.LIMS_Read));
            conn.Open();
            SqlCommand cmd = new(sql.ToString(), conn);
            cmd.Parameters.AddWithValue("Phrase_Type", Phrase_Type);
            cmd.Parameters.AddWithValue("Phrase_Id", Phrase_Id);
            SqlDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                rdr.Read();
                retVal = DataRowToObject(rdr);
            }
            conn.Close();
            return retVal;
        }



        public static List<Phrase> GetAll(string Phrase_Type)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine($"WHERE phrase_type = @Phrase_Type");

            List<Phrase> elements = new();
            using SqlConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.LIMS_Read));
            conn.Open();
            SqlCommand cmd = new(sql.ToString(), conn);
            cmd.Parameters.AddWithValue("Phrase_Type", Phrase_Type);
            SqlDataReader rdr = cmd.ExecuteReader();
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
            cols.AppendLine($"{ta}phrase_type {ColPrefix}phrase_type, {ta}phrase_id {ColPrefix}phrase_id, {ta}Order_Num {ColPrefix}Order_Num, ");
            cols.AppendLine($"{ta}phrase_text {ColPrefix}phrase_text, {ta}icon {ColPrefix}icon");
            return cols.ToString();
        }
        private static string GetSelect()
        {
            string db = MOO.Settings.LIMS_DB_Name;

            //we will assume 3 levels deep for parent location
            StringBuilder sql = new();
            sql.AppendLine("SELECT ");
            sql.AppendLine(GetColumns());
            sql.AppendLine($"FROM {db}.dbo.phrase");
            return sql.ToString();
        }


        internal static Phrase DataRowToObject(DbDataReader row, string ColPrefix = "")
        {
            Phrase RetVal = new();
            RetVal.Phrase_Type = (string)Util.GetRowVal(row, $"{ColPrefix}Phrase_Type");
            RetVal.Phrase_Id = (string)Util.GetRowVal(row, $"{ColPrefix}Phrase_Id");
            RetVal.Order_Num = int.Parse((string)Util.GetRowVal(row, $"{ColPrefix}Order_Num"));
            RetVal.Phrase_Text = (string)Util.GetRowVal(row, $"{ColPrefix}Phrase_Text");
            RetVal.Icon = (string)Util.GetRowVal(row, $"{ColPrefix}Icon");



            return RetVal;
        }
    }
}
