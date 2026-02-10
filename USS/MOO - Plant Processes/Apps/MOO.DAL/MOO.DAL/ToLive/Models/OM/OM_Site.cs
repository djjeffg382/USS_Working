using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MOO.Enums.Extension;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace MOO.DAL.ToLive.Models
#pragma warning restore IDE0130 // Namespace does not match folder structure
{
    public sealed class OM_Site
    {
        
        public enum SiteTypeEnum
        {
            [Display(Name = "Mine", Description = "Mine")]
            Mine = 1,
            [Display(Name = "Dock", Description = "Dock")]
            Dock = 2,
            [Display(Name = "USS Plant", Description = "USS Plant")]
            UssPlant = 3,
            [Display(Name = "External Customer", Description = "External Customer")]
            External = 4,
            [Display(Name = "Other", Description = "Other")]
            Other = 99
        }

        [Key]
        public int Site_Id { get; set; }
        public string Site_Name { get; set; }

        [NotMapped]
        public SiteTypeEnum SiteType { 
            get { 
                return (SiteTypeEnum)Site_Type_Id; 
            } set
            {
                Site_Type_Id = (short)value;
            }
        }

        /// <summary>
        /// mapped to the Location_Type_Id field
        /// </summary>
        public short Site_Type_Id { get; set; } = 1;

        public bool Active { get; set; } = true;

    }
}
