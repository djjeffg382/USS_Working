using MOO.DAL.ToLive.Models;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.ToLive.Services
{
    public static class Prop_Rmv_Form_ApprovalSvc
    {
        static Prop_Rmv_Form_ApprovalSvc()
        {
            Util.RegisterOracle();
        }
        internal const string TABLE_NAME = "TOLIVE.Prop_Rmv_Form_Approval";

        public static async Task<Prop_Rmv_Form_Approval> GetAsync(int Prop_Rmv_Form_Approval_Id)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine($"WHERE Prop_Rmv_Form_Approval_Id = :Prop_Rmv_Form_Approval_Id");
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.Parameters.Add("Prop_Rmv_Form_Approval_Id", Prop_Rmv_Form_Approval_Id);
            await conn.OpenAsync();
            var rdr = await cmd.ExecuteReaderAsync();

            Prop_Rmv_Form_Approval retVal = null;
            if (rdr.HasRows)
            {
                await rdr.ReadAsync();
                retVal = DataRowToObject(rdr);
            }

            return retVal;
        }


        public static async Task<List<Prop_Rmv_Form_Approval>> GetByFormId(int FormId)
        {
            StringBuilder sql = new();
            sql.Append(GetSelect());
            sql.AppendLine($"WHERE Prop_Rmv_form_id = :FormId");
            using OracleConnection conn = new(MOO.Data.GetConnectionString(Data.MNODatabase.DMART));
            OracleCommand cmd = new(sql.ToString(), conn);
            cmd.Parameters.Add("FormId", FormId);

            await conn.OpenAsync();
            var rdr = await cmd.ExecuteReaderAsync();
            List<Prop_Rmv_Form_Approval> retVal = [];

            while (await rdr.ReadAsync())
            {
                var obj = DataRowToObject(rdr);
                retVal.Add(obj);
            }
            return retVal;
        }

        /// <summary>
        /// Gets a list of required approvals for this form and who has approved it.
        /// </summary>
        /// <param name="FormId"></param>
        /// <returns></returns>
        public static async Task<Prop_Rmv_Required_Approval> GetRequiredApprovals(int FormId)
        {
            Prop_Rmv_Required_Approval retVal = new();
            //Get a list of who has approved this so far
            var approvals = await GetByFormId(FormId);
            //figure out if this form needs to have one approval or all approval.
            var form = await Prop_Rmv_FormSvc.GetAsync(FormId);
            retVal.AllRequired = form.Reason.Req_Special_Approval;
            //add to the approval list anyone who has already approved
            foreach (var approval in approvals)
            {
                Prop_Rmv_Required_Approval.ApprovalUser newRec = new()
                {
                    Approved = true,
                    ApprovedDate = approval.Approval_Date,
                    User = approval.Approval_User,
                    AdminOverride = approval.Admin_Override
                    
                };
                retVal.ApprovalList.Add(newRec);
            }

            if (form.Reason.Req_Special_Approval)
            {
                //the approval list is the reason approval list
                var reqApprovals = await Prop_Rmv_Reason_ApproverSvc.GetByReason(form.Reason.Prop_Rmv_Reason_Id);
                foreach(var reqApprover in reqApprovals)
                {
                    if (reqApprover.User.Active)
                    {
                        
                        //check if this user is already in the list as already approved
                        var findMe = retVal.ApprovalList.FirstOrDefault(x => x.User.User_Id == reqApprover.User.User_Id);                                                
                        if (findMe == null)
                        {
                            //uesr is not in the list of approvers yet, add it now with approved set to false
                            Prop_Rmv_Required_Approval.ApprovalUser newRec = new()
                            {
                                User = reqApprover.User,
                                Approved = false
                            };
                            retVal.ApprovalList.Add(newRec);
                        }
                    }
                }
            }
            else
            {
                //this form requires one approval from the area
                var reqApprovalsArea = await Prop_Rmv_Area_ApproverSvc.GetByArea(form.Area.Prop_Rmv_Area_Id);
                foreach (var reqApprover in reqApprovalsArea)
                {
                    if (reqApprover.User.Active)
                    {
                        var findMe = retVal.ApprovalList.FirstOrDefault(x => x.User.User_Id == reqApprover.User.User_Id);
                        if (findMe == null)
                        {
                            //uesr is not in the list of approvers yet, add it now with approved set to false
                            Prop_Rmv_Required_Approval.ApprovalUser newRec = new()
                            {
                                User = reqApprover.User,
                                Approved = false
                            };
                            retVal.ApprovalList.Add(newRec);
                        }
                    }
                }
            }
            return retVal;
        }


        public static string GetSelect()
        {
            StringBuilder sql = new();
            sql.Append("SELECT ");
            sql.AppendLine(SqlBuilder.GetColumnsForSelect(typeof(Prop_Rmv_Form_Approval), "prfa", "prfa_") + ",");
            sql.AppendLine(SqlBuilder.GetColumnsForSelect(typeof(Sec_Users), "su", "su_") );
            sql.AppendLine($"FROM {TABLE_NAME} prfa");
            sql.AppendLine("JOIN tolive.sec_users su");
            sql.AppendLine("    ON prfa.approval_user_id = su.user_id");


            return sql.ToString();
        }


        public static async Task<int> InsertAsync(Prop_Rmv_Form_Approval obj)
        {
            if (obj.Prop_Rmv_Form_Apv_Id <= 0)
                obj.Prop_Rmv_Form_Apv_Id = Convert.ToInt32(await MOO.Data.GetNextSequenceAsync("TOLIVE.SEQ_PROP_RMV_CONFIG"));

            return await SqlBuilder.InsertAsync(obj, Data.MNODatabase.DMART, TABLE_NAME);

        }


        public static async Task<int> InsertAsync(Prop_Rmv_Form_Approval obj, OracleConnection conn)
        {
            if (obj.Prop_Rmv_Form_Apv_Id <= 0)
                obj.Prop_Rmv_Form_Apv_Id = Convert.ToInt32(await MOO.Data.GetNextSequenceAsync("TOLIVE.SEQ_PROP_RMV_CONFIG"));

            return await SqlBuilder.InsertAsync(obj, conn, Data.DBType.Oracle, TABLE_NAME);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0017:Simplify object initialization")]
        internal static Prop_Rmv_Form_Approval DataRowToObject(DbDataReader row, string ColPrefix = "prfa_")
        {
            Prop_Rmv_Form_Approval RetVal = new();
            RetVal.Prop_Rmv_Form_Apv_Id = (int)Util.GetRowVal(row, $"{ColPrefix}Prop_Rmv_form_apv_id");
            RetVal.Prop_Rmv_Form_Id = (int)Util.GetRowVal(row, $"{ColPrefix}Prop_Rmv_form_id");
            RetVal.Approval_User= Sec_UserSvc.DataRowToObject(row,"su_");
            RetVal.Approval_Date = (DateTime)Util.GetRowVal(row, $"{ColPrefix}approval_date");
            RetVal.Admin_Override = (short)Util.GetRowVal(row, $"{ColPrefix}Admin_Override") == 1;
            return RetVal;
        }

    }
}
