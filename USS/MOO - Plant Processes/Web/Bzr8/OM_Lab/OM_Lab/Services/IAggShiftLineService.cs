using System;
using System.Threading.Tasks;

namespace OM_Lab.Services
{
    public interface IAggShiftLineService
    {
        /// <summary>
        /// Returns the pellet long tons and grate hours for the specified shift report date, shift,
        /// and pellet line.  Uses Agg2 data for lines 3–5 and Agg3 data for lines 6–7.
        /// </summary>
        /// <param name="reportDate">The shift report date.</param>
        /// <param name="shift">The 8-hour shift number (1–3).</param>
        /// <param name="line">The pellet line number (3–7).</param>
        Task<(decimal? PelLtons, decimal? GrateHrs)> GetPelTonsAndGrateHrsAsync(
            DateTime reportDate, byte shift, byte line);
    }
}
