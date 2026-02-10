using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.ToLive.Models
{
    public class KTC_Blast
    {
        public DateTime Blasted_Date { get; set; }

        /// <summary>
        /// Used to maintain the old value in case of change as this is part of the primary key
        /// </summary>
        internal string Old_Bench_Number { get; set; }
        /// <summary>
        /// Used to maintain the old value in case of change as this is part of the primary key
        /// </summary>
        internal string Old_Blast_Number { get; set; }

        private string _bench_Number;
        public string Bench_Number { get { return _bench_Number; }
            set
            {
                _bench_Number= value;
                if (string.IsNullOrEmpty(Old_Bench_Number))
                    Old_Bench_Number = value;
            }
        }

        private string _blast_Number;
        public string Blast_Number
        {
            get { return _blast_Number; }
            set
            {
                _blast_Number = value;
                if (string.IsNullOrEmpty(Old_Blast_Number))
                    Old_Blast_Number = value;
            }
        }

        public decimal? Blasted_Time { get; set; }
        public string Material { get; set; }
        public decimal? Material_Factor { get { 
                if (Material == "Taconite")
                {
                    return Convert.ToDecimal(11.2);
                }
                else
                {
                    return 27;
                }
            } }
        public string Property_Location { get; set; }
        public decimal? Total_Ft { get; set; }
        public decimal? Total_Sub_Ft { get; set; }
        public decimal? Actual_Total_Ft { get {
                return Total_Ft + Total_Sub_Ft;
            } }
        public decimal? Holes { get; set; }
        public decimal? Hole_Size_In { get; set; }
        public string Drill { get; set; }
        public decimal? Depth_Deep_Avg_Ft { get; set; }
        public decimal? Depth_Shallow_Avg { get; set; }
        public decimal? Area_Deep { get; set; }
        public decimal? Area_Shallow { get; set; }
        public decimal? Tons_Deep { get {
                return Depth_Deep_Avg_Ft * Area_Deep / Material_Factor;
            } }
        public decimal? Tons_Shallow { get {
                return Depth_Shallow_Avg * Area_Shallow / Material_Factor;
            } }
        public decimal? Tons_Total
        {
            get
            {
                return Tons_Deep + Tons_Shallow;
                    
            }
        }
        public decimal? Blasted_Deep_Gt { get; set; }
        public decimal? Blasted_Shallow_Gt { get; set; }
        public decimal? Stemming { get; set; }
        public string Full_Column_Load { get; set; }
        public decimal? Noise_North_Ktc_Db { get; set; }
        public decimal? Noise_West_Ktc_Db { get; set; }
        public string Burden_And_Spacing { get; set; }
        public string Subgrade_Ft { get; set; }
        public decimal? Explosive1_Lb { get; set; }
        public decimal? Explosive1_Uc { get; set; }
        public decimal? Explosive1_Lb_Ext_Cost { get {
                return Explosive1_Lb * Explosive1_Uc;
            } }
        public decimal? Explosive2_Lb { get; set; }
        public decimal? Explosive2_Uc { get; set; }
        public decimal? Explosive2_Lb_Ext_Cost { get { 
                return Explosive2_Lb * Explosive2_Uc;
            } }
        public decimal? Primer_1lb { get; set; }
        public decimal? Primer_1lb_Uc { get; set; }
        public decimal? Primer_1lb_Ext_Cost { get { 
                return Primer_1lb * Primer_1lb_Uc;
            } }

        public decimal? Total_Explosive_Pounds
        {
            get
            {
                return Explosive1_Lb + Explosive2_Lb + Primer_1lb;
            }
        }

        public decimal? A_Chord_Ft { get; set; }
        public decimal? A_Chord_Ft_Uc { get; set; }
        public decimal? A_Chord_Ft_Ext_Cost { get { 
                return A_Chord_Ft * A_Chord_Ft_Uc;
            } }

        public decimal? Ez_Trunkline_40ft { get; set; }
        public decimal? Ez_Trunkline_40ft_Uc { get; set; }
        public decimal? Ez_Trunkline_40ft_Ext_Cost { get { 
                return Ez_Trunkline_40ft * Ez_Trunkline_40ft_Uc;
            } }
        public decimal? Ez_Trunkline_60ft { get; set; }
        public decimal? Ez_Trunkline_60ft_Uc { get; set; }
        public decimal? Ez_Trunkline_60ft_Ext_Cost { get { 
                return Ez_Trunkline_60ft * Ez_Trunkline_60ft_Uc;
            } }

        public decimal? Primadets_30ft { get; set; }
        public decimal? Primadets_30ft_Uc { get; set; }
        public decimal? Primadets_30ft_Ext_Cost { get { 
                return Primadets_30ft * Primadets_30ft_Uc;
            } }

        public decimal? Primadets_40ft { get; set; }
        public decimal? Primadets_40ft_Uc { get; set; }
        public decimal? Primadets_40ft_Ext_Cost { get { 
                return Primadets_40ft * Primadets_40ft_Uc;
            } }

        public decimal? Primadets_50ft { get; set; }
        public decimal? Primadets_50ft_Uc { get; set; }
        public decimal? Primadets_50ft_Ext_Cost { get {
                return Primadets_50ft * Primadets_50ft_Uc;
            } }

        public decimal? Total_Primadets
        {
            get
            {
                return Primadets_30ft + Primadets_40ft + Primadets_50ft;
            }
        }

        public decimal? Caps_6ft { get; set; }
        public decimal? Caps_6ft_Uc { get; set; }
        public decimal? Caps_6ft_Ext_Cost { get { 
                return Caps_6ft * Caps_6ft_Uc;
            } }

        public decimal? Blasting_Wire_Ft { get; set; }
        public decimal? Blasting_Wire_Ft_Uc { get; set; }
        public decimal? Blasting_Wire_Ft_Ext_Cost { get { 
                return Blasting_Wire_Ft * Blasting_Wire_Ft_Uc;
            } }

        public string Blasters { get; set; }
        public string Forman { get; set; }
        public string Engineer { get; set; }
        public string Survey { get; set; }
        public string Cc { get; set; }
        public string Additional_Db_Site { get; set; }
        public decimal? Additional_Db { get; set; }
        public decimal Electric_Det_15_Met { get; set; }
        public decimal Electric_Det_15_Met_Uc { get; set; }
        public decimal Electric_Det_15_Met_Ext_Cost { get { 
                return Electric_Det_15_Met * Electric_Det_15_Met_Uc;
            } }
        public decimal Electric_Det_20_Met { get; set; }
        public decimal Electric_Det_20_Met_Uc { get; set; }
        public decimal Electric_Det_20_Met_Ext_Cost { get { 
                return Electric_Det_20_Met * Electric_Det_20_Met_Uc;
            } }

        public decimal M35_Bus_Line_Ft { get; set; }
        public decimal M35_Bus_Line_Ft_Uc { get; set; }

        public decimal M35_Bus_Line_Ft_Ext_Cost { get {
                return M35_Bus_Line_Ft * M35_Bus_Line_Ft_Uc;
            } }

        public decimal? Total_Accessory_Cost
        {
            get
            {
                return A_Chord_Ft_Ext_Cost + Ez_Trunkline_40ft_Ext_Cost + Ez_Trunkline_60ft_Ext_Cost + Primadets_30ft_Ext_Cost +
                    Primadets_40ft_Ext_Cost + Primadets_50ft_Ext_Cost + Electric_Det_15_Met_Ext_Cost + Electric_Det_20_Met_Ext_Cost +
                    Caps_6ft_Ext_Cost + Blasting_Wire_Ft_Ext_Cost + M35_Bus_Line_Ft_Ext_Cost;
            }
        }


        public decimal? Total_Explosive_Cost
        {
            get
            {
                return Explosive1_Lb_Ext_Cost + Explosive2_Lb_Ext_Cost + Primer_1lb_Ext_Cost;
            }
        }

        public decimal? Blast_Grand_Total
        {
            get
            {
                return Total_Explosive_Cost + Total_Accessory_Cost;
            }
        }

        public decimal? Lb_GTon
        {
            get
            {
                return  Total_Explosive_Pounds / Tons_Total;
            }
        }

        public decimal? Cost_GTon
        {
            get
            {
                return Blast_Grand_Total / Tons_Total;
            }
        }

        public decimal? GTon_Foot
        {
            get
            {
                return Tons_Total / Actual_Total_Ft;
            }
        }
    }
}
