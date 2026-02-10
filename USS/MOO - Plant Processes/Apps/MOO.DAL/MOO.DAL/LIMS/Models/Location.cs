using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.LIMS.Models
{
    public class Location
    {
        //note: I did not put every field from the DB in here.  Only put fields we may use
        public string Identity { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Location_Type { get; set; }
        public Location Parent_Location { get; set; }

        public string FullLocation
        {
            get
            {
                string retVal = Name;
                if (Parent_Location != null)
                {
                    retVal += "/" + Parent_Location.Name;
                    if (Parent_Location.Parent_Location != null)
                    {
                        retVal += "/" + Parent_Location.Parent_Location.Name;
                    }
                }
                return retVal;
            }
        }
    }
}
