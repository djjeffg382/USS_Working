using MOO.DAL.LIMS.Models;
using OM_Lab.Data.Models;
using System.ComponentModel;
using System.Data;
using System.Text;
using Microsoft.Data.SqlClient;
using System.Security.Principal;

namespace OM_Lab.Data.Services
{

    internal static class LIMSResultHistSvc
    {

        private const short SAMPLES_BEFORE = 15;
        private const short SAMPLES_AFTER = 10;


        public static List<LIMSResultHist> Get(long SampleID, string Component)
        {
            StringBuilder sql = new();
            //first let's get some info about the sample including the date and samplepoint
            sql.AppendLine("SELECT s.login_date, s.sampling_point, sp.name");
            sql.AppendLine("FROM sample s");
            sql.AppendLine("INNER JOIN sample_point sp ON s.sampling_point = sp.[IDENTITY]");
            sql.AppendLine($"WHERE s.id_numeric = {SampleID}");


            DataSet SampleDS = MOO.Data.ExecuteQuery(sql.ToString(), MOO.Data.MNODatabase.LIMS_Read);
            DateTime SampleDate = SampleDS.Tables[0].Rows[0].Field<DateTime>("login_date");
            string SamplePoint = SampleDS.Tables[0].Rows[0].Field<string>("sampling_point")!;
            //string SamplePointName = SampleDS.Tables[0].Rows[0].Field<string>("name")!;
            List<LIMSResultHist> hist = new();
            AddSamps(true, SampleDate, SamplePoint, Component, hist, false);
            AddSamps(true, SampleDate, SamplePoint, Component, hist, true);
            AddSamps(false, SampleDate, SamplePoint, Component, hist, false);

            return hist;

        }

        /// <summary>
        /// Adds the sample history to the list provided
        /// </summary>
        /// <param name="GetPrior">Whether we are getting the samples after this date or before (true = before)</param>
        /// <param name="SampleDate">The sample date of which to get samples before/after</param>
        /// <param name="SamplePoint">Sample point</param>
        /// <param name="Component">Component name</param>
        /// <param name="Hist">List to add the samples to</param>
        /// <param name="UseCommitted">Whether to use the committed set or active set (true = use committed)</param>
        private static void AddSamps(bool GetPrior, DateTime SampleDate, string SamplePoint, string Component, List<LIMSResultHist> Hist, bool UseCommitted)
        {
            string viewName = UseCommitted? "C_SAMP_TEST_RESULT": "SAMP_TEST_RESULT";
            StringBuilder sql = new();
            sql.AppendLine("SELECT login_date, result_value FROM (");
            sql.AppendLine("                SELECT login_date, RESULT_VALUE,");
            sql.AppendLine("                	ROW_NUMBER() OVER(ORDER BY sampled_date desc) rn");
            sql.AppendLine($"                FROM {viewName}");
            sql.AppendLine("                WHERE sampling_point = @SamplePoint");
            sql.AppendLine("                AND COMPONENT_NAME = @Component");
            sql.AppendLine("                AND STATUS IN ('A', 'C', 'V')");
            if(GetPrior)
                sql.AppendLine("                AND sampled_date <= @login_date");  //Get prior will include the selected sample
            else
                sql.AppendLine("                AND sampled_date > @login_date");  //Get after will NOT include the selected sample
            sql.AppendLine(") tbl");
            if(GetPrior && !UseCommitted)
                //just set this to something high, so we get everything from the active.  
                //Active set shouldn't contain many anyway
                sql.AppendLine($"WHERE rn < 500");  //just set this to something high, so we get everything from the active.  
            else if (GetPrior)
                sql.AppendLine($"WHERE rn < {SAMPLES_BEFORE + 1}");
            else
                sql.AppendLine($"WHERE rn < {SAMPLES_AFTER}");
            sql.AppendLine("ORDER BY login_date");

            using SqlConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.LIMS_Read));
            conn.Open();
            SqlCommand cmd = new(sql.ToString(), conn);
            cmd.Parameters.AddWithValue("SamplePoint", SamplePoint);
            cmd.Parameters.AddWithValue("Component", Component);
            cmd.Parameters.AddWithValue("login_date", SampleDate);
            SqlDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                while (rdr.Read())
                {
                    Hist.Add(new LIMSResultHist() { SampleDate = rdr.GetDateTime(0), Value = rdr.GetDouble(1) });
                }
            }
            conn.Close();

        }
    }
}
