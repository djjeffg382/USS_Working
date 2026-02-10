using MOO.DAL.ToLive.Enums;
using MOO.DAL.ToLive.Models;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Common;
using System.Linq;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MOO.DAL.ToLive.Services
{
    /// <summary>
    /// Service for accessing the Dust_Coll_Parms table
    /// </summary>
    /// <remarks>We will avoid an Insert function as we shouldn't need to add too often.  If we do, we can add those manually to the database.  We should also not need to delete from an application</remarks>
    public static class Dust_Coll_ParmsSvc
    {
        static Dust_Coll_ParmsSvc()
        {
            Util.RegisterOracle();
        }


        public static Dust_Coll_Parms Get(string equip_no)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine($"WHERE equip_no = :equip_no");


            Dust_Coll_Parms retVal = null;
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.Parameters.Add("equip_no", equip_no);
            OracleDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                rdr.Read();
                retVal = DataRowToObject(rdr);
            }
            conn.Close();
            return retVal;
        }


        /// <summary>
        /// Returns all records for dust coll Parms
        /// </summary>
        /// <returns></returns>
        public static List<Dust_Coll_Parms> GetAll()
        {
            return GetAll(null);
        }

        /// <summary>
        /// Returns all records for the specified plant
        /// </summary>
        /// <param name="Plant"></param>
        /// <returns></returns>
        public static List<Dust_Coll_Parms> GetAll(MOO.Plant? Plant)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            if(Plant != null)
                sql.AppendLine($"WHERE plant = :Plant");

            List<Dust_Coll_Parms> elements = [];
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);

            if (Plant != null)
                cmd.Parameters.Add("Plant", Plant == MOO.Plant.Minntac ? "MTC": "KTC");

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
        /// Gets an array of used areas in the Dust_Coll_Parms table
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<string> GetAreaListing()
        {
            StringBuilder sql = new();
            sql.AppendLine("SELECT DISTINCT area");
            sql.AppendLine("FROM tolive.dust_coll_parms");
            sql.AppendLine("ORDER BY area");

            List<string> retVal = [];
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);

            

            OracleDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                while (rdr.Read())
                {
                    retVal.Add(rdr[0].ToString());
                }
            }
            conn.Close();
            return retVal.ToArray();
        }


        internal static string GetColumns(string TableAlias = "", string ColPrefix = "")
        {
            string ta = "";
            if (!string.IsNullOrEmpty(TableAlias))
                ta = TableAlias + ".";
            StringBuilder cols = new();
            cols.AppendLine($"{ta}equip_no {ColPrefix}equip_no, {ta}pressure_ind {ColPrefix}pressure_ind, ");
            cols.AppendLine($"{ta}flow_ind {ColPrefix}flow_ind, {ta}pressure_lower_limit {ColPrefix}pressure_lower_limit, ");
            cols.AppendLine($"{ta}pressure_upper_limit {ColPrefix}pressure_upper_limit, ");
            cols.AppendLine($"{ta}flow_lower_limit {ColPrefix}flow_lower_limit, {ta}flow_upper_limit {ColPrefix}flow_upper_limit, ");
            cols.AppendLine($"{ta}last_updated_ck_num {ColPrefix}last_updated_ck_num, ");
            cols.AppendLine($"{ta}last_updated_date {ColPrefix}last_updated_date, {ta}equip_desc {ColPrefix}equip_desc, ");
            cols.AppendLine($"{ta}area {ColPrefix}area, {ta}sv_nbr {ColPrefix}sv_nbr, {ta}line {ColPrefix}line, ");
            cols.AppendLine($"{ta}press_or_amps_ind {ColPrefix}press_or_amps_ind, {ta}nsps_boolean {ColPrefix}nsps_boolean, ");
            cols.AppendLine($"{ta}press_id {ColPrefix}press_id, {ta}flow_id {ColPrefix}flow_id, {ta}mact_ind {ColPrefix}mact_ind, ");
            cols.AppendLine($"{ta}bh_ind_order {ColPrefix}bh_ind_order, {ta}plant {ColPrefix}plant, ");
            cols.AppendLine($"{ta}flow_tag {ColPrefix}flow_tag, {ta}pressure_tag {ColPrefix}pressure_tag, ");
            cols.AppendLine($"{ta}dc_tag {ColPrefix}dc_tag, {ta}eu_tag1 {ColPrefix}eu_tag1, {ta}eu_tag2 {ColPrefix}eu_tag2, ");
            cols.AppendLine($"{ta}eu_tag3 {ColPrefix}eu_tag3, {ta}eu_tag4 {ColPrefix}eu_tag4, ");
            cols.AppendLine($"{ta}particulate_tag {ColPrefix}particulate_tag, {ta}dc_type {ColPrefix}dc_type, ");
            cols.AppendLine($"{ta}dc_comments {ColPrefix}dc_comments, {ta}eu_analog_override {ColPrefix}eu_analog_override, ");
            cols.AppendLine($"{ta}snapshot_time {ColPrefix}snapshot_time");
            return cols.ToString();
        }

        private static string GetSelect()
        {
            StringBuilder sql = new();
            sql.AppendLine("SELECT ");
            sql.AppendLine(GetColumns());
            sql.AppendLine("FROM tolive.dust_coll_parms");
            return sql.ToString();
        }

        


        public static int Update(Dust_Coll_Parms obj, string UpdatedBy = "")
        {
            int recsAffected = 0;
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            //we will use a transaction because we will record to the audit table in the update statement
            using OracleTransaction trans = conn.BeginTransaction();
            try
            {
                recsAffected = Update(obj, conn, UpdatedBy);
                trans.Commit();
            }
            catch (Exception ex)
            {
                trans?.Rollback();
                throw;
            }
            finally
            {
                conn.Close();
            }
            
            return recsAffected;
        }


        public static int Update(Dust_Coll_Parms obj, OracleConnection conn, string UpdatedBy = "")
        {
            ValidateData(obj);
            var oldVal = Get(obj.Equip_No);
            //record to audit table
            Audit_Table auditRec = new()
            {
                Table_Name="Dust_Coll_Parm",
                Key_Value=obj.Equip_No,
                Old_Value = JsonSerializer.Serialize(oldVal),
                New_Value = JsonSerializer.Serialize(obj),
                Thedate = DateTime.Now,
                Modified_By = UpdatedBy
            };
            Audit_TableSvc.Insert(auditRec, conn);

            StringBuilder sql = new();
            sql.AppendLine("UPDATE tolive.Dust_Coll_Parms SET");
            sql.AppendLine("pressure_ind = :pressure_ind, ");
            sql.AppendLine("flow_ind = :flow_ind, ");
            sql.AppendLine("pressure_lower_limit = :pressure_lower_limit, ");
            sql.AppendLine("pressure_upper_limit = :pressure_upper_limit, ");
            sql.AppendLine("flow_lower_limit = :flow_lower_limit, ");
            sql.AppendLine("flow_upper_limit = :flow_upper_limit, ");
            sql.AppendLine("last_updated_date = :last_updated_date, ");
            sql.AppendLine("equip_desc = :equip_desc, ");
            sql.AppendLine("area = :area, ");
            sql.AppendLine("sv_nbr = :sv_nbr, ");
            sql.AppendLine("line = :line, ");
            sql.AppendLine("press_or_amps_ind = :press_or_amps_ind, ");
            sql.AppendLine("nsps_boolean = :nsps_boolean, ");
            sql.AppendLine("press_id = :press_id, ");
            sql.AppendLine("flow_id = :flow_id, ");
            sql.AppendLine("mact_ind = :mact_ind, ");
            sql.AppendLine("plant = :plant, ");
            sql.AppendLine("flow_tag = :flow_tag, ");
            sql.AppendLine("pressure_tag = :pressure_tag, ");
            sql.AppendLine("dc_tag = :dc_tag, ");
            sql.AppendLine("eu_tag1 = :eu_tag1, ");
            sql.AppendLine("eu_tag2 = :eu_tag2, ");
            sql.AppendLine("eu_tag3 = :eu_tag3, ");
            sql.AppendLine("eu_tag4 = :eu_tag4, ");
            sql.AppendLine("particulate_tag = :particulate_tag, ");
            sql.AppendLine("dc_type = :dc_type, ");
            sql.AppendLine("dc_comments = :dc_comments, ");
            sql.AppendLine("eu_analog_override = :eu_analog_override,");
            sql.AppendLine("snapshot_time = :snapshot_time");
            sql.AppendLine("WHERE equip_no = :equip_no");
            OracleCommand upd = new(sql.ToString(), conn);
            upd.BindByName = true;
            upd.Parameters.Add("pressure_ind", obj.Pressure_Ind ? "Y":"N");
            upd.Parameters.Add("flow_ind", obj.Flow_Ind ? "Y": "N");
            upd.Parameters.Add("pressure_lower_limit", obj.Pressure_Lower_Limit);
            upd.Parameters.Add("pressure_upper_limit", obj.Pressure_Upper_Limit);
            upd.Parameters.Add("flow_lower_limit", obj.Flow_Lower_Limit);
            upd.Parameters.Add("flow_upper_limit", obj.Flow_Upper_Limit);
            upd.Parameters.Add("last_updated_date", DateTime.Now);
            upd.Parameters.Add("equip_desc", obj.Equip_Desc);
            upd.Parameters.Add("area", obj.Area);
            upd.Parameters.Add("sv_nbr", obj.Sv_Nbr);
            upd.Parameters.Add("line", obj.Line);
            upd.Parameters.Add("press_or_amps_ind", obj.Press_Or_Amps_Ind == Enums.DustCollPressAmps.Pressure ? "P" : "A");
            upd.Parameters.Add("nsps_boolean", obj.Nsps_Boolean ? "T": "F");
            upd.Parameters.Add("press_id", obj.Press_Id);
            upd.Parameters.Add("flow_id", obj.Flow_Id);
            upd.Parameters.Add("mact_ind", obj.Mact_Ind ? "Y":"N");
            upd.Parameters.Add("plant", obj.Plant == Plant.Minntac ? "MTC":"KTC");
            upd.Parameters.Add("flow_tag", obj.Flow_Tag);
            upd.Parameters.Add("pressure_tag", obj.Pressure_Tag);
            upd.Parameters.Add("dc_tag", obj.DC_Tag);
            upd.Parameters.Add("eu_tag1", obj.EU_Tag1);
            upd.Parameters.Add("eu_tag2", obj.EU_Tag2);
            upd.Parameters.Add("eu_tag3", obj.EU_Tag3);
            upd.Parameters.Add("eu_tag4", obj.EU_Tag4);
            upd.Parameters.Add("particulate_tag", obj.Particulate_Tag);
            upd.Parameters.Add("dc_type", obj.DC_Type == null? DBNull.Value:(int)obj.DC_Type);
            upd.Parameters.Add("dc_comments", obj.DC_Comments);
            upd.Parameters.Add("eu_analog_override", obj.Eu_Analog_Override);
            upd.Parameters.Add("equip_no", obj.Equip_No);
            upd.Parameters.Add("snapshot_time", obj.Snapshot_Time);
            int recsAffected = MOO.Data.ExecuteNonQuery(upd);
            return recsAffected;
        }

        private static void ValidateData(Dust_Coll_Parms DustColl)
        {
            InvalidDataException invalidExc = new();
            if (DustColl.DC_Type == Enums.DustCollectorType.Wet_Scrubber)
            {
                if (!DustColl.Mact_Ind)
                    invalidExc.ValidationResults.Add(new ValidationResult("MACT Indicator must be set true if DC Type is Wet Scrubber", new string[] { "Mact_Ind", "DC_Type" }));
            }
            if (DustColl.DC_Type == Enums.DustCollectorType.Baghouse)
            {
                if (DustColl.Mact_Ind)
                    invalidExc.ValidationResults.Add(new ValidationResult("MACT Indicator must be set false if DC Type is Baghouse", new string[] { "Mact_Ind", "DC_Type" }));

                if(DustColl.Flow_Ind)
                    invalidExc.ValidationResults.Add(new ValidationResult("Flow Indicator must be set false if DC Type is Baghouse", new string[] { "Mact_Ind", "DC_Type" }));
            }

            if (DustColl.DC_Type == Enums.DustCollectorType.Snapshot)
            {
                if (DustColl.Mact_Ind)
                    invalidExc.ValidationResults.Add(new ValidationResult("MACT Indicator must be set false if DC Type is Snapshot", new string[] { "Mact_Ind", "DC_Type" }));
            }

            if(DustColl.Flow_Lower_Limit.HasValue && DustColl.Flow_Upper_Limit.HasValue && DustColl.Flow_Lower_Limit >= DustColl.Flow_Upper_Limit)
                invalidExc.ValidationResults.Add(new ValidationResult("Flow lower limit must be lower than upper limit", new string[] { "Flow_Lower_Limit", "Flow_Upper_Limit" }));

            if (DustColl.Pressure_Lower_Limit.HasValue && DustColl.Pressure_Upper_Limit.HasValue && DustColl.Pressure_Lower_Limit >= DustColl.Pressure_Upper_Limit)
                invalidExc.ValidationResults.Add(new ValidationResult("Pressure/Amps lower limit must be lower than upper limit", new string[] { "Pressure_Lower_Limit", "Pressure_Upper_Limit" }));


            if(!string.IsNullOrEmpty(DustColl.Snapshot_Time) && DustColl.Plant == Plant.Minntac)
                invalidExc.ValidationResults.Add(new ValidationResult("Snapshot time must be cleared for Minntac Dust Collectors", new string[] { "Snapshot_Time", "Plant" }));

            if (string.IsNullOrEmpty(DustColl.Snapshot_Time) && DustColl.Plant == Plant.Keetac && DustColl.DC_Type == DustCollectorType.Baghouse)
                invalidExc.ValidationResults.Add(new ValidationResult("Snapshot time must be set for Keetac Baghouse", new string[] { "Snapshot_Time", "Plant", "DC_Type" }));

            if (invalidExc.ValidationResults.Count > 0 ) 
                throw invalidExc;
        }



        internal static Dust_Coll_Parms DataRowToObject(DbDataReader row, string ColPrefix = "")
        {
            Dust_Coll_Parms RetVal = new();
            RetVal.Equip_No = (string)Util.GetRowVal(row, $"{ColPrefix}equip_no");
            RetVal.Pressure_Ind = (string)Util.GetRowVal(row, $"{ColPrefix}pressure_ind") == "Y";
            RetVal.Flow_Ind = (string)Util.GetRowVal(row, $"{ColPrefix}flow_ind") == "Y";
            RetVal.Pressure_Lower_Limit = (double?)Util.GetRowVal(row, $"{ColPrefix}pressure_lower_limit");
            RetVal.Pressure_Upper_Limit = (double?)Util.GetRowVal(row, $"{ColPrefix}pressure_upper_limit");
            RetVal.Flow_Lower_Limit = (double?)Util.GetRowVal(row, $"{ColPrefix}flow_lower_limit");
            RetVal.Flow_Upper_Limit = (double?)Util.GetRowVal(row, $"{ColPrefix}flow_upper_limit");
            RetVal.Last_Updated_Date = (DateTime?)Util.GetRowVal(row, $"{ColPrefix}last_updated_date");
            RetVal.Equip_Desc = (string)Util.GetRowVal(row, $"{ColPrefix}equip_desc");
            RetVal.Area = (string)Util.GetRowVal(row, $"{ColPrefix}area");
            RetVal.Sv_Nbr = (string)Util.GetRowVal(row, $"{ColPrefix}sv_nbr");
            RetVal.Line = Convert.ToInt32( (decimal?)Util.GetRowVal(row, $"{ColPrefix}line"));
            RetVal.Press_Or_Amps_Ind = (string)Util.GetRowVal(row, $"{ColPrefix}press_or_amps_ind") == "P" ? Enums.DustCollPressAmps.Pressure: Enums.DustCollPressAmps.Amps;
            RetVal.Nsps_Boolean = (string)Util.GetRowVal(row, $"{ColPrefix}nsps_boolean") == "T";
            RetVal.Press_Id = Convert.ToInt32((decimal?)Util.GetRowVal(row, $"{ColPrefix}press_id"));
            RetVal.Flow_Id = Convert.ToInt32((decimal?)Util.GetRowVal(row, $"{ColPrefix}flow_id"));
            RetVal.Mact_Ind = (string)Util.GetRowVal(row, $"{ColPrefix}mact_ind") == "Y";
            RetVal.Plant = (string)Util.GetRowVal(row, $"{ColPrefix}plant") == "MTC" ? Plant.Minntac:Plant.Keetac;
            RetVal.Flow_Tag = (string)Util.GetRowVal(row, $"{ColPrefix}flow_tag");
            RetVal.Pressure_Tag = (string)Util.GetRowVal(row, $"{ColPrefix}pressure_tag");
            RetVal.DC_Tag = (string)Util.GetRowVal(row, $"{ColPrefix}dc_tag");
            RetVal.EU_Tag1 = (string)Util.GetRowVal(row, $"{ColPrefix}eu_tag1");
            RetVal.EU_Tag2 = (string)Util.GetRowVal(row, $"{ColPrefix}eu_tag2");
            RetVal.EU_Tag3 = (string)Util.GetRowVal(row, $"{ColPrefix}eu_tag3");
            RetVal.EU_Tag4 = (string)Util.GetRowVal(row, $"{ColPrefix}eu_tag4");
            RetVal.Particulate_Tag = (string)Util.GetRowVal(row, $"{ColPrefix}particulate_tag");
            if (row.IsDBNull(row.GetOrdinal("dc_type")))
                RetVal.DC_Type = null;
            else
                RetVal.DC_Type = (DustCollectorType)(short)Util.GetRowVal(row, $"{ColPrefix}dc_type");

            RetVal.DC_Comments = (string)Util.GetRowVal(row, $"{ColPrefix}dc_comments");
            RetVal.Eu_Analog_Override = (string)Util.GetRowVal(row, $"{ColPrefix}eu_analog_override");

            RetVal.Snapshot_Time = (string)Util.GetRowVal(row, $"{ColPrefix}snapshot_time");

            return RetVal;
        }

    }
}
