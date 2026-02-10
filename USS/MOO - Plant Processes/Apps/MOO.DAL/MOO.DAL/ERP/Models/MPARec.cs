using MOO.DAL.ERP.Enums;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MOO.DAL.ERP.Models
{
    public sealed class MPARec
    {
        public MPARec(string ProductionUnit, ProcessCode ProcCode, Material ERPMaterial, Activity ERPActivity, string Value, string Units, string Measurement) {
            this.ProductionUnit = ProductionUnit;
            this.ProcessCode = ProcCode;
            this.MaterialName = ERPMaterial;
            this.ActivityName = ERPActivity;
            this.Weight = Value;
            this.WeightUOM = Units;
            this.WeightMeasurementType = Measurement;
        }

        public string StartOfRecord { get { return "MPA:"; } }
        public string ProductionUnit { get; set; }
        public ProcessCode ProcessCode { get; set; }
        public string MaterialLotIdentity {  get{  return "000";  } }
        public Material MaterialName { get; set; }
        public Activity ActivityName { get; set; }
        public string Weight { get; set; }
        public string WeightUOM { get; set; }
        public string WeightMeasurementType { get; set; }
    }
}
