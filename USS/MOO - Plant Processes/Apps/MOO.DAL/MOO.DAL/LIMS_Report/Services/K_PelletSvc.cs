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
    public static class K_PelletSvc
    {
        static K_PelletSvc()
        {
            Util.RegisterSqlServer();
        }

        public static List<K_Pellet> GetBySampleDate(DateTime StartDate, DateTime EndDate)
        {
            List<K_Pellet> retVal = new();
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
            sql.AppendLine("FROM LIMS_Report.dbo.K_pellet");
            return sql.ToString();
        }





        /// <summary>
        /// Inserts the Keetac Pellet Results in the  database
        /// </summary>
        /// <param name="Pell">Pellet Result To insert</param>
        /// <returns></returns>
        public static int Insert(K_Pellet Pell)
        {
            using SqlConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.LIMS_Report));
            conn.Open();

            return Insert(Pell, conn);

        }

        /// <summary>
        /// Inserts the Keetac Pellet Results in the  database
        /// </summary>
        /// <param name="Pell">Pellet Result To insert</param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public static int Insert(K_Pellet Pell, SqlConnection conn)
        {



            StringBuilder sql = new();
            sql.AppendLine("INSERT INTO LIMS_Report.dbo.k_pellet (Sample_Nbr, Status, Login_Date, sampled_date, recd_date, first_test_date, date_completed, ");
            sql.AppendLine("  sample_point, date_authorised, authoriser, shift_date8, shift_nbr8,shift_date12, shift_nbr12,");
            sql.AppendLine("fe, sio2, al2o3, cao, mgo, mn, p2o5, tio2, total, fe2o3, mno)");

            sql.AppendLine("Values(@Sample_Nbr, @Status, @Login_Date, @sampled_date, @recd_date, @first_test_date, @date_completed,");
            sql.AppendLine("  @sample_point, @date_authorised, @authoriser, @shift_date8, @shift_nbr8,@shift_date12, @shift_nbr12,");
            sql.AppendLine("@fe, @sio2, @al2o3, @cao, @mgo, @mn, @p2o5, @tio2, @total, @fe2o3, @mno)");


            SqlCommand cmd = new(sql.ToString(), conn);

            cmd.Parameters.AddWithValue("sample_nbr", Pell.Sample_Nbr);
            cmd.Parameters.AddWithValue("@status", Pell.Status);
            cmd.Parameters.AddWithValue("@Login_Date", Pell.Login_Date);
            cmd.Parameters.AddWithValue("@sampled_date", Pell.Sampled_Date);
            cmd.Parameters.AddWithValue("@recd_date", ((object)Pell.Recd_Date) ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@first_test_date", ((object)Pell.First_Test_Date) ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@date_completed", ((object)Pell.Date_Completed) ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@sample_point", Pell.Sample_Point);
            cmd.Parameters.AddWithValue("@date_authorised", ((Object)Pell.Date_Authorised) ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@authoriser", ((Object)Pell.Authoriser ?? DBNull.Value));

            cmd.Parameters.AddWithValue("@shift_date8", MOO.Shifts.Shift8.ShiftDate(Pell.Sampled_Date.GetValueOrDefault(Pell.Login_Date), Plant.Keetac));
            cmd.Parameters.AddWithValue("@shift_nbr8", MOO.Shifts.Shift8.ShiftNumber(Pell.Sampled_Date.GetValueOrDefault(Pell.Login_Date), Plant.Keetac));
            var shiftVal = MOO.Shifts.Shift.GetShiftVal(Plant.Keetac, Area.Concentrator, Pell.Sampled_Date.GetValueOrDefault(Pell.Login_Date));
            cmd.Parameters.AddWithValue("@shift_date12", shiftVal.ShiftDate);
            cmd.Parameters.AddWithValue("@shift_nbr12", shiftVal.ShiftNumber);


            cmd.Parameters.AddWithValue("@fe", ((Object)Pell.Fe) ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@sio2", ((Object)Pell.SiO2) ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@al2o3", ((Object)Pell.Al2O3) ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@cao", ((Object)Pell.CaO) ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@mgo", ((Object)Pell.MgO) ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@mn", ((Object)Pell.Mn) ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@p2o5", ((Object)Pell.P2O5) ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@tio2", ((Object)Pell.TiO2) ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@total", ((Object)Pell.Total) ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@fe2o3", ((Object)Pell.Fe2O3) ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@mno", ((Object)Pell.MnO) ?? DBNull.Value);



            return cmd.ExecuteNonQuery();

        }

        /// <summary>
        /// Updates the Keetac pellet Results database
        /// </summary>
        /// <param name="Pell">Keetac pellet Result To update</param>
        /// <returns></returns>
        public static int Update(K_Pellet Pell)
        {
            using SqlConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.LIMS_Report));
            conn.Open();

            return Update(Pell, conn);

        }

        /// <summary>
        /// Updates the Keetac pellet Result database
        /// </summary>
        /// <param name="Pell">pellet Result To update</param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public static int Update(K_Pellet Pell, SqlConnection conn)
        {
            StringBuilder sql = new();
            sql.AppendLine("UPDATE LIMS_Report.dbo.K_pellet SET status = @status, Login_Date = @Login_date, sampled_date = @sampled_date, ");
            sql.AppendLine("    recd_date = @recd_date, first_test_date = @first_test_date, date_completed = @date_completed,");
            sql.AppendLine("    sample_point = @sample_point, ");
            sql.AppendLine("    date_authorised = @date_authorised, authoriser = @authoriser,");
            sql.AppendLine("    Shift_Date8 = @Shift_Date8, Shift_Nbr8 = @Shift_Nbr8,");
            sql.AppendLine("    Shift_Date12 = @Shift_Date12, Shift_Nbr12 = @Shift_Nbr12,");
            sql.AppendLine("    Fe = @Fe, SiO2 = @SiO2,");
            sql.AppendLine("    Al2O3 = @Al2O3, CaO = @CaO,");
            sql.AppendLine("    MgO = @MgO, Mn = @Mn,");
            sql.AppendLine("    P2O5 = @P2O5, TiO2 = @TiO2,");
            sql.AppendLine("    Total = @Total, Fe2O3 = @Fe2O3,");
            sql.AppendLine("    MnO = @MnO");

            sql.AppendLine("WHERE sample_nbr = @sample_nbr");

            SqlCommand cmd = new(sql.ToString(), conn);

            cmd.Parameters.AddWithValue("@status", Pell.Status);
            cmd.Parameters.AddWithValue("@Login_Date", Pell.Login_Date);
            cmd.Parameters.AddWithValue("@sampled_date", Pell.Sampled_Date);
            cmd.Parameters.AddWithValue("@recd_date", ((object)Pell.Recd_Date) ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@first_test_date", ((object)Pell.First_Test_Date) ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@date_completed", ((object)Pell.Date_Completed) ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@sample_point", Pell.Sample_Point);
            cmd.Parameters.AddWithValue("@date_authorised", ((Object)Pell.Date_Authorised) ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@authoriser", ((Object)Pell.Authoriser ?? DBNull.Value));

            cmd.Parameters.AddWithValue("@shift_date8", MOO.Shifts.Shift8.ShiftDate(Pell.Sampled_Date.GetValueOrDefault(Pell.Login_Date), Plant.Keetac));
            cmd.Parameters.AddWithValue("@shift_nbr8", MOO.Shifts.Shift8.ShiftNumber(Pell.Sampled_Date.GetValueOrDefault(Pell.Login_Date), Plant.Keetac));
            var shiftVal = MOO.Shifts.Shift.GetShiftVal(Plant.Keetac, Area.Concentrator, Pell.Sampled_Date.GetValueOrDefault(Pell.Login_Date));
            cmd.Parameters.AddWithValue("@shift_date12", shiftVal.ShiftDate);
            cmd.Parameters.AddWithValue("@shift_nbr12", shiftVal.ShiftNumber);


            cmd.Parameters.AddWithValue("@fe", ((Object)Pell.Fe) ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@sio2", ((Object)Pell.SiO2) ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@al2o3", ((Object)Pell.Al2O3) ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@cao", ((Object)Pell.CaO) ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@mgo", ((Object)Pell.MgO) ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@mn", ((Object)Pell.Mn) ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@p2o5", ((Object)Pell.P2O5) ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@tio2", ((Object)Pell.TiO2) ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@total", ((Object)Pell.Total) ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@fe2o3", ((Object)Pell.Fe2O3) ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@mno", ((Object)Pell.MnO) ?? DBNull.Value);

            cmd.Parameters.AddWithValue("sample_nbr", Pell.Sample_Nbr);


            return cmd.ExecuteNonQuery();

        }


        internal static K_Pellet DataRowToObject(DataRow row)
        {
            K_Pellet retVal = new();
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
            retVal.Shift_Date8 = row.Field<DateTime>("shift_date8");
            retVal.Shift_Nbr8 = row.Field<byte>("shift_nbr8");
            retVal.Shift_Date12 = row.Field<DateTime>("shift_date12");
            retVal.Shift_Nbr12 = row.Field<byte>("shift_nbr12");
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
