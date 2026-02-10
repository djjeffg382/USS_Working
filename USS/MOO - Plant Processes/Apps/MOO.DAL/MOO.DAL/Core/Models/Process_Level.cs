using MOO.DAL.Core.Enums;
using MOO.DAL.ToLive.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.Core.Models
{
    public class Process_Level
    {
        public MOO.Plant Site { get; set; }
        public decimal Process_Level_Id { get; set; }
        public string Process_Level_Name { get; set; }
        public string Path { get; set; }
        public string Abbreviation { get; set; }
        public string Passport_Id { get; set; }
        public decimal? Parent_Id { get; set; }
        public Process_Type Process_Type { get; set; }
        public List<Process_Level> Children { get; set; } = new();
    }
}
