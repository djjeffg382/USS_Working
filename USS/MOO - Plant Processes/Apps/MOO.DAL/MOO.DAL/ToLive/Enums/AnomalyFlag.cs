using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.ToLive.Enums
{
    /// <summary>
    /// Enum list for flag values in TOLIVE.Anomalies table
    /// </summary>
    /// <remarks>This will be used in MOO.Blazor.AnomaliesView.  Use Description for the detailed table and Dispay(name=...) for the top table
    /// Many of the Descriptions will be the same as display</remarks>
    public enum AnomalyFlag
    {
        [Display(Name = "Line Down")]
        [Description("Line Down")]
        LineDown = -1,

        [Display(Name = "Normal")]
        [Description("Normal")]
        Normal = 0,

        [Display(Name = "Sensor Missing")]
        [Description("Sensor Missing")]
        SensorMissing = 1,

        [Display(Name = "Out Of Bounds")]
        [Description("Out Of Bounds")]
        OutOfBounds = 2,

        [Display(Name = "Flatlined")]
        [Description("Flatlined")]
        FlatLined = 3,

        [Display(Name ="Out Of Bounds")]
        [Description("Shift In Mean")]        
        ShiftInMean = 4,

        [Display(Name = "Out Of Bounds")]
        [Description("Linear Residual")]
        LinearResidual = 5,

        [Display(Name = "Saturated")]
        [Description("Saturated")]
        Saturated = 6


    }
}
