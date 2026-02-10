using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MOO.DAL.ToLive.Models;
using MOO.DAL.ToLive.Enums;

namespace MOO.DAL.ToLive.Services
{
    public static class AnalyticsSvc
    {
        static AnalyticsSvc()
        {
            Util.RegisterOracle();
        }

        public static List<Analytics> Get(string DataGroup)
        {
            return Get(DataGroup, null);
        }


        public static List<Analytics> Get(string DataGroup, int? LineNumber)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine("WHERE Data_Group = :Data_Group");
            if(LineNumber.HasValue) 
                sql.AppendLine($"AND Line_Nbr = {LineNumber.Value}");
            sql.AppendLine("AND value <> label");  //not sure why we get this.  maybe a bug from whoever is sending.  We don't want records where the value is the label
            sql.AppendLine("ORDER BY sort_order, date_val desc");

            List<Analytics> elements = [];
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.Parameters.Add("Data_Group", DataGroup.ToString().ToLower());
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
        public static AnalyticsGrouped GetGrouped(string DataGroup)
        {
            return GetGrouped(DataGroup, null);
        }

        public static AnalyticsGrouped GetGrouped(string DataGroup, int? LineNumber)
        {
            List<Analytics> vals = Get(DataGroup, LineNumber);
            //vals should be ordered by sort_order and date
            AnalyticsGrouped retVal = new()
            {
                DataGroup = DataGroup
            };
            string currentLabel = "";
            short dateIdx = 0;
            List<DateTime> Dates = new();
            //we need to first get the date array set up.  We will loop through the 
            var d = vals.GroupBy(x => x.Date_Val).Select(g => g.First()).OrderByDescending(x => x.Date_Val).ToArray();
            foreach (var obj in d)
                Dates.Add(obj.Date_Val);

            retVal.Dates = Dates.ToArray();



            AnalyticsGrpRec newRec = new();
            newRec.Values = new string[retVal.Dates.Length];
            newRec.Quality = new string[retVal.Dates.Length];
            newRec.Parent = retVal;

            foreach (Analytics ana in vals)
            {
                if (currentLabel != ana.Label)
                {
                    //we switched to the next grouped record
                    if (!string.IsNullOrEmpty(currentLabel))
                    {
                        //this is not first run, insert
                        retVal.Records.Add(newRec);
                        newRec = new();
                        newRec.Parent = retVal;
                        newRec.Values = new string[retVal.Dates.Length];
                        newRec.Quality = new string[retVal.Dates.Length];
                    }
                    dateIdx = 0;
                    currentLabel = ana.Label;
                    newRec.Label = ana.Label;
                    newRec.SortOrder = ana.Sort_Order;
                    newRec.LineNbr = ana.Line_Nbr;
                }
                //make sure to get the correct index for the date, this should be all in the same order
                //but we will not take any chances
                int findIdx = Array.FindIndex(retVal.Dates, x => x == ana.Date_Val);
                if (findIdx >= 0)
                {
                    newRec.Values[findIdx] = ana.Value;
                    newRec.Quality[findIdx] = ana.Quality;
                }

                dateIdx++;
            }
            //after loop add the final record
            retVal.Records.Add(newRec);
            return retVal;
        }


        internal static string GetColumns(string TableAlias = "", string ColPrefix = "")
        {
            string ta = "";
            if (!string.IsNullOrEmpty(TableAlias))
                ta = TableAlias + ".";
            StringBuilder cols = new();
            cols.AppendLine($"{ta}plant {ColPrefix}plant, {ta}area {ColPrefix}area, ");
            cols.AppendLine($"{ta}data_group {ColPrefix}data_group, {ta}line_nbr {ColPrefix}line_nbr, {ta}label {ColPrefix}label, ");
            cols.AppendLine($"{ta}date_val {ColPrefix}date_val, {ta}value {ColPrefix}value, {ta}sort_order {ColPrefix}sort_order, ");
            cols.AppendLine($"{ta}quality {ColPrefix}quality");
            return cols.ToString();
        }
        private static string GetSelect()
        {
            StringBuilder sql = new();
            sql.AppendLine("SELECT ");
            sql.AppendLine(GetColumns());
            sql.AppendLine("FROM tolive.analytics");
            return sql.ToString();
        }

        //Insert, Update, Delete should not be needed as these are coming from Analytics team, we will not insert,update,delete

        internal static Analytics DataRowToObject(DbDataReader row, string ColPrefix = "")
        {
            Analytics RetVal = new();
            RetVal.Plant =Enum.Parse<MOO.Plant>( (string)Util.GetRowVal(row, $"{ColPrefix}plant"),true);
            RetVal.Area = (string)Util.GetRowVal(row, $"{ColPrefix}area");
            RetVal.Data_Group = (string)Util.GetRowVal(row, $"{ColPrefix}data_group");
            RetVal.Line_Nbr = (short)Util.GetRowVal(row, $"{ColPrefix}line_nbr");
            RetVal.Label = (string)Util.GetRowVal(row, $"{ColPrefix}label");
            RetVal.Date_Val = (DateTime)Util.GetRowVal(row, $"{ColPrefix}date_val");
            RetVal.Value = (string)Util.GetRowVal(row, $"{ColPrefix}value");
            RetVal.Sort_Order = (short)Util.GetRowVal(row, $"{ColPrefix}sort_order");
            RetVal.Quality = (string)Util.GetRowVal(row, $"{ColPrefix}quality");
            return RetVal;
        }

    }
}
