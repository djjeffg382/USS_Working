using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
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
            InitLines();
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

        private void LoadData()
        {
            // TODO: Load data from database for SelectedDate / SelectedShift / SelectedHalf
            InitLines();
            // For M_LAB_4TH, reset read-only state when navigating to a new slot
            if (IsLabAnalyst)
                IsReadOnly = false;
            StateHasChanged();
        }

        private void Recalc(int line)
        {
            // Calculated properties update automatically; trigger re-render
            StateHasChanged();
        }

        private void PrevDate() { SelectedDate = SelectedDate.AddDays(-1); LoadData(); }
        private void NextDate() { SelectedDate = SelectedDate.AddDays(1); LoadData(); }

        private void PrevShift() { SelectedShift = SelectedShift == 1 ? 3 : SelectedShift - 1; LoadData(); }
        private void NextShift() { SelectedShift = SelectedShift == 3 ? 1 : SelectedShift + 1; LoadData(); }

        private void PrevHalf() { SelectedHalf = SelectedHalf == 1 ? 2 : 1; LoadData(); }
        private void NextHalf() { SelectedHalf = SelectedHalf == 2 ? 1 : 2; LoadData(); }

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

        public class TumblrLineData
        {
            public int LineNumber { get; set; }

            // ── Editable fields ───────────────────────────────────────────────────
            public decimal? TotWt { get; set; }

            public decimal? Mesh916_BT { get; set; }
            public decimal? Mesh916_AT { get; set; }

            public decimal? Mesh12_BT { get; set; }
            public decimal? Mesh12_AT { get; set; }

            public decimal? Mesh716_BT { get; set; }
            public decimal? Mesh716_AT { get; set; }

            public decimal? Mesh38_BT { get; set; }
            public decimal? Mesh38_AT { get; set; }

            public decimal? Mesh14_BT { get; set; }
            public decimal? Mesh14_AT { get; set; }

            public decimal? Mesh28M_BT { get; set; }
            public decimal? Mesh28M_AT { get; set; }

            public decimal? PelTons { get; set; }
            public decimal? GrateHrs { get; set; }

            // ── Calculated fields ─────────────────────────────────────────────────
            public decimal? Minus28M_BT => TotWt.HasValue
                ? TotWt - (Mesh916_BT.GetValueOrDefault()
                          + Mesh12_BT.GetValueOrDefault()
                          + Mesh716_BT.GetValueOrDefault()
                          + Mesh38_BT.GetValueOrDefault()
                          + Mesh14_BT.GetValueOrDefault()
                          + Mesh28M_BT.GetValueOrDefault())
                : null;

            public decimal? Minus28M_AT => TotWt.HasValue
                ? TotWt - (Mesh916_AT.GetValueOrDefault()
                          + Mesh12_AT.GetValueOrDefault()
                          + Mesh716_AT.GetValueOrDefault()
                          + Mesh38_AT.GetValueOrDefault()
                          + Mesh14_AT.GetValueOrDefault()
                          + Mesh28M_AT.GetValueOrDefault())
                : null;

            private decimal? Cum14_BT => TotWt.HasValue && TotWt > 0
                ? Math.Round(100m * (Mesh916_BT.GetValueOrDefault()
                                   + Mesh12_BT.GetValueOrDefault()
                                   + Mesh716_BT.GetValueOrDefault()
                                   + Mesh38_BT.GetValueOrDefault()
                                   + Mesh14_BT.GetValueOrDefault()) / TotWt.Value, 2)
                : null;

            private decimal? Cum14_AT => TotWt.HasValue && TotWt > 0
                ? Math.Round(100m * (Mesh916_AT.GetValueOrDefault()
                                   + Mesh12_AT.GetValueOrDefault()
                                   + Mesh716_AT.GetValueOrDefault()
                                   + Mesh38_AT.GetValueOrDefault()
                                   + Mesh14_AT.GetValueOrDefault()) / TotWt.Value, 2)
                : null;

            public string Cum14Display
            {
                get
                {
                    if (Cum14_BT.HasValue && Cum14_AT.HasValue)
                        return $"{Cum14_BT:N2}/{Cum14_AT:N2}";
                    if (Cum14_BT.HasValue)
                        return $"{Cum14_BT:N2}/";
                    if (Cum14_AT.HasValue)
                        return $"/{Cum14_AT:N2}";
                    return string.Empty;
                }
            }
        }
    }
}
