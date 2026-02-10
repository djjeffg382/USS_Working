using MOO.DAL.ToLive.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.ToLive.Models
{
    public sealed class Maint_Calendar
    {
        public int Event_Id { get; set; }
        public string Title { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        /// <summary>
        /// Used for the calendar gui.
        /// </summary>
        /// <remarks>Example Start date of Jan 1 and end date of Jan 2 needs to show as a block of 2 days on calendar.  We need to have the end time be  Jan 2 23:59</remarks>
        public DateTime? EndTime
        {
            get
            {
                if (EndDate.HasValue)
                    return EndDate.Value.AddDays(1).AddMilliseconds(-1);
                return null;
            }
        }

        public MaintCalArea Area { get; set; }
        public string AreaStr { get
            {
                return MOO.DAL.ToLive.Services.Maint_CalendarSvc.GetAreaName(Area);
            } 
        } 
        public string Description { get; set; }
        public bool Major { get; set; }

    }
}
