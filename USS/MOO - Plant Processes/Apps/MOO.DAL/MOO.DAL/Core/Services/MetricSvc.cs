using MOO.DAL.Core.Enums;
using MOO.DAL.Core.Models;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Common;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.Core.Services
{
    public static class MetricSvc
    {
        static MetricSvc()
        {
            Util.RegisterOracle();
        }


        public static Metric Get(decimal Metric_Id)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine($"WHERE m.metric_id = :metric_id");


            Metric retVal = null;
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.Parameters.Add("metric_id", Metric_Id);
            OracleDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                rdr.Read();
                retVal = DataRowToObject(rdr);
            }
            conn.Close();
            return retVal;
        }

        public static List<Metric> GetAll()
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());

            List<Metric> elements = [];
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

        /// <summary>
        /// Gets all of the metrics that are Metric Value Adjustments
        /// </summary>
        /// <returns></returns>
        public static List<Metric> GetMetricValueAdjustments()
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine($"WHERE m.coll_type_id = 6");

            List<Metric> elements = [];
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

        /// <summary>
        /// Gets the value of the adjustment field for a given Metric Value Adjust
        /// </summary>
        /// <param name="AdjustMetric"></param>
        /// <param name="ShiftDate"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        /// <remarks>Some metrics are set up as adjustments, these need to show the warehouse values</remarks>
        public static decimal? GetMetricValueAdjustmentValue(Metric AdjustMetric, DateTime ShiftDate)
        {
            string additionalFilter = "";
            if (AdjustMetric.Metric_Id == 641 ||  //Pellet tons Month Dry Adjust
                    AdjustMetric.Metric_Id == 935 || //Year Dry Adjust 
                    AdjustMetric.Metric_Id == 936 || //Year Natural Adjust
                    AdjustMetric.Metric_Id == 1089) // Month Natural Adjust
            {
                //K_PELL_TD.PELLET_TONS_DRY_GT, K_PELL_TD.PELLET_TONS_GT
                additionalFilter = "AND pellet_type = " + ((int)KTCPelletType.BlastFurnace);
            }else if (AdjustMetric.Metric_Id == 1131 ||  //tons Month Dry Adjust
                    AdjustMetric.Metric_Id == 1133 || //Year Dry Adjust 
                    AdjustMetric.Metric_Id == 1132 || //Year Natural Adjust
                    AdjustMetric.Metric_Id == 1130) // Month Natural Adjust
            {
                additionalFilter = "AND pellet_type = " + ((int)KTCPelletType.DRPellet);
            }

            StringBuilder sql = new();
            //adjustment field should be in table.column format
            string[] vals = AdjustMetric.Wh_Adjust_Field.Split('.');
            if (vals.Length != 2)
                throw new ArgumentException("Invalid parameter.  Must contain Table.Field format", "AdjustmentField");
            string tdType = "1";

            switch (AdjustMetric.Coll_Time.Coll_Time_Id)
            {
                case 6:
                    tdType = "1";  //Month Adjustment
                    break;
                case 7:
                    tdType = "0"; //Year Adjustment
                    break;
                default:
                    throw new ArgumentException("Invalid Coll_Time set for adjustment metric, must be either 6 = Month, or 7 = Year");
            }

            sql.AppendLine($"SELECT {vals[1]}");
            sql.AppendLine($"FROM warehouse.{vals[0]}");
            sql.AppendLine($"WHERE shift_date = {MOO.Dates.OraDate(ShiftDate, true)}");
            sql.AppendLine($"AND td_type = {tdType}");
            if (!string.IsNullOrEmpty(additionalFilter))
                sql.AppendLine(additionalFilter);

            var retVal = MOO.Data.ExecuteScalar(sql.ToString(), Data.MNODatabase.DMART, null);
            if (retVal != null)
                return Convert.ToDecimal(retVal);
            else
                return null;
        }


        internal static string GetColumns(string TableAlias = "", string ColPrefix = "")
        {
            string ta = "";
            if (!string.IsNullOrEmpty(TableAlias))
                ta = TableAlias + ".";
            StringBuilder cols = new();
            cols.AppendLine($"{ta}metric_id {ColPrefix}metric_id, {ta}metric_name {ColPrefix}metric_name, ");
            cols.AppendLine($"{ta}uom_id {ColPrefix}uom_id, {ta}metric_type_id {ColPrefix}metric_type_id, ");
            cols.AppendLine($"{ta}coll_type_id {ColPrefix}coll_type_id, {ta}coll_time_id {ColPrefix}coll_time_id, ");
            cols.AppendLine($"{ta}process_level_id {ColPrefix}process_level_id, {ta}tag_name {ColPrefix}tag_name, ");
            cols.AppendLine($"{ta}warn_min {ColPrefix}warn_min, {ta}warn_max {ColPrefix}warn_max, ");
            cols.AppendLine($"{ta}error_min {ColPrefix}error_min, {ta}error_max {ColPrefix}error_max, ");
            cols.AppendLine($"{ta}approvable {ColPrefix}approvable, {ta}scada_id {ColPrefix}scada_id, ");
            cols.AppendLine($"{ta}decimal_places {ColPrefix}decimal_places, {ta}default_group_id {ColPrefix}default_group_id, ");
            cols.AppendLine($"{ta}wh_adjust_field {ColPrefix}wh_adjust_field, {ta}value_type {ColPrefix}value_type, ");
            cols.AppendLine($"{ta}isactive {ColPrefix}isactive, {ta}metric_comments {ColPrefix}metric_comments");
            return cols.ToString();
        }
        private static string GetSelect()
        {
            StringBuilder sql = new();
            sql.AppendLine("SELECT ");
            sql.AppendLine(GetColumns("m") + ",");
            sql.AppendLine("ms.user_list, ms.role_list,");  //values from metric security
            sql.AppendLine(UomSvc.GetColumns("uom", "uom_") + ",");
            sql.AppendLine(Collection_TypeSvc.GetColumns("ctype", "ctype_") + ",");
            sql.AppendLine(Collection_TimeSvc.GetColumns("ctime", "ctime_") + ",");
            sql.AppendLine(Process_LevelSvc.GetColumns("p", "p_"));
            sql.AppendLine("FROM core.metric m");
            sql.AppendLine("JOIN core.uom ON uom.uom_id = m.uom_id ");
            sql.AppendLine("JOIN core.collection_type ctype ON ctype.coll_type_id = m.coll_type_id ");
            sql.AppendLine("JOIN core.collection_time ctime ON ctime.coll_time_id = m.coll_time_id ");
            sql.AppendLine("JOIN core.process_level_v p ON p.process_level_id = m.process_level_id ");
            sql.AppendLine("LEFT JOIN core.metric_security ms ON m.metric_id = ms.metric_id ");

            return sql.ToString();
        }

        public static int Insert(Metric obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleTransaction trans = conn.BeginTransaction();
            int recsAffected = Insert(obj, conn);
            trans.Commit();
            conn.Close();
            return recsAffected;
        }


        public static int Insert(Metric obj, OracleConnection conn)
        {
            if (obj.Metric_Id <= 0)
                obj.Metric_Id = Convert.ToInt32(MOO.Data.GetNextSequence("Core.SEQ_METRIC"));

            StringBuilder sql = new();
            sql.AppendLine("INSERT INTO Core.Metric(");
            sql.AppendLine("metric_id, metric_name, uom_id, metric_type_id, coll_type_id, coll_time_id, ");
            sql.AppendLine("process_level_id, tag_name, warn_min, warn_max, error_min, error_max, approvable, ");
            sql.AppendLine("scada_id, decimal_places, default_group_id, wh_adjust_field, value_type, isactive, ");
            sql.AppendLine("metric_comments)");
            sql.AppendLine("VALUES(");
            sql.AppendLine(":metric_id, :metric_name, :uom_id, :metric_type_id, :coll_type_id, :coll_time_id, ");
            sql.AppendLine(":process_level_id, :tag_name, :warn_min, :warn_max, :error_min, :error_max, ");
            sql.AppendLine(":approvable, :scada_id, :decimal_places, :default_group_id, :wh_adjust_field, ");
            sql.AppendLine(":value_type, :isactive, :metric_comments)");
            OracleCommand ins = new(sql.ToString(), conn);
            ins.BindByName = true;
            ins.Parameters.Add("metric_id", obj.Metric_Id);
            ins.Parameters.Add("metric_name", obj.Metric_Name);
            ins.Parameters.Add("uom_id", obj.Uom.Uom_Id);
            ins.Parameters.Add("metric_type_id", (int)obj.Metric_Type);
            ins.Parameters.Add("coll_type_id", obj.Coll_Type.Coll_Type_Id);
            ins.Parameters.Add("coll_time_id", obj.Coll_Time.Coll_Time_Id);
            ins.Parameters.Add("process_level_id", obj.Process_Level.Process_Level_Id);
            ins.Parameters.Add("tag_name", obj.Tag_Name);
            ins.Parameters.Add("warn_min", obj.Warn_Min);
            ins.Parameters.Add("warn_max", obj.Warn_Max);
            ins.Parameters.Add("error_min", obj.Error_Min);
            ins.Parameters.Add("error_max", obj.Error_Max);
            ins.Parameters.Add("approvable", obj.Approvable);
            ins.Parameters.Add("scada_id", (int)obj.Scada);
            ins.Parameters.Add("decimal_places", obj.Decimal_Places);
            ins.Parameters.Add("default_group_id", obj.Default_Group_Id);
            ins.Parameters.Add("wh_adjust_field", obj.Wh_Adjust_Field);
            ins.Parameters.Add("value_type", obj.Value_Type);
            ins.Parameters.Add("isactive", obj.Isactive ? 1 : 0);
            ins.Parameters.Add("metric_comments", obj.Metric_Comments);
            int recsAffected = MOO.Data.ExecuteNonQuery(ins);
            if (recsAffected > 0 && (obj.UserList.Any() || obj.RoleList.Any()))
                InsertMetricSecurity(obj, conn);
            return recsAffected;
        }


        public static int Update(Metric obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            OracleTransaction trans = conn.BeginTransaction();
            int recsAffected = Update(obj, conn);
            trans.Commit();
            conn.Close();
            return recsAffected;
        }


        public static int Update(Metric obj, OracleConnection conn)
        {
            
            StringBuilder sql = new();
            sql.AppendLine("UPDATE Core.Metric SET");
            sql.AppendLine("metric_name = :metric_name, ");
            sql.AppendLine("uom_id = :uom_id, ");
            sql.AppendLine("metric_type_id = :metric_type_id, ");
            sql.AppendLine("coll_type_id = :coll_type_id, ");
            sql.AppendLine("coll_time_id = :coll_time_id, ");
            sql.AppendLine("process_level_id = :process_level_id, ");
            sql.AppendLine("tag_name = :tag_name, ");
            sql.AppendLine("warn_min = :warn_min, ");
            sql.AppendLine("warn_max = :warn_max, ");
            sql.AppendLine("error_min = :error_min, ");
            sql.AppendLine("error_max = :error_max, ");
            sql.AppendLine("approvable = :approvable, ");
            sql.AppendLine("scada_id = :scada_id, ");
            sql.AppendLine("decimal_places = :decimal_places, ");
            sql.AppendLine("default_group_id = :default_group_id, ");
            sql.AppendLine("wh_adjust_field = :wh_adjust_field, ");
            sql.AppendLine("value_type = :value_type, ");
            sql.AppendLine("isactive = :isactive, ");
            sql.AppendLine("metric_comments = :metric_comments");
            sql.AppendLine("WHERE metric_id = :metric_id");
            OracleCommand upd = new(sql.ToString(), conn);
            upd.BindByName = true;
            upd.Parameters.Add("metric_name", obj.Metric_Name);
            upd.Parameters.Add("uom_id", obj.Uom.Uom_Id);
            upd.Parameters.Add("metric_type_id", (int)obj.Metric_Type);
            upd.Parameters.Add("coll_type_id", obj.Coll_Type.Coll_Type_Id);
            upd.Parameters.Add("coll_time_id", obj.Coll_Time.Coll_Time_Id);
            upd.Parameters.Add("process_level_id", obj.Process_Level.Process_Level_Id);
            upd.Parameters.Add("tag_name", obj.Tag_Name);
            upd.Parameters.Add("warn_min", obj.Warn_Min);
            upd.Parameters.Add("warn_max", obj.Warn_Max);
            upd.Parameters.Add("error_min", obj.Error_Min);
            upd.Parameters.Add("error_max", obj.Error_Max);
            upd.Parameters.Add("approvable", obj.Approvable);
            upd.Parameters.Add("scada_id", (int)obj.Scada);
            upd.Parameters.Add("decimal_places", obj.Decimal_Places);
            upd.Parameters.Add("default_group_id", obj.Default_Group_Id);
            upd.Parameters.Add("wh_adjust_field", obj.Wh_Adjust_Field);
            upd.Parameters.Add("value_type", obj.Value_Type);
            upd.Parameters.Add("isactive", obj.Isactive ? 1 : 0);
            upd.Parameters.Add("metric_comments", obj.Metric_Comments);
            upd.Parameters.Add("metric_id", obj.Metric_Id);
            int recsAffected = MOO.Data.ExecuteNonQuery(upd);
            if (recsAffected > 0 && (obj.UserList.Any() || obj.RoleList.Any()))
            {
                //perform an upsert on the metric security if any is set up
                int secAffected = UpdateMetricSecurity(obj, conn);
                if (secAffected == 0)
                    InsertMetricSecurity(obj, conn);
            }
            else
                DeleteMetricSecurity(obj.Metric_Id, conn); //no security is set up, make sure we don't have a record

            return recsAffected;
        }


        public static int Delete(Metric obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            OracleTransaction trans = conn.BeginTransaction();
            int recsAffected = Delete(obj, conn);
            trans.Commit();
            conn.Close();
            return recsAffected;
        }

        public static int Delete(Metric obj, OracleConnection conn)
        {
            StringBuilder sql = new();

            //Delete any records in the metric_security table first
            DeleteMetricSecurity(obj.Metric_Id, conn);



            sql.AppendLine("DELETE FROM Core.Metric");
            sql.AppendLine("WHERE metric_id = :metric_id");
            OracleCommand del = new(sql.ToString(), conn);
            del.BindByName = true;
            del.Parameters.Add("metric_id", obj.Metric_Id);
            int recsAffected = MOO.Data.ExecuteNonQuery(del);
            return recsAffected;
        }

        /// <summary>
        /// Deletes corresponding metric security records if they exist
        /// </summary>
        /// <param name="MetricID"></param>
        /// <param name="conn"></param>
        private static int DeleteMetricSecurity(int MetricID, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("DELETE FROM Core.Metric_Security");
            sql.AppendLine("WHERE metric_id = :metric_id");
            OracleCommand del = new(sql.ToString(), conn);
            del.BindByName = true;
            del.Parameters.Add("metric_id", MetricID);
            return MOO.Data.ExecuteNonQuery(del);
        }
        /// <summary>
        /// updates the metric_security record for this metric
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        private static int UpdateMetricSecurity(Metric obj, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("UPDATE Core.Metric_Security");
            sql.AppendLine("SET user_list = :user_list, role_list = :role_list");
            sql.AppendLine("WHERE metric_id = :metric_id");
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.BindByName = true;
            cmd.Parameters.Add("user_list", obj.UserListCSV);
            cmd.Parameters.Add("role_list", obj.RoleListCSV);
            cmd.Parameters.Add("metric_id", obj.Metric_Id);
            return MOO.Data.ExecuteNonQuery(cmd);
        }

        /// <summary>
        /// Inserts the metric_security record for this metric
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        private static int InsertMetricSecurity(Metric obj, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("INSERT INTO Core.Metric_Security");
            sql.AppendLine("(metric_id,user_list,role_list)");
            sql.AppendLine("VALUES(:metric_id,:user_list,:role_list)");
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.BindByName = true;
            cmd.Parameters.Add("user_list", obj.UserListCSV);
            cmd.Parameters.Add("role_list", obj.RoleListCSV);
            cmd.Parameters.Add("metric_id", obj.Metric_Id);
            return MOO.Data.ExecuteNonQuery(cmd);
        }


        internal static Metric DataRowToObject(DbDataReader row, string ColPrefix = "")
        {
            Metric RetVal = new();
            RetVal.Metric_Id = (int)(decimal)Util.GetRowVal(row, $"{ColPrefix}metric_id");
            RetVal.Metric_Name = (string)Util.GetRowVal(row, $"{ColPrefix}metric_name");
            RetVal.Uom = UomSvc.DataRowToObject(row, "uom_");
            RetVal.Metric_Type = (MOO.DAL.Core.Enums.Metric_Type)(decimal)Util.GetRowVal(row, $"{ColPrefix}metric_type_id");
            RetVal.Coll_Type = Collection_TypeSvc.DataRowToObject(row, "ctype_");
            RetVal.Coll_Time = Collection_TimeSvc.DataRowToObject(row, "ctime_");
            RetVal.Process_Level = Process_LevelSvc.DataRowToObject(row, "p_");
            RetVal.Tag_Name = (string)Util.GetRowVal(row, $"{ColPrefix}tag_name");
            RetVal.Warn_Min = (decimal?)Util.GetRowVal(row, $"{ColPrefix}warn_min");
            RetVal.Warn_Max = (decimal?)Util.GetRowVal(row, $"{ColPrefix}warn_max");
            RetVal.Error_Min = (decimal?)Util.GetRowVal(row, $"{ColPrefix}error_min");
            RetVal.Error_Max = (decimal?)Util.GetRowVal(row, $"{ColPrefix}error_max");
            RetVal.Approvable = (decimal)Util.GetRowVal(row, $"{ColPrefix}approvable");

            if (row.IsDBNull(row.GetOrdinal("scada_id")))
                RetVal.Scada = Enums.Scada.None_Unknown;
            else
                RetVal.Scada = (MOO.DAL.Core.Enums.Scada)(decimal?)Util.GetRowVal(row, $"{ColPrefix}scada_id");

            RetVal.Decimal_Places = (decimal?)Util.GetRowVal(row, $"{ColPrefix}decimal_places");
            RetVal.Default_Group_Id = (decimal?)Util.GetRowVal(row, $"{ColPrefix}default_group_id");
            RetVal.Wh_Adjust_Field = (string)Util.GetRowVal(row, $"{ColPrefix}wh_adjust_field");
            RetVal.Value_Type = (decimal?)Util.GetRowVal(row, $"{ColPrefix}value_type");
            RetVal.Isactive = (decimal)Util.GetRowVal(row, $"{ColPrefix}isactive") == 1;
            RetVal.Metric_Comments = (string)Util.GetRowVal(row, $"{ColPrefix}metric_comments");

            RetVal.UserListCSV = (string)Util.GetRowVal(row, $"user_list");
            RetVal.RoleListCSV = (string)Util.GetRowVal(row, $"role_list");


            return RetVal;
        }

    }
}
