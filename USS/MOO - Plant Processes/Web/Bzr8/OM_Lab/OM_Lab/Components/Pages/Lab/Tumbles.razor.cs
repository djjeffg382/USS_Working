using System.Linq;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using OM_Lab.Models;
using OM_Lab.Services;
using Radzen;
using Radzen.Blazor;

namespace OM_Lab.Components.Pages.Lab
{
    public partial class Tumbles
    {
        private DateTime SelectedDate = DateTime.Today;
        private int SelectedShift = 1;
        private int SelectedHalf = 1;

        // Prepopulate keys 3..7 so Razor can safely index Lines[line] on first render.
        private Dictionary<int, TumblrLineData> Lines = Enumerable.Range(3, 5)
            .ToDictionary(i => i, i => new TumblrLineData { LineNumber = i });

        // ── Role state ────────────────────────────────────────────────────────
        /// <summary>True when the current user is a 4th Floor Lab Analyst (M_LAB_4TH).</summary>
        private bool IsLabAnalyst { get; set; } = false;

        /// <summary>True when the current user is an Ore Movement Coordinator role.</summary>
        private bool IsOmCoordinator { get; set; } = true;

        /// <summary>
        /// When true, all entry fields are disabled.
        /// For M_LAB_4TH, this is set to true after a successful save.
        /// </summary>
        private bool IsReadOnly { get; set; }

        [Inject]
        private AuthenticationStateProvider AuthenticationStateProvider { get; set; } = null!;

        [Inject]
        private ITumblesService TumblrsService { get; set; } = null!;

        [Inject]
        private DialogService DialogService { get; set; } = null!;

        protected override async Task OnInitializedAsync()
        {
           // await DetermineUserRoleAsync();
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
                    // Guard: if the service returned an invalid or zero shift, default to 1
                    SelectedShift = (shift >= 1 && shift <= 3) ? shift : 1;
                    // Guard: ensure half is 1 or 2
                    SelectedHalf = (half == 1 || half == 2) ? half : 1;
                }
                else if (IsOmCoordinator)
                {
                    var (date, shift, half) = await TumblrsService.GetLatestUnauthorizedShiftHalfAsync();
                    SelectedDate = date;
                    SelectedShift = (shift >= 1 && shift <= 3) ? shift : 1;
                    SelectedHalf = (half == 1 || half == 2) ? half : 1;
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
                var fetched = await TumblrsService.GetTumblrLinesAsync(SelectedDate, SelectedShift, SelectedHalf);

                // Merge fetched results into the Lines dictionary, ensuring keys 3..7 exist.
                for (int line = 3; line <= 7; line++)
                {
                    if (fetched != null && fetched.TryGetValue(line, out var ld))
                    {
                        Lines[line] = ld;
                    }
                    else
                    {
                        // Ensure a default entry exists so the UI can safely index Lines[line]
                        Lines[line] = new TumblrLineData { LineNumber = line };
                    }
                }

                // If any record for this half-shift is already authorized, treat the whole shift as locked/read-only
                bool shiftLocked = fetched != null && fetched.Values.Any(l => l.BtAuthorized || l.AtAuthorized);

                // For coordinators, the read-only state should reflect whether the target shift is locked.
                // For lab analysts, preserve their read-only state (they cannot navigate anyway).
                if (IsOmCoordinator)
                {
                    IsReadOnly = shiftLocked;
                }
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

        private async Task SaveData()
        {
            var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            string username = authState.User.Identity?.Name ?? string.Empty;
            await PersistDataAsync(username, authorizedBy: null);

            // M_LAB_4TH: lock the form after saving so the record cannot be modified.
            if (IsLabAnalyst)
                IsReadOnly = true;
        }

        private async Task SaveAndAuthorizeData()
        {
            var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            string username = authState.User.Identity?.Name ?? string.Empty;
            await PersistDataAsync(username, authorizedBy: username);
        }

        /// <summary>
        /// Persists all line data to the database via <see cref="ITumblesService"/>.
        /// Passes <paramref name="authorizedBy"/> only when called from Save &amp; Authorize.
        /// </summary>
        private async Task PersistDataAsync(string username, string? authorizedBy)
        {
            try
            {
                await TumblrsService.SaveTumblrLinesAsync(
                    SelectedDate, SelectedShift, SelectedHalf, Lines, username, authorizedBy);

                // Reload so record IDs are up-to-date for any subsequent save.
                await LoadData();

                notificationService.Notify(new NotificationMessage
                {
                    Severity  = NotificationSeverity.Success,
                    Summary   = authorizedBy != null ? "Saved & Authorized" : "Saved",
                    Detail    = authorizedBy != null
                        ? $"Data saved and authorized by {authorizedBy}."
                        : "Data saved successfully.",
                    Duration  = 3000
                });
            }
            catch (Exception ex)
            {
                notificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary  = "Save Failed",
                    Detail   = ex.Message,
                    Duration = 5000
                });
            }
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
            if (!Lines.ContainsKey(line))
                Lines[line] = new TumblrLineData { LineNumber = line };

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
