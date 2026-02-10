using Microsoft.Data.SqlClient;
using MOO.DAL.LIMS_Report.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.LIMS_Report.Services
{
    public static class K_XRF_PellShift_StdSvc
    {
        static K_XRF_PellShift_StdSvc()
        {
            Util.RegisterSqlServer();
        }

        public static List<K_XRF_PellShift_Std> GetBySampleDate(DateTime StartDate, DateTime EndDate)
        {
            List<K_XRF_PellShift_Std> retVal = new();
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
            sql.AppendLine("FROM LIMS_Report.dbo.K_XRF_PellShift_Std");
            return sql.ToString();
        }





        /// <summary>
        /// Inserts the Keetac XRF Standard Results in the  database
        /// </summary>
        /// <param name="Obj"> XRF Standard Result To insert</param>
        /// <returns></returns>
        public static int Insert(K_XRF_PellShift_Std Obj)
        {
            using SqlConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.LIMS_Report));
            conn.Open();

            return Insert(Obj, conn);

        }

        /// <summary>
        /// Inserts the Keetac XRF Standard Results in the  database
        /// </summary>
        /// <param name="Obj"> XRF Standard Result To insert</param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public static int Insert(K_XRF_PellShift_Std Obj, SqlConnection conn)
        {



            StringBuilder sql = new();
            sql.AppendLine("INSERT INTO LIMS_Report.dbo.K_XRF_PellShift_Std (Sample_Nbr, Status, Login_Date, sampled_date, recd_date, first_test_date, date_completed, ");
            sql.AppendLine("  sample_point, date_authorised, authoriser,");
            sql.AppendLine("fe, sio2, al2o3, cao, mgo, mn, p2o5, tio2, total, fe2o3, mno)");

            sql.AppendLine("Values(@Sample_Nbr, @Status, @Login_Date, @sampled_date, @recd_date, @first_test_date, @date_completed,");
            sql.AppendLine("  @sample_point, @date_authorised, @authoriser,");
            sql.AppendLine("@fe, @sio2, @al2o3, @cao, @mgo, @mn, @p2o5, @tio2, @total, @fe2o3, @mno)");


            SqlCommand cmd = new(sql.ToString(), conn);

            cmd.Parameters.AddWithValue("sample_nbr", Obj.Sample_Nbr);
            cmd.Parameters.AddWithValue("@status", Obj.Status);
            cmd.Parameters.AddWithValue("@Login_Date", Obj.Login_Date);
            cmd.Parameters.AddWithValue("@sampled_date", Obj.Sampled_Date);
            cmd.Parameters.AddWithValue("@recd_date", ((object)Obj.Recd_Date) ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@first_test_date", ((object)Obj.First_Test_Date) ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@date_completed", ((object)Obj.Date_Completed) ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@sample_point", Obj.Sample_Point);
            cmd.Parameters.AddWithValue("@date_authorised", ((Object)Obj.Date_Authorised) ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@authoriser", ((Object)Obj.Authoriser ?? DBNull.Value));




            cmd.Parameters.AddWithValue("@fe", ((Object)Obj.Fe) ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@sio2", ((Object)Obj.SiO2) ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@al2o3", ((Object)Obj.Al2O3) ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@cao", ((Object)Obj.CaO) ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@mgo", ((Object)Obj.MgO) ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@mn", ((Object)Obj.Mn) ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@p2o5", ((Object)Obj.P2O5) ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@tio2", ((Object)Obj.TiO2) ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@total", ((Object)Obj.Total) ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@fe2o3", ((Object)Obj.Fe2O3) ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@mno", ((Object)Obj.MnO) ?? DBNull.Value);



            return cmd.ExecuteNonQuery();

        }

        /// <summary>
        /// Updates the Keetac XRF Standard Results database
        /// </summary>
        /// <param name="Obj">Keetac XRF Standard Result To update</param>
        /// <returns></returns>
        public static int Update(K_XRF_PellShift_Std Obj)
        {
            using SqlConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.LIMS_Report));
            conn.Open();

            return Update(Obj, conn);

        }

        /// <summary>
        /// Updates the Keetac XRF Standard Results database
        /// </summary>
        /// <param name="Obj">Keetac XRF Standard Result To update</param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public static int Update(K_XRF_PellShift_Std Obj, SqlConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("UPDATE LIMS_Report.dbo.K_XRF_PellShift_Std SET status = @status, Login_Date = @Login_date, sampled_date = @sampled_date, ");
            sql.AppendLine("    recd_date = @recd_date, first_test_date = @first_test_date, date_completed = @date_completed,");
            sql.AppendLine("    sample_point = @sample_point, ");
            sql.AppendLine("    date_authorised = @date_authorised, authoriser = @authoriser,");
            sql.AppendLine("    Fe = @Fe, SiO2 = @SiO2,");
            sql.AppendLine("    Al2O3 = @Al2O3, CaO = @CaO,");
            sql.AppendLine("    MgO = @MgO, Mn = @Mn,");
            sql.AppendLine("    P2O5 = @P2O5, TiO2 = @TiO2,");
            sql.AppendLine("    Total = @Total, Fe2O3 = @Fe2O3,");
            sql.AppendLine("    MnO = @MnO");

            sql.AppendLine("WHERE sample_nbr = @sample_nbr");

            SqlCommand cmd = new(sql.ToString(), conn);

            cmd.Parameters.AddWithValue("@status", Obj.Status);
            cmd.Parameters.AddWithValue("@Login_Date", Obj.Login_Date);
            cmd.Parameters.AddWithValue("@sampled_date", Obj.Sampled_Date);
            cmd.Parameters.AddWithValue("@recd_date", ((object)Obj.Recd_Date) ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@first_test_date", ((object)Obj.First_Test_Date) ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@date_completed", ((object)Obj.Date_Completed) ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@sample_point", Obj.Sample_Point);
            cmd.Parameters.AddWithValue("@date_authorised", ((Object)Obj.Date_Authorised) ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@authoriser", ((Object)Obj.Authoriser ?? DBNull.Value));



            cmd.Parameters.AddWithValue("@fe", ((Object)Obj.Fe) ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@sio2", ((Object)Obj.SiO2) ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@al2o3", ((Object)Obj.Al2O3) ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@cao", ((Object)Obj.CaO) ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@mgo", ((Object)Obj.MgO) ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@mn", ((Object)Obj.Mn) ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@p2o5", ((Object)Obj.P2O5) ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@tio2", ((Object)Obj.TiO2) ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@total", ((Object)Obj.Total) ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@fe2o3", ((Object)Obj.Fe2O3) ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@mno", ((Object)Obj.MnO) ?? DBNull.Value);

            cmd.Parameters.AddWithValue("sample_nbr", Obj.Sample_Nbr);


            return cmd.ExecuteNonQuery();

        }


        internal static K_XRF_PellShift_Std DataRowToObject(DataRow row)
        {
            K_XRF_PellShift_Std retVal = new();
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
            retVal.Fe = row.Field<decimal?>("fe");
            retVal.SiO2 = row.Field<decimal?>("SiO2");
            retVal.Al2O3 = row.Field<decimal?>("Al2O3");
            retVal.CaO = row.Field<decimal?>("CaO");
            retVal.MgO = row.Field<decimal?>("MgO");
            retVal.Mn = row.Field<decimal?>("Mn");
            retVal.P2O5 = row.Field<decimal?>("P2O5");
            retVal.TiO2 = row.Field<decimal?>("TiO2");
            retVal.Total = row.Field<decimal?>("Total");
            retVal.Fe2O3 = row.Field<decimal?>("Fe2O3");
            retVal.MnO = row.Field<decimal?>("MnO");

            return retVal;
        }
    }
}
