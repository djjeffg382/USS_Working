using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.ToLive.Models
{
    public sealed class Lab_Compression
    {
        public enum LabCompressionType
        {
            [Display(Name = "Other")]
            Other = 0,
            [Display(Name = "Minntac Agglomerator Pellet")]
            MtcPellet = 1,
            [Display(Name = "Keetac Pellet")]
            KtcPellet = 2
        }


        [Key]
        public int Comp_Id { get; set; }
        public DateTime Created_Date { get; set; } = DateTime.Now;
        public short? Line_Nbr { get; set; }
        public short Test_Nbr { get; set; }
        public DateTime? Shift_Date { get; set; }
        public short? Shift { get; set; }
        public short Instrument { get; set; }
        public double? Comp200 { get; set; }
        public double? Comp300 { get; set; }
        public double? Average { get; set; }
        public short? Shift_Half { get; set; }
        public LabCompressionType? Compression_Type { get; set; }
        public string Comments { get; set; }

        [NotMapped]
        public int? Seq_Nbr { get { return Instrument * 100 + Test_Nbr; } }

    }
}
