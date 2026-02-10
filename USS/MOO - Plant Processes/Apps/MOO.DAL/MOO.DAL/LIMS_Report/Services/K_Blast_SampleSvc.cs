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
    public class K_Blast_SampleSvc
    {
        static K_Blast_SampleSvc()
        {
            Util.RegisterSqlServer();
        }

        public static List<K_Blast_Sample> GetBySampleDate(DateTime StartDate, DateTime EndDate)
        {
            List<K_Blast_Sample> retVal = new();
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
            sql.AppendLine("FROM LIMS_Report.dbo.K_Blast_Sample");
            return sql.ToString();
        }





        /// <summary>
        /// Inserts the Keetac Blast Sample Results in the  database
        /// </summary>
        /// <param name="Obj"> Blast Sample Result To insert</param>
        /// <returns></returns>
        public static int Insert(K_Blast_Sample Obj)
        {
            using SqlConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.LIMS_Report));
            conn.Open();

            return Insert(Obj, conn);

        }

        /// <summary>
        /// Inserts the Keetac Blast Sample Results in the  database
        /// </summary>
        /// <param name="Obj"> Blast Sample Result To insert</param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public static int Insert(K_Blast_Sample Obj, SqlConnection conn)
        {



            StringBuilder sql = new();
            sql.AppendLine("INSERT INTO LIMS_Report.dbo.K_Blast_Sample (Sample_Nbr, Status, Login_Date, sampled_date, recd_date, first_test_date, date_completed, ");
            sql.AppendLine("  sample_point, date_authorised, authoriser,");
            sql.AppendLine("    Parent_Sample_Nbr, Pattern_Number, Hole_Number, Preparation, ");
            sql.AppendLine("    ScreenMinus325Mesh, FeCalc, SiO2, SiO2Calc, AL2O3, CaO, CaOCalc, MgO, P2O5,");
            sql.AppendLine("    TiO2, Fe2O3, Mn3O4, LOI_Balance, Na2O, K2O, SO3, V2O5, Cr2O3, SrO, ZrO2,");
            sql.AppendLine("    BaO, NiO, CuO, ZnO, PbO, HfO2, DT_WgtRecovery, Fe2Plus)");

            sql.AppendLine("Values(@Sample_Nbr, @Status, @Login_Date, @sampled_date, @recd_date, @first_test_date, @date_completed,");
            sql.AppendLine("  @sample_point, @date_authorised, @authoriser,");
            sql.AppendLine("    @Parent_Sample_Nbr, @Pattern_Number, @Hole_Number, @Preparation, ");
            sql.AppendLine("    @ScreenMinus325Mesh, @FeCalc, @SiO2, @SiO2Calc, @AL2O3, @CaO, @CaOCalc, @MgO, @P2O5,");
            sql.AppendLine("    @TiO2, @Fe2O3, @Mn3O4, @LOI_Balance, @Na2O, @K2O, @SO3, @V2O5, @Cr2O3, @SrO, @ZrO2,");
            sql.AppendLine("    @BaO, @NiO, @CuO, @ZnO, @PbO, @HfO2, @DT_WgtRecovery, @Fe2Plus)");


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


            cmd.Parameters.AddWithValue("@Parent_Sample_Nbr", Obj.Parent_Sample_Nbr);
            cmd.Parameters.AddWithValue("@Pattern_Number", Obj.Pattern_Number);
            cmd.Parameters.AddWithValue("@Hole_Number", Obj.Hole_Number);
            cmd.Parameters.AddWithValue("@Preparation", Obj.Preparation);

            cmd.Parameters.AddWithValue("@ScreenMinus325Mesh", ((Object)Obj.ScreenMinus325Mesh) ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@FeCalc", ((Object)Obj.FeCalc) ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@SiO2", ((Object)Obj.SiO2) ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@SiO2Calc", ((Object)Obj.SiO2Calc) ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@AL2O3", ((Object)Obj.AL2O3) ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@CaO", ((Object)Obj.CaO) ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@CaOCalc", ((Object)Obj.CaOCalc) ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@MgO", ((Object)Obj.MgO) ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@P2O5", ((Object)Obj.P2O5) ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@TiO2", ((Object)Obj.TiO2) ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Fe2O3", ((Object)Obj.Fe2O3) ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Mn3O4", ((Object)Obj.Mn3O4) ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@LOI_Balance", ((Object)Obj.LOI_Balance) ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Na2O", ((Object)Obj.Na2O) ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@K2O", ((Object)Obj.K2O) ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@SO3", ((Object)Obj.SO3) ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@V2O5", ((Object)Obj.V2O5) ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Cr2O3", ((Object)Obj.Cr2O3) ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@SrO", ((Object)Obj.SrO) ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ZrO2", ((Object)Obj.ZrO2) ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@BaO", ((Object)Obj.BaO) ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@NiO", ((Object)Obj.NiO) ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@CuO", ((Object)Obj.CuO) ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ZnO", ((Object)Obj.ZnO) ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@PbO", ((Object)Obj.PbO) ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@HfO2", ((Object)Obj.HfO2) ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@DT_WgtRecovery", ((Object)Obj.DT_WgtRecovery) ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Fe2Plus", ((Object)Obj.Fe2Plus) ?? DBNull.Value);



            return cmd.ExecuteNonQuery();

        }

        /// <summary>
        /// Updates the Keetac Blast Sample Results database
        /// </summary>
        /// <param name="Obj">Keetac Blast Sample Result To update</param>
        /// <returns></returns>
        public static int Update(K_Blast_Sample Obj)
        {
            using SqlConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.LIMS_Report));
            conn.Open();

            return Update(Obj, conn);

        }

        /// <summary>
        /// Updates the Keetac Blast Sample Results database
        /// </summary>
        /// <param name="Obj">Keetac Blast Sample Result To update</param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public static int Update(K_Blast_Sample Obj, SqlConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("UPDATE LIMS_Report.dbo.K_Blast_Sample SET status = @status, Login_Date = @Login_date, sampled_date = @sampled_date, ");
            sql.AppendLine("    recd_date = @recd_date, first_test_date = @first_test_date, date_completed = @date_completed,");
            sql.AppendLine("    sample_point = @sample_point, ");
            sql.AppendLine("    date_authorised = @date_authorised, authoriser = @authoriser,");
            sql.AppendLine("    Parent_Sample_Nbr = @Parent_Sample_Nbr, Pattern_Number = @Pattern_Number,");
            sql.AppendLine("    Hole_Number = @Hole_Number, Preparation = @Preparation,");
            sql.AppendLine("    ScreenMinus325Mesh = @ScreenMinus325Mesh, FeCalc = @FeCalc,");
            sql.AppendLine("    SiO2 = @SiO2, SiO2Calc = @SiO2Calc, AL2O3 = @AL2O3, ");
            sql.AppendLine("    CaO = @CaO, CaOCalc = @CaOCalc, MgO = @MgO, P2O5 = @P2O5, ");
            sql.AppendLine("    TiO2 = @TiO2, Fe2O3 = @Fe2O3, Mn3O4 = @Mn3O4, LOI_Balance = @LOI_Balance, ");
            sql.AppendLine("    Na2O = @Na2O, K2O = @K2O, SO3 = @SO3, V2O5 = @V2O5,");
            sql.AppendLine("    Cr2O3 = @Cr2O3, SrO = @SrO, ZrO2 = @ZrO2, BaO = @BaO,");
            sql.AppendLine("    NiO = @NiO, CuO = @CuO, ZnO = @ZnO, PbO = @PbO, HfO2 = @HfO2,");
            sql.AppendLine("    DT_WgtRecovery = @DT_WgtRecovery, Fe2Plus = @Fe2Plus");

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

            cmd.Parameters.AddWithValue("@Parent_Sample_Nbr", Obj.Parent_Sample_Nbr);
            cmd.Parameters.AddWithValue("@Pattern_Number", Obj.Pattern_Number);
            cmd.Parameters.AddWithValue("@Hole_Number", Obj.Hole_Number);
            cmd.Parameters.AddWithValue("@Preparation", Obj.Preparation);

            cmd.Parameters.AddWithValue("@ScreenMinus325Mesh", ((Object)Obj.ScreenMinus325Mesh) ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@FeCalc", ((Object)Obj.FeCalc) ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@SiO2", ((Object)Obj.SiO2) ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@SiO2Calc", ((Object)Obj.SiO2Calc) ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@AL2O3", ((Object)Obj.AL2O3) ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@CaO", ((Object)Obj.CaO) ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@CaOCalc", ((Object)Obj.CaOCalc) ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@MgO", ((Object)Obj.MgO) ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@P2O5", ((Object)Obj.P2O5) ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@TiO2", ((Object)Obj.TiO2) ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Fe2O3", ((Object)Obj.Fe2O3) ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Mn3O4", ((Object)Obj.Mn3O4) ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@LOI_Balance", ((Object)Obj.LOI_Balance) ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Na2O", ((Object)Obj.Na2O) ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@K2O", ((Object)Obj.K2O) ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@SO3", ((Object)Obj.SO3) ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@V2O5", ((Object)Obj.V2O5) ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Cr2O3", ((Object)Obj.Cr2O3) ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@SrO", ((Object)Obj.SrO) ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ZrO2", ((Object)Obj.ZrO2) ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@BaO", ((Object)Obj.BaO) ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@NiO", ((Object)Obj.NiO) ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@CuO", ((Object)Obj.CuO) ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ZnO", ((Object)Obj.ZnO) ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@PbO", ((Object)Obj.PbO) ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@HfO2", ((Object)Obj.HfO2) ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@DT_WgtRecovery", ((Object)Obj.DT_WgtRecovery) ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Fe2Plus", ((Object)Obj.Fe2Plus) ?? DBNull.Value);

            cmd.Parameters.AddWithValue("sample_nbr", Obj.Sample_Nbr);


            return cmd.ExecuteNonQuery();

        }


        internal static K_Blast_Sample DataRowToObject(DataRow row)
        {
            K_Blast_Sample retVal = new();
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

            retVal.Parent_Sample_Nbr = row.Field<long>("Parent_Sample_Nbr");
            retVal.Pattern_Number = row.Field<string>("Pattern_Number");
            retVal.Hole_Number = row.Field<string>("Hole_Number");
            retVal.Preparation = row.Field<string>("Preparation");

            retVal.ScreenMinus325Mesh = row.Field<decimal>("ScreenMinus325Mesh");
            retVal.FeCalc = row.Field<decimal>("FeCalc");
            retVal.SiO2 = row.Field<decimal>("SiO2");
            retVal.SiO2Calc = row.Field<decimal>("SiO2Calc");
            retVal.AL2O3 = row.Field<decimal>("AL2O3");
            retVal.CaO = row.Field<decimal>("CaO");
            retVal.CaOCalc = row.Field<decimal>("CaOCalc");
            retVal.MgO = row.Field<decimal>("MgO");
            retVal.P2O5 = row.Field<decimal>("P2O5");
            retVal.TiO2 = row.Field<decimal>("TiO2");
            retVal.Fe2O3 = row.Field<decimal>("Fe2O3");
            retVal.Mn3O4 = row.Field<decimal>("Mn3O4");
            retVal.LOI_Balance = row.Field<decimal>("LOI_Balance");
            retVal.Na2O = row.Field<decimal>("Na2O");
            retVal.K2O = row.Field<decimal>("K2O");
            retVal.SO3 = row.Field<decimal>("SO3");
            retVal.V2O5 = row.Field<decimal>("V2O5");
            retVal.Cr2O3 = row.Field<decimal>("Cr2O3");
            retVal.SrO = row.Field<decimal>("SrO");
            retVal.ZrO2 = row.Field<decimal>("ZrO2");
            retVal.BaO = row.Field<decimal>("BaO");
            retVal.NiO = row.Field<decimal>("NiO");
            retVal.CuO = row.Field<decimal>("CuO");
            retVal.ZnO = row.Field<decimal>("ZnO");
            retVal.PbO = row.Field<decimal>("PbO");
            retVal.HfO2 = row.Field<decimal>("HfO2");
            retVal.DT_WgtRecovery = row.Field<decimal>("DT_WgtRecovery");
            retVal.Fe2Plus = row.Field<decimal>("Fe2Plus");

            return retVal;
        }
    }
}
