using MOO.DAL.ToLive.Enums;
using MOO.DAL.ToLive.Models;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Common;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.ToLive.Services
{
    public static class Lab_Chem_AnalysisSvc
    {
        static Lab_Chem_AnalysisSvc()
        {
            Util.RegisterOracle();
        }


        public static Lab_Chem_Analysis Get(int lab_chem_analysis_id)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine("WHERE lab_chem_analysis_id = :lab_chem_analysis_id");


            Lab_Chem_Analysis retVal = null;
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.BindByName = true;
            cmd.Parameters.Add("lab_chem_analysis_id", lab_chem_analysis_id);
            OracleDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                rdr.Read();
                retVal = DataRowToObject(rdr, "lca_");
            }
            conn.Close();
            return retVal;
        }


        /// <summary>
        /// Gets the Lab Chem Analysis object by Sample Manager Id number
        /// </summary>
        /// <param name="SampleMgrId"></param>
        /// <returns></returns>
        public static Lab_Chem_Analysis GetBySampleMgrId(int SampleMgrId)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine("WHERE samplemgr_id = :samplemgr_id");


            Lab_Chem_Analysis retVal = null;
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.BindByName = true;
            cmd.Parameters.Add("samplemgr_id", SampleMgrId);
            OracleDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                rdr.Read();
                retVal = DataRowToObject(rdr, "lca_");
            }
            conn.Close();
            return retVal;
        }


        public static List<Lab_Chem_Analysis> GetByShiftDate(int labChemTypeId, DateTime startDate, DateTime endDate, ShiftType shiftType,  byte? lineNbr = null )
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine("WHERE lca.Lab_chem_type_id = :labChemTypeId");
            if(shiftType == ShiftType.ShiftType8Hour)
            {
                sql.AppendLine("AND lca.shift_date8 BETWEEN :startDate AND :endDate");
            }else if(shiftType == ShiftType.ShiftType12Hour)
            {
                sql.AppendLine("AND lca.shift_date12 BETWEEN :startDate AND :endDate");
            }
            if(lineNbr != null)
                sql.AppendLine("AND lca.line_nbr = :lineNbr");


            List<Lab_Chem_Analysis> elements = [];
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.BindByName = true;
            cmd.Parameters.Add("labChemTypeId", labChemTypeId);
            cmd.Parameters.Add("startDate", startDate);
            cmd.Parameters.Add("endDate", endDate);
            if (lineNbr != null)
                cmd.Parameters.Add("lineNbr", lineNbr);

            OracleDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                while (rdr.Read())
                {
                    elements.Add(DataRowToObject(rdr, "lca_"));
                }
            }
            conn.Close();
            return elements;
        }

        public static List<Lab_Chem_Analysis> GetByAnalysisDate(int labChemTypeId, DateTime startDate, DateTime endDate, byte? lineNbr = null)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine("WHERE lca.Lab_chem_type_id = :labChemTypeId");
            sql.AppendLine("AND lca.analysis_date BETWEEN :startDate AND :endDate");
            if (lineNbr != null)
                sql.AppendLine("AND lca.line_nbr = :lineNbr");


            List<Lab_Chem_Analysis> elements = [];
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.BindByName = true;
            cmd.Parameters.Add("labChemTypeId", labChemTypeId);
            cmd.Parameters.Add("startDate", startDate);
            cmd.Parameters.Add("endDate", endDate);
            if (lineNbr != null)
                cmd.Parameters.Add("lineNbr", lineNbr);

            OracleDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                while (rdr.Read())
                {
                    elements.Add(DataRowToObject(rdr, "lca_"));
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
            cols.AppendLine($"{ta}lab_chem_analysis_id {ColPrefix}lab_chem_analysis_id, ");
            cols.AppendLine($"{ta}lab_chem_type_id {ColPrefix}lab_chem_type_id, {ta}line_nbr {ColPrefix}line_nbr, ");
            cols.AppendLine($"{ta}analysis_date {ColPrefix}analysis_date, {ta}shift_date8 {ColPrefix}shift_date8, ");
            cols.AppendLine($"{ta}Approved_By {ColPrefix}Approved_By, {ta}Approval_Date {ColPrefix}Approval_Date, ");
            cols.AppendLine($"{ta}SM_Authorized_Date {ColPrefix}SM_Authorized_Date, {ta}SampleMgr_Id {ColPrefix}SampleMgr_Id, ");
            cols.AppendLine($"{ta}shift_nbr8 {ColPrefix}shift_nbr8, {ta}shift_date12 {ColPrefix}shift_date12, ");
            cols.AppendLine($"{ta}update_date {ColPrefix}update_date, {ta}last_update_by {ColPrefix}last_update_by, ");
            cols.AppendLine($"{ta}shift_nbr12 {ColPrefix}shift_nbr12, {ta}fe {ColPrefix}fe, {ta}sio2 {ColPrefix}sio2, ");
            cols.AppendLine($"{ta}cao {ColPrefix}cao, {ta}al2o3 {ColPrefix}al2o3, {ta}mn {ColPrefix}mn, {ta}mgo {ColPrefix}mgo,");
            cols.AppendLine($"{ta}p2o5 {ColPrefix}p2o5, {ta}tio2 {ColPrefix}tio2,");
            cols.AppendLine($"{ta}recovery {ColPrefix}recovery, {ta}custom1 {ColPrefix}custom1, {ta}custom2 {ColPrefix}custom2");
            return cols.ToString();
        }
        private static string GetSelect()
        {
            StringBuilder sql = new();
            sql.AppendLine("SELECT ");
            sql.AppendLine(GetColumns("lca", "lca_") + ",");
            sql.AppendLine(Lab_Chem_TypeSvc.GetColumns("lct", "lct_"));
            sql.AppendLine("FROM tolive.lab_chem_analysis lca");
            sql.AppendLine("JOIN tolive.lab_chem_type lct ON lca.lab_chem_type_id = lct.lab_chem_type_id");
            return sql.ToString();
        }


        public static int Insert(Lab_Chem_Analysis obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Insert(obj, conn);
            conn.Close();
            return recsAffected;
        }


        public static int Insert(Lab_Chem_Analysis obj, OracleConnection conn)
        {
            if (obj.Lab_Chem_Analysis_Id <= 0)
                obj.Lab_Chem_Analysis_Id = Convert.ToInt32(MOO.Data.GetNextSequence("tolive.seq_lab_analysis"));

            StringBuilder sql = new();
            sql.AppendLine("INSERT INTO tolive.Lab_Chem_Analysis(");
            sql.AppendLine("lab_chem_analysis_id, lab_chem_type_id, line_nbr, analysis_date, approved_by, approval_date,");
            sql.AppendLine("SM_Authorized_Date, SampleMgr_Id, shift_date8, ");
            sql.AppendLine("shift_nbr8, shift_date12, shift_nbr12, update_date, last_update_by, fe, sio2, cao, al2o3, mn, mgo,");
            sql.AppendLine("p2o5, tio2, recovery, custom1, custom2)");
            sql.AppendLine("VALUES(");
            sql.AppendLine(":lab_chem_analysis_id, :lab_chem_type_id, :line_nbr, :analysis_date, :approved_by, :approval_date, ");
            sql.AppendLine(":SM_Authorized_Date, :SampleMgr_Id, :shift_date8, ");
            sql.AppendLine(":shift_nbr8, :shift_date12, :shift_nbr12, :update_date, :last_update_by, :fe, :sio2, :cao, :al2o3, :mn, :mgo,");
            sql.AppendLine(":p2o5, :tio2,:recovery, :custom1, :custom2)");
            OracleCommand ins = new(sql.ToString(), conn);
            ins.BindByName = true;
            ins.Parameters.Add("lab_chem_analysis_id", obj.Lab_Chem_Analysis_Id);
            ins.Parameters.Add("lab_chem_type_id", obj.Lab_Chem_Type.Lab_Chem_Type_Id);
            ins.Parameters.Add("line_nbr", obj.Line_Nbr);
            ins.Parameters.Add("analysis_date", obj.Analysis_Date);
            ins.Parameters.Add("approved_by", obj.Approved_By);
            ins.Parameters.Add("approval_date", obj.Approval_Date);
            ins.Parameters.Add("SM_Authorized_Date", obj.SM_Authorized_Date);
            ins.Parameters.Add("SampleMgr_Id", obj.SampleMgr_Id);
            ins.Parameters.Add("shift_date8", obj.Shift_Date8);
            ins.Parameters.Add("shift_nbr8", obj.Shift_Nbr8);
            ins.Parameters.Add("shift_date12", obj.Shift_Date12);
            ins.Parameters.Add("shift_nbr12", obj.Shift_Nbr12 );
            ins.Parameters.Add("update_date", DateTime.Now );
            ins.Parameters.Add("last_update_by", obj.Last_Update_By );
            ins.Parameters.Add("fe", obj.Fe);
            ins.Parameters.Add("sio2", obj.SiO2);
            ins.Parameters.Add("cao", obj.CaO);
            ins.Parameters.Add("al2o3", obj.Al2O3);
            ins.Parameters.Add("mn", obj.Mn);
            ins.Parameters.Add("mgo", obj.MgO);
            ins.Parameters.Add("p2o5", obj.P2O5);
            ins.Parameters.Add("tio2", obj.TiO2);
            ins.Parameters.Add("recovery", obj.Recovery);
            ins.Parameters.Add("custom1", obj.Custom1);
            ins.Parameters.Add("custom2", obj.Custom2);
            int recsAffected = MOO.Data.ExecuteNonQuery(ins);
            return recsAffected;
        }


        public static int Update(Lab_Chem_Analysis obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Update(obj, conn);
            conn.Close();
            return recsAffected;
        }


        public static int Update(Lab_Chem_Analysis obj, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("UPDATE tolive.Lab_Chem_Analysis SET");
            sql.AppendLine("lab_chem_type_id = :lab_chem_type_id, ");
            sql.AppendLine("line_nbr = :line_nbr, ");
            sql.AppendLine("analysis_date = :analysis_date, ");
            sql.AppendLine("approved_by = :approved_by, ");
            sql.AppendLine("approval_date = :approval_date, ");
            sql.AppendLine("SM_Authorized_Date = :SM_Authorized_Date, ");
            sql.AppendLine("SampleMgr_Id = :SampleMgr_Id, ");
            sql.AppendLine("shift_date8 = :shift_date8, ");
            sql.AppendLine("shift_nbr8 = :shift_nbr8, ");
            sql.AppendLine("shift_date12 = :shift_date12, ");
            sql.AppendLine("shift_nbr12 = :shift_nbr12, ");
            sql.AppendLine("update_date = :update_date, ");
            sql.AppendLine("last_update_by = :last_update_by, ");
            sql.AppendLine("fe = :fe, ");
            sql.AppendLine("sio2 = :sio2, ");
            sql.AppendLine("cao = :cao, ");
            sql.AppendLine("al2o3 = :al2o3, ");
            sql.AppendLine("mn = :mn, ");
            sql.AppendLine("mgo = :mgo,");
            sql.AppendLine("p2o5 = :p2o5,");
            sql.AppendLine("tio2 = :tio2,");
            sql.AppendLine("recovery = :recovery,");
            sql.AppendLine("custom1 = :custom1,");
            sql.AppendLine("custom2 = :custom2");
            sql.AppendLine("WHERE lab_chem_analysis_id = :lab_chem_analysis_id");
            OracleCommand upd = new(sql.ToString(), conn);
            upd.BindByName = true;
            upd.Parameters.Add("lab_chem_type_id", obj.Lab_Chem_Type.Lab_Chem_Type_Id);
            upd.Parameters.Add("line_nbr", obj.Line_Nbr);
            upd.Parameters.Add("analysis_date", obj.Analysis_Date);
            upd.Parameters.Add("approved_by", obj.Approved_By);
            upd.Parameters.Add("approval_date", obj.Approval_Date);
            upd.Parameters.Add("SM_Authorized_Date", obj.SM_Authorized_Date);
            upd.Parameters.Add("SampleMgr_Id", obj.SampleMgr_Id);
            upd.Parameters.Add("shift_date8", obj.Shift_Date8);
            upd.Parameters.Add("shift_nbr8", obj.Shift_Nbr8 );
            upd.Parameters.Add("shift_date12", obj.Shift_Date12);
            upd.Parameters.Add("shift_nbr12", obj.Shift_Nbr12);
            upd.Parameters.Add("update_date", DateTime.Now);
            upd.Parameters.Add("last_update_by", obj.Last_Update_By);
            upd.Parameters.Add("fe", obj.Fe);
            upd.Parameters.Add("sio2", obj.SiO2);
            upd.Parameters.Add("cao", obj.CaO);
            upd.Parameters.Add("al2o3", obj.Al2O3);
            upd.Parameters.Add("mn", obj.Mn);
            upd.Parameters.Add("mgo", obj.MgO);
            upd.Parameters.Add("p2o5", obj.P2O5);
            upd.Parameters.Add("tio2", obj.TiO2);
            upd.Parameters.Add("recovery", obj.Recovery);
            upd.Parameters.Add("custom1", obj.Custom1);
            upd.Parameters.Add("custom2", obj.Custom2);
            upd.Parameters.Add("lab_chem_analysis_id", obj.Lab_Chem_Analysis_Id);
            int recsAffected = MOO.Data.ExecuteNonQuery(upd);
            return recsAffected;
        }


        public static int Delete(Lab_Chem_Analysis obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Delete(obj, conn);
            conn.Close();
            return recsAffected;
        }

        public static int Delete(Lab_Chem_Analysis obj, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("DELETE FROM tolive.Lab_Chem_Analysis");
            sql.AppendLine("WHERE lab_chem_analysis_id = :lab_chem_analysis_id");
            OracleCommand del = new(sql.ToString(), conn);
            del.BindByName = true;
            del.Parameters.Add("lab_chem_analysis_id", obj.Lab_Chem_Analysis_Id);
            int recsAffected = MOO.Data.ExecuteNonQuery(del);
            return recsAffected;
        }


        internal static Lab_Chem_Analysis DataRowToObject(DbDataReader row, string ColPrefix = "")
        {
            Lab_Chem_Analysis RetVal = new();
            RetVal.Lab_Chem_Analysis_Id = (int)Util.GetRowVal(row, $"{ColPrefix}lab_chem_analysis_id");
            RetVal.Lab_Chem_Type = Lab_Chem_TypeSvc.DataRowToObject(row,"lct_");
            RetVal.Line_Nbr = (short?)Util.GetRowVal(row, $"{ColPrefix}line_nbr");
            RetVal.Analysis_Date = (DateTime)Util.GetRowVal(row, $"{ColPrefix}analysis_date");
            RetVal.Approved_By = (string)Util.GetRowVal(row, $"{ColPrefix}Approved_By");
            RetVal.Approval_Date = (DateTime?)Util.GetRowVal(row, $"{ColPrefix}Approval_Date");
            RetVal.SM_Authorized_Date = (DateTime?)Util.GetRowVal(row, $"{ColPrefix}SM_Authorized_Date");
            RetVal.SampleMgr_Id = (int?)Util.GetRowVal(row, $"{ColPrefix}SampleMgr_Id");
            RetVal.Shift_Date8 = (DateTime)Util.GetRowVal(row, $"{ColPrefix}shift_date8");
            RetVal.Shift_Nbr8 = (short)Util.GetRowVal(row, $"{ColPrefix}shift_nbr8");
            RetVal.Shift_Date12 = (DateTime)Util.GetRowVal(row, $"{ColPrefix}shift_date12");
            RetVal.Shift_Nbr12 = (short)Util.GetRowVal(row, $"{ColPrefix}shift_nbr12");
            RetVal.Update_Date = (DateTime)Util.GetRowVal(row, $"{ColPrefix}update_date");
            RetVal.Last_Update_By = (string)Util.GetRowVal(row, $"{ColPrefix}last_update_by");
            RetVal.Fe = (double?)Util.GetRowVal(row, $"{ColPrefix}fe");
            RetVal.SiO2 = (double?)Util.GetRowVal(row, $"{ColPrefix}sio2");
            RetVal.CaO = (double?)Util.GetRowVal(row, $"{ColPrefix}cao");
            RetVal.Al2O3 = (double?)Util.GetRowVal(row, $"{ColPrefix}al2o3");
            RetVal.Mn = (double?)Util.GetRowVal(row, $"{ColPrefix}mn");
            RetVal.MgO = (double?)Util.GetRowVal(row, $"{ColPrefix}mgo");
            RetVal.P2O5 = (double?)Util.GetRowVal(row, $"{ColPrefix}P2O5");
            RetVal.TiO2 = (double?)Util.GetRowVal(row, $"{ColPrefix}TiO2");
            RetVal.Recovery = (double?)Util.GetRowVal(row, $"{ColPrefix}recovery");
            RetVal.Custom1 = (string)Util.GetRowVal(row, $"{ColPrefix}custom1");
            RetVal.Custom2 = (string)Util.GetRowVal(row, $"{ColPrefix}custom2");
            return RetVal;
        }

    }
}
