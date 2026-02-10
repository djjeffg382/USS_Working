using MOO.DAL.ERP.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.ERP.Models
{
    public sealed class RAWRec
    {
        public RAWRec(Material RawMaterialName, string RawMaterialWeight, string Units, string MeasurementType,
            string PropertyCode = "", string PropertyGroupCode = "", string LeaseCode = "", string LeaseCodeName = "",
            int? LessorCode = null, string LessorGroupCodeName = "")
        {
            this.RawMaterialName = RawMaterialName;
            this.RawMaterialWeight = RawMaterialWeight;
            this.RawMaterialWeightUOM = Units;
            this.RawMaterialWeightMeasurementType = MeasurementType;
            this.PropertyCode = PropertyCode;
            this.PropertyGroupCode = PropertyGroupCode;
            this.LeaseCode = LeaseCode;
            this.LeaseCodeName = LeaseCodeName;
            this.LessorCode = LessorCode;
            this.LessorGroupCodeName = LessorGroupCodeName;
        }

        public string StartOfRecord { get { return "RAW:"; } }
        public Material RawMaterialName { get; set; }
        public string RawMaterialWeight { get; set; }
        public string RawMaterialWeightUOM { get; set; }
        public string RawMaterialWeightMeasurementType { get; set; }
        public string PropertyCode { get; set; }
        public string PropertyGroupCode { get; set; }
        public string LeaseCode { get; set; }
        public string LeaseCodeName { get; set; }
        public int? LessorCode { get; set; }
        public string LessorGroupCodeName { get; set; }



    }
}
