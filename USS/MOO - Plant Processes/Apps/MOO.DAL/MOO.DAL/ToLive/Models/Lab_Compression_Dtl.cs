using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.ToLive.Models
{
    public sealed class Lab_Compression_Dtl
    {
        [Key]
        public int Pellet_Id { get; set; }
        public DateTime Comp_Date { get; set; }
        public double Comp_Lbs { get; set; }
        public short Pellet_Nbr { get; set; }
        public int Comp_Id { get; set; }

    }
}
