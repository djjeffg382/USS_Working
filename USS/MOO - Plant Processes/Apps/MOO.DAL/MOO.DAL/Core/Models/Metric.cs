using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MOO.DAL.Core.Enums;

namespace MOO.DAL.Core.Models
{
    public class Metric
    {
        public int Metric_Id { get; set; }
        public string Metric_Name { get; set; }
        public string FullNameWithPath { get { return Process_Level?.Path + "->" + Metric_Name; } }
        /// <summary>
        /// Unit Of Measure
        /// </summary>
        public Uom Uom { get; set; }
        public Metric_Type Metric_Type { get; set; }
        public Collection_Type Coll_Type { get; set; }
        public Collection_Time Coll_Time { get; set; }
        public Process_Level Process_Level { get; set; }

        /// <summary>
        /// The SCADA Tag Name used for this
        /// </summary>
        public string Tag_Name { get; set; }
        public decimal? Warn_Min { get; set; }
        public decimal? Warn_Max { get; set; }
        public decimal? Error_Min { get; set; }
        public decimal? Error_Max { get; set; }
        public decimal Approvable { get; set; }
        public Scada Scada { get; set; }
        public decimal? Decimal_Places { get; set; }
        public decimal? Default_Group_Id { get; set; }
        public string Wh_Adjust_Field { get; set; }
        public decimal? Value_Type { get; set; }
        public bool Isactive { get; set; }
        public string Metric_Comments { get; set; }

        /// <summary>
        /// List of users that have access to modify this metric value
        /// </summary>
        public List<string> UserList { get; set; } = new();

        /// <summary>
        /// List of users that have access to modify this metric value
        /// </summary>
        public string UserListCSV
        {
            get {  return string.Join("," , UserList.ToArray()); }
            set
            {
                try
                {
                    if (string.IsNullOrEmpty(value))
                        UserList.Clear();
                    else
                        UserList = value.Split(',').ToList();
                }
                catch (Exception)
                {

                    throw;
                } 
            }
        }

        /// <summary>
        /// List of roles that have access to modify this metric value
        /// </summary>
        public List<string> RoleList { get; set; } = new();


        /// <summary>
        /// List of roles that have access to modify this metric value
        /// </summary>
        public string RoleListCSV
        {
            get { return string.Join(",", RoleList.ToArray()); }
            set
            {
                try
                {
                    if (string.IsNullOrEmpty(value))
                        RoleList.Clear();
                    else
                        RoleList = value.Split(',').ToList();
                }
                catch (Exception)
                {

                    throw;
                }
            }
        }



    }
}
