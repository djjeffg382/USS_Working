using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using OM_Lab.Models;
using OM_Lab.Services;
using Radzen;
using Radzen.Blazor;

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

        [Inject]
        private DialogService DialogService { get; set; } = null!;

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
        /// Resets all entry values for the specified line without saving.
        /// Visible only to OM Coordinators.
        /// </summary>
        private void ClearLine(int line)
        {
            ResetLine(line);
            StateHasChanged();
        }

        /// <summary>
        /// Resets all editable values for a line, clearing DefaultsUsed state and all weight fields.
        /// </summary>
        private void ResetLine(int line)
        {
            Lines[line] = new TumblrLineData { LineNumber = line };
        }

        /// <summary>
        /// Called when the "Defaults Used" checkbox changes for a line.
        /// On check: opens the Acid/Flux selection dialog; on successful selection,
        /// fills the line with the appropriate defaults and makes the fields read-only.
        /// On uncheck: clears the defaults and restores editable fields.
        /// </summary>
        private async Task DefaultsUsedChanged(int line, bool isChecked)
        {
            if (isChecked)
            {
                // Open the Acid / Flux selection dialog.
                var result = await DialogService.OpenAsync<OM_Lab.Components.Dialogs.TumblrDefaultsDialog>(
                    "Select Default Type",
                    options: new DialogOptions { Width = "300px", CloseDialogOnOverlayClick = false });

                if (result is string defaultType)
                {
                    // Fetch the appropriate placeholder defaults.
                    var defaults = TumblrDefaults.Get(line, defaultType);

                    // Copy all weight fields from defaults into the current line.
                    var current = Lines[line];
                    current.TotWt      = defaults.TotWt;
                    current.TotWt_AT   = defaults.TotWt_AT;
                    current.Mesh916_BT = defaults.Mesh916_BT;
                    current.Mesh916_AT = defaults.Mesh916_AT;
                    current.Mesh12_BT  = defaults.Mesh12_BT;
                    current.Mesh12_AT  = defaults.Mesh12_AT;
                    current.Mesh716_BT = defaults.Mesh716_BT;
                    current.Mesh716_AT = defaults.Mesh716_AT;
                    current.Mesh38_BT  = defaults.Mesh38_BT;
                    current.Mesh38_AT  = defaults.Mesh38_AT;
                    current.Mesh14_BT  = defaults.Mesh14_BT;
                    current.Mesh14_AT  = defaults.Mesh14_AT;
                    current.Mesh28M_BT = defaults.Mesh28M_BT;
                    current.Mesh28M_AT = defaults.Mesh28M_AT;
                    current.DefaultsUsed = true;
                    current.DefaultType  = defaultType;
                }
                else
                {
                    // User cancelled – leave the checkbox unchecked.
                    Lines[line].DefaultsUsed = false;
                }
            }
            else
            {
                // Uncheck: clear the default values and re-enable editing.
                ResetLine(line);
            }

            StateHasChanged();
        }

        /// <summary>
        /// Returns the HTML tabindex value so that tab order moves vertically down each line
        /// before advancing to the next line.
        /// Fields per line: TotWt_BT(1) + TotWt_AT(1) + 6 mesh rows × 2 (BT+AT)(12) + PelTons(1) + GrateHrs(1) = 16.
        /// </summary>
        private const int FieldsPerLine = 16;
        private static int GetTabIndex(int line, int fieldIndex) => (line - 3) * FieldsPerLine + fieldIndex + 1;
    }
}
