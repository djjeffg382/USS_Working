using MOO.DAL.Core.Enums;
using MOO.DAL.Core.Models;
using MOO.DAL.ToLive.Models;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.Core.Services
{
    public static class Process_LevelSvc
    {
        static Process_LevelSvc()
        {
            Util.RegisterOracle();
        }

        public static Process_Level Get(decimal Process_Level_Id)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine($"WHERE process_level_id = :process_level_id");


            Process_Level retVal = null;
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.Parameters.Add("process_level_id", Process_Level_Id);
            OracleDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                rdr.Read();
                retVal = DataRowToObject(rdr);
            }
            conn.Close();
            return retVal;
        }

        public static List<Process_Level> GetAllWithChildren()
        {
            List<Process_Level> elements = GetAll();

            //loop through the elements list and rearrange them to parent->child relation
            for (int nIdx = elements.Count - 1; nIdx >= 0; nIdx--)
            {
                var lnk = elements[nIdx];
                if (lnk.Parent_Id.HasValue)
                {
                    //this level has a parent
                    var parent = getParent(lnk.Parent_Id.Value, elements);

                    if (parent != null)
                    {
                        parent.Children.Add(lnk);
                    }
                    elements.RemoveAt(nIdx);
                }
            }

            return elements;
        }

        internal static Process_Level getParent(decimal? id, List<Process_Level> searchList)
        {
            foreach (var link in searchList)
            {
                if (link.Process_Level_Id == id)
                {
                    return link;
                }
                if (link.Children.Count > 0)
                {
                    var ret = getParent(id, link.Children);
                    if (ret != null)
                    {
                        return ret;
                    }
                }
            }
            return null;
        }


        public static List<Process_Level> GetAll()
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());

            List<Process_Level> elements = new();
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


        internal static string GetColumns(string TableAlias = "", string ColPrefix = "")
        {
            string ta = "";
            if (!string.IsNullOrEmpty(TableAlias))
                ta = TableAlias + ".";
            StringBuilder cols = new();
            cols.AppendLine($"{ta}site {ColPrefix}site, {ta}process_level_id {ColPrefix}process_level_id, {ta}path {ColPrefix}path,");
            cols.AppendLine($"{ta}process_level_name {ColPrefix}process_level_name, {ta}abbreviation {ColPrefix}abbreviation, ");
            cols.AppendLine($"{ta}passport_id {ColPrefix}passport_id, {ta}parent_id {ColPrefix}parent_id, ");
            cols.AppendLine($"{ta}process_type_id {ColPrefix}process_type");
            return cols.ToString();
        }
        private static string GetSelect()
        {
            StringBuilder sql = new();
            sql.AppendLine("SELECT ");
            sql.AppendLine(GetColumns());
            sql.AppendLine("FROM core.process_level_v");
            return sql.ToString();
        }

        public static int Insert(Process_Level obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Insert(obj, conn);
            conn.Close();
            return recsAffected;
        }


        public static int Insert(Process_Level obj, OracleConnection conn)
        {
            if (obj.Process_Level_Id <= 0)
                obj.Process_Level_Id = Convert.ToInt32(MOO.Data.GetNextSequence("Core.seq_proc_level"));

            StringBuilder sql = new();
            sql.AppendLine("INSERT INTO Core.Process_Level(");
            sql.AppendLine("process_level_id, process_level_name, abbreviation, passport_id, parent_id, ");
            sql.AppendLine("process_type)");
            sql.AppendLine("VALUES(");
            sql.AppendLine(":process_level_id, :process_level_name, :abbreviation, :passport_id, :parent_id, ");
            sql.AppendLine(":process_type)");
            OracleCommand ins = new(sql.ToString(), conn);
            ins.BindByName = true;
            ins.Parameters.Add("process_level_id", obj.Process_Level_Id);
            ins.Parameters.Add("process_level_name", obj.Process_Level_Name);
            ins.Parameters.Add("abbreviation", obj.Abbreviation);
            ins.Parameters.Add("passport_id", obj.Passport_Id);
            ins.Parameters.Add("parent_id", obj.Parent_Id);
            ins.Parameters.Add("process_type", (int)obj.Process_Type);
            int recsAffected = MOO.Data.ExecuteNonQuery(ins);
            return recsAffected;
        }


        public static int Update(Process_Level obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Update(obj, conn);
            conn.Close();
            return recsAffected;
        }


        public static int Update(Process_Level obj, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("UPDATE Core.Process_Level SET");
            sql.AppendLine("process_level_name = :process_level_name, ");
            sql.AppendLine("abbreviation = :abbreviation, ");
            sql.AppendLine("passport_id = :passport_id, ");
            sql.AppendLine("parent_id = :parent_id, ");
            sql.AppendLine("process_type = :process_type");
            sql.AppendLine("WHERE process_level_id = :process_level_id");
            OracleCommand upd = new(sql.ToString(), conn);
            upd.BindByName = true;
            upd.Parameters.Add("process_level_name", obj.Process_Level_Name);
            upd.Parameters.Add("abbreviation", obj.Abbreviation);
            upd.Parameters.Add("passport_id", obj.Passport_Id);
            upd.Parameters.Add("parent_id", obj.Parent_Id);
            upd.Parameters.Add("process_type", (int)obj.Process_Type);
            upd.Parameters.Add("process_level_id", obj.Process_Level_Id);
            int recsAffected = MOO.Data.ExecuteNonQuery(upd);
            return recsAffected;
        }


        public static int Delete(Process_Level obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Delete(obj, conn);
            conn.Close();
            return recsAffected;
        }

        public static int Delete(Process_Level obj, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("DELETE FROM Core.Process_Level");
            sql.AppendLine("WHERE process_level_id = :process_level_id");
            OracleCommand del = new(sql.ToString(), conn);
            del.BindByName = true;
            del.Parameters.Add("process_level_id", obj.Process_Level_Id);
            int recsAffected = MOO.Data.ExecuteNonQuery(del);
            return recsAffected;
        }

        internal static Process_Level DataRowToObject(DbDataReader row, string ColPrefix = "")
        {
            Process_Level RetVal = new();
            RetVal.Process_Level_Id = (decimal)Util.GetRowVal(row, $"{ColPrefix}process_level_id");
            RetVal.Process_Level_Name = (string)Util.GetRowVal(row, $"{ColPrefix}process_level_name");
            RetVal.Abbreviation = (string)Util.GetRowVal(row, $"{ColPrefix}abbreviation");
            RetVal.Passport_Id = (string)Util.GetRowVal(row, $"{ColPrefix}passport_id");
            RetVal.Parent_Id = (decimal?)Util.GetRowVal(row, $"{ColPrefix}parent_id");
            RetVal.Site = Enum.Parse<MOO.Plant>((string)Util.GetRowVal(row, $"{ColPrefix}site"));
            RetVal.Process_Type = (Process_Type)(decimal)Util.GetRowVal(row, $"{ColPrefix}Process_Type");
            RetVal.Path = (string)Util.GetRowVal(row, $"{ColPrefix}path");
            return RetVal;
        }

    }
}
