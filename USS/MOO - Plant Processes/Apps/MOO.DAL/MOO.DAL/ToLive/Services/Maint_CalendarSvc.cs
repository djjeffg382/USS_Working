using MOO.DAL.ToLive.Enums;
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
    public static class Maint_CalendarSvc
    {

        public const string CRANE1_DBKEY = "MaintCal_Crane1";
        public const string CRANE2_DBKEY = "MaintCal_Crane2";
        public const string CRANE3_DBKEY = "MaintCal_Crane3";
        public const string CRANE4_DBKEY = "MaintCal_Crane4";
        public const string CRANE5_DBKEY = "MaintCal_Crane5";
        public const string CRANE6_DBKEY = "MaintCal_Crane6";

        static Maint_CalendarSvc()
        {
            Util.RegisterOracle();
        }
        /// <summary>
        /// converts the area enum to a friendly name
        /// </summary>
        /// <param name="Area"></param>
        /// <returns></returns>
        public static string GetAreaName(MaintCalArea Area)
        {
            switch (Area)
            {
                case MaintCalArea.SUPP:
                    return "Support";
                case MaintCalArea.MINE:
                    return "Mine";
                case MaintCalArea.CRUSH:
                    return "Crusher";
                case MaintCalArea.CONC:
                    return "Concentrator";
                case MaintCalArea.AGGL:
                    return "Agglomerator";
                case MaintCalArea.Crane_1:
                    return MOO.Data.ReadDBKey(CRANE1_DBKEY, "Crane_1");
                case MaintCalArea.Crane_2:
                    return MOO.Data.ReadDBKey(CRANE2_DBKEY, "Crane_2");
                case MaintCalArea.Crane_3:
                    return MOO.Data.ReadDBKey(CRANE3_DBKEY, "Crane_3");
                case MaintCalArea.Crane_4:
                    return MOO.Data.ReadDBKey(CRANE4_DBKEY, "Crane_4");
                case MaintCalArea.Crane_5:
                    return MOO.Data.ReadDBKey(CRANE5_DBKEY, "Crane_5");
                case MaintCalArea.Crane_6:
                    return MOO.Data.ReadDBKey(CRANE6_DBKEY, "Crane_6");
                default:
                    return Area.ToString();
            }
        }


        public static Maint_Calendar Get(int event_id)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine($"WHERE event_id = :event_id");


            Maint_Calendar retVal = null;
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.Parameters.Add("event_id", event_id);
            OracleDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                rdr.Read();
                retVal = DataRowToObject(rdr);
            }
            conn.Close();
            return retVal;
        }


        public static List<Maint_Calendar> GetAll(DateTime StartDate, DateTime EndDate)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine("WHERE StartDate BETWEEN :StartDate AND :EndDate");
            sql.AppendLine("OR EndDate BETWEEN :StartDate AND :EndDate");

            List<Maint_Calendar> elements = new();
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.BindByName = true;
            cmd.Parameters.Add("StartDate", StartDate);
            cmd.Parameters.Add("EndDate", EndDate);
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
            cols.AppendLine($"{ta}event_id {ColPrefix}event_id, {ta}title {ColPrefix}title, {ta}startdate {ColPrefix}startdate, ");
            cols.AppendLine($"{ta}enddate {ColPrefix}enddate, {ta}area {ColPrefix}area, {ta}description {ColPrefix}description, ");
            cols.AppendLine($"{ta}major {ColPrefix}major");
            return cols.ToString();
        }
        private static string GetSelect()
        {
            StringBuilder sql = new();
            sql.AppendLine("SELECT ");
            sql.AppendLine(GetColumns());
            sql.AppendLine("FROM tolive.maint_calendar");
            return sql.ToString();
        }


        public static int Insert(Maint_Calendar obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Insert(obj, conn);
            conn.Close();
            return recsAffected;
        }


        public static int Insert(Maint_Calendar obj, OracleConnection conn)
        {
            if (obj.Event_Id <= 0)
                obj.Event_Id = Convert.ToInt32(MOO.Data.GetNextSequence("tolive.SEQ_MAINT_CALENDAR"));

            StringBuilder sql = new();
            sql.AppendLine("INSERT INTO tolive.Maint_Calendar(");
            sql.AppendLine("event_id, title, startdate, enddate, area, description, major)");
            sql.AppendLine("VALUES(");
            sql.AppendLine(":event_id, :title, :startdate, :enddate, :area, :description, :major)");
            OracleCommand ins = new(sql.ToString(), conn);
            ins.BindByName = true;
            ins.Parameters.Add("event_id", obj.Event_Id);
            ins.Parameters.Add("title", obj.Title);
            ins.Parameters.Add("startdate", obj.StartDate);
            ins.Parameters.Add("enddate", obj.EndDate);
            ins.Parameters.Add("area", obj.Area.ToString());
            ins.Parameters.Add("description", obj.Description);
            ins.Parameters.Add("major", obj.Major ? 1 : 0);
            int recsAffected = MOO.Data.ExecuteNonQuery(ins);
            return recsAffected;
        }


        public static int Update(Maint_Calendar obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Update(obj, conn);
            conn.Close();
            return recsAffected;
        }


        public static int Update(Maint_Calendar obj, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("UPDATE tolive.Maint_Calendar SET");
            sql.AppendLine("title = :title, ");
            sql.AppendLine("startdate = :startdate, ");
            sql.AppendLine("enddate = :enddate, ");
            sql.AppendLine("area = :area, ");
            sql.AppendLine("description = :description, ");
            sql.AppendLine("major = :major");
            sql.AppendLine("WHERE event_id = :event_id");
            OracleCommand upd = new(sql.ToString(), conn);
            upd.BindByName = true;
            upd.Parameters.Add("title", obj.Title);
            upd.Parameters.Add("startdate", obj.StartDate);
            upd.Parameters.Add("enddate", obj.EndDate);
            upd.Parameters.Add("area", obj.Area.ToString());
            upd.Parameters.Add("description", obj.Description);
            upd.Parameters.Add("major", obj.Major ? 1 : 0);
            upd.Parameters.Add("event_id", obj.Event_Id);
            int recsAffected = MOO.Data.ExecuteNonQuery(upd);
            return recsAffected;
        }


        public static int Delete(Maint_Calendar obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Delete(obj, conn);
            conn.Close();
            return recsAffected;
        }

        public static int Delete(Maint_Calendar obj, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("DELETE FROM tolive.Maint_Calendar");
            sql.AppendLine("WHERE event_id = :event_id");
            OracleCommand del = new(sql.ToString(), conn);
            del.BindByName = true;
            del.Parameters.Add("event_id", obj.Event_Id);
            int recsAffected = MOO.Data.ExecuteNonQuery(del);
            return recsAffected;
        }


        internal static Maint_Calendar DataRowToObject(DbDataReader row, string ColPrefix = "")
        {
            Maint_Calendar RetVal = new();
            RetVal.Event_Id = (int)(decimal)Util.GetRowVal(row, $"{ColPrefix}event_id");
            RetVal.Title = (string)Util.GetRowVal(row, $"{ColPrefix}title");
            RetVal.StartDate = (DateTime?)Util.GetRowVal(row, $"{ColPrefix}startdate");
            RetVal.EndDate = (DateTime?)Util.GetRowVal(row, $"{ColPrefix}enddate");
            RetVal.Area = Enum.Parse<MaintCalArea>((string)Util.GetRowVal(row, $"{ColPrefix}area"));
            RetVal.Description = (string)Util.GetRowVal(row, $"{ColPrefix}description");
            RetVal.Major = (decimal?)Util.GetRowVal(row, $"{ColPrefix}major") == 1;
            return RetVal;
        }

    }
}
