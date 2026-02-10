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
    public static class Tire_MaintSvc
    {
        static List<People> Everyone;
        static Tire_MaintSvc()
        {
            Util.RegisterOracle();
        }


        public static Tire_Maint Get(int tire_maint_id)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine("WHERE tm.tire_maint_id = :tire_maint_id");


            Tire_Maint retVal = null;
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.BindByName = true;
            cmd.Parameters.Add("tire_maint_id", tire_maint_id);
            OracleDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                rdr.Read();
                retVal = DataRowToObject(rdr);
            }
            conn.Close();
            return retVal;
        }


        public static List<Tire_Maint> GetAll()
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());

            List<Tire_Maint> elements = [];
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.BindByName = true;
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
            cols.AppendLine($"{ta}tire_maint_id {ColPrefix}tire_maint_id, {ta}maint_date {ColPrefix}maint_date, ");
            cols.AppendLine($"{ta}plant {ColPrefix}plant, {ta}equip_id {ColPrefix}equip_id, ");
            cols.AppendLine($"{ta}workorder_nbr {ColPrefix}workorder_nbr, {ta}mems_verified {ColPrefix}mems_verified");
            return cols.ToString();
        }
        private static string GetSelect()
        {
            StringBuilder sql = new();
            sql.AppendLine("SELECT DISTINCT ");
            sql.AppendLine(GetColumns("tm","tm") + ",");
            sql.AppendLine("LISTAGG(tmm.person_id, ',') WITHIN GROUP(ORDER BY tm.tire_maint_id) OVER (PARTITION BY tmm.tire_maint_id) tiremechanics");
            sql.AppendLine("FROM tolive.tire_maint tm");
            sql.AppendLine("LEFT JOIN tolive.tire_maint_mechanics tmm");
            sql.AppendLine("ON tm.tire_maint_id = tmm.tire_maint_id");






            //sql.AppendLine("FROM tolive.tire_maint tm");
            //sql.AppendLine("LEFT JOIN (SELECT MIN(tire_maint_id) tire_maint_id, LISTAGG(person_id, ',') WITHIN GROUP(ORDER BY tire_maint_id) tireMechanics");
            //sql.AppendLine("FROM tolive.tire_maint_mechanics) tmm");
            //sql.AppendLine("    ON tm.tire_maint_id = tmm.tire_maint_id");
            return sql.ToString();
        }


        public static int Insert(Tire_Maint obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Insert(obj, conn);
            conn.Close();
            return recsAffected;
        }

        public static int InsertMechanics(Tire_Maint obj, OracleConnection conn)
        {
            int recsAffected = 0;
            foreach (var mechanic in obj.Mechanics)
            {
                StringBuilder sql = new();
                sql.AppendLine("INSERT INTO tolive.tire_maint_mechanics(");
                sql.AppendLine("tire_maint_id, person_id)");
                sql.AppendLine("VALUES(");
                sql.AppendLine(":tire_maint_id, :person_id)");
                OracleCommand ins = new(sql.ToString(), conn);
                ins.BindByName = true;
                ins.Parameters.Add("tire_maint_id", obj.Tire_Maint_Id);
                ins.Parameters.Add("person_id", mechanic.Person_Id);
                recsAffected += MOO.Data.ExecuteNonQuery(ins);
            }
            return recsAffected;
        }


        public static int Insert(Tire_Maint obj, OracleConnection conn)
        {
            if (obj.Tire_Maint_Id <= 0)
                obj.Tire_Maint_Id = Convert.ToInt32(MOO.Data.GetNextSequence("tolive.seq_tire_maint"));

            StringBuilder sql = new();
            sql.AppendLine("INSERT INTO tolive.tire_maint(");
            sql.AppendLine("tire_maint_id, maint_date, plant, equip_id, workorder_nbr, mems_verified)");
            sql.AppendLine("VALUES(");
            sql.AppendLine(":tire_maint_id, :maint_date, :plant, :equip_id, :workorder_nbr, :mems_verified)");
            OracleCommand ins = new(sql.ToString(), conn);
            ins.BindByName = true;
            ins.Parameters.Add("tire_maint_id", obj.Tire_Maint_Id);
            ins.Parameters.Add("maint_date", obj.Maint_Date);
            ins.Parameters.Add("plant", obj.Plant.ToString());
            ins.Parameters.Add("equip_id", obj.Equip_Id);
            ins.Parameters.Add("workorder_nbr", obj.Workorder_Nbr);
            ins.Parameters.Add("mems_verified", (obj.Mems_Verified ?? false) ? 1 : 0);
            int recsAffected = MOO.Data.ExecuteNonQuery(ins) + InsertMechanics(obj,conn);
            return recsAffected;
        }


        public static int Update(Tire_Maint obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Update(obj, conn);
            conn.Close();
            return recsAffected;
        }


        public static int Update(Tire_Maint obj, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("UPDATE tolive.tire_maint SET");
            sql.AppendLine("maint_date = :maint_date, ");
            sql.AppendLine("plant = :plant, ");
            sql.AppendLine("equip_id = :equip_id, ");
            sql.AppendLine("workorder_nbr = :workorder_nbr, ");
            sql.AppendLine("mems_verified = :mems_verified");
            sql.AppendLine("WHERE tire_maint_id = :tire_maint_id");
            OracleCommand upd = new(sql.ToString(), conn);
            upd.BindByName = true;
            upd.Parameters.Add("maint_date", obj.Maint_Date);
            upd.Parameters.Add("plant", obj.Plant.ToString());
            upd.Parameters.Add("equip_id", obj.Equip_Id);
            upd.Parameters.Add("workorder_nbr", obj.Workorder_Nbr);
            upd.Parameters.Add("mems_verified", (obj.Mems_Verified ?? false) ? 1 : 0);
            upd.Parameters.Add("tire_maint_id", obj.Tire_Maint_Id);
            int recsAffected = DeleteMechanics(obj,conn);
            recsAffected += InsertMechanics(obj, conn);
            recsAffected += MOO.Data.ExecuteNonQuery(upd);
            return recsAffected;
        }


        public static int Delete(Tire_Maint obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Delete(obj, conn);
            conn.Close();
            return recsAffected;
        }
        public static int DeleteMechanics(Tire_Maint obj, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("DELETE FROM tolive.tire_maint_mechanics");
            sql.AppendLine("WHERE tire_maint_id = :tire_maint_id");
            OracleCommand del = new(sql.ToString(), conn);
            del.BindByName = true;
            del.Parameters.Add("tire_maint_id", obj.Tire_Maint_Id);
            int recsAffected = MOO.Data.ExecuteNonQuery(del);
            return recsAffected;
        }


        public static int Delete(Tire_Maint obj, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("DELETE FROM tolive.tire_maint");
            sql.AppendLine("WHERE tire_maint_id = :tire_maint_id");
            OracleCommand del = new(sql.ToString(), conn);
            del.BindByName = true;
            del.Parameters.Add("tire_maint_id", obj.Tire_Maint_Id);
            int recsAffected = DeleteMechanics(obj,conn);
            recsAffected += MOO.Data.ExecuteNonQuery(del);
            return recsAffected;
        }


        internal static Tire_Maint DataRowToObject(DbDataReader row, string ColPrefix = "tm")
        {
            if (Everyone == null)
            {
                Everyone = PeopleSvc.GetAll(getOnlyActive:false);
            }
            Tire_Maint RetVal = new();
            RetVal.Tire_Maint_Id = (long)Util.GetRowVal(row, $"{ColPrefix}tire_maint_id");
            RetVal.Maint_Date = (DateTime)Util.GetRowVal(row, $"{ColPrefix}maint_date");
            RetVal.Plant = (MOO.Plant)Enum.Parse(typeof(MOO.Plant),(string)Util.GetRowVal(row, $"{ColPrefix}plant"));
            RetVal.Equip_Id = (string)Util.GetRowVal(row, $"{ColPrefix}equip_id");
            RetVal.Workorder_Nbr = (string)Util.GetRowVal(row, $"{ColPrefix}workorder_nbr");
            RetVal.Mems_Verified = (short)Util.GetRowVal(row, $"{ColPrefix}mems_verified") == 1;
            var tireMechanics = Util.GetRowVal(row, "tiremechanics");
            if (tireMechanics != null)
            {
                foreach(var mechanic in tireMechanics.ToString().Split(','))
                {
                    RetVal.Mechanics.Add(Everyone.FirstOrDefault(x => x.Person_Id == Convert.ToInt32(mechanic)));
                }
            }
            return RetVal;
        }
    }
}
