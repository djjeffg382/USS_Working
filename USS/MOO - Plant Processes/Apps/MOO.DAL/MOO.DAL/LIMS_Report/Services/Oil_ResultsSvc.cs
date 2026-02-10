using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MOO.DAL.LIMS_Report.Models;
using Microsoft.Data.SqlClient;

namespace MOO.DAL.LIMS_Report.Services
{
    public static class Oil_ResultsSvc
    {
        /// <summary>
        /// list of components used for oils (will make queries easier if we use this)
        /// </summary>
        public static readonly string[] ComponentsUsed = {"Cu", "Fe", "Cr", "Al", "Si", "Pb", "Ca", "Mg", "Zn", "P",
                        "Mo", "Sn", "K", "Na", "Ni", "Ag", "ISO4", "ISO6", "ISO14", "Viscosity", "PQ", "Glycol_GC", "Diesel",
                        "Oxidation", "Soot_1980", "Soot_3800", "Water", "Glycol"};


        static Oil_ResultsSvc()
        {
            Util.RegisterSqlServer();
        }

        public static List<Oil_Results> GetBySampleDate(DateTime StartDate, DateTime EndDate)
        {
            List<Oil_Results> retVal = new();
            StringBuilder sql = new();
            sql.AppendLine(GetSelect());
            sql.AppendLine("WHERE sampled_date BETWEEN @StartDate AND @EndDate");
            SqlDataAdapter da = new(sql.ToString(), MOO.Data.GetConnectionString(Data.MNODatabase.LIMS_Report));
            da.SelectCommand.Parameters.AddWithValue("StartDate", StartDate);
            da.SelectCommand.Parameters.AddWithValue("EndDate", EndDate);

            DataSet ds = MOO.Data.ExecuteQuery(da);
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                retVal.Add(DataRowToObject(dr));
            }
            return retVal;

        }


        private static string GetSelect()
        {
            StringBuilder sql = new();
            sql.AppendLine("SELECT *");
            sql.AppendLine("FROM LIMS_Report.dbo.Oil_Results");
            return sql.ToString();
        }





        /// <summary>
        /// Inserts the Oil Results in the  database
        /// </summary>
        /// <param name="Oil">Oil Result To insert</param>
        /// <returns></returns>
        public static int Insert(Oil_Results Oil)
        {
            using SqlConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.LIMS_Report));
            conn.Open();
            
            return Insert(Oil, conn);
           
        }

        /// <summary>
        /// Inserts the Oil Result into the Drill Database
        /// </summary>
        /// <param name="Oil">Oil Result To insert</param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public static int Insert(Oil_Results Oil, SqlConnection conn)
        {

            

            StringBuilder sql = new();
            sql.AppendLine("INSERT INTO LIMS_Report.dbo.Oil_Results (Sample_Nbr, Login_Date, sampled_date, recd_date, first_test_date, date_completed, oil_type, spec, out_of_spec,");
            sql.AppendLine("  plant, area, equipment, sample_point, date_authorised, authoriser, equip_type, Lube_Pnt_Type, status, equip_ltd, component_ltd,");
            sql.AppendLine("  equip_id, component_id, unit_type, transfer_date, collected_date");
            foreach (string comp in ComponentsUsed)
            {
                sql.AppendLine($",{comp}, {comp}_OOS,{comp}_Range");
            }
            sql.AppendLine(")");
            sql.AppendLine("Values(@Sample_Nbr, @Login_Date, @sampled_date, @recd_date, @first_test_date, @date_completed, @oil_type, @spec, @out_of_spec,");
            sql.AppendLine("  @plant, @area, @equipment, @sample_point, @date_authorised, @authoriser, @equip_type, @Lube_Pnt_Type, @status, @equip_ltd, @component_ltd,");
            sql.AppendLine("  @equip_id, @component_id, @unit_type, @transfer_date, @collected_date");
            foreach (string comp in ComponentsUsed)
            {
                sql.AppendLine($",@{comp}, @{comp}_OOS,@{comp}_Range");
            }
            sql.AppendLine(")");

            SqlCommand cmd = new(sql.ToString(), conn);

            cmd.Parameters.AddWithValue("sample_nbr", Oil.Sample_Nbr);
            cmd.Parameters.Add(new SqlParameter("@Login_Date", Oil.Login_Date));
            cmd.Parameters.Add(new SqlParameter("@sampled_date", ((Object)Oil.Sampled_Date) ?? DBNull.Value));
            cmd.Parameters.Add(new SqlParameter("@recd_date", ((object)Oil.Recd_Date) ?? DBNull.Value));
            cmd.Parameters.Add(new SqlParameter("@first_test_date", ((object)Oil.First_Test_Date) ?? DBNull.Value));
            cmd.Parameters.Add(new SqlParameter("@date_completed", ((object)Oil.Date_Completed) ?? DBNull.Value));
            cmd.Parameters.Add(new SqlParameter("@oil_type", ((object)Oil.Oil_Type) ?? DBNull.Value));
            cmd.Parameters.Add(new SqlParameter("@spec", ((object)Oil.Spec) ?? DBNull.Value));
            cmd.Parameters.Add(new SqlParameter("@out_of_spec", ((object)Oil.Out_Of_Spec) ?? DBNull.Value));
            cmd.Parameters.Add(new SqlParameter("@plant", Oil.Plant.ToString()));
            cmd.Parameters.Add(new SqlParameter("@area", Oil.Area));
            cmd.Parameters.Add(new SqlParameter("@equipment", Oil.Equipment));
            cmd.Parameters.Add(new SqlParameter("@sample_point", Oil.Sample_Point));
            cmd.Parameters.Add(new SqlParameter("@date_authorised", ((Object)Oil.Date_Authorised) ?? DBNull.Value));
            cmd.Parameters.Add(new SqlParameter("@authoriser", Oil.Authoriser));
            cmd.Parameters.Add(new SqlParameter("@equip_type", ((Object)Oil.Equip_Type) ?? DBNull.Value));
            cmd.Parameters.Add(new SqlParameter("@Lube_Pnt_Type", ((Object)Oil.Lube_Pnt_Type) ?? DBNull.Value));
            cmd.Parameters.Add(new SqlParameter("@status", Oil.Status));
            cmd.Parameters.Add(new SqlParameter("@equip_ltd", Oil.Equip_LTD));
            cmd.Parameters.Add(new SqlParameter("@component_ltd", Oil.Component_LTD));
            cmd.Parameters.Add(new SqlParameter("@equip_id", ((Object)Oil.Equip_Id) ?? DBNull.Value));
            cmd.Parameters.Add(new SqlParameter("@component_id", ((Object)Oil.Component_Id) ?? DBNull.Value));
            cmd.Parameters.Add(new SqlParameter("@unit_type", ((Object)Oil.Unit_Type) ?? DBNull.Value));
            cmd.Parameters.Add(new SqlParameter("@transfer_date", ((Object)Oil.Transfer_Date) ?? DBNull.Value));
            cmd.Parameters.Add(new SqlParameter("@collected_date", ((Object)Oil.Collected_Date) ?? DBNull.Value));

            Type myType = typeof(Oil_Results);
            foreach (string comp in ComponentsUsed)
            {
                cmd.Parameters.AddWithValue(comp, myType.GetProperty(comp).GetValue(Oil) ?? "");
                cmd.Parameters.AddWithValue(comp + "_OOS", myType.GetProperty(comp + "_OOS").GetValue(Oil) ?? "");
                cmd.Parameters.AddWithValue(comp + "_Range", myType.GetProperty(comp + "_Range").GetValue(Oil) ?? "");
            }

            return cmd.ExecuteNonQuery();

        }

        /// <summary>
        /// Updates the Oil Results database
        /// </summary>
        /// <param name="Oil">Oil Result To update</param>
        /// <returns></returns>
        public static int Update(Oil_Results Oil)
        {
            using SqlConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.LIMS_Report));
            conn.Open();

            return Update(Oil, conn);

        }

        /// <summary>
        /// Updates the Oil Result database
        /// </summary>
        /// <param name="Oil">Oil Result To update</param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public static int Update(Oil_Results Oil, SqlConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("UPDATE LIMS_Report.dbo.Oil_Results SET  Login_Date = @Login_date, sampled_date = @sampled_date, ");
            sql.AppendLine("    recd_date = @recd_date, first_test_date = @first_test_date, date_completed = @date_completed,");
            sql.AppendLine("    oil_type = @oil_type, spec = @spec, out_of_spec = @out_of_spec,");
            sql.AppendLine("    plant = @plant, area = @area, equipment = @equipment, sample_point = @sample_point, ");
            sql.AppendLine("    date_authorised = @date_authorised, authoriser = @authoriser,");
            sql.AppendLine("    equip_type = @equip_type, Lube_Pnt_Type = @Lube_Pnt_Type,");
            sql.AppendLine("    status = @status, equip_ltd = @equip_ltd, component_ltd = @component_ltd,");
            sql.AppendLine("    equip_id = @equip_id, component_id = @component_id, unit_type = @unit_type,");
            sql.AppendLine("    transfer_date = @transfer_date, collected_date = @collected_date");
            foreach (string comp in ComponentsUsed)
            {
                sql.Append($"   ,{comp} = @{comp}");
                sql.Append($", {comp}_OOS = @{comp}_OOS");
                sql.AppendLine($", {comp}_Range = @{comp}_Range");
            }

            sql.AppendLine("WHERE sample_nbr = @sample_nbr");

            SqlCommand cmd = new(sql.ToString(), conn);

            cmd.Parameters.Add(new SqlParameter("@Login_Date", Oil.Login_Date));
            cmd.Parameters.Add(new SqlParameter("@sampled_date", ((Object)Oil.Sampled_Date) ?? DBNull.Value));
            cmd.Parameters.Add(new SqlParameter("@recd_date", ((Object)Oil.Recd_Date) ?? DBNull.Value));
            cmd.Parameters.Add(new SqlParameter("@first_test_date", ((Object)Oil.First_Test_Date) ?? DBNull.Value));
            cmd.Parameters.Add(new SqlParameter("@date_completed", ((Object)Oil.Date_Completed) ?? DBNull.Value));
            cmd.Parameters.Add(new SqlParameter("@oil_type", ((Object)Oil.Oil_Type) ?? DBNull.Value));
            cmd.Parameters.Add(new SqlParameter("@spec", ((Object)Oil.Spec) ?? DBNull.Value));
            cmd.Parameters.Add(new SqlParameter("@out_of_spec", ((Object)Oil.Out_Of_Spec) ?? DBNull.Value));
            cmd.Parameters.Add(new SqlParameter("@plant", Oil.Plant.ToString()));
            cmd.Parameters.Add(new SqlParameter("@area", Oil.Area));
            cmd.Parameters.Add(new SqlParameter("@equipment", Oil.Equipment));
            cmd.Parameters.Add(new SqlParameter("@sample_point", Oil.Sample_Point));
            cmd.Parameters.Add(new SqlParameter("@date_authorised", ((Object)Oil.Date_Authorised) ?? DBNull.Value));
            cmd.Parameters.Add(new SqlParameter("@authoriser", ((Object)Oil.Authoriser) ?? DBNull.Value));
            cmd.Parameters.Add(new SqlParameter("@equip_type", ((Object)Oil.Equip_Type) ?? DBNull.Value));
            cmd.Parameters.Add(new SqlParameter("@Lube_Pnt_Type", ((Object)Oil.Lube_Pnt_Type) ?? DBNull.Value));
            cmd.Parameters.Add(new SqlParameter("@status", Oil.Status));
            cmd.Parameters.Add(new SqlParameter("@equip_ltd", Oil.Equip_LTD));
            cmd.Parameters.Add(new SqlParameter("@component_ltd", Oil.Component_LTD));
            cmd.Parameters.Add(new SqlParameter("@equip_id", ((Object)Oil.Equip_Id) ?? DBNull.Value));
            cmd.Parameters.Add(new SqlParameter("@component_id", ((Object)Oil.Component_Id) ?? DBNull.Value));
            cmd.Parameters.Add(new SqlParameter("@unit_type", ((Object)Oil.Unit_Type) ?? DBNull.Value));
            cmd.Parameters.Add(new SqlParameter("@transfer_date", ((Object)Oil.Transfer_Date) ?? DBNull.Value));
            cmd.Parameters.Add(new SqlParameter("@collected_date", ((Object)Oil.Collected_Date) ?? DBNull.Value));

            Type myType = typeof(Oil_Results);
            foreach (string comp in ComponentsUsed)
            {
                cmd.Parameters.AddWithValue(comp, myType.GetProperty(comp).GetValue(Oil) ?? "");
                cmd.Parameters.AddWithValue(comp + "_OOS", myType.GetProperty(comp + "_OOS").GetValue(Oil) ?? "");
                cmd.Parameters.AddWithValue(comp + "_Range", myType.GetProperty(comp + "_Range").GetValue(Oil) ?? "");
            }
            cmd.Parameters.AddWithValue("sample_nbr", Oil.Sample_Nbr);

            return cmd.ExecuteNonQuery();

        }


        internal static Oil_Results DataRowToObject(DataRow row)
        {
            Oil_Results retVal = new();
            retVal.Sample_Nbr = row.Field<long>("Sample_Nbr");
            retVal.Status = row.Field<string>("status");
            retVal.Login_Date = row.Field<DateTime>("Login_Date");
            retVal.Sampled_Date = row.Field<DateTime?>("sampled_date");
            retVal.Recd_Date = row.Field<DateTime?>("recd_date");
            retVal.First_Test_Date = row.Field<DateTime?>("first_test_date");
            retVal.Date_Completed = row.Field<DateTime?>("date_completed");
            retVal.Oil_Type = row.Field<string>("oil_type");
            retVal.Spec = row.Field<string>("spec");
            retVal.Out_Of_Spec = row.Field<bool>("out_of_spec");
            retVal.Plant = Enum.Parse<MOO.Plant>(row.Field<string>($"plant"));
            retVal.Area = row.Field<string>("area");
            retVal.Equipment = row.Field<string>("equipment");
            retVal.Sample_Point = row.Field<string>("sample_point");
            retVal.Date_Authorised = row.Field<DateTime?>("date_authorised");
            retVal.Authoriser = row.Field<string>("authoriser");
            retVal.Equip_Type = row.Field<string>("equip_type");
            retVal.Lube_Pnt_Type = row.Field<string>("Lube_Pnt_Type");
            retVal.Equip_LTD = row.Field<decimal>("equip_ltd");
            retVal.Component_LTD = row.Field<decimal>("component_ltd");
            retVal.Equip_Id = row.Field<string>("Equip_Id");
            retVal.Component_Id = row.Field<int?>("Component_id");
            retVal.Unit_Type = row.Field<byte?>("unit_type");
            retVal.Transfer_Date = row.Field<DateTime?>("transfer_date");





            //now loop through the components, each component has value, and out_of_spec and a range column
            Type myType = typeof(Oil_Results);
            foreach (string component in ComponentsUsed)
            {
                if(double.TryParse(row.Field<string>(component), out double compValue)){
                    myType.GetProperty(component).SetValue(retVal, compValue);
                    myType.GetProperty($"{component}_OOS").SetValue(retVal, row.Field<bool?>($"{component}_oos"));
                    myType.GetProperty($"{component}_Range").SetValue(retVal, row.Field<string>($"{component}_range"));
                }
                
            }
            return retVal;
        }

    }
}
