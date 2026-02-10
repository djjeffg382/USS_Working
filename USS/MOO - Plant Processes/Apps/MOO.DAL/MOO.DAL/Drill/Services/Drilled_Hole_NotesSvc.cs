using Microsoft.Data.SqlClient;
using MOO.DAL.Drill.Models;
using MOO.DAL.LIMS.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.Drill.Services
{
    public static class Drilled_Hole_NotesSvc
    {

        static Drilled_Hole_NotesSvc()
        {
            Util.RegisterSqlServer();
        }


        public static Drilled_Hole_Notes Get(short DhNotesId)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine($"WHERE Drilled_Hole_Notes_Id = {DhNotesId}");


            DataSet ds = MOO.Data.ExecuteQuery(sql.ToString(), Data.MNODatabase.USSDrillData);
            if (ds.Tables[0].Rows.Count == 1)
                return DataRowToObject(ds.Tables[0].Rows[0]);
            else
                return null;
        }

        /// <summary>
        /// Gets the list of Drilled Hole Notes
        /// </summary>
        /// <param name="IncludeInactive">Whether to include inactive Hole Notes</param>
        /// <returns></returns>
        public static List<Drilled_Hole_Notes> GetAll(bool IncludeInactive = false)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            if(!IncludeInactive) 
                sql.AppendLine("WHERE active = 1");

            sql.AppendLine("ORDER BY sort_order, notes");  //order this by sort order first then alphabetical

            DataSet ds = MOO.Data.ExecuteQuery(sql.ToString(), Data.MNODatabase.USSDrillData);

            List<Drilled_Hole_Notes> retVal = new();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                retVal.Add(DataRowToObject(dr));
            }
            return retVal;
        }




        internal static string GetColumns(string TableAlias = "", string ColPrefix = "")
        {
            string ta = "";
            if (!string.IsNullOrEmpty(TableAlias))
                ta = TableAlias + ".";
            StringBuilder cols = new();
            cols.AppendLine($"{ta}Drilled_Hole_Notes_Id {ColPrefix}Drilled_Hole_Notes_Id, {ta}Notes {ColPrefix}Notes, {ta}Active {ColPrefix}Active ");
            return cols.ToString();
        }
        private static string GetSelect()
        {
            StringBuilder sql = new();
            sql.AppendLine("SELECT ");
            sql.AppendLine(GetColumns());
            sql.AppendLine($"FROM Drill.dbo.Drilled_Hole_Notes");
            return sql.ToString();
        }


        internal static Drilled_Hole_Notes DataRowToObject(DataRow row, string ColPrefix = "")
        {
            Drilled_Hole_Notes RetVal = new();
            RetVal.Drilled_Hole_Notes_Id = row.Field<short>($"{ColPrefix}Drilled_Hole_Notes_id");
            RetVal.Notes = row.Field<string>($"{ColPrefix}Notes");
            RetVal.Active = row.Field<bool>($"{ColPrefix}Active");

            return RetVal;
        }
    }
}
