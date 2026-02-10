using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.ERP.Models
{
    /// <summary>
    /// Basic model for holding data by date and values
    /// </summary>
    public sealed class ERPValues
    {
        public DateTime Shift_Date { get; set; }
        public int? Line { get; set; }
        public decimal Value { get; set; }





        /// <summary>
        /// Gets Date, Values for a given Metric ID grouped by shift_date
        /// </summary>
        /// <param name="MetricId"></param>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <returns></returns>
        public static List<ERPValues> GetMetricValuesByShiftDate(int MetricId, DateTime StartDate, DateTime EndDate)
        {

            var metricFore = MOO.DAL.Core.Services.Metric_ValueSvc.GetByShiftDate(MetricId, StartDate, EndDate);

            var fore = (from rec in metricFore
                        group rec by rec.Shift_Date into grp

                        select new ERPValues
                        {
                            Shift_Date = grp.Key.Value,
                            Value = grp.Sum(x => x.Value).GetValueOrDefault(0)
                        }
                        ).OrderBy(x => x.Shift_Date).ToList();
            return fore;
        }

        /// <summary>
        /// Converts the values in the MinVu Plans to a list of ERPValues
        /// </summary>
        /// <param name="Plant">Minntac/Keetac</param>
        /// <param name="StartDate">Begin Date</param>
        /// <param name="EndDate">End Date</param>
        /// <param name="TagIds">MinVu Tag Id's</param>
        /// <returns></returns>
        public static List<ERPValues> GetMinVuPlansByShiftDate(MOO.Plant Plant, DateTime StartDate, DateTime EndDate, int[] TagIds)
        {
            var plans = MOO.DAL.MinVu.Services.ForecastSvc.GetByDateRange(Plant,StartDate, 1, EndDate,2, TagIds);

            var fore = (from rec in plans
                        group rec by rec.ShiftDate into grp

                        select new ERPValues
                        {
                            Shift_Date = grp.Key,
                            Value = (decimal)grp.Sum(x => x.ForecastValue)
                        }
                        ).OrderBy(x => x.Shift_Date).ToList();
            return fore;
        }
    }
}
