using Microsoft.AspNetCore.Components;
using Radzen;
using DAL = MOO.DAL.ToLive;
using Oracle.ManagedDataAccess.Client;
using System.Text;
using System.Data;


namespace OM_Lab.Components.Pages.Met
{
    partial class Met_Crush
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        [Inject]
        Radzen.DialogService DialogSvc { get; set; }

        [Inject]
        Radzen.NotificationService NotificationSvc { get; set; }

        [Inject]
        OM_Lab.Data.Models.MetChangeDateVals MetParams { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        private List<DAL.Models.Met_Crush_Primary> PrimaryData = [];
        private List<DAL.Models.Met_Crush_Secondary> SecondaryData = [];
        private List<DAL.Models.Met_Crush_Tertiary> TertiaryData = [];
        private DAL.Models.Met_Crush_Plant PlantData = new();
        private DAL.Models.Met_Crush_Plant PlantDataYear = new();



        /// <summary>
        /// Determines if data has been modified, enables/disables the save button
        /// </summary>
        private bool DataIsModified = false;

        /// <summary>
        /// Determines if there was an invalid date selection and we do not have valid data to save, disables save button if true
        /// </summary>
        private bool InvalidDateSelection = false;

        /// <summary>
        /// True when async saving is happening
        /// </summary>
        private bool IsSaving = false;


        protected override async Task OnInitializedAsync()
        {
            await LoadDataAsync();
            await base.OnInitializedAsync();
        }


        private async Task LoadDataAsync()
        {
            if (!MetParams.IsMonthEnd)
            {
                PrimaryData = await Task.Run(() => DAL.Services.Met_Crush_PrimarySvc.Get(MetParams.MetDate, MetParams.MetDate));
                SecondaryData = await Task.Run(() => DAL.Services.Met_Crush_SecondarySvc.Get(MetParams.MetDate, MetParams.MetDate, 1, 15));
                TertiaryData = await Task.Run(() => DAL.Services.Met_Crush_TertiarySvc.Get(MetParams.MetDate, MetParams.MetDate, 1, 28));

                PlantData = await Task.Run(() => DAL.Services.Met_Crush_PlantSvc.Get(MetParams.MetDate));
            }
            else
            {
                //code for pulling in Month End data
                PrimaryData = await Task.Run(() => DAL.Services.Met_Crush_PrimarySvc.GetMonthSummary(MetParams.MetDate, 1,4));
                SecondaryData = await Task.Run(() => DAL.Services.Met_Crush_SecondarySvc.GetMonthSummary(MetParams.MetDate, 1,15));
                TertiaryData = await Task.Run(() => DAL.Services.Met_Crush_TertiarySvc.GetMonthSummary(MetParams.MetDate, 1,28));
                PlantData = await Task.Run(() => DAL.Services.Met_Crush_PlantSvc.GetMonthSummary(MetParams.MetDate));
                PlantDataYear = await Task.Run(() => DAL.Services.Met_Crush_PlantSvc.GetYearRecord(MetParams.MetDate));
                //set data is modified becuase the gets call stored proecedure which may/may not have the record in the DB yet
                DataIsModified = true;
            }
            //validate we have good records
            if (PlantData == null || PrimaryData.Count < 4 || SecondaryData.Count < 15 || TertiaryData.Count < 27) 
            {
                NotificationSvc.Notify(new NotificationMessage()
                {
                    Severity = NotificationSeverity.Error,
                    Summary = "Invalid or missing Crusher Data",
                    Detail = "Data is missing for selected date, please select a new date",
                    Duration = 8000
                });
                InvalidDateSelection = true;
                PlantData = new();  //need to create a new plantdata that is empty or we will crash as objects are pointing to this
            }
            else
                InvalidDateSelection = false;
        }


        private async Task EditDateBtnClickAsnc()
        {
            bool? metDiagResult = await DialogSvc.OpenAsync<Dialogs.MetChangeDate>("Change Met Conc2 Date",
                   new Dictionary<string, object>() { { "ShowMaterialOption", false } },
                   new DialogOptions() { Width = Dialogs.MetChangeDate.DEFAULT_WIDTH, Height = Dialogs.MetChangeDate.DEFAULT_HEIGHT, Resizable = false, Draggable = true });
            if (metDiagResult.HasValue && metDiagResult.Value )
            {                
                DataIsModified = false;
                await LoadDataAsync();
            }
        }

        private async Task SaveBtnClickAsync()
        {
            IsSaving = true;
            using OracleConnection conn = new(MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            await conn.OpenAsync();
            OracleTransaction trans = conn.BeginTransaction();
            int recsUpdated;
            try
            {
                if (MetParams.IsMonthEnd)
                {
                    //month end may require an insert, if update records = 0 then insert
                    recsUpdated = await Task.Run(() => DAL.Services.Met_Crush_PlantSvc.Update(PlantData, conn));
                    if (recsUpdated == 0)
                        await Task.Run(() => DAL.Services.Met_Crush_PlantSvc.Insert(PlantData, conn));

                    //month end may require an insert, if update records = 0 then insert
                    recsUpdated = await Task.Run(() => DAL.Services.Met_Crush_PlantSvc.Update(PlantDataYear, conn));
                    if (recsUpdated == 0)
                        await Task.Run(() => DAL.Services.Met_Crush_PlantSvc.Insert(PlantDataYear, conn));
                    //primary
                    foreach (var pri in PrimaryData)
                    {
                        recsUpdated = await Task.Run(() => DAL.Services.Met_Crush_PrimarySvc.Update(pri, conn));
                        if (recsUpdated == 0)
                            await Task.Run(() => DAL.Services.Met_Crush_PrimarySvc.Insert(pri, conn));
                    }

                    //secondary
                    foreach (var sec in SecondaryData)
                    {
                        recsUpdated = await Task.Run(() => DAL.Services.Met_Crush_SecondarySvc.Update(sec, conn));
                        if (recsUpdated == 0)
                            await Task.Run(() => DAL.Services.Met_Crush_SecondarySvc.Insert(sec, conn));
                    }
                    //tertiary
                    foreach (var ter in TertiaryData)
                    {
                        recsUpdated = await Task.Run(() => DAL.Services.Met_Crush_TertiarySvc.Update(ter, conn));
                        if (recsUpdated == 0)
                            await Task.Run(() => DAL.Services.Met_Crush_TertiarySvc.Insert(ter, conn));
                    }
                }
                else
                {
                    //not month end, this should only be a update statement
                    recsUpdated = await Task.Run(() => DAL.Services.Met_Crush_PlantSvc.Update(PlantData, conn));
                    
                    //primary
                    foreach (var pri in PrimaryData)
                        recsUpdated = await Task.Run(() => DAL.Services.Met_Crush_PrimarySvc.Update(pri, conn));                     

                    //secondary
                    foreach (var sec in SecondaryData)
                        recsUpdated = await Task.Run(() => DAL.Services.Met_Crush_SecondarySvc.Update(sec, conn));
                    
                    //tertiary
                    foreach (var ter in TertiaryData)
                        recsUpdated = await Task.Run(() => DAL.Services.Met_Crush_TertiarySvc.Update(ter, conn));
                   
                }

                trans.Commit();
                DataIsModified = false;
            }
            catch (Exception ex)
            {
                trans.Rollback();
                NotificationSvc.Notify(new NotificationMessage()
                {
                    Severity = NotificationSeverity.Error,
                    Summary = "Error Saving Data",
                    Detail = $"Unable to save data: {ex.Message}",
                    Duration = 8000
                });
                MOO.Exceptions.ErrorLog.LogError("/Bzr/OM_Lab", ex);
            }
            finally
            {
                conn.Close();
            }
            IsSaving = false;

        }

        /// <summary>
        /// gets data from minvu for month end numbers
        /// </summary>
        /// <returns></returns>
        private async Task GetMinvuDataAsync()
        {
            string firstOfMonth = MOO.Dates.FirstDayOfMonth(MetParams.MetDate).ToString("yyyyMMdd") + 1;
            string lastOfMonth = MOO.Dates.LastDayOfMonth(MetParams.MetDate).ToString("yyyyMMdd") + 2;

            StringBuilder sql = new();

            sql.AppendLine("select ROUND(SUM(CASE WHEN MatMovement_Enum IN (1,2) THEN m.SurvAdjQty ELSE 0 END),0) CrudeActual,");
            sql.AppendLine("	ROUND(SUM(CASE WHEN MatMovement_Enum IN (4) THEN m.SurvAdjQty ELSE 0 END),0) Waste,");
            sql.AppendLine("	ROUND(SUM(CASE WHEN MatMovement_Enum IN (6,7) THEN m.SurvAdjQty ELSE 0 END),0) Surface,");
            sql.AppendLine("	ROUND(SUM(CASE WHEN MatMovement_Enum IN (5) THEN m.SurvAdjQty ELSE 0 END),0) OMH,");
            sql.AppendLine("	ROUND(SUM(CASE WHEN MatMovement_Enum IN (3) THEN m.SurvAdjQty ELSE 0 END),0) OreStocked,");
            sql.AppendLine("	ROUND(SUM(CASE WHEN MatMovement_Enum IN (1,2,3,4,5,6,7) THEN m.SurvAdjQty ELSE 0 END),0) TotalMoved");
            sql.AppendLine("FROM material_movement_by_load m");
            sql.AppendLine("where m.RECORD_EXISTS='Y'");
            sql.AppendLine($"and m.shiftindex BETWEEN {firstOfMonth} AND {lastOfMonth}");

            DataSet ds = await Task.Run(() => MOO.Data.ExecuteQuery(sql.ToString(), MOO.Data.MNODatabase.MTC_MinVu));

            if(ds.Tables[0].Rows.Count > 0)
            {
                DataRow dr = ds.Tables[0].Rows[0];
                PlantData.Crude_To_Crusher_Tons = (decimal)dr.Field<double>("CrudeActual");
                PlantData.Surface_Tons = (decimal)dr.Field<double>("Surface");
                PlantData.Omr_Tons = (decimal)dr.Field<double>("Waste");
                PlantData.Omh_Tons = (decimal)dr.Field<double>("OMH");
                PlantData.Stocked_Tons = (decimal)dr.Field<double>("OreStocked");
                PlantData.Total_Handled_Tons = (decimal)dr.Field<double>("TotalMoved");
            }

        }
    }
}
