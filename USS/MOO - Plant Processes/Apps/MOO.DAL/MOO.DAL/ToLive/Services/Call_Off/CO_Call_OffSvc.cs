using MOO.DAL.ToLive.Models;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Common;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.ToLive.Services
{
    public static class CO_Call_OffSvc
    {
        static CO_Call_OffSvc()
        {
            Util.RegisterOracle();
        }


        public static CO_Call_Off Get(int call_off_id)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine($"WHERE ccf.call_off_id = :call_off_id");


            CO_Call_Off retVal = null;
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.Parameters.Add("call_off_id", call_off_id);
            OracleDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                rdr.Read();
                retVal = DataRowToObject(rdr, "ccf_");
                //the query will return a record for each reason so add the reason here
                retVal.Reasons.Add(CO_ReasonSvc.DataRowToObject(rdr, "cr_"));
                while (rdr.Read())
                {
                    retVal.Reasons.Add(CO_ReasonSvc.DataRowToObject(rdr, "cr_"));
                }
            }
            conn.Close();
            return retVal;
        }

        /// <summary>
        /// Gets list of call offs by entered date
        /// </summary>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <returns></returns>
        public static List<CO_Call_Off> GetByEnteredDate(DateTime StartDate, DateTime EndDate)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine("WHERE entry_date BETWEEN :StartDate AND :EndDate");

            //This line is important, this is needed to loop through the reasons as we will have one record for every reason selected
            //example one call off may have 3 reasons.  The query returns 3 records but we want to return one call off object
            sql.AppendLine("ORDER BY ccf.call_off_id");

            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.Parameters.Add("StartDate", StartDate);
            cmd.Parameters.Add("EndDate", EndDate);
            OracleDataReader rdr = cmd.ExecuteReader();
            List<CO_Call_Off> elements = ReaderToObject(rdr, "ccf_");
            conn.Close();
            return elements;
        }


        /// <summary>
        /// Gets list of call offs by entered date and filtered by department
        /// </summary>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <returns></returns>
        public static List<CO_Call_Off> GetByEnteredDate(DateTime StartDate, DateTime EndDate, IEnumerable<CO_Dept> DeptList)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine("WHERE entry_date BETWEEN :StartDate AND :EndDate");
            bool isFirstRec = true;
            foreach (CO_Dept dept in DeptList)
            {
                if (isFirstRec)
                {
                    isFirstRec = false;
                    sql.AppendLine($"AND (cd.dept_id = {dept.Dept_Id}");
                }
                else
                    sql.AppendLine($"OR cd.dept_id = {dept.Dept_Id}");
            }
            if (DeptList.Count() > 0)
                sql.AppendLine(")"); //need to end the parenthesis that started with the and part

            //This line is important, this is needed to loop through the reasons as we will have one record for every reason selected
            //example one call off may have 3 reasons.  The query returns 3 records but we want to return one call off object
            sql.AppendLine("ORDER BY ccf.call_off_id");

            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.Parameters.Add("StartDate", StartDate);
            cmd.Parameters.Add("EndDate", EndDate);



            OracleDataReader rdr = cmd.ExecuteReader();
            List<CO_Call_Off> elements = ReaderToObject(rdr, "ccf_");
            conn.Close();
            return elements;
        }

        /// <summary>
        /// Converts the reader to a list of Call offs
        /// </summary>
        /// <param name="rdr"></param>
        /// <returns></returns>
        private static List<CO_Call_Off> ReaderToObject(DbDataReader rdr, string ColPrefix = "")
        {
            List<CO_Call_Off> elements = new();
            CO_Call_Off currCallOff = null;
            int colNbr = rdr.GetOrdinal($"{ColPrefix}call_off_id");
            if (rdr.HasRows)
            {
                while (rdr.Read())
                {
                    if (currCallOff == null || currCallOff.Call_Off_Id != Convert.ToInt32(rdr[colNbr]))
                    {
                        //this is a new call off
                        currCallOff = DataRowToObject(rdr, ColPrefix);
                        elements.Add(currCallOff);
                    }
                    currCallOff.Reasons.Add(CO_ReasonSvc.DataRowToObject(rdr, "cr_"));
                }
            }
            return elements;
        }


        internal static string GetColumns(string TableAlias = "", string ColPrefix = "")
        {
            string ta = "";
            if (!string.IsNullOrEmpty(TableAlias))
                ta = TableAlias + ".";
            StringBuilder cols = new();
            cols.AppendLine($"{ta}call_off_id {ColPrefix}call_off_id, {ta}entry_date {ColPrefix}entry_date, {ta}SHIFTS_MISSED {ColPrefix}SHIFTS_MISSED,");
            cols.AppendLine($"{ta}start_date {ColPrefix}start_date, {ta}return_date {ColPrefix}return_date, ");
            cols.AppendLine($"{ta}shift1 {ColPrefix}shift1, {ta}shift2 {ColPrefix}shift2, {ta}shift3 {ColPrefix}shift3, ");
            cols.AppendLine($"{ta}extension_marker {ColPrefix}extension_marker, {ta}forced_turn {ColPrefix}forced_turn, {ta}acceptable_excuse {ColPrefix}acceptable_excuse, ");
            cols.AppendLine($"{ta}person_id {ColPrefix}person_id, {ta}dept_id {ColPrefix}dept_id, ");
            cols.AppendLine($"{ta}comments {ColPrefix}comments, {ta}manager_comments {ColPrefix}manager_comments, {ta}file_path {ColPrefix}file_path, {ta}file_desc {ColPrefix}file_desc, ");
            cols.AppendLine($"{ta}created_by {ColPrefix}created_by, {ta}esst_hrs {ColPrefix}esst_hrs");
            return cols.ToString();
        }
        private static string GetSelect()
        {
            StringBuilder sql = new();
            sql.AppendLine("SELECT ");
            sql.AppendLine(GetColumns("ccf", "ccf_") + ",");
            sql.AppendLine(PeopleSvc.GetColumns("ppl", "ppl_") + ",");
            sql.AppendLine(CO_DeptSvc.GetColumns("cd", "cd_") + ",");
            sql.AppendLine(CO_ReasonSvc.GetColumns("cr", "cr_"));
            sql.AppendLine("FROM tolive.co_call_off ccf");
            sql.AppendLine("JOIN tolive.people ppl ON ccf.person_id = ppl.person_id");
            sql.AppendLine("JOIN tolive.co_dept cd ON ccf.dept_id = cd.dept_id");
            sql.AppendLine("JOIN tolive.co_call_off_reason ccfr ON ccf.call_off_id = ccfr.call_off_id");
            sql.AppendLine("JOIN tolive.co_reason cr ON ccfr.reason_id = cr.reason_id");
            return sql.ToString();
        }


        public static int Insert(CO_Call_Off obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            var trans = conn.BeginTransaction();
            int recsAffected = Insert(obj, conn);
            trans.Commit();
            conn.Close();
            return recsAffected;
        }


        public static int Insert(CO_Call_Off obj, OracleConnection conn)
        {
            if (obj.Call_Off_Id <= 0)
                obj.Call_Off_Id = Convert.ToInt32(MOO.Data.GetNextSequence("tolive.seq_call_off"));

            StringBuilder sql = new();
            sql.AppendLine("INSERT INTO tolive.CO_Call_Off(");
            sql.AppendLine("call_off_id, entry_date, start_date, return_date, shift1, shift2, shift3, person_id, ");
            sql.AppendLine("dept_id, comments,manager_comments,file_path,file_desc,ACCEPTABLE_EXCUSE,FORCED_TURN,EXTENSION_MARKER,");
            sql.AppendLine("SHIFTS_MISSED, created_by, esst_hrs)");
            sql.AppendLine("VALUES(");
            sql.AppendLine(":call_off_id, :entry_date, :start_date, :return_date, :shift1, :shift2, :shift3, ");
            sql.AppendLine(":person_id, :dept_id, :comments,:manager_comments,:file_path,:file_desc,:ACCEPTABLE_EXCUSE,:FORCED_TURN,:EXTENSION_MARKER, ");
            sql.AppendLine(":SHIFTS_MISSED, :created_by, :esst_hrs)");
            OracleCommand ins = new(sql.ToString(), conn);
            ins.BindByName = true;
            ins.Parameters.Add("call_off_id", obj.Call_Off_Id);
            ins.Parameters.Add("entry_date", obj.Entry_Date);
            ins.Parameters.Add("start_date", obj.Start_Date);
            ins.Parameters.Add("return_date", obj.Return_Date);
            ins.Parameters.Add("shift1", obj.Shift1 ? 1 : 0);
            ins.Parameters.Add("shift2", obj.Shift2 ? 1 : 0);
            ins.Parameters.Add("shift3", obj.Shift3 ? 1 : 0);
            ins.Parameters.Add("person_id", obj.Person.Person_Id);
            ins.Parameters.Add("dept_id", obj.Dept.Dept_Id);
            ins.Parameters.Add("comments", obj.Comments);
            ins.Parameters.Add("manager_comments", obj.Manager_Comments);
            ins.Parameters.Add("file_path", obj.File_Path);
            ins.Parameters.Add("file_desc", obj.File_Desc);
            ins.Parameters.Add("ACCEPTABLE_EXCUSE", obj.Acceptable_Excuse ? 1 : 0);
            ins.Parameters.Add("FORCED_TURN", obj.Forced_Turn ? 1 : 0);
            ins.Parameters.Add("EXTENSION_MARKER", obj.Extension_Marker ? 1 : 0);
            ins.Parameters.Add("SHIFTS_MISSED", obj.Shifts_Missed);
            ins.Parameters.Add("created_by", obj.Created_By);
            ins.Parameters.Add("esst_hrs", obj.Esst_Hrs);

            int recsAffected = MOO.Data.ExecuteNonQuery(ins);

            //now insert the reasons
            sql.Clear();
            sql.AppendLine("INSERT INTO tolive.co_call_off_reason (call_off_id, reason_id)");
            sql.AppendLine("VALUES (:call_off_id, :reason_id)");
            OracleCommand insRsn = new(sql.ToString(), conn);
            insRsn.BindByName = true;
            insRsn.Parameters.Add("call_off_id", obj.Call_Off_Id);
            OracleParameter rsnIdParam = new("reason_id", OracleDbType.Int32);
            insRsn.Parameters.Add(rsnIdParam);
            foreach (CO_Reason rsn in obj.Reasons)
            {
                rsnIdParam.Value = rsn.Reason_Id;
                insRsn.ExecuteNonQuery();
            }

            return recsAffected;
        }


        public static int Update(CO_Call_Off obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            var trans = conn.BeginTransaction();
            int recsAffected = Update(obj, conn);
            trans.Commit();
            conn.Close();
            return recsAffected;
        }


        public static int Update(CO_Call_Off obj, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("UPDATE tolive.CO_Call_Off SET");
            sql.AppendLine("entry_date = :entry_date, ");
            sql.AppendLine("start_date = :start_date, ");
            sql.AppendLine("return_date = :return_date, ");
            sql.AppendLine("shift1 = :shift1, ");
            sql.AppendLine("shift2 = :shift2, ");
            sql.AppendLine("shift3 = :shift3, ");
            sql.AppendLine("person_id = :person_id, ");
            sql.AppendLine("dept_id = :dept_id, ");
            sql.AppendLine("comments = :comments, ");
            sql.AppendLine("manager_comments = :manager_comments, ");
            sql.AppendLine("file_path = :file_path, ");
            sql.AppendLine("file_desc = :file_desc, ");
            sql.AppendLine("ACCEPTABLE_EXCUSE = :ACCEPTABLE_EXCUSE, ");
            sql.AppendLine("FORCED_TURN = :FORCED_TURN, ");
            sql.AppendLine("EXTENSION_MARKER = :EXTENSION_MARKER, ");
            sql.AppendLine("SHIFTS_MISSED = :SHIFTS_MISSED, ");
            sql.AppendLine("created_by = :created_by, ");
            sql.AppendLine("esst_hrs = :esst_hrs ");
            sql.AppendLine("WHERE call_off_id = :call_off_id");
            OracleCommand upd = new(sql.ToString(), conn);
            upd.BindByName = true;
            upd.Parameters.Add("entry_date", obj.Entry_Date);
            upd.Parameters.Add("start_date", obj.Start_Date);
            upd.Parameters.Add("return_date", obj.Return_Date);
            upd.Parameters.Add("shift1", obj.Shift1 ? 1 : 0);
            upd.Parameters.Add("shift2", obj.Shift2 ? 1 : 0);
            upd.Parameters.Add("shift3", obj.Shift3 ? 1 : 0);
            upd.Parameters.Add("person_id", obj.Person.Person_Id);
            upd.Parameters.Add("dept_id", obj.Dept.Dept_Id);
            upd.Parameters.Add("comments", obj.Comments);
            upd.Parameters.Add("manager_comments", obj.Manager_Comments);
            upd.Parameters.Add("file_path", obj.File_Path);
            upd.Parameters.Add("file_desc", obj.File_Desc);
            upd.Parameters.Add("ACCEPTABLE_EXCUSE", obj.Acceptable_Excuse ? 1 : 0);
            upd.Parameters.Add("FORCED_TURN", obj.Forced_Turn ? 1 : 0);
            upd.Parameters.Add("EXTENSION_MARKER", obj.Extension_Marker ? 1 : 0);
            upd.Parameters.Add("SHIFTS_MISSED", obj.Shifts_Missed);
            upd.Parameters.Add("created_by", obj.Created_By);
            upd.Parameters.Add("esst_hrs", obj.Esst_Hrs);
            upd.Parameters.Add("call_off_id", obj.Call_Off_Id);
            int recsAffected = MOO.Data.ExecuteNonQuery(upd);




            //now insert the reasons but we must delete the old reasons first
            sql.Clear();
            OracleCommand DelRsn = new($"DELETE FROM tolive.co_call_off_reason WHERE call_off_id = {obj.Call_Off_Id}", conn);
            DelRsn.ExecuteNonQuery();
            //old reasons deleted now re-insert
            sql.Clear();
            sql.AppendLine("INSERT INTO tolive.co_call_off_reason (call_off_id, reason_id)");
            sql.AppendLine("VALUES (:call_off_id, :reason_id)");
            OracleCommand insRsn = new(sql.ToString(), conn);
            insRsn.BindByName = true;
            insRsn.Parameters.Add("call_off_id", obj.Call_Off_Id);
            OracleParameter rsnIdParam = new("reason_id", OracleDbType.Int32);
            insRsn.Parameters.Add(rsnIdParam);
            foreach (CO_Reason rsn in obj.Reasons)
            {
                rsnIdParam.Value = rsn.Reason_Id;
                insRsn.ExecuteNonQuery();
            }


            return recsAffected;
        }


        public static int Delete(CO_Call_Off obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            var trans = conn.BeginTransaction();
            int recsAffected = Delete(obj, conn);
            trans.Commit();
            conn.Close();
            return recsAffected;
        }

        public static int Delete(CO_Call_Off obj, OracleConnection conn)
        {
            //must delete from the child table first

            OracleCommand DelRsn = new($"DELETE FROM tolive.co_call_off_reason WHERE call_off_id = {obj.Call_Off_Id}", conn);
            DelRsn.ExecuteNonQuery();
            StringBuilder sql = new();
            sql.AppendLine("DELETE FROM tolive.CO_Call_Off");
            sql.AppendLine("WHERE call_off_id = :call_off_id");
            OracleCommand del = new(sql.ToString(), conn);
            del.BindByName = true;
            del.Parameters.Add("call_off_id", obj.Call_Off_Id);
            int recsAffected = MOO.Data.ExecuteNonQuery(del);
            return recsAffected;
        }


        internal static CO_Call_Off DataRowToObject(DbDataReader row, string ColPrefix = "")
        {
            CO_Call_Off RetVal = new();
            RetVal.Call_Off_Id = Convert.ToInt32(Util.GetRowVal(row, $"{ColPrefix}call_off_id"));
            RetVal.Entry_Date = (DateTime)Util.GetRowVal(row, $"{ColPrefix}entry_date");
            RetVal.Start_Date = (DateTime)Util.GetRowVal(row, $"{ColPrefix}start_date");
            RetVal.Return_Date = (DateTime?)Util.GetRowVal(row, $"{ColPrefix}return_date");
            RetVal.Shift1 = (short)Util.GetRowVal(row, $"{ColPrefix}shift1") == 1;
            RetVal.Shift2 = (short)Util.GetRowVal(row, $"{ColPrefix}shift2") == 1;
            RetVal.Shift3 = (short)Util.GetRowVal(row, $"{ColPrefix}shift3") == 1;
            RetVal.Person = PeopleSvc.DataRowToObject(row, "ppl_");
            RetVal.Dept = CO_DeptSvc.DataRowToObject(row, "cd_");
            RetVal.Comments = (string)Util.GetRowVal(row, $"{ColPrefix}comments");
            RetVal.Manager_Comments = (string)Util.GetRowVal(row, $"{ColPrefix}manager_comments");
            RetVal.File_Path = (string)Util.GetRowVal(row, $"{ColPrefix}file_path");
            RetVal.File_Desc = (string)Util.GetRowVal(row, $"{ColPrefix}file_desc");

            RetVal.Extension_Marker = (short?)Util.GetRowVal(row, $"{ColPrefix}extension_marker") == 1;
            RetVal.Forced_Turn = (short?)Util.GetRowVal(row, $"{ColPrefix}forced_turn") == 1;
            RetVal.Acceptable_Excuse = (short?)Util.GetRowVal(row, $"{ColPrefix}acceptable_excuse") == 1;
            RetVal.Shifts_Missed = Convert.ToInt32(Util.GetRowVal(row, $"{ColPrefix}shifts_missed"));
            RetVal.Created_By = (string)Util.GetRowVal(row, $"{ColPrefix}created_by");
            RetVal.Esst_Hrs = (float?)Util.GetRowVal(row, $"{ColPrefix}esst_hrs");

            return RetVal;
        }

    }
}
