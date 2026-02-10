using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.ToLive.Models
{
    public class KTC_Blast_Unit_Price
    {
        public DateTime Last_Updated { get; set; }
        public decimal Explosive1_UC { get; set; }
        public decimal Explosive2_UC { get; set; }
        public decimal Primer_1lb_UC { get; set; }
        public decimal A_Chord_Ft_UC { get; set; }
        public decimal EZ_Trunkline_40ft_UC { get; set; }
        public decimal EZ_Trunkline_60ft_UC { get; set; }
        public decimal Primadets_30ft_UC { get; set; }
        public decimal Primadets_40ft_UC { get; set; }
        public decimal Primadets_50ft_UC { get; set; }
        public decimal Caps_6ft_UC { get; set; }
        public decimal Blasting_Wire_Ft_UC { get; set; }
        public string Blasters { get; set; }
        public string Forman { get; set; }
        public string Engineer { get; set; }
        public string Survey { get; set; }
        public string CC { get; set; }
        public decimal Electric_Det_15_Met_UC { get; set; }
        public decimal Electric_Det_20_Met_UC { get; set; }
        public decimal M35_Bus_Line_Ft_UC { get; set; }
    }
}
