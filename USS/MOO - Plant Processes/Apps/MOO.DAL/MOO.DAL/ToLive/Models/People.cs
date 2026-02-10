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
    public class People
    {
        [Key]
        public int Person_Id { get; set; }
        public string Last_Name { get; set; }
        public string First_Name { get; set; }
        public string Middle_Name { get; set; }
        public string Employee_Number { get; set; }
        public int? Work_Location_Code { get; set; }
        public int? Supervisor_Person_Id { get; set; }
        public string Low_Level_Group { get; set; }
        public string Base_Level_Group { get; set; }
        public string Cost_Center { get; set; }
        public string Status_Ind { get; set; }
        public string Windows_Ad_Account { get; set; }
        public string Mobile_Number { get; set; }
        public string Office_Ext { get; set; }
        public string Home_Number { get; set; }

        /// <summary>
        /// Displayed as John Hogan Doe
        /// </summary>
        [NotMapped]
        [ReadOnly(true)]
        public string Full_Name { get
            {
                if (string.IsNullOrEmpty(Middle_Name))
                {
                    return First_Name + " " + Last_Name;
                }
                else
                {
                    return First_Name + " " + Middle_Name + " " + Last_Name;
                }
               
            }
        }

        /// <summary>
        /// Will not show full middle name
        /// </summary>
        [NotMapped]
        [ReadOnly(true)]
        public string Full_Name_Middle_Initial
        {
            get
            {
                if (string.IsNullOrEmpty(Middle_Name))
                {
                    return First_Name + " " + Last_Name;
                }
                else
                {
                    return First_Name + " " + Middle_Name[..1] + " " + Last_Name;
                }
                
            }
        }

        /// <summary>
        /// Name display as 'John H Doe (456321)'
        /// Only shows middle initial
        /// </summary>
        [NotMapped]
        [ReadOnly(true)]
        public string Full_Name_WithID {
            get
            {
                return Full_Name_Middle_Initial + " (" + (Employee_Number ?? Person_Id.ToString()) + ")";
            }
        }
    }
}
