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
    public static class Conc_AnalyticsSvc
    {
        static Conc_AnalyticsSvc()
        {
            Util.RegisterOracle();
        }




        public static List<Conc_Analytics> Get(Conc_Analytics_Group DataGroup, int LineNumber)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine("WHERE Data_Group = :Data_Group");
            sql.AppendLine("AND Line_Nbr = :Line_Nbr");
            sql.AppendLine("AND value <> label");  //not sure why we get this.  maybe a bug from whoever is sending.  We don't want records where the value is the label
            sql.AppendLine("ORDER BY sort_order, date_val desc");

            List<Conc_Analytics> elements = new();
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.Parameters.Add("Data_Group", DataGroup.ToString().ToLower());
            cmd.Parameters.Add("Line_Nbr", LineNumber);
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

        /// <summary>
        /// Gets all Conc_Analytics records for a given DataGroup
        /// </summary>
        /// <param name="DataGroup"></param>
        /// <returns></returns>
        public static List<Conc_Analytics> Get(Conc_Analytics_Group DataGroup)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine("WHERE Data_Group = :Data_Group");
            sql.AppendLine("ORDER BY sort_order, date_val desc");

            List<Conc_Analytics> elements = new();
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


        /// <summary>
        /// Gets the average value for a given set of Conc_Analytics records
        /// </summary>
        /// <param name="DataGroup">Data_Group Column value</param>
        /// <param name="Label">Label column</param>
        /// <param name="LineStart">Start Line Number Filter</param>
        /// <param name="LineEnd">End Line Number Filter</param>
        /// <param name="PreviousNValues">Number of values to get the average over</param>
        /// <remarks>This was added as needed for the Analytics line summary xgb where we show ltph of a line vs average of the step</remarks>
        /// <returns></returns>
        public static double GetAvg(Conc_Analytics_Group DataGroup, string Label, int LineStart, int LineEnd, int PreviousNValues)
        {
            StringBuilder sql = new();
            sql.AppendLine("SELECT value FROM (");

            sql.AppendLine("SELECT value,");
            sql.AppendLine("ROW_NUMBER() OVER( partition by line_nbr ORDER BY date_val desc) rn");
            sql.AppendLine("FROM tolive.conc_analytics");
            sql.AppendLine("WHERE Data_Group = :Data_Group");
            sql.AppendLine("AND Line_Nbr BETWEEN :LineStart AND :LineEnd");

            sql.AppendLine(")");
            sql.AppendLine("WHERE rn <= :PreviousNValues");


            List<Conc_Analytics> elements = new();
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.Parameters.Add("Data_Group", DataGroup.ToString().ToLower());
            cmd.Parameters.Add("LineStart", LineStart);
            cmd.Parameters.Add("LineEnd", LineEnd);
            cmd.Parameters.Add("PreviousNValues", PreviousNValues);

            OracleDataReader rdr = cmd.ExecuteReader();

            double sum = 0;
            int count = 0;
            if (rdr.HasRows)
            {
                while (rdr.Read())
                {
                    if (!rdr.IsDBNull(0) && !rdr[0].ToString().Equals("nan", StringComparison.CurrentCultureIgnoreCase))
                    {
                        if (double.TryParse(rdr[0].ToString(), out double val))
                        {
                            sum += val;
                            count++;
                        }
                    }
                }
            }
            conn.Close();

            if(count > 0)
                return Math.Round(sum/count,2,MidpointRounding.AwayFromZero);
            else
                return 0;
        }


        public static ConcAnalyticsPivot GetPivot(Conc_Analytics_Group DataGroup, int LineNumber)
        {
            List<Conc_Analytics> vals = Get(DataGroup, LineNumber);
            //vals should be ordered by sort_order and date
            ConcAnalyticsPivot retVal = new()
            {
                DataGroup = DataGroup,
                LineNumber = LineNumber
            };
            string currentLabel = "";
            short dateIdx = 0;
            List<DateTime> Dates = new();
            //we need to first get the date array set up.  We will loop through the 
            var d = vals.GroupBy(x => x.Date_Val).Select(g => g.First()).OrderByDescending(x => x.Date_Val).ToArray();
            foreach (var obj in d)
                Dates.Add(obj.Date_Val);

            retVal.Dates = Dates.ToArray();



            ConcAnalyticsPivotRec newRec = new();
            newRec.Values = new string[retVal.Dates.Length];
            newRec.Quality = new string[retVal.Dates.Length];
            newRec.Parent = retVal;

            foreach (Conc_Analytics ca in vals)
            {
                if (currentLabel != ca.Label)
                {
                    //we switched to the next pivot record
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
                    currentLabel = ca.Label;
                    newRec.Label = ca.Label;
                    newRec.SortOrder = (int)ca.Sort_Order;
                }
                //make sure to get the correct index for the date, this should be all in the same order
                //but we will not take any chances
                int findIdx = Array.FindIndex(retVal.Dates, x => x == ca.Date_Val);
                if (findIdx >= 0)
                {
                    newRec.Values[findIdx] = ca.Value;
                    newRec.Quality[findIdx] = ca.Quality;
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
            sql.AppendLine("FROM tolive.conc_analytics");
            return sql.ToString();
        }

        //Insert, Update, Delete should not be needed as these are coming from Analytics team, we will not insert,update,delete

        internal static Conc_Analytics DataRowToObject(DbDataReader row, string ColPrefix = "")
        {
            Conc_Analytics RetVal = new();
            RetVal.Data_Group = Enum.Parse<Conc_Analytics_Group>((string)Util.GetRowVal(row, $"{ColPrefix}data_group"), true);
            RetVal.Line_Nbr = (decimal)Util.GetRowVal(row, $"{ColPrefix}line_nbr");
            RetVal.Label = (string)Util.GetRowVal(row, $"{ColPrefix}label");
            RetVal.Date_Val = (DateTime)Util.GetRowVal(row, $"{ColPrefix}date_val");
            RetVal.Value = (string)Util.GetRowVal(row, $"{ColPrefix}value");
            RetVal.Sort_Order = (decimal)Util.GetRowVal(row, $"{ColPrefix}sort_order");
            RetVal.Quality = (string)Util.GetRowVal(row, $"{ColPrefix}quality");
            return RetVal;
        }

    }
}
