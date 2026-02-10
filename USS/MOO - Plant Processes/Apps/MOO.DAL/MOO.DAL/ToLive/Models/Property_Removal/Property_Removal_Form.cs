using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.ToLive.Models
{
    /// <summary>
    /// Model for Property removal form
    /// </summary>
    /// <remarks>
    /// This got a little messy with notmapped fields as I needed to convert these correctly.  The table wasn't quite designed with the best data types
    /// </remarks>
    public sealed class Property_Removal_Form
    {
        /// <summary>
        /// Id for reason "Other" in the database.  Other reason requires explanation
        /// </summary>
        public const int OTHER_REASON_ID = 5;

        public enum PR_Status
        {
            Voided,
            Closed,
            Open
        }

        [Key]
        public int Form_Nbr { get; set; }


        private string _plant_Initial = "M";

        [NotMapped]
        public MOO.Plant Plant
        {
            get => _plant_Initial == "M" ? MOO.Plant.Minntac : MOO.Plant.Keetac;
            set
            {
                _plant_Initial = value.ToString().Substring(0, 1);
            }
        }

        /// <summary>
        /// The first Initial of the plant
        /// </summary>
        /// <remarks>This is how the database stores it.  We will not make this available publicly</remarks>
        internal string Plant_Initial
        {
            get => _plant_Initial;
            set
            {
                if (!string.Equals("M", value) && !string.Equals("K", value))
                    throw new Exception("Invalid value for plant initial, must be 'M' or 'K'");
                _plant_Initial = value;
            }
        }

        /// <summary>
        /// Mapped to column, only used for the SQLBuilder
        /// </summary>
        internal int? Company_Name { get => Vendor?.Company_Id; }

        [NotMapped]
        public Property_Removal_Vendors Vendor { get; set; }


        public string Plant_Area { get; set; }



        /// <summary>
        /// Mapped to column, only used for the SQLBuilder
        /// </summary>
        internal int? Removal_Reason { get => Prop_Removal_Reason?.Reason_Id; }


        /// <summary>
        /// Combines the reason and reason_other column 
        /// </summary>
        [NotMapped]
        public string Removal_Reason_String { get
            {
                if (Prop_Removal_Reason == null)
                    return string.Empty;

                if (Prop_Removal_Reason.Reason_Id == OTHER_REASON_ID)
                    return $"Other - {Explain_Other}";
                else
                    return Prop_Removal_Reason.Reason;
            }
        }

        [NotMapped]
        public Property_Removal_Reasons Prop_Removal_Reason { get; set; }


        /// <summary>
        /// Mapped to column, only used for the SQLBuilder
        /// </summary>
        internal int? Authorized_By { get => Authorizer?.User_Id; }

        [NotMapped]
        public Sec_Users Authorizer { get; set; }


        public DateTime? Date_Gate { get; set; }


        /// <summary>
        /// Mapped to column, only used for the SQLBuilder
        /// </summary>
        internal int? Tracked_By { get => Tracker?.User_Id; }

        [NotMapped]
        public Sec_Users Tracker { get; set; }


        internal string _status = "O";


        /// <summary>
        /// Mapped to column, only used for the SQLBuilder
        /// </summary>
        internal string Status { get => _status; }

        [NotMapped]
        public PR_Status Prop_Removal_Status
        {
            get
            {
                return _status switch
                {
                    "O" => PR_Status.Open,
                    "C" => PR_Status.Closed,
                    "V" => PR_Status.Voided,
                    _ => throw new NotImplementedException($"Invalid value for property removal status - ({_status})"),
                };
            }
            set
            {
                switch (value)
                {
                    case PR_Status.Open:
                        _status = "O";
                        break;
                    case PR_Status.Closed:
                        _status = "C";
                        break;
                    case PR_Status.Voided:
                        _status = "V";
                        break;
                }
            }
        }



        public string Explain_Other { get; set; }


        /// <summary>
        /// Mapped to column, only used for the SQLBuilder
        /// </summary>
        internal int? Created_By { get => Creator?.User_Id; }
        [NotMapped]
        public Sec_Users Creator { get; set; }


        public DateTime? Created_Date { get; set; }
        public DateTime? Last_Updated_Date { get; set; }
        public string Vendor_Contact { get; set; }
        public DateTime? Close_Date { get; set; }


        /// <summary>
        /// Mapped to column, only used for the SQLBuilder
        /// </summary>
        internal int? Last_Updated_By { get => Updater?.User_Id; }
        [NotMapped]
        public Sec_Users Updater { get; set; }


        /// <summary>
        /// Mapped to column, only used for the SQLBuilder
        /// </summary>
        internal int? Close_By { get => Closer?.User_Id; }
        [NotMapped]
        public Sec_Users Closer { get; set; }


        /// <summary>
        /// Mapped to column, only used for the SQLBuilder
        /// </summary>
        internal int? Open_By { get => Opener?.User_Id; }
        [NotMapped]
        public Sec_Users Opener { get; set; }

        public DateTime? Open_Date { get; set; }
        public DateTime? Void_Date { get; set; }


        /// <summary>
        /// Mapped to column, only used for the SQLBuilder
        /// </summary>
        internal int? Void_By { get => Voider?.User_Id; }
        [NotMapped]
        public Sec_Users Voider { get; set; }

        private string _Was_Printed = "False";


        /// <summary>
        /// Mapped to column, only used for the SQLBuilder
        /// </summary>
        internal string Was_Printed { get => _Was_Printed; }

        [NotMapped]
        public bool Printed
        {
            get
            {
                return bool.Parse(_Was_Printed);
            }
            set
            {
                _Was_Printed = value ? "True" : "False";
            }
        }

        public bool To_Be_Replaced { get; set; } = false;

    }
}
