using Microsoft.AspNetCore.Components;
using Radzen;
using DAL = MOO.DAL.ToLive;
using Oracle.ManagedDataAccess.Client;

namespace OM_Lab.Components.Pages.Met
{
    partial class Met_Agg3
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        [Inject]
        Radzen.DialogService DialogSvc { get; set; }

        [Inject]
        Radzen.NotificationService NotificationSvc { get; set; }

        [Inject]
        OM_Lab.Data.Models.MetChangeDateVals MetParams { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.



        private List<DAL.Models.Met_Agg_Line> LineData = [];
        private DAL.Models.Met_Agg_Plant3 Plant3Data = new();



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
            Plant3Data = await Task.Run(() => DAL.Services.Met_Agg_Plant3Svc.Get(MetParams.MetDate, MetParams.Material));
            if (Plant3Data == null)
            {
                //if we get a null here, switch the selected material automatically
                if (MetParams.Material == DAL.Models.Met_Material.Flux)
                    MetParams.Material = DAL.Models.Met_Material.Acid;
                else
                    MetParams.Material = DAL.Models.Met_Material.Flux;
            }

            await LoadDataAsync();
            await base.OnInitializedAsync();
        }


        private async Task LoadDataAsync()
        {
            if (!MetParams.IsMonthEnd)
            {
                LineData = await Task.Run(() => DAL.Services.Met_Agg_LineSvc.Get(MetParams.MetDate, MetParams.MetDate, 6,7, MetParams.Material));

                Plant3Data = await Task.Run(() => DAL.Services.Met_Agg_Plant3Svc.Get(MetParams.MetDate, MetParams.Material));
                if (Plant3Data == null)
                    Plant3Data = new();
            }
            else
            {
                //code for pulling in Month End data
                LineData = await Task.Run(() => DAL.Services.Met_Agg_LineSvc.GetMonthSummary(MetParams.MetDate, 6,7, MetParams.Material));
                Plant3Data = await Task.Run(() => DAL.Services.Met_Agg_Plant3Svc.GetMonthSummary(MetParams.MetDate, MetParams.Material));
                //set data is modified becuase the gets call stored proecedure which may/may not have the record in the DB yet
                DataIsModified = true;
            }
            //validate we have good records
            if (Plant3Data == null || LineData.Count < 2)
            {
                NotificationSvc.Notify(new NotificationMessage()
                {
                    Severity = NotificationSeverity.Error,
                    Summary = "Invalid or missing Agglomerator Data",
                    Detail = "Data is missing for selected date and material, please select a new date/material",
                    Duration = 8000
                });
                InvalidDateSelection = true;
            }
            else
                InvalidDateSelection = false;
        }

        private async Task EditDateBtnClickAsnc()
        {
            bool? metDiagResult = await DialogSvc.OpenAsync<Dialogs.MetChangeDate>("Change Met Agg3 Date",
                   new Dictionary<string, object>() { { "ShowMaterialOption", true } },
                   new DialogOptions() { Width = Dialogs.MetChangeDate.DEFAULT_WIDTH, Height = Dialogs.MetChangeDate.DEFAULT_HEIGHT, Resizable = false, Draggable = true });

            //only update if they changed their selection
            if (metDiagResult.HasValue && metDiagResult.Value)
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
                    recsUpdated = await Task.Run(() => DAL.Services.Met_Agg_Plant3Svc.Update(Plant3Data, conn));
                    if (recsUpdated == 0)
                        await Task.Run(() => DAL.Services.Met_Agg_Plant3Svc.Insert(Plant3Data, conn));

                    foreach (var lne in LineData)
                    {
                        recsUpdated = await Task.Run(() => DAL.Services.Met_Agg_LineSvc.Update(lne, conn));
                        if (recsUpdated == 0)
                            await Task.Run(() => DAL.Services.Met_Agg_LineSvc.Insert(lne, conn));
                    }
                }
                else
                {
                    //this is not month end, we should never need an insert
                    await Task.Run(() => DAL.Services.Met_Agg_Plant3Svc.Update(Plant3Data, conn));
                    foreach (var lne in LineData)
                        await Task.Run(() => DAL.Services.Met_Agg_LineSvc.Update(lne, conn));
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


        private async Task CalcReclaimBtnClickAsync()
        {
            /*this code was taken from the old Oracle form
             * Talking with Heather in Ore Movement, it sounds like this is always set to zero.  I will maintain the code here though
             * instead of pushing this to all zero in case we find that we need this later
             */
            //Get prev record, if yesterday was last day of month then get end of month record
            DAL.Models.Met_Agg_Plant3 prevAggPlant;
            if (MetParams.MetDate.AddDays(-1) == MOO.Dates.LastDayOfMonth(MetParams.MetDate.AddDays(-1)))
            {
                //yesterday was end of month, get end of month record

                //the prev reclaim value was normally called from MET_ROLL_DAILY.prev_reclaim_bal.  shouldn't need to call this as we can just grab the record
                prevAggPlant = await Task.Run(() => DAL.Services.Met_Agg_Plant3Svc.GetMonthSummary(MetParams.MetDate.AddDays(-1), MetParams.Material));
            }
            else
            {
                //yesterday was not end of month
                prevAggPlant = await Task.Run(() => DAL.Services.Met_Agg_Plant3Svc.Get(MetParams.MetDate.AddDays(-1), MetParams.Material));
            }
            //prevAggPlant will be null here if we switched from Acid to Flux or vice versa, just use a new record so that zeros are used
            if (prevAggPlant == null)
                prevAggPlant = new();

            Plant3Data.Recl_Bal_W = Math.Round(prevAggPlant.Recl_Bal_W.GetValueOrDefault(0) + Plant3Data.West_To_Stockpile.GetValueOrDefault(0) + Plant3Data.Truck_To_West.GetValueOrDefault(0) -
                                            Plant3Data.West_To_Plant.GetValueOrDefault(0) - Plant3Data.Truck_From_West.GetValueOrDefault(0),0,MidpointRounding.AwayFromZero);

            Plant3Data.Trucked_Recl_Bal = prevAggPlant.Trucked_Recl_Bal.GetValueOrDefault(0) + Plant3Data.Truck_To_West.GetValueOrDefault(0) - Plant3Data.West_To_Plant.GetValueOrDefault(0) -
                                            Plant3Data.Truck_From_West.GetValueOrDefault(0);
            //don't allow value to be zero
            Plant3Data.Trucked_Recl_Bal = Math.Round(Math.Max(0M, Plant3Data.Trucked_Recl_Bal.GetValueOrDefault(0)),0,MidpointRounding.AwayFromZero);
        }

        private async Task GetPileDataAsync()
        {
            //get the pile data from the agg2_day table
            DAL.Models.Agg2_Day a2d = await Task.Run(() => DAL.Services.Agg2_DaySvc.Get(MetParams.MetDate));
            Plant3Data.West_To_Stockpile = Math.Round(a2d.Recl_Out_Act_Ltons.GetValueOrDefault(0),0, MidpointRounding.AwayFromZero);

            DAL.Models.Agg3_Day a3d = await Task.Run(() => DAL.Services.Agg3_DaySvc.Get(MetParams.MetDate));
            Plant3Data.West_To_Plant = Math.Round(a3d.Recl_In_Act_Ltons.GetValueOrDefault(0), 0, MidpointRounding.AwayFromZero);
        }

    }
}
