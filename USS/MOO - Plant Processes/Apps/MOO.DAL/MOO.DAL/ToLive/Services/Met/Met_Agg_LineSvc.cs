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
    public static class Met_Agg_LineSvc
    {
        static Met_Agg_LineSvc()
        {
            Util.RegisterOracle();
        }


        /// <summary>
        /// Gets the Met_Conc_Line records between the date range
        /// </summary>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <returns></returns>
        public static List<Met_Agg_Line> Get(DateTime StartDate, DateTime EndDate, byte StartLineNbr, byte EndLineNbr, Met_Material Material, Met_DMY Dmy = Met_DMY.Day)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine($"WHERE datex BETWEEN :StartDate AND :EndDate");
            sql.AppendLine("AND DMY = :Dmy");
            sql.AppendLine("AND line BETWEEN :StartLine AND :EndLine");
            sql.AppendLine("AND Material = :Material");
            sql.AppendLine("ORDER BY Datex");

            OracleDataAdapter da = new(sql.ToString(), MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            da.SelectCommand.Parameters.Add("StartDate", StartDate);
            da.SelectCommand.Parameters.Add("EndDate", EndDate);
            da.SelectCommand.Parameters.Add("Dmy", (short)Dmy);
            da.SelectCommand.Parameters.Add("StartLine", StartLineNbr);
            da.SelectCommand.Parameters.Add("EndLine", EndLineNbr);
            da.SelectCommand.Parameters.Add("Material", (int)Material);

            DataSet ds = MOO.Data.ExecuteQuery(da);

            List<Met_Agg_Line> retVal = new();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                retVal.Add(DataRowToObject(dr));
            }

            return retVal;
        }

        /// <summary>
        /// Gets the month summary met data for list of line numbers
        /// </summary>
        /// <param name="RollDate"></param>
        /// <param name="StartLineNbr"></param>
        /// <param name="EndLineNbr"></param>
        /// <returns></returns>
        public static List<Met_Agg_Line> GetMonthSummary(DateTime RollDate, byte StartLineNbr, byte EndLineNbr, Met_Material Material)
        {
            //first get the data from the Met_Agg_Line table.
            List<Met_Agg_Line> fromMetTable = Get(RollDate,RollDate,StartLineNbr,EndLineNbr,Material,Met_DMY.Month);


            List<Met_Agg_Line> retVal = new();



            //limit for loop to only be 3 -7 if start and line number are outside that range
            for (byte i = Math.Max(StartLineNbr, (byte)3); i <= Math.Min(EndLineNbr, (byte)7); i++)
            {
                decimal? pelTons = null, gbTons = null;
                //check if we have this line already if so copy off the gbtons and peltons.  We do this because the met_roll.met_agg_line_m procedure
                //copies the gbtons_adj to gbtons and also copies the pel_ton_adj to peltons
                var row = fromMetTable.FirstOrDefault(x => x.Line == i);
                if(row != null)
                {
                    pelTons = row.PelTons;
                    gbTons = row.GbTons;
                }

                var newRow = GetMonthSummary(RollDate, i, Material);
                //now see if we need to change peltons and gbtons back, we do this because we don't want to modify the original peltons/gbtons. 
                //I am not sure why the procedure copies the adjusted column to the non-adjusted, maybe because this was mostly used for reporting
                if(row != null)
                {
                    newRow.PelTons = pelTons;
                    newRow.GbTons = gbTons;
                }



                retVal.Add(newRow);
            
            }
            return retVal;
        }


        /// <summary>
        /// Gets the Month summary met data  
        /// </summary>
        /// <param name="RollDate"></param>
        /// <param name="LineNumber"></param>
        /// <returns></returns>
        public static Met_Agg_Line GetMonthSummary(DateTime RollDate, byte LineNumber, Met_Material Material)
        {

            StringBuilder sql = new();
            sql.AppendLine("DECLARE");
            sql.AppendLine("    RollDate Date;");
            sql.AppendLine("    Line Number;");
            sql.AppendLine("    ptype Number;");
            sql.AppendLine("    mal met_agg_line%rowtype;");
            sql.AppendLine("BEGIN");
            sql.AppendLine("    RollDate := :RollDate;");
            sql.AppendLine("    Line := :LineNumber;");
            sql.AppendLine("    ptype := :PelletType;");
            sql.AppendLine("    tolive.met_roll.met_agg_line_m(RollDate, Line, ptype, mal);");
            sql.AppendLine("    open :cursor FOR SELECT RollDate datex, Line line, 2 dmy, ptype material,");
            sql.AppendLine("            ROUND(mal.gbtons,4) gbtons, ROUND(mal.peltons,4) peltons, ");
            sql.AppendLine("            ROUND(mal.gratehours,4) gratehours, ROUND(mal.bdhours1,4) bdhours1, ");
            sql.AppendLine("            ROUND(mal.bdhours2,4) bdhours2, ROUND(mal.bdhours3,4) bdhours3,");
            sql.AppendLine("            ROUND(mal.bdhours4,4) bdhours4, ROUND(mal.bdhours5,4) bdhours5, ROUND(mal.bentlbs,4) bentlbs, ");
            sql.AppendLine("            ROUND(mal.gasmcf,4) gasmcf, ROUND(mal.fueloilgals,4) fueloilgals, ROUND(mal.coaltons,4) coaltons, ");
            sql.AppendLine("            ROUND(mal.woodtons,4) woodtons, ROUND(mal.b58,4) b58, ROUND(mal.b916,4) b916,");
            sql.AppendLine("            ROUND(mal.b12,4) b12, ROUND(mal.b716,4) b716, ROUND(mal.b38,4) b38, ROUND(mal.b14,4) b14, ROUND(mal.b4m,4) b4m, ");
            sql.AppendLine("            ROUND(mal.b28,4) b28, ROUND(mal.bm28,4) bm28, ROUND(mal.a58,4) a58, ROUND(mal.a916, 4) a916, ROUND(mal.a12,4) a12,");
            sql.AppendLine("            ROUND(mal.a716,4) a716, ROUND(mal.a38,4) a38, ROUND(mal.a14,4) a14, ROUND(mal.a4m,4) a4m, ROUND(mal.a28,4) a28, ");
            sql.AppendLine("            ROUND(mal.am28,4) am28, ROUND(mal.comp,4) comp, ROUND(mal.comp200,4) comp200, ROUND(mal.comp300,4) comp300,");
            sql.AppendLine("            ROUND(mal.comp_std_dev,4) comp_std_dev, ROUND(mal.pelfe,4) pelfe, ROUND(mal.pelsio2,4) pelsio2, ");
            sql.AppendLine("            ROUND(mal.pelal,4) pelal, ROUND(mal.pelca,4) pelca, ROUND(mal.pelmg,4) pelmg, ");
            sql.AppendLine("            ROUND(mal.pelmn,4) pelmn, ROUND(mal.pel_ton_gas,4) pel_ton_gas, ROUND(mal.pel_ton_oil,4) pel_ton_oil, ");
            sql.AppendLine("            ROUND(mal.pel_ton_coal,4) pel_ton_coal, ROUND(mal.pel_ton_wood,4) pel_ton_wood, ROUND(mal.ltb,4) ltb, ");
            sql.AppendLine("            ROUND(mal.gbdrop,4) gbdrop, ROUND(mal.eom_lock,4) eom_lock, ");
            sql.AppendLine("            ROUND(mal.gbtons_adj,4) gbtons_adj, ROUND(mal.peltons_adj,4) peltons_adj, ROUND(mal.schedhours,4) schedhours, ");
            sql.AppendLine("            ROUND(mal.reduce,4) reduce, ROUND(mal.coal_mmbtus,4) coal_mmbtus, ROUND(mal.fueloil_mmbtus,4) fueloil_mmbtus,");
            sql.AppendLine("            ROUND(mal.gas_mmbtus,4) gas_mmbtus, ROUND(mal.wood_mmbtus,4) wood_mmbtus");
            sql.AppendLine("FROM DUAL;");
            sql.AppendLine("END;");


            OracleDataAdapter da = new(sql.ToString(), MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            da.SelectCommand.CommandType = CommandType.Text;
            da.SelectCommand.Parameters.Add("RollDate", RollDate);
            da.SelectCommand.Parameters.Add("LineNumber", LineNumber);
            da.SelectCommand.Parameters.Add("PelletType", (int)Material);
            OracleParameter outTable = new OracleParameter("cursor", OracleDbType.RefCursor, ParameterDirection.Output);
            da.SelectCommand.Parameters.Add(outTable);
            DataSet ds = MOO.Data.ExecuteQuery(da);
            if (ds.Tables[0].Rows.Count > 0)
                return DataRowToObject(ds.Tables[0].Rows[0]);
            else
                return null;
        }


        private static string GetSelect()
        {
            StringBuilder sql = new();
            sql.AppendLine("SELECT ");
            sql.AppendLine("material, line, datex, dmy, ");
            sql.AppendLine("ROUND(gbtons,4) gbtons, ROUND(peltons,4) peltons, ");
            sql.AppendLine("ROUND(gratehours,4) gratehours, ROUND(bdhours1,4) bdhours1, ");
            sql.AppendLine("ROUND(bdhours2,4) bdhours2, ROUND(bdhours3,4) bdhours3,");
            sql.AppendLine("ROUND(bdhours4,4) bdhours4, ROUND(bdhours5,4) bdhours5, ROUND(bentlbs,4) bentlbs, ");
            sql.AppendLine("ROUND(gasmcf,4) gasmcf, ROUND(fueloilgals,4) fueloilgals, ROUND(coaltons,4) coaltons, ");
            sql.AppendLine("ROUND(woodtons,4) woodtons, ROUND(b58,4) b58, ROUND(b916,4) b916,");
            sql.AppendLine("ROUND(b12,4) b12, ROUND(b716,4) b716, ROUND(b38,4) b38, ROUND(b14,4) b14, ROUND(b4m,4) b4m, ");
            sql.AppendLine("ROUND(b28,4) b28, ROUND(bm28,4) bm28, ROUND(a58,4) a58, ROUND(a916, 4) a916, ROUND(a12,4) a12,");
            sql.AppendLine("ROUND(a716,4) a716, ROUND(a38,4) a38, ROUND(a14,4) a14, ROUND(a4m,4) a4m, ROUND(a28,4) a28, ");
            sql.AppendLine("ROUND(am28,4) am28, ROUND(comp,4) comp, ROUND(comp200,4) comp200, ROUND(comp300,4) comp300,");
            sql.AppendLine("ROUND(comp_std_dev,4) comp_std_dev, ROUND(pelfe,4) pelfe, ROUND(pelsio2,4) pelsio2, ");
            sql.AppendLine("ROUND(pelal,4) pelal, ROUND(pelca,4) pelca, ROUND(pelmg,4) pelmg, ");
            sql.AppendLine("ROUND(pelmn,4) pelmn, ROUND(pel_ton_gas,4) pel_ton_gas, ROUND(pel_ton_oil,4) pel_ton_oil, ");
            sql.AppendLine("ROUND(pel_ton_coal,4) pel_ton_coal, ROUND(pel_ton_wood,4) pel_ton_wood, ROUND(ltb,4) ltb, ");
            sql.AppendLine("ROUND(gbdrop,4) gbdrop, ROUND(eom_lock,4) eom_lock, ");
            sql.AppendLine("ROUND(gbtons_adj,4) gbtons_adj, ROUND(peltons_adj,4) peltons_adj, ROUND(schedhours,4) schedhours, ");
            sql.AppendLine("ROUND(reduce,4) reduce, ROUND(coal_mmbtus,4) coal_mmbtus, ROUND(fueloil_mmbtus,4) fueloil_mmbtus,");
            sql.AppendLine("ROUND(gas_mmbtus,4) gas_mmbtus, ROUND(wood_mmbtus,4) wood_mmbtus");

            sql.AppendLine("FROM tolive.met_agg_line");
            return sql.ToString();
        }


        public static int Insert(Met_Agg_Line obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Insert(obj, conn);
            conn.Close();
            return recsAffected;
        }


        public static int Insert(Met_Agg_Line obj, OracleConnection conn)
        {


            StringBuilder sql = new();
            sql.AppendLine("INSERT INTO tolive.Met_Agg_Line(");
            sql.AppendLine("material, line, datex, dmy, gbtons, peltons, gratehours, bdhours1, bdhours2, ");
            sql.AppendLine("bdhours3, bdhours4, bdhours5, bentlbs, gasmcf, fueloilgals, coaltons, woodtons, b58, ");
            sql.AppendLine("b916, b12, b716, b38, b14, b4m, b28, bm28, a58, a916, a12, a716, a38, a14, a4m, a28, ");
            sql.AppendLine("am28, comp, comp200, comp300, comp_std_dev, pelfe, pelsio2, pelal, pelca, pelmg, ");
            sql.AppendLine("pelmn, pel_ton_gas, pel_ton_oil, pel_ton_coal, pel_ton_wood, ltb, gbdrop, eom_lock, ");
            sql.AppendLine("gbtons_adj, peltons_adj, schedhours, reduce, coal_mmbtus, fueloil_mmbtus, gas_mmbtus, ");
            sql.AppendLine("wood_mmbtus)");
            sql.AppendLine("VALUES(");
            sql.AppendLine(":material, :line, :datex, :dmy, :gbtons, :peltons, :gratehours, :bdhours1, :bdhours2, ");
            sql.AppendLine(":bdhours3, :bdhours4, :bdhours5, :bentlbs, :gasmcf, :fueloilgals, :coaltons, ");
            sql.AppendLine(":woodtons, :b58, :b916, :b12, :b716, :b38, :b14, :b4m, :b28, :bm28, :a58, :a916, :a12, ");
            sql.AppendLine(":a716, :a38, :a14, :a4m, :a28, :am28, :comp, :comp200, :comp300, :comp_std_dev, ");
            sql.AppendLine(":pelfe, :pelsio2, :pelal, :pelca, :pelmg, :pelmn, :pel_ton_gas, :pel_ton_oil, ");
            sql.AppendLine(":pel_ton_coal, :pel_ton_wood, :ltb, :gbdrop, :eom_lock, :gbtons_adj, :peltons_adj, ");
            sql.AppendLine(":schedhours, :reduce, :coal_mmbtus, :fueloil_mmbtus, :gas_mmbtus, :wood_mmbtus)");
            OracleCommand ins = new(sql.ToString(), conn);
            ins.Parameters.Add("material", (int)obj.Material);
            ins.Parameters.Add("line", obj.Line);
            ins.Parameters.Add("datex", obj.Datex);
            ins.Parameters.Add("dmy", (int)obj.Dmy);
            ins.Parameters.Add("gbtons", obj.GbTons);
            ins.Parameters.Add("peltons", obj.PelTons);
            ins.Parameters.Add("gratehours", obj.GrateHours);
            ins.Parameters.Add("bdhours1", obj.BdHours1);
            ins.Parameters.Add("bdhours2", obj.BdHours2);
            ins.Parameters.Add("bdhours3", obj.BdHours3);
            ins.Parameters.Add("bdhours4", obj.BdHours4);
            ins.Parameters.Add("bdhours5", obj.BdHours5);
            ins.Parameters.Add("bentlbs", obj.BentLbs);
            ins.Parameters.Add("gasmcf", obj.GasMCF);
            ins.Parameters.Add("fueloilgals", obj.FuelOilGals);
            ins.Parameters.Add("coaltons", obj.CoalTons);
            ins.Parameters.Add("woodtons", obj.WoodTons);
            ins.Parameters.Add("b58", obj.B58);
            ins.Parameters.Add("b916", obj.B916);
            ins.Parameters.Add("b12", obj.B12);
            ins.Parameters.Add("b716", obj.B716);
            ins.Parameters.Add("b38", obj.B38);
            ins.Parameters.Add("b14", obj.B14);
            ins.Parameters.Add("b4m", obj.B4m);
            ins.Parameters.Add("b28", obj.B28);
            ins.Parameters.Add("bm28", obj.Bm28);
            ins.Parameters.Add("a58", obj.A58);
            ins.Parameters.Add("a916", obj.A916);
            ins.Parameters.Add("a12", obj.A12);
            ins.Parameters.Add("a716", obj.A716);
            ins.Parameters.Add("a38", obj.A38);
            ins.Parameters.Add("a14", obj.A14);
            ins.Parameters.Add("a4m", obj.A4m);
            ins.Parameters.Add("a28", obj.A28);
            ins.Parameters.Add("am28", obj.Am28);
            ins.Parameters.Add("comp", obj.Comp);
            ins.Parameters.Add("comp200", obj.Comp200);
            ins.Parameters.Add("comp300", obj.Comp300);
            ins.Parameters.Add("comp_std_dev", obj.Comp_Std_Dev);
            ins.Parameters.Add("pelfe", obj.PelFe);
            ins.Parameters.Add("pelsio2", obj.PelSio2);
            ins.Parameters.Add("pelal", obj.PelAl);
            ins.Parameters.Add("pelca", obj.PelCa);
            ins.Parameters.Add("pelmg", obj.PelMg);
            ins.Parameters.Add("pelmn", obj.PelMn);
            ins.Parameters.Add("pel_ton_gas", obj.Pel_Ton_Gas);
            ins.Parameters.Add("pel_ton_oil", obj.Pel_Ton_Oil);
            ins.Parameters.Add("pel_ton_coal", obj.Pel_Ton_Coal);
            ins.Parameters.Add("pel_ton_wood", obj.Pel_Ton_Wood);
            ins.Parameters.Add("ltb", obj.Ltb);
            ins.Parameters.Add("gbdrop", obj.GbDrop);
            ins.Parameters.Add("eom_lock", obj.Eom_Lock);
            ins.Parameters.Add("gbtons_adj", obj.GbTons_Adj);
            ins.Parameters.Add("peltons_adj", obj.PelTons_Adj);
            ins.Parameters.Add("schedhours", obj.SchedHours);
            ins.Parameters.Add("reduce", obj.Reduce);
            ins.Parameters.Add("coal_mmbtus", obj.Coal_Mmbtus);
            ins.Parameters.Add("fueloil_mmbtus", obj.FuelOil_Mmbtus);
            ins.Parameters.Add("gas_mmbtus", obj.Gas_Mmbtus);
            ins.Parameters.Add("wood_mmbtus", obj.Wood_Mmbtus);
            int recsAffected = MOO.Data.ExecuteNonQuery(ins);
            return recsAffected;
        }


        public static int Update(Met_Agg_Line obj)
        {
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            int recsAffected = Update(obj, conn);
            conn.Close();
            return recsAffected;
        }


        public static int Update(Met_Agg_Line obj, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("UPDATE tolive.Met_Agg_Line SET");
            sql.AppendLine("gbtons = :gbtons, ");
            sql.AppendLine("peltons = :peltons, ");
            sql.AppendLine("gratehours = :gratehours, ");
            sql.AppendLine("bdhours1 = :bdhours1, ");
            sql.AppendLine("bdhours2 = :bdhours2, ");
            sql.AppendLine("bdhours3 = :bdhours3, ");
            sql.AppendLine("bdhours4 = :bdhours4, ");
            sql.AppendLine("bdhours5 = :bdhours5, ");
            sql.AppendLine("bentlbs = :bentlbs, ");
            sql.AppendLine("gasmcf = :gasmcf, ");
            sql.AppendLine("fueloilgals = :fueloilgals, ");
            sql.AppendLine("coaltons = :coaltons, ");
            sql.AppendLine("woodtons = :woodtons, ");
            sql.AppendLine("b58 = :b58, ");
            sql.AppendLine("b916 = :b916, ");
            sql.AppendLine("b12 = :b12, ");
            sql.AppendLine("b716 = :b716, ");
            sql.AppendLine("b38 = :b38, ");
            sql.AppendLine("b14 = :b14, ");
            sql.AppendLine("b4m = :b4m, ");
            sql.AppendLine("b28 = :b28, ");
            sql.AppendLine("bm28 = :bm28, ");
            sql.AppendLine("a58 = :a58, ");
            sql.AppendLine("a916 = :a916, ");
            sql.AppendLine("a12 = :a12, ");
            sql.AppendLine("a716 = :a716, ");
            sql.AppendLine("a38 = :a38, ");
            sql.AppendLine("a14 = :a14, ");
            sql.AppendLine("a4m = :a4m, ");
            sql.AppendLine("a28 = :a28, ");
            sql.AppendLine("am28 = :am28, ");
            sql.AppendLine("comp = :comp, ");
            sql.AppendLine("comp200 = :comp200, ");
            sql.AppendLine("comp300 = :comp300, ");
            sql.AppendLine("comp_std_dev = :comp_std_dev, ");
            sql.AppendLine("pelfe = :pelfe, ");
            sql.AppendLine("pelsio2 = :pelsio2, ");
            sql.AppendLine("pelal = :pelal, ");
            sql.AppendLine("pelca = :pelca, ");
            sql.AppendLine("pelmg = :pelmg, ");
            sql.AppendLine("pelmn = :pelmn, ");
            sql.AppendLine("pel_ton_gas = :pel_ton_gas, ");
            sql.AppendLine("pel_ton_oil = :pel_ton_oil, ");
            sql.AppendLine("pel_ton_coal = :pel_ton_coal, ");
            sql.AppendLine("pel_ton_wood = :pel_ton_wood, ");
            sql.AppendLine("ltb = :ltb, ");
            sql.AppendLine("gbdrop = :gbdrop, ");
            sql.AppendLine("eom_lock = :eom_lock, ");
            sql.AppendLine("gbtons_adj = :gbtons_adj, ");
            sql.AppendLine("peltons_adj = :peltons_adj, ");
            sql.AppendLine("schedhours = :schedhours, ");
            sql.AppendLine("reduce = :reduce, ");
            sql.AppendLine("coal_mmbtus = :coal_mmbtus, ");
            sql.AppendLine("fueloil_mmbtus = :fueloil_mmbtus, ");
            sql.AppendLine("gas_mmbtus = :gas_mmbtus, ");
            sql.AppendLine("wood_mmbtus = :wood_mmbtus");
            sql.AppendLine("WHERE datex = :datex AND line = :line AND material = :material AND dmy = :dmy");
            OracleCommand upd = new(sql.ToString(), conn);
            upd.Parameters.Add("gbtons", obj.GbTons);
            upd.Parameters.Add("peltons", obj.PelTons);
            upd.Parameters.Add("gratehours", obj.GrateHours);
            upd.Parameters.Add("bdhours1", obj.BdHours1);
            upd.Parameters.Add("bdhours2", obj.BdHours2);
            upd.Parameters.Add("bdhours3", obj.BdHours3);
            upd.Parameters.Add("bdhours4", obj.BdHours4);
            upd.Parameters.Add("bdhours5", obj.BdHours5);
            upd.Parameters.Add("bentlbs", obj.BentLbs);
            upd.Parameters.Add("gasmcf", obj.GasMCF);
            upd.Parameters.Add("fueloilgals", obj.FuelOilGals);
            upd.Parameters.Add("coaltons", obj.CoalTons);
            upd.Parameters.Add("woodtons", obj.WoodTons);
            upd.Parameters.Add("b58", obj.B58);
            upd.Parameters.Add("b916", obj.B916);
            upd.Parameters.Add("b12", obj.B12);
            upd.Parameters.Add("b716", obj.B716);
            upd.Parameters.Add("b38", obj.B38);
            upd.Parameters.Add("b14", obj.B14);
            upd.Parameters.Add("b4m", obj.B4m);
            upd.Parameters.Add("b28", obj.B28);
            upd.Parameters.Add("bm28", obj.Bm28);
            upd.Parameters.Add("a58", obj.A58);
            upd.Parameters.Add("a916", obj.A916);
            upd.Parameters.Add("a12", obj.A12);
            upd.Parameters.Add("a716", obj.A716);
            upd.Parameters.Add("a38", obj.A38);
            upd.Parameters.Add("a14", obj.A14);
            upd.Parameters.Add("a4m", obj.A4m);
            upd.Parameters.Add("a28", obj.A28);
            upd.Parameters.Add("am28", obj.Am28);
            upd.Parameters.Add("comp", obj.Comp);
            upd.Parameters.Add("comp200", obj.Comp200);
            upd.Parameters.Add("comp300", obj.Comp300);
            upd.Parameters.Add("comp_std_dev", obj.Comp_Std_Dev);
            upd.Parameters.Add("pelfe", obj.PelFe);
            upd.Parameters.Add("pelsio2", obj.PelSio2);
            upd.Parameters.Add("pelal", obj.PelAl);
            upd.Parameters.Add("pelca", obj.PelCa);
            upd.Parameters.Add("pelmg", obj.PelMg);
            upd.Parameters.Add("pelmn", obj.PelMn);
            upd.Parameters.Add("pel_ton_gas", obj.Pel_Ton_Gas);
            upd.Parameters.Add("pel_ton_oil", obj.Pel_Ton_Oil);
            upd.Parameters.Add("pel_ton_coal", obj.Pel_Ton_Coal);
            upd.Parameters.Add("pel_ton_wood", obj.Pel_Ton_Wood);
            upd.Parameters.Add("ltb", obj.Ltb);
            upd.Parameters.Add("gbdrop", obj.GbDrop);
            upd.Parameters.Add("eom_lock", obj.Eom_Lock);
            upd.Parameters.Add("gbtons_adj", obj.GbTons_Adj);
            upd.Parameters.Add("peltons_adj", obj.PelTons_Adj);
            upd.Parameters.Add("schedhours", obj.SchedHours);
            upd.Parameters.Add("reduce", obj.Reduce);
            upd.Parameters.Add("coal_mmbtus", obj.Coal_Mmbtus);
            upd.Parameters.Add("fueloil_mmbtus", obj.FuelOil_Mmbtus);
            upd.Parameters.Add("gas_mmbtus", obj.Gas_Mmbtus);
            upd.Parameters.Add("wood_mmbtus", obj.Wood_Mmbtus);

            //where clause
            upd.Parameters.Add("datex", obj.Datex);
            upd.Parameters.Add("line", obj.Line);
            upd.Parameters.Add("material", (int)obj.Material);
            upd.Parameters.Add("dmy", (int)obj.Dmy);

            int recsAffected = MOO.Data.ExecuteNonQuery(upd);
            return recsAffected;
        }


        private static Met_Agg_Line DataRowToObject(DataRow row)
        {
            Met_Agg_Line RetVal = new();
            RetVal.Material = (Met_Material)row.Field<decimal>("material");
            RetVal.Line = (byte)row.Field<decimal>("line");
            RetVal.Datex = row.Field<DateTime>("datex");
            RetVal.Dmy = (Met_DMY)row.Field<decimal>("dmy");
            RetVal.GbTons = row.Field<decimal?>("gbtons");
            RetVal.PelTons = row.Field<decimal?>("peltons");
            RetVal.GrateHours = row.Field<decimal?>("gratehours");
            RetVal.BdHours1 = row.Field<decimal?>("bdhours1");
            RetVal.BdHours2 = row.Field<decimal?>("bdhours2");
            RetVal.BdHours3 = row.Field<decimal?>("bdhours3");
            RetVal.BdHours4 = row.Field<decimal?>("bdhours4");
            RetVal.BdHours5 = row.Field<decimal?>("bdhours5");
            RetVal.BentLbs = row.Field<decimal?>("bentlbs");
            RetVal.GasMCF = row.Field<decimal?>("gasmcf");
            RetVal.FuelOilGals = row.Field<decimal?>("fueloilgals");
            RetVal.CoalTons = row.Field<decimal?>("coaltons");
            RetVal.WoodTons = row.Field<decimal?>("woodtons");
            RetVal.B58 = row.Field<decimal?>("b58");
            RetVal.B916 = row.Field<decimal?>("b916");
            RetVal.B12 = row.Field<decimal?>("b12");
            RetVal.B716 = row.Field<decimal?>("b716");
            RetVal.B38 = row.Field<decimal?>("b38");
            RetVal.B14 = row.Field<decimal?>("b14");
            RetVal.B4m = row.Field<decimal?>("b4m");
            RetVal.B28 = row.Field<decimal?>("b28");
            RetVal.Bm28 = row.Field<decimal?>("bm28");
            RetVal.A58 = row.Field<decimal?>("a58");
            RetVal.A916 = row.Field<decimal?>("a916");
            RetVal.A12 = row.Field<decimal?>("a12");
            RetVal.A716 = row.Field<decimal?>("a716");
            RetVal.A38 = row.Field<decimal?>("a38");
            RetVal.A14 = row.Field<decimal?>("a14");
            RetVal.A4m = row.Field<decimal?>("a4m");
            RetVal.A28 = row.Field<decimal?>("a28");
            RetVal.Am28 = row.Field<decimal?>("am28");
            RetVal.Comp = row.Field<decimal?>("comp");
            RetVal.Comp200 = row.Field<decimal?>("comp200");
            RetVal.Comp300 = row.Field<decimal?>("comp300");
            RetVal.Comp_Std_Dev = row.Field<decimal?>("comp_std_dev");
            RetVal.PelFe = row.Field<decimal?>("pelfe");
            RetVal.PelSio2 = row.Field<decimal?>("pelsio2");
            RetVal.PelAl = row.Field<decimal?>("pelal");
            RetVal.PelCa = row.Field<decimal?>("pelca");
            RetVal.PelMg = row.Field<decimal?>("pelmg");
            RetVal.PelMn = row.Field<decimal?>("pelmn");
            RetVal.Pel_Ton_Gas = row.Field<decimal?>("pel_ton_gas");
            RetVal.Pel_Ton_Oil = row.Field<decimal?>("pel_ton_oil");
            RetVal.Pel_Ton_Coal = row.Field<decimal?>("pel_ton_coal");
            RetVal.Pel_Ton_Wood = row.Field<decimal?>("pel_ton_wood");
            RetVal.Ltb = row.Field<decimal?>("ltb");
            RetVal.GbDrop = row.Field<decimal?>("gbdrop");
            RetVal.Eom_Lock = row.Field<decimal?>("eom_lock");
            RetVal.GbTons_Adj = row.Field<decimal?>("gbtons_adj");
            RetVal.PelTons_Adj = row.Field<decimal?>("peltons_adj");
            RetVal.SchedHours = row.Field<decimal?>("schedhours");
            RetVal.Reduce = row.Field<decimal?>("reduce");
            RetVal.Coal_Mmbtus = row.Field<decimal?>("coal_mmbtus");
            RetVal.FuelOil_Mmbtus = row.Field<decimal?>("fueloil_mmbtus");
            RetVal.Gas_Mmbtus = row.Field<decimal?>("gas_mmbtus");
            RetVal.Wood_Mmbtus = row.Field<decimal?>("wood_mmbtus");
            return RetVal;
        }

    }
}
