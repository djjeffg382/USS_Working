using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using OM_Lab.Models;
using OM_Lab.Services;
using Radzen;

namespace OM_Lab.Components.Pages.Lab
{
    public partial class Tumblrs
    {
        private DateTime SelectedDate = DateTime.Today;
        private int SelectedShift = 1;
        private int SelectedHalf = 1;

        private Dictionary<int, TumblrLineData> Lines = new();

        // ── Role state ────────────────────────────────────────────────────────
        /// <summary>True when the current user is a 4th Floor Lab Analyst (M_LAB_4TH).</summary>
        private bool IsLabAnalyst { get; set; }

        /// <summary>True when the current user is an Ore Movement Coordinator role.</summary>
        private bool IsOmCoordinator { get; set; }

        /// <summary>
        /// When true, all entry fields are disabled.
        /// For M_LAB_4TH, this is set to true after a successful save.
        /// </summary>
        private bool IsReadOnly { get; set; }

        [Inject]
        private AuthenticationStateProvider AuthenticationStateProvider { get; set; } = null!;

        [Inject]
        private ITumblrsService TumblrsService { get; set; } = null!;

        protected override async Task OnInitializedAsync()
        {
            await DetermineUserRoleAsync();
            await ApplyRoleDefaultsAsync();
            await LoadData();
            await base.OnInitializedAsync();
        }

        /// <summary>Populates <see cref="IsLabAnalyst"/> and <see cref="IsOmCoordinator"/>
        /// using the current user's claims.</summary>
        private async Task DetermineUserRoleAsync()
        {
            var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;

            IsLabAnalyst = user.IsInRole("M_LAB_4TH");
            IsOmCoordinator = user.IsInRole("M_OM_ADMIN")
                           || user.IsInRole("M_OM_COOR")
                           || user.IsInRole("M_OM_MET")
                           || user.IsInRole("M_OM_MGR");
        }

        /// <summary>Sets the default Date/Shift/Half based on role:
        /// <list type="bullet">
        ///   <item>M_LAB_4TH — defaults to the shift/half after the last entered record.</item>
        ///   <item>OM Coordinator — defaults to the latest unauthorized record.</item>
        /// </list>
        /// </summary>
        private async Task ApplyRoleDefaultsAsync()
        {
            try
            {
                if (IsLabAnalyst)
                {
                    var (date, shift, half) = await TumblrsService.GetNextShiftHalfAfterLastEntryAsync();
                    SelectedDate = date;
                    SelectedShift = shift;
                    SelectedHalf = half;
                }
                else if (IsOmCoordinator)
                {
                    var (date, shift, half) = await TumblrsService.GetLatestUnauthorizedShiftHalfAsync();
                    SelectedDate = date;
                    SelectedShift = shift;
                    SelectedHalf = half;
                }
            }
            catch
            {
                // If the query fails (e.g. no DB in dev), keep today's defaults.
            }
        }

        private void InitLines()
        {
            Lines.Clear();
            for (int line = 3; line <= 7; line++)
                Lines[line] = new TumblrLineData { LineNumber = line };
        }

        private async Task LoadData()
        {
            // Show empty fields immediately while the DB query runs.
            InitLines();
            if (IsLabAnalyst)
                IsReadOnly = false;
            StateHasChanged();

            try
            {
                Lines = await TumblrsService.GetTumblrLinesAsync(SelectedDate, SelectedShift, SelectedHalf);
            }
            catch
            {
                // If the query fails (e.g. no DB in dev), keep the empty fields.
            }

            StateHasChanged();
        }

        private void Recalc(int line)
        {
            // Calculated properties update automatically; trigger re-render
            StateHasChanged();
        }

        private async Task PrevDate() { SelectedDate = SelectedDate.AddDays(-1); await LoadData(); }
        private async Task NextDate() { SelectedDate = SelectedDate.AddDays(1); await LoadData(); }

        private async Task PrevShift() { SelectedShift = SelectedShift == 1 ? 3 : SelectedShift - 1; await LoadData(); }
        private async Task NextShift() { SelectedShift = SelectedShift == 3 ? 1 : SelectedShift + 1; await LoadData(); }

        private async Task PrevHalf() { SelectedHalf = SelectedHalf == 1 ? 2 : 1; await LoadData(); }
        private async Task NextHalf() { SelectedHalf = SelectedHalf == 2 ? 1 : 2; await LoadData(); }

        private void SaveData()
        {
            // TODO: Persist data to database (see issue #7)
            notificationService.Notify(new NotificationMessage
            {
                Severity = NotificationSeverity.Info,
                Summary = "Save",
                Detail = "Save functionality not yet implemented.",
                Duration = 3000
            });

            // M_LAB_4TH: lock the form after saving so the record cannot be modified.
            if (IsLabAnalyst)
                IsReadOnly = true;
        }

        private void SaveAndAuthorizeData()
        {
            // TODO: Persist data and set Authorized_By to current user (see issues #7/#8)
            notificationService.Notify(new NotificationMessage
            {
                Severity = NotificationSeverity.Info,
                Summary = "Save & Authorize",
                Detail = "Save and Authorize functionality not yet implemented.",
                Duration = 3000
            });
        }

        /// <summary>
        /// Returns the HTML tabindex value so that tab order moves vertically down each line
        /// before advancing to the next line.
        /// Fields per line: TotWt(1) + 6 mesh rows × 2 (BT+AT)(12) + PelTons(1) + GrateHrs(1) = 15.
        /// </summary>
        private const int FieldsPerLine = 15;
        private static int GetTabIndex(int line, int fieldIndex) => (line - 3) * FieldsPerLine + fieldIndex + 1;
    }
}
