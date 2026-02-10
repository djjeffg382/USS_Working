using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.Core.Models.KTCLab
{
    public class PelletLab
    {
        

        public DateTime Start_Date_No_DST { get; set; }
        public DateTime Start_Date { get; set; }

        public DateTime SampleTime { get { return Start_Date.AddHours(2); } }

        /// <summary>
        /// Shift Number
        /// </summary>
        public byte? Shift { get; set; }

        /// <summary>
        /// Shift Half
        /// </summary>
        public byte? Half { get; set; }

        /// <summary>
        /// Shift hour
        /// </summary>
        public byte? Hour { get; set; }

        /// <summary>
        /// Shift Date
        /// </summary>
        public DateTime? Shift_Date { get; set; }

        /// <summary>
        /// boolean of whether defaults were used on this half shift
        /// </summary>
        public bool DefaultsUsed { get; set; }

        /*********************************************************Before Tumbles***********************************************/

        public decimal? Bt_StartingWeight { get; set; }

        /// <summary>
        /// Before Tumbles plus 1/2 inch
        /// </summary>
        public decimal? Bt_12 { get; set; }

        /// <summary>
        /// Before Tumbles plus 7/16
        /// </summary>
        public decimal? Bt_716 { get; set; }

        /// <summary>
        /// Before Tumbles plus 3/8 inch
        /// </summary>
        public decimal? Bt_38 { get; set; }

        /// <summary>
        /// Before Tumbles plus 1/4 Inch
        /// </summary>
        public decimal? Bt_14 { get; set; }

        public decimal? Bt_Plus14Pct { get
            {
                if (Bt_14.HasValue && Bt_StartingWeight.HasValue && Bt_StartingWeight.Value > 0)
                    return (Bt_14.Value / Bt_StartingWeight.Value) * 100;
                else
                    return null;
            } 
        }

        public decimal? Bt_Plus12Pct
        {
            get
            {
                if (Bt_12.HasValue && Bt_StartingWeight.HasValue && Bt_StartingWeight.Value > 0)
                    return (Bt_12.Value / Bt_StartingWeight.Value) * 100;
                else
                    return null;
            }
        }


        /*********************************************************After Tumbles***********************************************/
        public decimal? At_StartingWeight { get; set; }

        /// <summary>
        /// After Tumbles plus 1/2 inch
        /// </summary>
        public decimal? At_12 { get; set; }

        /// <summary>
        /// After Tumbles plus 7/16
        /// </summary>
        public decimal? At_716 { get; set; }

        /// <summary>
        /// After Tumbles plus 3/8 inch
        /// </summary>
        public decimal? At_38 { get; set; }

        /// <summary>
        /// After Tumbles plus 1/4 Inch
        /// </summary>
        public decimal? At_14 { get; set; }

        /// <summary>
        /// After Tumbles + 30 Mesh
        /// </summary>
        public decimal? At_30Mesh { get; set; }


        public decimal? At_Plus14Pct
        {
            get
            {
                if (At_14.HasValue && At_StartingWeight.HasValue && At_StartingWeight.Value > 0)
                    return (At_14.Value / At_StartingWeight.Value) * 100;
                else
                    return null;
            }
        }

        public decimal? At_Minus30MeshPct
        {
            get
            {
                if (At_30Mesh.HasValue && At_StartingWeight.HasValue && At_StartingWeight.Value > 0)
                    return 100 - ( (At_30Mesh.Value / At_StartingWeight.Value) * 100);
                else
                    return null;
            }
        }

        //**********************************************************************Pellet Compression***************************************************//
        public decimal? CompressionLbs { get; set; }
        public decimal? CompLess300 { get; set; }
        public decimal? CompLess200 { get; set; }
        public decimal? PctMagPell { get; set; }

        /********************************************************************XRF Data*************************************************************/
        public decimal? PellFe { get; set; }
        public decimal? PellSiO2 { get; set; }
        public decimal? PellCao { get; set; }
        public decimal? PellMoisture { get; set; }
        public decimal? PellAl2O3 { get; set; }
        public decimal? PellMgO { get; set; }
        public decimal? PellMn { get; set; }

        public decimal? PellFerrous { get; set; }
        public Approval Approval { get; set; }



    }
}
