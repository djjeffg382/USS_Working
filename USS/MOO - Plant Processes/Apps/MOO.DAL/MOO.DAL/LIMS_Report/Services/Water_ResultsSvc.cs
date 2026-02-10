using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MOO.DAL.LIMS_Report.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace MOO.DAL.LIMS_Report.Services
{
    public  class Water_ResultsSvc
    {

        static Water_ResultsSvc()
        {
            Util.RegisterSqlServer();
        }

        public static List<Water_Results> GetBySampleDate(DateTime StartDate, DateTime EndDate)
        {
            List<Water_Results> retVal = new();
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
            sql.AppendLine("FROM LIMS_Report.dbo.Water_Results");
            return sql.ToString();
        }





        /// <summary>
        /// Inserts the Water Results in the  database
        /// </summary>
        /// <param name="Water">Water Result To insert</param>
        /// <returns></returns>
        public static int Insert(Water_Results Water)
        {
            using SqlConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.LIMS_Report));
            conn.Open();

            return Insert(Water, conn);

        }

        /// <summary>
        /// Inserts the Water Result into the Drill Database
        /// </summary>
        /// <param name="Oil">Water Result To insert</param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public static int Insert(Water_Results Water, SqlConnection conn)
        {



            StringBuilder sql = new();
            sql.AppendLine("INSERT INTO LIMS_Report.dbo.Water_Results (Sample_Nbr, Status, Login_Date, sampled_date, recd_date, first_test_date, date_completed, ");
            sql.AppendLine("  sample_point, date_authorised, authoriser,");
            sql.AppendLine("alkalinity, conductivity, fluoride, chloride, nitrite, bromide, nitrate, phosphate, sulfate, dilution, fe, ca, mg, k, na, ph)");

            sql.AppendLine("Values(@Sample_Nbr, @Status, @Login_Date, @sampled_date, @recd_date, @first_test_date, @date_completed,");
            sql.AppendLine("  @sample_point, @date_authorised, @authoriser,");
            sql.AppendLine("@alkalinity, @conductivity, @fluoride, @chloride, @nitrite, @bromide, @nitrate, @phosphate, @sulfate, @dilution, @fe, @ca, @mg, @k, @na, @ph)");


            SqlCommand cmd = new(sql.ToString(), conn);

            cmd.Parameters.AddWithValue("sample_nbr", Water.Sample_Nbr);
            cmd.Parameters.Add(new SqlParameter("@status", Water.Status));
            cmd.Parameters.Add(new SqlParameter("@Login_Date", Water.Login_Date));
            cmd.Parameters.Add(new SqlParameter("@sampled_date", ((object)Water.Sampled_Date) ?? DBNull.Value));
            cmd.Parameters.Add(new SqlParameter("@recd_date", ((object)Water.Recd_Date) ?? DBNull.Value));
            cmd.Parameters.Add(new SqlParameter("@first_test_date", ((object)Water.First_Test_Date) ?? DBNull.Value));
            cmd.Parameters.Add(new SqlParameter("@date_completed", ((object)Water.Date_Completed) ?? DBNull.Value));
            cmd.Parameters.Add(new SqlParameter("@sample_point", Water.Sample_Point));
            cmd.Parameters.Add(new SqlParameter("@date_authorised", ((Object)Water.Date_Authorised) ?? DBNull.Value));
            cmd.Parameters.Add(new SqlParameter("@authoriser", Water.Authoriser));

            cmd.Parameters.Add(new SqlParameter("@Alkalinity", ((Object)Water.Alkalinity) ?? DBNull.Value));
            cmd.Parameters.Add(new SqlParameter("@Conductivity", ((Object)Water.Conductivity) ?? DBNull.Value));
            cmd.Parameters.Add(new SqlParameter("@Fluoride", ((Object)Water.Fluoride) ?? DBNull.Value));
            cmd.Parameters.Add(new SqlParameter("@Chloride", ((Object)Water.Chloride) ?? DBNull.Value));
            cmd.Parameters.Add(new SqlParameter("@Nitrite", ((Object)Water.Nitrite) ?? DBNull.Value));
            cmd.Parameters.Add(new SqlParameter("@Bromide", ((Object)Water.Bromide) ?? DBNull.Value));
            cmd.Parameters.Add(new SqlParameter("@Nitrate", ((Object)Water.Nitrate) ?? DBNull.Value));
            cmd.Parameters.Add(new SqlParameter("@Phosphate", ((Object)Water.Phosphate) ?? DBNull.Value));
            cmd.Parameters.Add(new SqlParameter("@Sulfate", ((Object)Water.Sulfate) ?? DBNull.Value));
            cmd.Parameters.Add(new SqlParameter("@Dilution", ((Object)Water.Dilution) ?? DBNull.Value));
            cmd.Parameters.Add(new SqlParameter("@Fe", ((Object)Water.Fe) ?? DBNull.Value));
            cmd.Parameters.Add(new SqlParameter("@Ca", ((Object)Water.Ca) ?? DBNull.Value));
            cmd.Parameters.Add(new SqlParameter("@Mg", ((Object)Water.Mg) ?? DBNull.Value));
            cmd.Parameters.Add(new SqlParameter("@K", ((Object)Water.K) ?? DBNull.Value));
            cmd.Parameters.Add(new SqlParameter("@Na", ((Object)Water.Na) ?? DBNull.Value));
            cmd.Parameters.Add(new SqlParameter("@Ph", ((Object)Water.Ph) ?? DBNull.Value));



            return cmd.ExecuteNonQuery();

        }

        /// <summary>
        /// Updates the Water Results database
        /// </summary>
        /// <param name="Water">Water Result To update</param>
        /// <returns></returns>
        public static int Update(Water_Results Water)
        {
            using SqlConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.LIMS_Report));
            conn.Open();

            return Update(Water, conn);

        }

        /// <summary>
        /// Updates the Oil Result database
        /// </summary>
        /// <param name="Oil">Water Result To update</param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public static int Update(Water_Results Water, SqlConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("UPDATE LIMS_Report.dbo.Water_Results SET status = @status, Login_Date = @Login_date, sampled_date = @sampled_date, ");
            sql.AppendLine("    recd_date = @recd_date, first_test_date = @first_test_date, date_completed = @date_completed,");
            sql.AppendLine("    sample_point = @sample_point, ");
            sql.AppendLine("    date_authorised = @date_authorised, authoriser = @authoriser,");
            sql.AppendLine("    Alkalinity = @Alkalinity, Conductivity = @Conductivity,");
            sql.AppendLine("    Fluoride = @Fluoride, Chloride = @Chloride,");
            sql.AppendLine("    Nitrite = @Nitrite, Bromide = @Bromide,");
            sql.AppendLine("    Sulfate = @Sulfate, Dilution = @Dilution,");
            sql.AppendLine("    Fe = @Fe, Ca = @Ca,");
            sql.AppendLine("    Mg = @Mg, K = @K,");
            sql.AppendLine("    Na = @Na, Ph = @Ph");

            sql.AppendLine("WHERE sample_nbr = @sample_nbr");

            SqlCommand cmd = new(sql.ToString(), conn);

            cmd.Parameters.Add(new SqlParameter("@status", Water.Status));
            cmd.Parameters.Add(new SqlParameter("@Login_Date", Water.Login_Date));
            cmd.Parameters.Add(new SqlParameter("@sampled_date", ((object)Water.Sampled_Date) ?? DBNull.Value));
            cmd.Parameters.Add(new SqlParameter("@recd_date", ((object)Water.Recd_Date) ?? DBNull.Value));
            cmd.Parameters.Add(new SqlParameter("@first_test_date", ((object)Water.First_Test_Date) ?? DBNull.Value));
            cmd.Parameters.Add(new SqlParameter("@date_completed", ((object)Water.Date_Completed) ?? DBNull.Value));
            cmd.Parameters.Add(new SqlParameter("@sample_point", Water.Sample_Point));
            cmd.Parameters.Add(new SqlParameter("@date_authorised", ((Object)Water.Date_Authorised) ?? DBNull.Value));
            cmd.Parameters.Add(new SqlParameter("@authoriser", Water.Authoriser));

            cmd.Parameters.Add(new SqlParameter("@Alkalinity", ((Object)Water.Alkalinity) ?? DBNull.Value));
            cmd.Parameters.Add(new SqlParameter("@Conductivity", ((Object)Water.Conductivity) ?? DBNull.Value));
            cmd.Parameters.Add(new SqlParameter("@Fluoride", ((Object)Water.Fluoride) ?? DBNull.Value));
            cmd.Parameters.Add(new SqlParameter("@Chloride", ((Object)Water.Chloride) ?? DBNull.Value));
            cmd.Parameters.Add(new SqlParameter("@Nitrite", ((Object)Water.Nitrite) ?? DBNull.Value));
            cmd.Parameters.Add(new SqlParameter("@Bromide", ((Object)Water.Bromide) ?? DBNull.Value));
            cmd.Parameters.Add(new SqlParameter("@Nitrate", ((Object)Water.Nitrate) ?? DBNull.Value));
            cmd.Parameters.Add(new SqlParameter("@Phosphate", ((Object)Water.Phosphate) ?? DBNull.Value));
            cmd.Parameters.Add(new SqlParameter("@Sulfate", ((Object)Water.Sulfate) ?? DBNull.Value));
            cmd.Parameters.Add(new SqlParameter("@Dilution", ((Object)Water.Dilution) ?? DBNull.Value));
            cmd.Parameters.Add(new SqlParameter("@Fe", ((Object)Water.Fe) ?? DBNull.Value));
            cmd.Parameters.Add(new SqlParameter("@Ca", ((Object)Water.Ca) ?? DBNull.Value));
            cmd.Parameters.Add(new SqlParameter("@Mg", ((Object)Water.Mg) ?? DBNull.Value));
            cmd.Parameters.Add(new SqlParameter("@K", ((Object)Water.K) ?? DBNull.Value));
            cmd.Parameters.Add(new SqlParameter("@Na", ((Object)Water.Na) ?? DBNull.Value));
            cmd.Parameters.Add(new SqlParameter("@Ph", ((Object)Water.Ph) ?? DBNull.Value));
            cmd.Parameters.AddWithValue("sample_nbr", Water.Sample_Nbr);


            return cmd.ExecuteNonQuery();

        }


        internal static Water_Results DataRowToObject(DataRow row)
        {
            Water_Results retVal = new();
            retVal.Sample_Nbr = row.Field<long>("Sample_Nbr");
            retVal.Status = row.Field<string>("status");
            retVal.Login_Date = row.Field<DateTime>("Login_Date");
            retVal.Sampled_Date = row.Field<DateTime?>("sampled_date");
            retVal.Recd_Date = row.Field<DateTime?>("recd_date");
            retVal.First_Test_Date = row.Field<DateTime?>("first_test_date");
            retVal.Date_Completed = row.Field<DateTime?>("date_completed");
            retVal.Sample_Point = row.Field<string>("sample_point");
            retVal.Date_Authorised = row.Field<DateTime?>("date_authorised");
            retVal.Authoriser = row.Field<string>("authoriser");

            retVal.Alkalinity = ParseVal(row.Field<string>("alkalinity"));
            retVal.Conductivity = ParseVal(row.Field<string>("Conductivity"));
            retVal.Fluoride = ParseVal(row.Field<string>("Fluoride"));
            retVal.Chloride = ParseVal(row.Field<string>("Chloride"));
            retVal.Nitrite = ParseVal(row.Field<string>("Nitrite"));
            retVal.Bromide = ParseVal(row.Field<string>("Bromide"));
            retVal.Nitrate = ParseVal(row.Field<string>("Nitrate"));
            retVal.Phosphate = ParseVal(row.Field<string>("Phosphate"));
            retVal.Sulfate = ParseVal(row.Field<string>("Sulfate"));
            retVal.Dilution = ParseVal(row.Field<string>("Dilution"));
            retVal.Fe = ParseVal(row.Field<string>("Fe"));
            retVal.Ca = ParseVal(row.Field<string>("Ca"));
            retVal.Mg = ParseVal(row.Field<string>("Mg"));
            retVal.K = ParseVal(row.Field<string>("K"));
            retVal.Na = ParseVal(row.Field<string>("Na"));
            retVal.Ph = ParseVal(row.Field<string>("Ph"));

            return retVal;
        }

        private static double? ParseVal(string val)
        {
            if (double.TryParse(val,out double result))
                return result;
            else
                return null;
        }
        
    }
}
