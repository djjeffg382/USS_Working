using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.ToLive.Models
{
    public sealed class Sec_Web_Menu_Links
    {
        public int Wml_Id { get; set; }
        public int Web_Menu_Id { get; set; }
        public int? Parent_Id { get; set; }
        public string Display_Text { get; set; }
        public short? Sort_Order { get; set; }
        public string Url { get; set; }
        public bool Open_New_Window { get; set; }
        public string Roles { get; set; }
        public bool No_Access_Show { get; set; }
        public string Tooltip { get; set; }
        public bool Is_Iis_Id_App { get; set; }
        public bool Active { get; set; }
        public string Sy_Program_Code { get; set; }
        public string Image_Url { get; set; }
        public bool Recurse_Folder { get; set; }
        public Enums.SecWebMenuType Menu_Type { get; set; }
        public string Modified_By { get; set; }
        public bool? Include_In_Global_Menu { get; set; }
        public List<Sec_Web_Menu_Links> Children { get; set; } = new();
        public IEnumerable<string> RoleList { 
            get {
                if(string.IsNullOrEmpty(Roles))
                    return new List<string>();
                else
                {
                    var retVal = Roles.Split(',').ToList();
                    if (retVal == null)
                        return new List<string>();
                    else
                        return retVal;
                }
                
            }
            set { 
                StringBuilder sb = new StringBuilder();
                foreach (var item in value)
                {
                    if(sb.Length > 0) 
                        sb.Append(",");
                    sb.Append(item);

                }
                Roles = sb.ToString();
            }
        }
    }
}
