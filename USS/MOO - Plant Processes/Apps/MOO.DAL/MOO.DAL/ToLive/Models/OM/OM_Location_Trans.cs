using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MOO.DAL.ToLive.Models.OM_Site;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace MOO.DAL.ToLive.Models
#pragma warning restore IDE0130 // Namespace does not match folder structure
{
    public sealed class OM_Location_Trans
    {
        [Key]
        public int Location_Trans_Id { get; set; }
        public DateTime Transaction_Date { get; set; }

        /// <summary>
        /// Location/Source Types
        /// </summary>
        /// <remarks>IMPORTANT! This must match OM_ENUM type "Location_Trans_Src/Dest_Type"</remarks>
        public enum LocTransSrcDestType
        {
            [Display(Name = "Location", Description = "Location")]
            Location = 1,
            [Display(Name = "Ship", Description = "Ship")]
            Ship = 2,
            [Display(Name = "Train", Description = "Train")]
            Train = 3,
            [Display(Name = "Adjustment In", Description = "Adjustment In")]
            AdjustIn = 4,
            [Display(Name = "Adjustment Out", Description = "Adjustment Out")]
            AdjustOut = 5
        }

        [NotMapped]
        public LocTransSrcDestType SourceType
        {
            get
            {
                return (LocTransSrcDestType)Source_Type_Id;
            }
            set
            {
                Source_Type_Id = (short)value;
            }
        }

        public short Source_Type_Id { get; set; }
        public int? Source_Id { get; set; }


        [NotMapped]
        public LocTransSrcDestType DestType
        {
            get
            {
                return (LocTransSrcDestType)Dest_Type_Id;
            }
            set
            {
                Dest_Type_Id = (short)value;
            }
        }
        public short Dest_Type_Id { get; set; }
        public int? Dest_Id { get; set; }
        public double Quantity { get; set; }
        public double? Fe { get; set; }

    }
}
