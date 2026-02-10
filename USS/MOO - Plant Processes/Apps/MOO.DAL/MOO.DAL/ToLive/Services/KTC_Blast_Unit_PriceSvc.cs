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
    public static class KTC_Blast_Unit_PriceSvc
    {
        static KTC_Blast_Unit_PriceSvc() 
        {
            Util.RegisterOracle();
        }

        public static KTC_Blast_Unit_Price Get()
        {
            StringBuilder sql = new();
            sql.AppendLine("SELECT *");
            sql.AppendLine("FROM tolive.ktc_blast_unit_price");
           
            DataSet ds = MOO.Data.ExecuteQuery(sql.ToString(), Data.MNODatabase.DMART);
            if (ds.Tables[0].Rows.Count == 0)
                return null;
            else
                return DataRowToObject(ds.Tables[0].Rows[0]);
        }

        public static int Update(KTC_Blast_Unit_Price obj)
        {
            OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            conn.Open();
            try
            {
                return Update(obj, conn);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                conn.Close();
            }
        }

        public static int Update(KTC_Blast_Unit_Price obj, OracleConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("UPDATE tolive.KTC_Blast_Unit_Price SET");
            sql.AppendLine("last_updated = :last_updated, ");
            sql.AppendLine("explosive1_uc = :explosive1_uc, ");
            sql.AppendLine("explosive2_uc = :explosive2_uc, ");
            sql.AppendLine("primer_1lb_uc = :primer_1lb_uc, ");
            sql.AppendLine("a_chord_ft_uc = :a_chord_ft_uc, ");
            sql.AppendLine("ez_trunkline_40ft_uc = :ez_trunkline_40ft_uc, ");
            sql.AppendLine("ez_trunkline_60ft_uc = :ez_trunkline_60ft_uc, ");
            sql.AppendLine("primadets_30ft_uc = :primadets_30ft_uc, ");
            sql.AppendLine("primadets_40ft_uc = :primadets_40ft_uc, ");
            sql.AppendLine("primadets_50ft_uc = :primadets_50ft_uc, ");
            sql.AppendLine("caps_6ft_UC = :caps_6ft_UC, ");
            sql.AppendLine("blasting_wire_ft_uc = :blasting_wire_ft_uc, ");
            sql.AppendLine("blasters = :blasters, ");
            sql.AppendLine("forman = :forman, ");
            sql.AppendLine("engineer = :engineer, ");
            sql.AppendLine("survey = :survey, ");
            sql.AppendLine("cc = :cc, ");
            sql.AppendLine("electric_det_15_met_uc = :electric_det_15_met_uc, ");
            sql.AppendLine("electric_det_20_met_uc = :electric_det_20_met_uc, ");
            sql.AppendLine("m35_bus_line_ft_uc = :m35_bus_line_ft_uc");
            OracleCommand upd = new(sql.ToString(), conn);
            upd.BindByName = true;
            upd.Parameters.Add("last_updated", obj.Last_Updated);
            upd.Parameters.Add("explosive1_uc", obj.Explosive1_UC);
            upd.Parameters.Add("explosive2_uc", obj.Explosive2_UC);
            upd.Parameters.Add("primer_1lb_uc", obj.Primer_1lb_UC);
            upd.Parameters.Add("a_chord_ft_uc", obj.A_Chord_Ft_UC);
            upd.Parameters.Add("ez_trunkline_40ft_uc", obj.EZ_Trunkline_40ft_UC);
            upd.Parameters.Add("ez_trunkline_60ft_uc", obj.EZ_Trunkline_60ft_UC);
            upd.Parameters.Add("primadets_30ft_uc", obj.Primadets_30ft_UC);
            upd.Parameters.Add("primadets_40ft_uc", obj.Primadets_40ft_UC);
            upd.Parameters.Add("primadets_50ft_uc", obj.Primadets_50ft_UC);
            upd.Parameters.Add("caps_6ft_UC", obj.Caps_6ft_UC);
            upd.Parameters.Add("blasting_wire_ft_uc", obj.Blasting_Wire_Ft_UC);
            upd.Parameters.Add("blasters", obj.Blasters);
            upd.Parameters.Add("forman", obj.Forman);
            upd.Parameters.Add("engineer", obj.Engineer);
            upd.Parameters.Add("survey", obj.Survey);
            upd.Parameters.Add("cc", obj.CC);
            upd.Parameters.Add("electric_det_15_met_uc", obj.Electric_Det_15_Met_UC);
            upd.Parameters.Add("electric_det_20_met_uc", obj.Electric_Det_20_Met_UC);
            upd.Parameters.Add("m35_bus_line_ft_uc", obj.M35_Bus_Line_Ft_UC);

            int recsAffected = MOO.Data.ExecuteNonQuery(upd);
            return recsAffected;
        }


        private static KTC_Blast_Unit_Price DataRowToObject(DataRow row)
        {
            KTC_Blast_Unit_Price RetVal = new();
            RetVal.Last_Updated = row.Field<DateTime>("last_updated");
            RetVal.Explosive1_UC = row.Field<decimal>("explosive1_uc");
            RetVal.Explosive2_UC = row.Field<decimal>("explosive2_uc");
            RetVal.Primer_1lb_UC = row.Field<decimal>("primer_1lb_uc");
            RetVal.A_Chord_Ft_UC = row.Field<decimal>("a_chord_ft_uc");
            RetVal.EZ_Trunkline_40ft_UC = row.Field<decimal>("ez_trunkline_40ft_uc");
            RetVal.EZ_Trunkline_60ft_UC = row.Field<decimal>("ez_trunkline_60ft_uc");
            RetVal.Primadets_30ft_UC = row.Field<decimal>("primadets_30ft_uc");
            RetVal.Primadets_40ft_UC = row.Field<decimal>("primadets_40ft_uc");
            RetVal.Primadets_50ft_UC = row.Field<decimal>("primadets_50ft_uc");
            RetVal.Caps_6ft_UC = row.Field<decimal>("caps_6ft_UC");
            RetVal.Blasting_Wire_Ft_UC = row.Field<decimal>("blasting_wire_ft_uc");
            RetVal.Blasters = row.Field<string>("blasters");
            RetVal.Forman = row.Field<string>("forman");
            RetVal.Engineer = row.Field<string>("engineer");
            RetVal.Survey = row.Field<string>("survey");
            RetVal.CC = row.Field<string>("cc");
            RetVal.Electric_Det_15_Met_UC = row.Field<decimal>("electric_det_15_met_uc");
            RetVal.Electric_Det_20_Met_UC = row.Field<decimal>("electric_det_20_met_uc");
            RetVal.M35_Bus_Line_Ft_UC = row.Field<decimal>("m35_bus_line_ft_uc");
            return RetVal;
        }

    }
}
