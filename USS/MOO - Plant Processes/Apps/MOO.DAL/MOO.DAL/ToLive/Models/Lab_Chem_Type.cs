using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.ToLive.Models
{
    public sealed class Lab_Chem_Type
    {
        public short Lab_Chem_Type_Id { get; set; }
        public Plant Plant { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        /// <summary>
        /// The tables the data will be forwarded to in DMART for backwards compatibility
        /// </summary>
        public string Forwarded_To { get; set; }

    }
}
