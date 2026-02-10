using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.ToLive.Models
{
    public sealed class ERP_Shipping_RZ_Orders
    {
        private MOO.Plant _Plant = Plant.Minntac;

        public MOO.Plant Plant { get => _Plant; 
                                set { _Plant = value; } 
                                }

        public short Works { get
            {
                if (_Plant == Plant.Minntac)
                    return 635;
                else if (_Plant == Plant.Keetac)
                    return 641;
                else
                    throw new Exception($"Unknown plant {_Plant}");
            }
            set
            {
                if (value == 635)
                    _Plant = Plant.Minntac;
                else if (value == 641)
                    _Plant = Plant.Keetac;
                else
                    throw new Exception($"Invalid Works Value - {value}.  Must be either 635 (Minntac) or 641 (Keetac)");
                    }
        }
               

        public string Order_Num { get; set; }
        public int Date_Ent { get; set; }
        public short Time_Ent { get; set; }
        public string Unit_Type { get; set; }
        public bool Active { get; set; }
        public string Frt_Terms { get; set; }
        public short? Expire_Date { get; set; }
        public int? Customer { get; set; }
        public short? Location { get; set; }
        public string Consignee { get; set; }
        public string Destination { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string Country { get; set; }
        public string Description { get; set; }
        public long? Total_Order_Wgt { get; set; }
        public long? Wgt_Remaining { get; set; }
        public string Forced_Referral { get; set; }
        public string Ref_English { get; set; }
        public string Ref_Date { get; set; }
        public long? Ebs_Order { get; set; }
        public string Ebs_Line { get; set; }
        public string Descrete_Org { get; set; }
        public string Ebs_Item { get; set; }
        public string Non_Ebs { get; set; }

    }
}
