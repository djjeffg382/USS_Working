using OM_Lab.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OM_Lab.Services
{
    public interface ITumblrsService
    {
        /// <summary>
        /// Returns the next shift date, shift number, and half (1 or 2) after
        /// the most recently entered Tumblrs record.  Used to default the
        /// 4th Floor Lab Analyst to the next entry period on page load.
        /// </summary>
        Task<(DateTime date, int shift, int half)> GetNextShiftHalfAfterLastEntryAsync();

        /// <summary>
        /// Returns the shift date, shift number, and half of the most recent
        /// Tumblrs record that has not yet been authorized.  Used to default
        /// Ore Movement Coordinator roles to the next record needing authorization.
        /// </summary>
        Task<(DateTime date, int shift, int half)> GetLatestUnauthorizedShiftHalfAsync();

        /// <summary>
        /// Loads the Before-Tumbles and After-Tumbles <c>Lab_Phys_Analysis</c> records for
        /// lines 3–7 that fall within the specified shift date, shift number, and within-shift
        /// half (1 or 2).  Returns a dictionary keyed by line number; lines with no database
        /// record are returned with all weight fields set to <c>null</c> so the UI can present
        /// empty entry fields for insertion.
        /// </summary>
        /// <param name="shiftDate">The 8-hour shift date (Shift_Date8).</param>
        /// <param name="shiftNumber">Shift number 1–3.</param>
        /// <param name="half">Within-shift half: 1 (first 4 hrs) or 2 (second 4 hrs).</param>
        Task<Dictionary<int, TumblrLineData>> GetTumblrLinesAsync(DateTime shiftDate, int shiftNumber, int half);
        /// <summary>
        /// Saves all Tumblr lines for the specified shift date, shift number, and half to the database.
        /// Performs an INSERT for lines with no existing record (id == 0) and an UPDATE otherwise.
        /// When <paramref name="authorizedBy"/> is provided, the <c>Authorized_By</c> field is set
        /// to that value (used by Save &amp; Authorize for OM Coordinators).
        /// </summary>
        /// <param name="shiftDate">The 8-hour shift date.</param>
        /// <param name="shiftNumber">Shift number 1–3.</param>
        /// <param name="half">Within-shift half: 1 (first 4 hrs) or 2 (second 4 hrs).</param>
        /// <param name="lines">The line data dictionary keyed by line number (3–7).</param>
        /// <param name="username">Current user's login name; stored in <c>Last_Update_By</c>.</param>
        /// <param name="authorizedBy">When non-null, stored in <c>Authorized_By</c>.</param>
        Task SaveTumblrLinesAsync(DateTime shiftDate, int shiftNumber, int half,
            Dictionary<int, TumblrLineData> lines, string username, string? authorizedBy = null);
    }
}
