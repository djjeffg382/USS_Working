using MOO.DAL.ToLive.Models;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MOO.DAL.ToLive.Models.Hangup;

namespace MOO.DAL.ToLive.Services
{
    public class HangupSvc
    {
        static HangupSvc()
        {
            Util.RegisterOracle();
            Util.RegisterSqlServer();
        }

 

        public static List<Hangup> GetRealtimeListing()
        {
            StringBuilder sql = new();
            sql.Append(GetSelect(false));
            sql.Append("WHERE start_date > sysdate - 1/24");
            sql.Append("OR (status = 'Pending' AND start_date > sysdate - 1)");
            sql.Append("ORDER BY start_date DESC");


            List<Hangup> elements = new();
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
        public static Hangup Get(decimal hangup_id)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect(true));
            sql.AppendLine($"WHERE hangup_id = :hangup_id");


            Hangup retVal = null;
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.Parameters.Add("hangup_id", hangup_id);
            OracleDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                rdr.Read();
                retVal = DataRowToObject(rdr,null,true);
            }
            conn.Close();
            return retVal;
        }


        public static List<Hangup> GetAll(DateTime? startDate = null)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect(false));
            if (startDate.HasValue)
            {
                sql.Append(" WHERE start_date > :start_date");
            }
            List<Hangup> elements = new();
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);
            if (startDate.HasValue)
            {
                cmd.Parameters.Add(new OracleParameter("start_date", OracleDbType.Date)).Value = startDate.Value;
            }
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


        internal static string GetColumns(string TableAlias = "", string ColPrefix = "", bool includeImage=false)
        {
            string ta = "";
            if (!string.IsNullOrEmpty(TableAlias))
                ta = TableAlias + ".";
            StringBuilder cols = new();
            cols.AppendLine($"{ta}hangup_id {ColPrefix}hangup_id, {ta}crusher_number {ColPrefix}crusher_number, ");
            cols.AppendLine($"{ta}start_date {ColPrefix}start_date, {ta}end_date {ColPrefix}end_date, ");
            cols.AppendLine($"{ta}status {ColPrefix}status, {ta}hangup_type {ColPrefix}hangup_type, ");
            cols.AppendLine($"{ta}cleared_by {ColPrefix}cleared_by, {ta}hangup_comments {ColPrefix}hangup_comments, ");
            cols.AppendLine($"{ta}haul_cycle_rec_ident {ColPrefix}haul_cycle_rec_ident, {ta}truck {ColPrefix}truck, ");
            cols.AppendLine($"{ta}load_unit {ColPrefix}load_unit, {ta}load_location {ColPrefix}load_location, ");
            cols.AppendLine($"{ta}block {ColPrefix}block ");
            if(includeImage) { 
               cols.AppendLine($", {ta}hangup_image {ColPrefix}hangup_image");
               cols.AppendLine($", {ta}truck_image {ColPrefix}truck_image");
            }
            return cols.ToString();
        }
        private static string GetSelect(bool includeImage)
        {
            StringBuilder sql = new();
            sql.AppendLine("SELECT ");
            sql.AppendLine(GetColumns(null,null,includeImage));
            sql.AppendLine("FROM tolive.hangup");
            return sql.ToString();
        }


        public static int Insert(Hangup obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Insert(obj, conn);
            conn.Close();
            return recsAffected;
        }


        public static int Insert(Hangup obj, OracleConnection conn)
        {
            if (obj.Hangup_Id <= 0)
                obj.Hangup_Id = Convert.ToInt32(MOO.Data.GetNextSequence("tolive.seq_hangup"));

            StringBuilder sql = new();
            sql.AppendLine("INSERT INTO tolive.hangup(");
            sql.AppendLine("hangup_id, crusher_number, start_date, end_date, status, hangup_type, cleared_by, ");
            sql.AppendLine("hangup_comments, haul_cycle_rec_ident, truck, load_unit, load_location, block, ");
            sql.AppendLine("hangup_image)");
            sql.AppendLine("VALUES(");
            sql.AppendLine(":hangup_id, :crusher_number, :start_date, :end_date, :status, :hangup_type, ");
            sql.AppendLine(":cleared_by, :hangup_comments, :haul_cycle_rec_ident, :truck, :load_unit, ");
            sql.AppendLine(":load_location, :block, :hangup_image)");
            OracleCommand ins = new(sql.ToString(), conn);
            ins.BindByName = true;
            ins.Parameters.Add("hangup_id", obj.Hangup_Id);
            ins.Parameters.Add("crusher_number", obj.Crusher_Number);
            ins.Parameters.Add("start_date", obj.Start_Date);
            ins.Parameters.Add("end_date", obj.End_Date);
            ins.Parameters.Add("status", obj.Status);
            ins.Parameters.Add("hangup_type", obj.Hangup_Type.DisplayName);
            ins.Parameters.Add("cleared_by", obj.Cleared_By.DisplayName);
            ins.Parameters.Add("hangup_comments", obj.Hangup_Comments);
            ins.Parameters.Add("haul_cycle_rec_ident", obj.Haul_Cycle_Rec_Ident);
            ins.Parameters.Add("truck", obj.Truck);
            ins.Parameters.Add("load_unit", obj.Load_Unit);
            ins.Parameters.Add("load_location", obj.Load_Location);
            ins.Parameters.Add("block", obj.Block);
            ins.Parameters.Add("hangup_image", obj.Hangup_Image);
            int recsAffected = MOO.Data.ExecuteNonQuery(ins);
            return recsAffected;
        }

        public static int UpdateHangupImage(Hangup obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = UpdateHangupImage(obj, conn);
            conn.Close();
            return recsAffected;
        }
        public static int UpdateHangupImage(Hangup obj, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("UPDATE tolive.hangup SET");
            sql.AppendLine("Hangup_Image = :Hangup_Image ");
            sql.AppendLine("WHERE hangup_id = :hangup_id");
            OracleCommand upd = new(sql.ToString(), conn);
            upd.BindByName = true;
            upd.Parameters.Add("Hangup_Image", obj.Hangup_Image);
            upd.Parameters.Add("hangup_id", obj.Hangup_Id);
            int recsAffected = MOO.Data.ExecuteNonQuery(upd);
            return recsAffected;
        }

        public static int UpdateTruckImage(Hangup obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = UpdateTruckImage(obj, conn);
            conn.Close();
            return recsAffected;
        }
        public static int UpdateTruckImage(Hangup obj, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("UPDATE tolive.hangup SET");
            sql.AppendLine("Truck_Image = :Truck_Image ");
            sql.AppendLine("WHERE hangup_id = :hangup_id");
            OracleCommand upd = new(sql.ToString(), conn);
            upd.BindByName = true;
            upd.Parameters.Add("Truck_Image", obj.Truck_Image);
            upd.Parameters.Add("hangup_id", obj.Hangup_Id);
            int recsAffected = MOO.Data.ExecuteNonQuery(upd);
            return recsAffected;
        }
        public static int Update(Hangup obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Update(obj, conn);
            conn.Close();
            return recsAffected;
        }


        public static int Update(Hangup obj, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("UPDATE tolive.hangup SET");
            sql.AppendLine("crusher_number = :crusher_number, ");
            sql.AppendLine("status = :status, ");
            sql.AppendLine("hangup_type = :hangup_type, ");
            sql.AppendLine("cleared_by = :cleared_by, ");
            sql.AppendLine("hangup_comments = :hangup_comments, ");
            sql.AppendLine("haul_cycle_rec_ident = :haul_cycle_rec_ident, ");
            sql.AppendLine("truck = :truck, ");
            sql.AppendLine("load_unit = :load_unit, ");
            sql.AppendLine("load_location = :load_location, ");
            sql.AppendLine("block = :block ");
            if (obj.End_Date != null)
            {
                sql.AppendLine(",End_Date = :End_Date ");
            }
            sql.AppendLine("WHERE hangup_id = :hangup_id");
            OracleCommand upd = new(sql.ToString(), conn);
            upd.BindByName = true;
            upd.Parameters.Add("crusher_number", obj.Crusher_Number);
            upd.Parameters.Add("status", obj.Status);
            upd.Parameters.Add("hangup_type", obj.Hangup_Type.DisplayName);
            upd.Parameters.Add("cleared_by", obj.Cleared_By.DisplayName);
            upd.Parameters.Add("hangup_comments", obj.Hangup_Comments);
            upd.Parameters.Add("haul_cycle_rec_ident", obj.Haul_Cycle_Rec_Ident);
            upd.Parameters.Add("truck", obj.Truck);
            upd.Parameters.Add("load_unit", obj.Load_Unit);
            upd.Parameters.Add("load_location", obj.Load_Location);
            upd.Parameters.Add("block", obj.Block);
            if (obj.End_Date != null)
            {
                upd.Parameters.Add("End_Date", obj.End_Date);
            }
            upd.Parameters.Add("hangup_id", obj.Hangup_Id);
            int recsAffected = MOO.Data.ExecuteNonQuery(upd);
            return recsAffected;
        }


        public static int Delete(Hangup obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Delete(obj, conn);
            conn.Close();
            return recsAffected;
        }

        public static int Delete(Hangup obj, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("DELETE FROM tolive.hangup");
            sql.AppendLine("WHERE hangup_id = :hangup_id");
            OracleCommand del = new(sql.ToString(), conn);
            del.BindByName = true;
            del.Parameters.Add("hangup_id", obj.Hangup_Id);
            int recsAffected = MOO.Data.ExecuteNonQuery(del);
            return recsAffected;
        }
        public static List<Hangup> GetPending(int crusherNumber)
        {

            StringBuilder sql = new();
            sql.AppendLine("SELECT ");
            sql.AppendLine(GetColumns(null, null, true));
            sql.AppendLine("FROM tolive.hangup");
            sql.AppendLine("WHERE Crusher_Number = " + crusherNumber + " AND end_date IS NULL");

            List<Hangup> elements = new();
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
        /// Checks data in TOLIVE.Hangup table for previous days and makes sure data matches linked Wenco Haul cycle.
        /// </summary>
        /// <param name="NumDaysToCheck">How many days back to check</param>
        public static void CleanupHaulCycleTrans(int NumDaysToCheck)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(GetSelect(false));
            sql.AppendLine("WHERE start_date >= SYSDATE - " + NumDaysToCheck);


            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);
            OracleDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                while (rdr.Read())
                {
                    Hangup hangup = DataRowToObject(rdr);

                    if (hangup.Haul_Cycle_Rec_Ident != null)
                    {
                        sql.Clear();
                        sql.AppendLine("SELECT Haul_cycle_rec_ident, Load_location_sname load_location, block_sname block, truck.name truck, load_unit.name load_unit");
                        sql.AppendLine("FROM HAUL_CYCLE_TRANS hct WITH(NOLOCK)");
                        sql.AppendLine("INNER JOIN equip truck WITH(NOLOCK)");
                        sql.AppendLine("    ON hct.HAULING_UNIT_IDENT = truck.EQUIP_IDENT");
                        sql.AppendLine("INNER JOIN equip Load_unit WITH(NOLOCK)");
                        sql.AppendLine("    ON hct.LOADING_UNIT_IDENT = Load_unit.EQUIP_IDENT");
                        sql.AppendLine("WHERE Haul_cycle_rec_ident =" + hangup.Haul_Cycle_Rec_Ident);

                        DataSet dsWenco = MOO.Data.ExecuteQuery(sql.ToString(), Data.MNODatabase.MTC_Wenco);

                        DataRow wencoRow = dsWenco.Tables[0].Rows[0];
                        if (hangup.Truck != GetWencoRowVal(wencoRow, "truck") ||
                            hangup.Load_Location != GetWencoRowVal(wencoRow, "load_location") ||
                            hangup.Block != GetWencoRowVal(wencoRow, "block") ||
                            hangup.Load_Unit != GetWencoRowVal(wencoRow, "load_unit"))
                        {
                            hangup.Truck = GetWencoRowVal(wencoRow, "truck");
                            hangup.Load_Location = GetWencoRowVal(wencoRow, "load_location");
                            hangup.Block = GetWencoRowVal(wencoRow, "block");
                            hangup.Load_Unit = GetWencoRowVal(wencoRow, "load_unit");
                            Update(hangup);
                        }

                    }
                }
            }
            conn.Close();
        }

        public static string GetWencoRowVal(DataRow row, string columnName)
        {
            return row.IsNull(columnName) ? string.Empty : (string)row[columnName];
        }
        internal static Hangup DataRowToObject(DbDataReader row, string ColPrefix = "",bool includeImage=false)
        {
            Hangup RetVal = new();
            RetVal.Hangup_Id = (decimal)Util.GetRowVal(row, $"{ColPrefix}hangup_id");
            RetVal.Crusher_Number = (decimal)Util.GetRowVal(row, $"{ColPrefix}crusher_number");
            RetVal.Start_Date = (DateTime)Util.GetRowVal(row, $"{ColPrefix}start_date");
            RetVal.End_Date = (DateTime?)Util.GetRowVal(row, $"{ColPrefix}end_date");
            RetVal.Status = (string)Util.GetRowVal(row, $"{ColPrefix}status");
            RetVal.Hangup_Type = Hangup.GetHangupType((string)Util.GetRowVal(row, $"{ColPrefix}hangup_type"));
            RetVal.Cleared_By = Hangup.GetClearedBy((string)Util.GetRowVal(row, $"{ColPrefix}cleared_by"));
            RetVal.Hangup_Comments = (string)Util.GetRowVal(row, $"{ColPrefix}hangup_comments");
            RetVal.Haul_Cycle_Rec_Ident = (decimal?)Util.GetRowVal(row, $"{ColPrefix}haul_cycle_rec_ident");
            RetVal.Truck = (string)Util.GetRowVal(row, $"{ColPrefix}truck");
            RetVal.Load_Unit = (string)Util.GetRowVal(row, $"{ColPrefix}load_unit");
            RetVal.Load_Location = (string)Util.GetRowVal(row, $"{ColPrefix}load_location");
            RetVal.Block = (string)Util.GetRowVal(row, $"{ColPrefix}block");
            if(includeImage) {
                //We dont always include image in col list
                RetVal.Hangup_Image = (byte[])Util.GetRowVal(row, $"{ColPrefix}hangup_image");
                RetVal.Truck_Image = (byte[])Util.GetRowVal(row, $"{ColPrefix}truck_image");
            }
            return RetVal;
        }

    }
}
