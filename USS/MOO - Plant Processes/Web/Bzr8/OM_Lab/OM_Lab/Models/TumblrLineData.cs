namespace OM_Lab.Models
{
    /// <summary>
    /// Holds all editable and calculated values for one pellet line (3–7) in a single Tumblrs
    /// half-shift entry.  Instances of this class are populated by <see cref="Services.ITumblrsService"/>
    /// from the <c>Lab_Phys_Analysis</c> table (BT = type 14, AT = type 15) and are used
    /// directly by the Tumblrs Blazor page for binding.
    /// </summary>
    public class TumblrLineData
    {
        public int LineNumber { get; set; }

        // ── Database record identifiers (0 = not yet persisted) ──────────────
        /// <summary>Primary key of the Before-Tumbles Lab_Phys_Analysis row, or 0 if new.</summary>
        public int BtAnalysisId { get; set; }

        /// <summary>Analysis date of the Before-Tumbles record; null for new (unpersisted) records.</summary>
        public DateTime? BtAnalysisDate { get; set; }

        /// <summary>Primary key of the After-Tumbles Lab_Phys_Analysis row, or 0 if new.</summary>
        public int AtAnalysisId { get; set; }

        /// <summary>Analysis date of the After-Tumbles record; null for new (unpersisted) records.</summary>
        public DateTime? AtAnalysisDate { get; set; }

        // ── Editable fields ───────────────────────────────────────────────────
        /// <summary>Start weight from the Before-Tumbles record (also used as the AT denominator when no AT start weight is present).</summary>
        public decimal? TotWt { get; set; }

        /// <summary>Start weight from the After-Tumbles record. When null, AT calculations fall back to <see cref="TotWt"/>.</summary>
        public decimal? TotWt_AT { get; set; }

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

        // ── Defaults-Used state ────────────────────────────────────────────────
        /// <summary>True when the "Defaults Used" checkbox is checked for this line.</summary>
        public bool DefaultsUsed { get; set; }

        /// <summary>"Acid" or "Flux" – set when <see cref="DefaultsUsed"/> is true, otherwise null.</summary>
        public string? DefaultType { get; set; }

        // ── Calculated fields ─────────────────────────────────────────────────
        public decimal? Minus28M_BT => TotWt.HasValue
            ? TotWt - (Mesh916_BT.GetValueOrDefault()
                      + Mesh12_BT.GetValueOrDefault()
                      + Mesh716_BT.GetValueOrDefault()
                      + Mesh38_BT.GetValueOrDefault()
                      + Mesh14_BT.GetValueOrDefault()
                      + Mesh28M_BT.GetValueOrDefault())
            : null;

        public decimal? Minus28M_AT
        {
            get
            {
                decimal? atStartWt = TotWt_AT ?? TotWt;
                return atStartWt.HasValue
                    ? atStartWt - (Mesh916_AT.GetValueOrDefault()
                                  + Mesh12_AT.GetValueOrDefault()
                                  + Mesh716_AT.GetValueOrDefault()
                                  + Mesh38_AT.GetValueOrDefault()
                                  + Mesh14_AT.GetValueOrDefault()
                                  + Mesh28M_AT.GetValueOrDefault())
                    : null;
            }
        }

        private decimal? Cum14_BT => TotWt.HasValue && TotWt > 0
            ? Math.Round(100m * (Mesh916_BT.GetValueOrDefault()
                               + Mesh12_BT.GetValueOrDefault()
                               + Mesh716_BT.GetValueOrDefault()
                               + Mesh38_BT.GetValueOrDefault()
                               + Mesh14_BT.GetValueOrDefault()) / TotWt.Value, 2)
            : null;

        private decimal? Cum14_AT
        {
            get
            {
                decimal? atStartWt = TotWt_AT ?? TotWt;
                return atStartWt.HasValue && atStartWt > 0
                    ? Math.Round(100m * (Mesh916_AT.GetValueOrDefault()
                                       + Mesh12_AT.GetValueOrDefault()
                                       + Mesh716_AT.GetValueOrDefault()
                                       + Mesh38_AT.GetValueOrDefault()
                                       + Mesh14_AT.GetValueOrDefault()) / atStartWt.Value, 2)
                    : null;
            }
        }

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
