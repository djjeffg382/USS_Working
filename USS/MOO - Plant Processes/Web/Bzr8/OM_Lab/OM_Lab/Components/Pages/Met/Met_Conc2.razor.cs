using Microsoft.AspNetCore.Components;
using Radzen;
using DAL = MOO.DAL.ToLive;
using Oracle.ManagedDataAccess.Client;
using System.Text;
using System.Data;

namespace OM_Lab.Components.Pages.Met
{
    partial class Met_Conc2
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        [Inject]
        Radzen.DialogService DialogSvc { get; set; }

        [Inject]
        Radzen.NotificationService NotificationSvc { get; set; }
        [Inject]
        OM_Lab.Data.Models.MetChangeDateVals MetParams { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        private List<DAL.Models.Met_Conc_Line> LineData = [];
        private List<DAL.Models.Met_Conc_Line> FluxData = [];
        private DAL.Models.Met_Conc_Plant2 Plant2Data = new();



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
                LineData = await Task.Run(() => DAL.Services.Met_Conc_LineSvc.Get(MetParams.MetDate, MetParams.MetDate, 2, 12));

                Plant2Data = await Task.Run(() => DAL.Services.Met_Conc_Plant2Svc.Get(MetParams.MetDate));

                FluxData = LineData.FindAll(l => l.Line <= 3);
                if (Plant2Data == null)
                    Plant2Data = new();
            }
            else
            {
                //code for pulling in Month End data
                LineData = await Task.Run(() => DAL.Services.Met_Conc_LineSvc.GetMonthSummary(MetParams.MetDate, 2, 12));
                Plant2Data = await Task.Run(() => DAL.Services.Met_Conc_Plant2Svc.GetMonthSummary(MetParams.MetDate));
                FluxData = LineData.FindAll(l => l.Line <= 3);

                if (Plant2Data == null)
                    Plant2Data = new();

            }
            //validate we have good records
            if (Plant2Data == null || LineData.Count < 11)
            {
                NotificationSvc.Notify(new NotificationMessage()
                {
                    Severity = NotificationSeverity.Error,
                    Summary = "Invalid or missing Conc Data",
                    Detail = "Data is missing for selected date, please select a new date",
                    Duration = 8000
                });
                InvalidDateSelection = true;
            }
            else
            {
                InvalidDateSelection = false;
                if (MetParams.IsMonthEnd)
                {
                    //we will do a save immediately.  Heather does not change anything on the Conc Month End so we will want to make sure there is 
                    //a month end record.  Just going to this with month end checked will force a save and ensure there is a DMY = 2 record for conc.
                    await SaveBtnClickAsync();
                }
            }
                
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
                    recsUpdated = await Task.Run(() => DAL.Services.Met_Conc_Plant2Svc.Update(Plant2Data, conn));
                    if (recsUpdated == 0)
                        await Task.Run(() => DAL.Services.Met_Conc_Plant2Svc.Insert(Plant2Data, conn));

                    foreach (var lne in LineData)
                    {
                        recsUpdated = await Task.Run(() => DAL.Services.Met_Conc_LineSvc.Update(lne, conn));
                        if (recsUpdated == 0)
                            await Task.Run(() => DAL.Services.Met_Conc_LineSvc.Insert(lne, conn));
                    }
                }
                else
                {
                    //this is not month end, we should never need an insert
                    await Task.Run(() => DAL.Services.Met_Conc_Plant2Svc.Update(Plant2Data, conn));
                    foreach (var lne in LineData)
                        await Task.Run(() => DAL.Services.Met_Conc_LineSvc.Update(lne, conn));
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
        /// Gets the delay hours and auto fills the hours fields
        /// </summary>
        /// <returns></returns>
        private async Task FillHoursAsync()
        {
            //This will eventually be replaced with data from Corp Delay tracking so I am not going to put this in the DAL
            StringBuilder sb = new();

            
            sb.AppendLine("SELECT ROUND(SUM(CASE WHEN sched_ind = 'S' THEN DelayTime ELSE 0 END) * 24,2) SchedTotal,");
            sb.AppendLine("    ROUND(SUM(CASE WHEN sched_ind = 'U'  AND minor_code = 11 THEN DelayTime ELSE 0 END) * 24,2) UnSchedTotal,");
            sb.AppendLine("    ROUND(SUM(CASE WHEN specific_code = 354 THEN DelayTime Else 0 END) * 24,2) ImposedHiPower,");
            sb.AppendLine("    ROUND(SUM(CASE WHEN specific_code = 362 THEN DelayTime Else 0 END) * 24,2) ImposedAggReq,");
            sb.AppendLine("    ROUND(SUM(CASE WHEN specific_code = 341 THEN DelayTime Else 0 END) * 24,2) ImposedLowInv");
            sb.AppendLine("FROM (");
            
            sb.AppendLine("SELECT d.*,        ");
            sb.AppendLine("        CASE WHEN start_time > :EndDate  OR start_time IS NULL THEN :EndDate ELSE start_time END -");
            sb.AppendLine("        CASE WHEN stop_time < :StartDate THEN :StartDate ELSE stop_time END DelayTime");
            sb.AppendLine("        FROM delays d");
            sb.AppendLine("       WHERE delay_type = 'CONC' AND");
            sb.AppendLine("             substr(delay_name,7,2) in ('3','4','5','6','7','8','9','10','11','12') AND");
            sb.AppendLine("             (stop_time between  :StartDate and :EndDate");
            sb.AppendLine("              OR");
            sb.AppendLine("              start_time between :StartDate and :EndDate");
            sb.AppendLine("              OR");
            sb.AppendLine("              (stop_time < :StartDate)");
            sb.AppendLine("                 AND (start_time is null OR start_time > :EndDate))");
            sb.AppendLine(")");

            OracleDataAdapter da = new(sb.ToString(), MOO.Data.GetConnectionString(MOO.Data.MNODatabase.DMART));
            da.SelectCommand.BindByName = true;
            da.SelectCommand.Parameters.Add("StartDate", OracleDbType.Date);
            da.SelectCommand.Parameters["StartDate"].Value = MetParams.MetDate.AddMinutes(-90);
            da.SelectCommand.Parameters.Add("EndDate", OracleDbType.Date);
            da.SelectCommand.Parameters["EndDate"].Value = MetParams.MetDate.AddMinutes(-90).AddDays(1);
            DataSet ds = await Task.Run(() => MOO.Data.ExecuteQuery(da));
            if (ds.Tables[0].Rows.Count >= 0)
            {
                DataRow dr = ds.Tables[0].Rows[0];
                Plant2Data.Aggl_Request_Hours = dr.Field<decimal>("ImposedAggReq");
                Plant2Data.Sched_Maint_Hours = dr.Field<decimal>("SchedTotal");
                Plant2Data.Unsched_Maint_Hours = dr.Field<decimal>("UnschedTotal");
                Plant2Data.Hi_Power_Lim_Hours = dr.Field<decimal>("ImposedHiPower");
                Plant2Data.No_Ore_Hours = dr.Field<decimal>("ImposedLowInv");
                DataIsModified = true;

            }
        }

    }
}
