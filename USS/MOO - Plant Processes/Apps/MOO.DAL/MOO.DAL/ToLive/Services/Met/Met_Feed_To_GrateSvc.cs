using MOO.DAL.ToLive.Models;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.ToLive.Services
{
    public static class Met_Feed_To_GrateSvc
    {
        static Met_Feed_To_GrateSvc()
        {
            Util.RegisterOracle();
        }


        public static Met_Feed_To_Grate Get(DateTime datex, byte StepNbr, Met_Material Material)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine($"WHERE datex = :datex");
            sql.AppendLine($"AND step = :StepNbr");
            sql.AppendLine($"AND material = :Material");
            sql.AppendLine("ORDER BY Datex");

            OracleDataAdapter da = new(sql.ToString(), MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            da.SelectCommand.Parameters.Add("datex", datex);
            da.SelectCommand.Parameters.Add("StepNbr", StepNbr);
            da.SelectCommand.Parameters.Add("Material", (int)Material);
            da.SelectCommand.BindByName = true;

            DataSet ds = MOO.Data.ExecuteQuery(da);
            if (ds.Tables[0].Rows.Count == 1)
                return DataRowToObject(ds.Tables[0].Rows[0]);
            else
                return null;
        }

        public static List<Met_Feed_To_Grate> GetAll(DateTime StartDate, DateTime EndDate)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine($"WHERE datex BETWEEN :StartDate AND :EndDate");

            OracleDataAdapter da = new(sql.ToString(), MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            da.SelectCommand.Parameters.Add("StartDate", StartDate);
            da.SelectCommand.Parameters.Add("EndDate", EndDate);
            da.SelectCommand.BindByName = true;

            DataSet ds = MOO.Data.ExecuteQuery(da);
            List<Met_Feed_To_Grate> elements = new();
            if (ds.Tables[0].Rows.Count > 1)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    elements.Add(DataRowToObject(dr));
                }
            }
            return elements;
        }

        /// <summary>
        /// Runs the MET_ROLL.FEED_TO_GRATE function that calculates the feed to grate
        /// </summary>
        /// <param name="FTGCalcDate"></param>
        /// <param name="StepNbr"></param>
        /// <param name="Material"></param>
        /// <returns></returns>
        public static Met_Feed_To_Grate Calculate(DateTime FTGCalcDate, byte StepNbr, Met_Material Material)
        {
            StringBuilder sql = new();
            sql.AppendLine("DECLARE");
            sql.AppendLine("    RollDate Date;");
            sql.AppendLine("    StepNbr NUMBER;");
            sql.AppendLine("    Mat NUMBER;");
            sql.AppendLine("    ftg met_feed_to_grate%rowtype;");
            sql.AppendLine("BEGIN");
            sql.AppendLine("    RollDate := :RollDate;");
            sql.AppendLine("    StepNbr := :StepNbr;");
            sql.AppendLine("    Mat := :Mat;");
            sql.AppendLine("    tolive.met_roll.feed_to_grate(RollDate, StepNbr, Mat, ftg);");
            sql.AppendLine("    open :cursor FOR SELECT RollDate datex, 2 dmy, ftg.step step, ftg.material material,");

            sql.AppendLine("        ROUND(ftg.conc_tons,4) conc_tons, ROUND(ftg.F_T_G,4) F_T_G, ROUND(ftg.factor,4) factor,");
            sql.AppendLine("        ROUND(ftg.train_h2o,4) train_h2o, ROUND(ftg.inventory,4) inventory ");

            sql.AppendLine("FROM DUAL;");
            sql.AppendLine("END;");


            OracleDataAdapter da = new(sql.ToString(), MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            da.SelectCommand.CommandType = CommandType.Text;
            da.SelectCommand.Parameters.Add("RollDate", FTGCalcDate);
            da.SelectCommand.Parameters.Add("StepNbr", StepNbr);
            da.SelectCommand.Parameters.Add("Mat", (int)Material);
            OracleParameter outTable = new OracleParameter("cursor", OracleDbType.RefCursor, ParameterDirection.Output);
            da.SelectCommand.Parameters.Add(outTable);
            DataSet ds = MOO.Data.ExecuteQuery(da);
            if (ds.Tables[0].Rows.Count > 0)
                return DataRowToObject(ds.Tables[0].Rows[0]);
            else
                return null;
        }

        internal static string GetColumns(string TableAlias = "", string ColPrefix = "")
        {
            string ta = "";
            if (!string.IsNullOrEmpty(TableAlias))
                ta = TableAlias + ".";
            StringBuilder cols = new();
            cols.AppendLine($"{ta}datex {ColPrefix}datex, {ta}step {ColPrefix}step, {ta}material {ColPrefix}material, ");
            cols.AppendLine($"{ta}dmy {ColPrefix}dmy, ROUND({ta}conc_tons,4) {ColPrefix}conc_tons, ROUND({ta}f_t_g,4) {ColPrefix}f_t_g, ");
            cols.AppendLine($"ROUND({ta}factor,6) {ColPrefix}factor, ROUND({ta}train_h2o,4) {ColPrefix}train_h2o, ");
            cols.AppendLine($"ROUND({ta}inventory,4) {ColPrefix}inventory");
            return cols.ToString();
        }
        private static string GetSelect()
        {
            StringBuilder sql = new();
            sql.AppendLine("SELECT ");
            sql.AppendLine(GetColumns());
            sql.AppendLine("FROM tolive.met_feed_to_grate");
            return sql.ToString();
        }


        public static int Insert(Met_Feed_To_Grate obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Insert(obj, conn);
            conn.Close();
            return recsAffected;
        }


        public static int Insert(Met_Feed_To_Grate obj, OracleConnection conn)
        {
            if(obj.Dmy != Met_DMY.Month)
                throw new Exception("Unable to insert Met_Feed_To_Grate.  Record must be DMY = 2");
            StringBuilder sql = new();
            sql.AppendLine("INSERT INTO tolive.Met_Feed_To_Grate(");
            sql.AppendLine("datex, step, material, dmy, conc_tons, f_t_g, factor, train_h2o, inventory)");
            sql.AppendLine("VALUES(");
            sql.AppendLine(":datex, :step, :material, :dmy, :conc_tons, :f_t_g, :factor, :train_h2o, :inventory)");
            OracleCommand ins = new(sql.ToString(), conn);
            ins.BindByName = true;
            ins.Parameters.Add("datex", obj.Datex);
            ins.Parameters.Add("step", obj.Step);
            ins.Parameters.Add("material", (int)obj.Material);
            ins.Parameters.Add("dmy", (int)obj.Dmy);
            ins.Parameters.Add("conc_tons", obj.Conc_Tons);
            ins.Parameters.Add("f_t_g", obj.F_T_G);
            ins.Parameters.Add("factor", obj.Factor);
            ins.Parameters.Add("train_h2o", obj.Train_H2o);
            ins.Parameters.Add("inventory", obj.Inventory);
            int recsAffected = MOO.Data.ExecuteNonQuery(ins);
            return recsAffected;
        }


        public static int Update(Met_Feed_To_Grate obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Update(obj, conn);
            conn.Close();
            return recsAffected;
        }


        public static int Update(Met_Feed_To_Grate obj, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("UPDATE tolive.Met_Feed_To_Grate SET");
            sql.AppendLine("conc_tons = :conc_tons, ");
            sql.AppendLine("f_t_g = :f_t_g, ");
            sql.AppendLine("factor = :factor, ");
            sql.AppendLine("train_h2o = :train_h2o, ");
            sql.AppendLine("inventory = :inventory");
            sql.AppendLine("WHERE datex = :datex");
            sql.AppendLine("AND step = :step ");
            sql.AppendLine("AND material = :material ");
            OracleCommand upd = new(sql.ToString(), conn);
            upd.BindByName = true;
            upd.Parameters.Add("conc_tons", obj.Conc_Tons);
            upd.Parameters.Add("f_t_g", obj.F_T_G);
            upd.Parameters.Add("factor", obj.Factor);
            upd.Parameters.Add("train_h2o", obj.Train_H2o);
            upd.Parameters.Add("inventory", obj.Inventory);

            upd.Parameters.Add("datex", obj.Datex);
            upd.Parameters.Add("step", obj.Step);
            upd.Parameters.Add("material", (int)obj.Material);
            int recsAffected = MOO.Data.ExecuteNonQuery(upd);
            return recsAffected;
        }


        internal static Met_Feed_To_Grate DataRowToObject(DataRow row, string ColPrefix = "")
        {
            Met_Feed_To_Grate RetVal = new();
            RetVal.Datex = row.Field<DateTime>($"{ColPrefix}datex");
            RetVal.Step = (byte)row.Field<decimal>($"{ColPrefix}step");
            RetVal.Material =(Met_Material)row.Field<decimal>($"{ColPrefix}material");
            RetVal.Dmy = (Met_DMY)row.Field<decimal>($"{ColPrefix}dmy");
            RetVal.Conc_Tons = row.Field<decimal?>($"{ColPrefix}conc_tons");
            RetVal.F_T_G = row.Field<decimal?>($"{ColPrefix}f_t_g");
            RetVal.Factor = row.Field<decimal?>($"{ColPrefix}factor");
            RetVal.Train_H2o = row.Field<decimal?>($"{ColPrefix}train_h2o");
            RetVal.Inventory = row.Field<decimal?>($"{ColPrefix}inventory");
            return RetVal;
        }

    }
}
