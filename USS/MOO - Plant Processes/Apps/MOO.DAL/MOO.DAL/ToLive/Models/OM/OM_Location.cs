using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace MOO.DAL.ToLive.Models
#pragma warning restore IDE0130 // Namespace does not match folder structure
{
    public sealed class OM_Location
    {

        [Key]
        public int Location_Id { get; set; }

        [NotMapped]
        public OM_Site Site { get; set; }

        public int Site_id { get { return Site.Site_Id; } }

        public string Location_Name { get; set; }

        public enum LocationTypeEnum
        {
            [Display(Name = "Stockpile",Description ="Stockpile")]
            Stockpile = 1,
            [Display(Name = "Shiploader", Description = "Shiploader")]
            Shiploader = 2,
            [Display(Name = "Surge Pile", Description = "Surge Pile")]
            SurgePile = 3
        }

        [NotMapped]
        public LocationTypeEnum LocationType
        {
            get
            {
                return (LocationTypeEnum)Location_Type_Id;
            }
            set
            {
                Location_Type_Id = (short)value;
            }
        }

        /// <summary>
        /// mapped to the Location_Type_Id field
        /// </summary>
        public short Location_Type_Id { get; set; } = 1;

        public bool Active { get; set; } = true;

    }
}
