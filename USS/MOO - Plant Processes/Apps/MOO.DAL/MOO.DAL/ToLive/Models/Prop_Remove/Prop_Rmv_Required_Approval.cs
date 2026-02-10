using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.ToLive.Models
{
    /// <summary>
    /// This class will be used to get the list of users required to approve a Property Removal Form.
    /// </summary>
    /// <remarks>NOTE: This is NOT linked to a table</remarks>
    public sealed class Prop_Rmv_Required_Approval
    {

        /// <summary>
        /// Determines if this form is approved based on the approval list and if all are required for approval
        /// </summary>
        public bool IsApproved { get
            {
                bool retVal = false;
                //check if there is an admin approval, this overrides all
                var adm = ApprovalList.Count(x => x.AdminOverride);
                if (adm > 0)
                    return true;

                if (AllRequired)
                {
                    //if all are required, it is approved if we have at least one approval user and the count of approved equals the total count
                    retVal = ApprovalList.Count > 0 &&
                            (ApprovalList.Count(x => x.Approved) == ApprovalList.Count);
                }
                else
                {
                    //we only need one approval
                    retVal = ApprovalList.Count(x => x.Approved) > 0;
                }
                return retVal;
            }
        }

        public class ApprovalUser
        {
            public Sec_Users User { get; set; }
            public bool Approved { get; set; } = false;
            public DateTime? ApprovedDate { get; set; }
            public bool AdminOverride { get; set; } = false;
        }

        /// <summary>
        /// Determines whether all users are required to approve
        /// </summary>
        public bool AllRequired { get; set; }

        /// <summary>
        /// List of users that can or must approve the Property Removal Form
        /// </summary>
        public List<ApprovalUser> ApprovalList { get; set; } = [];

    }
}
