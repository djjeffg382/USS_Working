using MOO.DAL.ToLive.Services;
using System;
using System.Threading.Tasks;

namespace OM_Lab.Services
{
    public class AggShiftLineService : IAggShiftLineService
    {
        /// <inheritdoc />
        public async Task<(decimal? PelLtons, decimal? GrateHrs)> GetPelTonsAndGrateHrsAsync(
            DateTime reportDate, byte shift, byte line)
        {
            return await Task.Run(() =>
            {
                if (line >= 3 && line <= 5)
                {
                    var rec = Agg2_Shift_LineSvc.Get(reportDate, shift, line);
                    return rec == null
                        ? ((decimal?)null, (decimal?)null)
                        : (rec.Pel_Ltons, rec.Grate_Hrs);
                }

                if (line == 6 || line == 7)
                {
                    var rec = Agg3_Shift_LineSvc.Get(reportDate, shift, line);
                    return rec == null
                        ? ((decimal?)null, (decimal?)null)
                        : (rec.Pel_Ltons, rec.Grate_Hrs);
                }

                return ((decimal?)null, (decimal?)null);
            });
        }
    }
}
