using Microsoft.AspNetCore.Components;
using Radzen;
using DAL = MOO.DAL.ToLive;
using Oracle.ManagedDataAccess.Client;

namespace OM_Lab.Components.Pages.Met
{
    partial class Met_Agg2
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
        private DAL.Models.Met_Agg_Plant2 Plant2Data = new();



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
            Plant2Data = await Task.Run(() => DAL.Services.Met_Agg_Plant2Svc.Get(MetParams.MetDate, MetParams.Material));
            if (Plant2Data == null)
            {
                //if we get a null here, switch the selected material automatically
                if(MetParams.Material == DAL.Models.Met_Material.Flux)
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
                LineData = await Task.Run(() => DAL.Services.Met_Agg_LineSvc.Get(MetParams.MetDate, MetParams.MetDate, 3,5, MetParams.Material));

                Plant2Data = await Task.Run(() => DAL.Services.Met_Agg_Plant2Svc.Get(MetParams.MetDate, MetParams.Material));
                if (Plant2Data == null)
                    Plant2Data = new();
            }
            else
            {
                //code for pulling in Month End data
                LineData = await Task.Run(() => DAL.Services.Met_Agg_LineSvc.GetMonthSummary(MetParams.MetDate, 3, 5, MetParams.Material));
                Plant2Data = await Task.Run(() => DAL.Services.Met_Agg_Plant2Svc.GetMonthSummary(MetParams.MetDate, MetParams.Material));
                //set data is modified becuase the gets call stored proecedure which may/may not have the record in the DB yet
                DataIsModified = true;
            }
            //validate we have good records
            if (Plant2Data == null || LineData.Count < 3)
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
            bool? metDiagResult = await DialogSvc.OpenAsync<Dialogs.MetChangeDate>("Change Met Agg2 Date",
                   new Dictionary<string, object>() {{ "ShowMaterialOption", true } },
                   new DialogOptions() { Width = Dialogs.MetChangeDate.DEFAULT_WIDTH, Height = Dialogs.MetChangeDate.DEFAULT_HEIGHT, Resizable = false, Draggable = true });

            //only update if they changed their selection
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
                    recsUpdated = await Task.Run(() => DAL.Services.Met_Agg_Plant2Svc.Update(Plant2Data, conn));
                    if (recsUpdated == 0)
                        await Task.Run(() => DAL.Services.Met_Agg_Plant2Svc.Insert(Plant2Data, conn));

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
                    await Task.Run(() => DAL.Services.Met_Agg_Plant2Svc.Update(Plant2Data, conn));
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
            DAL.Models.Met_Agg_Plant2 prevAggPlant;
            if(MetParams.MetDate.AddDays(-1) == MOO.Dates.LastDayOfMonth(MetParams.MetDate.AddDays(-1)))
            {
                //yesterday was end of month, get end of month record

                //the prev reclaim value was normally called from MET_ROLL_DAILY.prev_reclaim_bal.  shouldn't need to call this as we can just grab the record
                prevAggPlant = await Task.Run(() => DAL.Services.Met_Agg_Plant2Svc.GetMonthSummary(MetParams.MetDate.AddDays(-1), MetParams.Material));
            }
            else
            {
                //yesterday was not end of month
                prevAggPlant = await Task.Run(() => DAL.Services.Met_Agg_Plant2Svc.Get(MetParams.MetDate.AddDays(-1), MetParams.Material));
            }
            //prevAggPlant will be null here if we switched from Acid to Flux or vice versa, just use a new record so that zeros are used
            if (prevAggPlant == null)
                prevAggPlant = new();

            Plant2Data.Recl_Bal_E = prevAggPlant.Recl_Bal_E.GetValueOrDefault(0) + Plant2Data.East_To_Stockpile.GetValueOrDefault(0) + Plant2Data.Truck_To_East.GetValueOrDefault(0) -
                                            Plant2Data.East_To_Plant.GetValueOrDefault(0) - Plant2Data.Truck_From_East.GetValueOrDefault(0);

            Plant2Data.Trucked_Recl_Bal = prevAggPlant.Trucked_Recl_Bal.GetValueOrDefault(0) + Plant2Data.Truck_To_East.GetValueOrDefault(0) - Plant2Data.East_To_Plant.GetValueOrDefault(0) -
                                            Plant2Data.Truck_From_East.GetValueOrDefault(0);
            //don't allow value to be zero
            Plant2Data.Trucked_Recl_Bal = Math.Max(0M, Plant2Data.Trucked_Recl_Bal.GetValueOrDefault(0));
        }

    }
}
