using Microsoft.AspNetCore.Components;
using Radzen;
using DAL = MOO.DAL.ToLive;
using Oracle.ManagedDataAccess.Client;
using System.Text;
using System.Data;
using Newtonsoft.Json.Linq;

namespace OM_Lab.Components.Pages.Met
{
    partial class FTG_Calc
    {

        private DAL.Models.Met_Agg_Plant2 Agg2CurrMonth = new();
        private DAL.Models.Met_Agg_Plant2 Agg2PrevMonth = new();

        private DAL.Models.Met_Agg_Plant3 Agg3CurrMonth = new();
        private DAL.Models.Met_Agg_Plant3 Agg3PrevMonth = new();

        private List<DAL.Models.Met_Agg_Line> AggLineData = [];
        private DateTime SelectedDate = DateTime.Today;
        private DAL.Models.Met_Material SelectedMaterial = DAL.Models.Met_Material.Flux;
        private readonly decimal[] PelTons = [0, 0, 0, 0, 0];
        private readonly decimal[] TrainAnalysisVal = [0, 0];
        private readonly decimal[] MatFactorVal = [0, 0];
        private readonly decimal[] FeedToGrateVal = [0, 0];
        private readonly decimal[] ConcToAgglomVal = [0, 0];
        private decimal Agg3TotalTrucked;

        private readonly DateTime FTGInvChangeoverDate = DateTime.Parse("10/20/2024");

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
        }

        private static void DateRender(DateRenderEventArgs args)
        {
            //disable dates later than today
            args.Disabled = (args.Date > DateTime.Today.AddDays(-1));
        }
        private async Task LoadData()
        {
            DateTime PrevMonthLastDay = MOO.Dates.FirstDayOfMonth(SelectedDate).AddDays(-1);
            Agg2CurrMonth = await Task.Run(() => DAL.Services.Met_Agg_Plant2Svc.GetMonthSummary(SelectedDate, SelectedMaterial));
            Agg2PrevMonth = await Task.Run(() => DAL.Services.Met_Agg_Plant2Svc.GetMonthSummary(PrevMonthLastDay, SelectedMaterial));


            Agg3CurrMonth = await Task.Run(() => DAL.Services.Met_Agg_Plant3Svc.GetMonthSummary(SelectedDate, SelectedMaterial));
            Agg3PrevMonth = await Task.Run(() => DAL.Services.Met_Agg_Plant3Svc.GetMonthSummary(PrevMonthLastDay, SelectedMaterial));


            AggLineData = await Task.Run(() => DAL.Services.Met_Agg_LineSvc.GetMonthSummary(SelectedDate, 3, 7, SelectedMaterial));

            int lineIdx = 0;
            foreach (var lne in AggLineData)
            {
                PelTons[lineIdx] = lne.PelTons_Adj.GetValueOrDefault(lne.PelTons.GetValueOrDefault(0));
                lineIdx++;
            }

            TrainAnalysisVal[0] = TrainAnalysis(1);
            TrainAnalysisVal[1] = TrainAnalysis(3);
            MatFactorVal[0] = MaterialFactor(1);
            MatFactorVal[1] = MaterialFactor(3);

            //Calcs for aggl2
            FeedToGrateVal[0] = (PelTons[0] + PelTons[1] + PelTons[2]) / (MatFactorVal[0] / (100 - TrainAnalysisVal[0]));
            ConcToAgglomVal[0] = FeedToGrateVal[0] + (Agg2CurrMonth.Inventory - Agg2PrevMonth.Inventory) +
                                        (Agg2CurrMonth.Truck_From_West.GetValueOrDefault(0) + Agg2CurrMonth.Truck_From_East.GetValueOrDefault(0) + Agg2CurrMonth.Grp13_Fines.GetValueOrDefault(0));

            //Calcs for aggl3
            FeedToGrateVal[1] = (PelTons[3] + PelTons[4]) / (MatFactorVal[1] / (100 - TrainAnalysisVal[1]));
            Agg3TotalTrucked = Agg3CurrMonth.Truck_To_West.GetValueOrDefault(0) + Agg3PrevMonth.Trucked_Recl_Bal.GetValueOrDefault(0) - Agg3CurrMonth.Trucked_Recl_Bal.GetValueOrDefault(0);
            ConcToAgglomVal[1] = FeedToGrateVal[1] + (Agg3CurrMonth.Inventory - Agg3PrevMonth.Inventory) +
                                        (Agg3CurrMonth.Truck_From_West.GetValueOrDefault(0) + Agg3CurrMonth.Truck_From_East.GetValueOrDefault(0)) -
                                        Agg3TotalTrucked;


        }

        private decimal TrainAnalysis(byte StepNbr)
        {

            StringBuilder sql = new();
            sql.AppendLine("DECLARE");
            sql.AppendLine("    Datex Date;");
            sql.AppendLine("    Material Number;");
            sql.AppendLine("    Step Number;");
            sql.AppendLine("    wb WBILL%ROWTYPE;");
            sql.AppendLine("BEGIN");
            sql.AppendLine("    Datex := :Datex;");
            sql.AppendLine("    Material := :Material;");
            sql.AppendLine("    Step := :Step;");
            sql.AppendLine("    wb := tolive.met_roll.train_analysis(Datex, Material, Step, 'm');");
            sql.AppendLine("    open :cursor FOR SELECT ROUND(wb.h2o,4) h2o");
            sql.AppendLine("FROM DUAL;");
            sql.AppendLine("END;");


            OracleDataAdapter da = new(sql.ToString(), MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            da.SelectCommand.CommandType = CommandType.Text;
            da.SelectCommand.Parameters.Add("Datex", SelectedDate);
            da.SelectCommand.Parameters.Add("Material", (int)SelectedMaterial);
            da.SelectCommand.Parameters.Add("Step", StepNbr);
            OracleParameter outTable = new("cursor", OracleDbType.RefCursor, ParameterDirection.Output);
            da.SelectCommand.Parameters.Add(outTable);
            DataSet ds = MOO.Data.ExecuteQuery(da);
            if (ds.Tables[0].Rows.Count > 0)
                return ds.Tables[0].Rows[0].Field<decimal?>(0).GetValueOrDefault(0);
            else
                return 0;
        }


        private decimal MaterialFactor(byte StepNbr)
        {
            string cmdText = "tolive.met_roll.material_factor";
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            OracleCommand cmd = new(cmdText, conn)
            {
                CommandType = CommandType.StoredProcedure,
                BindByName = true
            };
            cmd.Parameters.Add("roll_date", SelectedDate);
            cmd.Parameters.Add("mat", (int)SelectedMaterial);
            cmd.Parameters.Add("step", StepNbr);
            cmd.Parameters.Add("m_y", "m");
            cmd.Parameters.Add(new OracleParameter("retVal", OracleDbType.Decimal));
            //cmd.Parameters["retVal"].DbType = DbType.Decimal;
            cmd.Parameters["retVal"].Direction = ParameterDirection.ReturnValue;
            conn.Open();
            cmd.ExecuteNonQuery();

            object dbVal = cmd.Parameters["retVal"].Value;
            //dbVal should now be of type Oracle.ManagedDataAccess.Types.OracleDecimal
            //this has too many decimal points and results in Arithmetic overflow in .Net
            //Therefore we will convert to string and then just take the first 10 characters and then convert to decimal
            decimal retVal = decimal.Parse(dbVal!.ToString()![..10]);
            return retVal;

        }

    }
}
