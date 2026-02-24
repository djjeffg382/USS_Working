namespace OM_Lab.Models
{
    /// <summary>
    /// Holds the default mesh-fraction values applied when a user checks "Defaults Used"
    /// on the Tumblrs entry page and selects either "Acid" or "Flux".
    ///
    /// Values are split by pellet type (Acid / Flux) and by line group
    /// (Line 3 alone vs. Lines 4–7).  BT = Before-Tumbles, AT = After-Tumbles.
    ///
    /// NOTE: The values below are placeholders.  Replace them with the
    ///       confirmed production defaults once they are supplied.
    /// </summary>
    public static class TumblrDefaults
    {
        // ── Acid – Line 3 ───────────────────────────────────────────────────
        public static TumblrLineData AcidLine3() => new()
        {
            TotWt    = 500, TotWt_AT  = 500,
            Mesh916_BT = 50,  Mesh916_AT = 45,
            Mesh12_BT  = 100, Mesh12_AT  = 95,
            Mesh716_BT = 150, Mesh716_AT = 140,
            Mesh38_BT  = 100, Mesh38_AT  = 95,
            Mesh14_BT  = 50,  Mesh14_AT  = 50,
            Mesh28M_BT = 25,  Mesh28M_AT = 25,
        };

        // ── Acid – Lines 4–7 ────────────────────────────────────────────────
        public static TumblrLineData AcidLines4To7() => new()
        {
            TotWt    = 500, TotWt_AT  = 500,
            Mesh916_BT = 55,  Mesh916_AT = 50,
            Mesh12_BT  = 105, Mesh12_AT  = 100,
            Mesh716_BT = 145, Mesh716_AT = 135,
            Mesh38_BT  = 100, Mesh38_AT  = 95,
            Mesh14_BT  = 50,  Mesh14_AT  = 50,
            Mesh28M_BT = 20,  Mesh28M_AT = 20,
        };

        // ── Flux – Line 3 ───────────────────────────────────────────────────
        public static TumblrLineData FluxLine3() => new()
        {
            TotWt    = 500, TotWt_AT  = 500,
            Mesh916_BT = 60,  Mesh916_AT = 55,
            Mesh12_BT  = 115, Mesh12_AT  = 110,
            Mesh716_BT = 140, Mesh716_AT = 130,
            Mesh38_BT  = 95,  Mesh38_AT  = 90,
            Mesh14_BT  = 45,  Mesh14_AT  = 45,
            Mesh28M_BT = 20,  Mesh28M_AT = 20,
        };

        // ── Flux – Lines 4–7 ────────────────────────────────────────────────
        public static TumblrLineData FluxLines4To7() => new()
        {
            TotWt    = 500, TotWt_AT  = 500,
            Mesh916_BT = 65,  Mesh916_AT = 60,
            Mesh12_BT  = 120, Mesh12_AT  = 115,
            Mesh716_BT = 135, Mesh716_AT = 125,
            Mesh38_BT  = 90,  Mesh38_AT  = 85,
            Mesh14_BT  = 45,  Mesh14_AT  = 45,
            Mesh28M_BT = 20,  Mesh28M_AT = 20,
        };

        /// <summary>
        /// Returns the appropriate default data for <paramref name="lineNumber"/> and
        /// <paramref name="defaultType"/> ("Acid" or "Flux").
        /// </summary>
        public static TumblrLineData Get(int lineNumber, string defaultType)
        {
            bool isLine3 = lineNumber == 3;
            bool isAcid  = string.Equals(defaultType, "Acid", StringComparison.OrdinalIgnoreCase);

            return (isLine3, isAcid) switch
            {
                (true,  true)  => AcidLine3(),
                (true,  false) => FluxLine3(),
                (false, true)  => AcidLines4To7(),
                (false, false) => FluxLines4To7(),
            };
        }
    }
}
