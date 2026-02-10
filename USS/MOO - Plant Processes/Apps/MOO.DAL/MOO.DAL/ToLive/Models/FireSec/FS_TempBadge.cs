using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.ToLive.Models
{
    public class FS_TempBadge
    {
        public long Temp_Id { get; set; }
        public People Employee_Id { get; set; }
        public People Manager_Id { get; set; }
        public string Validbadge { get; set; }
        public string C_Name { get; set; }
        public string C_Supername { get; set; }
        public string C_Companyname { get; set; }
        public MOO.Plant Plantloc { get; set; }
        public DateTime Msha_Expdate { get; set; }
        public long Temp_Badgenbr { get; set; }
        public DateTime Temp_Badge_Issued { get; set; }
        public DateTime? Temp_Badge_Return { get; set; }
        public string Sec_Officer { get; set; }
        public string Sec_Officerreturn { get; set; }
        public string Reasonfor { get; set; }
        public string Is_Emp_Conc { get; set; }
        public string Filename { get; set; }
        public Temp_Badge_Department Department { get; set; }

        public string Temp_Badge_Name
        {
            get
            {
                if (Employee_Id == null)
                {
                    return C_Name;
                }
                else
                {
                    return Employee_Id.Full_Name_WithID;

                }
            }
        }

        public string Temp_Badge_Supervisor_Name
        {
            get
            {
                if (Manager_Id == null)
                {
                    return C_Supername;
                }
                else
                {
                    return Manager_Id.Full_Name_WithID;

                }
            }
        }

    }
}
