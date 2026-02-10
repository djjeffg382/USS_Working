using System;

namespace MOO.DAL.ToLive.Models
{
    public class CAD_Drawings
    {
        public decimal Drawing_Id { get; set; }
        public MOO.Plant Plant { get; set; }
        public string Area { get; set; }
        public string Title { get; set; }

        internal string _File_Location;
        /// <summary>
        /// DEPRECATED - Use full_path
        /// </summary>
        public string File_Location { get { return _File_Location; } }
        public string Full_Path { get; set; }
        public string Drawing_Number { get; set; }
        public string Drawing_Name { get; set; }
        public DateTime? Drawing_Date { get; set; }
        public string Plant_Location { get; set; }
        public string Print_Location { get; set; }
        public string Reference_Number { get; set; }
        public string Equip_Number { get; set; }
        internal DateTime? _Last_Modified;
        public DateTime? Last_Modified { get { return _Last_Modified; } }
        public string Last_Modified_By { get; set; }
        public string Comments { get; set; }

    }
}
