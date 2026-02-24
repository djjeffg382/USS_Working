using System;
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
    }
}
