using MOO.DAL.ToLive.Enums;
using MOO.DAL.ToLive.Models;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.ToLive.Services
{
    public static class Conc_Analytics_GeneralSvc
    {
        static Conc_Analytics_GeneralSvc()
        {
            Util.RegisterOracle();
        }



        /// <summary>
        /// Gets the line impacts data from the conc_analytics_summary table
        /// </summary>
        /// <returns></returns>
        public static List<ConcAnalyticsImpact> GetImpacts()
        {            
            return GetImpactsList(ConcAnalyticsGnrlGroup.Impact);
        }
        /// <summary>
        /// Gets the plant and step impacts data from the conc_analytics_summary table
        /// </summary>
        /// <returns></returns>
        /// <remarks>if line number is 0 then it is the plant, otherwise line number refers to step</remarks>
        public static List<ConcAnalyticsImpact> GetImpactsAggregated()
        {
            return GetImpactsList(ConcAnalyticsGnrlGroup.Impact_Aggregated);
        }

        private static List<ConcAnalyticsImpact> GetImpactsList(ConcAnalyticsGnrlGroup ImpactType)
        {
            if (ImpactType != ConcAnalyticsGnrlGroup.Impact && ImpactType != ConcAnalyticsGnrlGroup.Impact_Aggregated)
                throw new InvalidEnumArgumentException("Impact type must be either Impact or Impcat_Aggregated");
            List<ConcAnalyticsImpact> retVal = new();
            List<Conc_Analytics_General> recs = Get(ImpactType);
            foreach (var rec in recs)
            {
                var newRec = new ConcAnalyticsImpact();
                newRec.LineNbr = (short)rec.Line_Nbr;
                if (DateTime.TryParse(rec.Value1, out DateTime dt))
                    newRec.TheDate = dt;
                switch (rec.Value2.ToLower())
                {
                    case "shift":
                        newRec.TimeCat = "Shft";
                        break;
                    case "today":
                        newRec.TimeCat = "Day";
                        break;
                    case "7_days":
                        newRec.TimeCat = "7Dy";
                        break;
                    case "30_days":
                        newRec.TimeCat = "30Dy";
                        break;
                    default:
                        newRec.TimeCat = rec.Value2;
                        break;
                };


                if (decimal.TryParse(rec.Value3, out decimal upTons))
                    newRec.Uplift_Long_Tons = Math.Round(upTons, 0, MidpointRounding.AwayFromZero);

                if (decimal.TryParse(rec.Value4, out decimal upPct))
                    newRec.Uplift_Pct = Math.Round(upPct, 1, MidpointRounding.AwayFromZero);
                retVal.Add(newRec);

            }
            return retVal;
        }



        /// <summary>
        /// constraint summary is stored in the General table.  We will pull the general records and then translate that to Constriaint summary records
        /// </summary>
        /// <returns></returns>
        public static List<Conc_Analytics_Cnstrnt_Smry> GetConstraintSummary(int LineNumber)
        {
            List<Conc_Analytics_Cnstrnt_Smry> retVal = new();
            List<Conc_Analytics_General> recs = Get(ConcAnalyticsGnrlGroup.Pareto, LineNumber);
            foreach(var rec in recs)
            {
                var newRec = new Conc_Analytics_Cnstrnt_Smry();
                newRec.Line_Nbr = rec.Line_Nbr;
                newRec.Label = rec.Value3;
                if (DateTime.TryParse(rec.Value1, out DateTime d))
                    newRec.Date_Val = d;
                newRec.Frequency = rec.Value2;

                if(decimal.TryParse(rec.Value4, out decimal active))
                    newRec.ActiveCount = active;
                else 
                    newRec.ActiveCount = 0;

                if (decimal.TryParse(rec.Value5, out decimal inactive))
                    newRec.InactiveCount = inactive;
                else
                    newRec.InactiveCount = 0;
                retVal.Add(newRec);

            }
            return retVal;
        }

        /// <summary>
        /// returns a list of floatation potential records
        /// </summary>
        /// <returns></returns>
        public static List<ConcAnalyticsFloatPotential> GetFloataionPotential()
        {
            List<ConcAnalyticsFloatPotential> retVal = new();
            List<Conc_Analytics_General> recs = Get(ConcAnalyticsGnrlGroup.Floatation_Potential, 0);
            foreach (var rec in recs)
            {
                var newRec = new ConcAnalyticsFloatPotential(rec);                
                retVal.Add(newRec);
            }
            recs = Get(ConcAnalyticsGnrlGroup.Floatation_Potential_MOO, 0);
            foreach (var rec in recs)
            {
                var newRec = new ConcAnalyticsFloatPotential(rec);
                retVal.Add(newRec);
            }
            return retVal;
        }



        public static List<Conc_Analytics_General> Get(ConcAnalyticsGnrlGroup DataGroup, int? LineNumber = null)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine("WHERE Data_Group = :DataGroup");
            if(LineNumber.HasValue)
                sql.AppendLine("AND Line_Nbr = :LineNumber");
            sql.AppendLine("ORDER BY sort_order");

            List<Conc_Analytics_General> elements = new();
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.Parameters.Add("DataGroup", DataGroup.ToString().ToLower());
            if(LineNumber.HasValue)
                cmd.Parameters.Add("LineNumber", LineNumber);
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
            cols.AppendLine($"{ta}data_group {ColPrefix}data_group, {ta}line_nbr {ColPrefix}line_nbr, ");
            cols.AppendLine($"{ta}sort_order {ColPrefix}sort_order, {ta}value1 {ColPrefix}value1, {ta}value2 {ColPrefix}value2, ");
            cols.AppendLine($"{ta}value3 {ColPrefix}value3, {ta}value4 {ColPrefix}value4, {ta}value5 {ColPrefix}value5, {ta}value6 {ColPrefix}value6,");
            cols.AppendLine($"{ta}value7 {ColPrefix}value7, {ta}value8 {ColPrefix}value8, {ta}value9 {ColPrefix}value9");
            return cols.ToString();
        }
        private static string GetSelect()
        {
            StringBuilder sql = new();
            sql.AppendLine("SELECT ");
            sql.AppendLine(GetColumns());
            sql.AppendLine("FROM tolive.conc_analytics_general");
            return sql.ToString();
        }



        public static int Insert(Conc_Analytics_General obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Insert(obj, conn);
            conn.Close();
            return recsAffected;
        }


        public static int Insert(Conc_Analytics_General obj, OracleConnection conn)
        {

            StringBuilder sql = new();
            sql.AppendLine("INSERT INTO tolive.conc_analytics_general(");
            sql.AppendLine("data_group, line_nbr, sort_order, value1, value2, value3, value4, value5, value6, value7, value8, value9)");
            sql.AppendLine("VALUES(");
            sql.AppendLine(":data_group, :line_nbr, :sort_order, :value1, :value2, :value3, :value4, :value5, :value6, :value7, :value8, :value9)");
            OracleCommand ins = new(sql.ToString(), conn);
            ins.BindByName = true;
            //we are getting data from hdq and they are doing an delete/insert, if we don't append something, hdq will delete our entries
            ins.Parameters.Add("data_group", obj.Data_Group.ToString().ToLower());  
            ins.Parameters.Add("line_nbr", obj.Line_Nbr);
            ins.Parameters.Add("sort_order", obj.Sort_Order);
            ins.Parameters.Add("value1", obj.Value1);
            ins.Parameters.Add("value2", obj.Value2);
            ins.Parameters.Add("value3", obj.Value3);
            ins.Parameters.Add("value4", obj.Value4);
            ins.Parameters.Add("value5", obj.Value5);
            ins.Parameters.Add("value6", obj.Value6);
            ins.Parameters.Add("value7", obj.Value7);
            ins.Parameters.Add("value8", obj.Value8);
            ins.Parameters.Add("value9", obj.Value9);
            int recsAffected = MOO.Data.ExecuteNonQuery(ins);
            return recsAffected;
        }




        /// <summary>
        /// Deletes a specific conc analytics record
        /// </summary>
        /// <param name="Data_Group"></param>
        /// <returns></returns>
        public static int Delete(MOO.DAL.ToLive.Enums.ConcAnalyticsGnrlGroup Data_Group, int LineNumber, int SortOrder)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Delete(Data_Group, LineNumber, SortOrder, conn);
            conn.Close();
            return recsAffected;
        }


        /// <summary>
        /// Deletes a specific conc analytics record
        /// </summary>
        /// <param name="Data_Group"></param>
        /// <returns></returns>
        public static int Delete(MOO.DAL.ToLive.Enums.ConcAnalyticsGnrlGroup Data_Group, int LineNumber, int SortOrder, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("DELETE FROM tolive.conc_analytics_general");
            sql.AppendLine("WHERE data_group = :data_group");
            sql.AppendLine("AND Line_Nbr = :Line");
            sql.AppendLine("AND sort_order = :sortOrd");
            OracleCommand del = new(sql.ToString(), conn);
            del.BindByName = true;
            del.Parameters.Add("data_group", Data_Group.ToString().ToLower());
            del.Parameters.Add("Line", LineNumber);
            del.Parameters.Add("sortOrd", SortOrder);
            int recsAffected = MOO.Data.ExecuteNonQuery(del);
            return recsAffected;
        }


        /// <summary>
        /// Deletes all of the records of the data group
        /// </summary>
        /// <param name="Data_Group"></param>
        /// <returns></returns>
        public static int DeleteGroup(MOO.DAL.ToLive.Enums.ConcAnalyticsGnrlGroup Data_Group)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = DeleteGroup(Data_Group, conn);
            conn.Close();
            return recsAffected;
        }


        /// <summary>
        /// Deletes all of the records of the data group
        /// </summary>
        /// <param name="Data_Group"></param>
        /// <returns></returns>
        public static int DeleteGroup(MOO.DAL.ToLive.Enums.ConcAnalyticsGnrlGroup Data_Group, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("DELETE FROM tolive.conc_analytics_general");
            sql.AppendLine("WHERE data_group = :data_group");
            OracleCommand del = new(sql.ToString(), conn);
            del.BindByName = true;
            del.Parameters.Add("data_group", Data_Group.ToString().ToLower());
            int recsAffected = MOO.Data.ExecuteNonQuery(del);
            return recsAffected;
        }



        internal static Conc_Analytics_General DataRowToObject(DbDataReader row, string ColPrefix = "")
        {
            Conc_Analytics_General RetVal = new();
            RetVal.Data_Group = Enum.Parse<ConcAnalyticsGnrlGroup>((string)Util.GetRowVal(row, $"{ColPrefix}data_group"), true);
            RetVal.Line_Nbr = (decimal)Util.GetRowVal(row, $"{ColPrefix}line_nbr");
            RetVal.Sort_Order = (decimal)Util.GetRowVal(row, $"{ColPrefix}sort_order");
            RetVal.Value1 = (string)Util.GetRowVal(row, $"{ColPrefix}value1");
            RetVal.Value2 = (string)Util.GetRowVal(row, $"{ColPrefix}value2");
            RetVal.Value3 = (string)Util.GetRowVal(row, $"{ColPrefix}value3");
            RetVal.Value4 = (string)Util.GetRowVal(row, $"{ColPrefix}value4");
            RetVal.Value5 = (string)Util.GetRowVal(row, $"{ColPrefix}value5");
            RetVal.Value6 = (string)Util.GetRowVal(row, $"{ColPrefix}value6");
            RetVal.Value7 = (string)Util.GetRowVal(row, $"{ColPrefix}value7");
            RetVal.Value8 = (string)Util.GetRowVal(row, $"{ColPrefix}value8");
            RetVal.Value9 = (string)Util.GetRowVal(row, $"{ColPrefix}value9");
            return RetVal;
        }

    }
}
