using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace MOO.Blazor.Security
{
    public class MOOClaimsPrincipal : ClaimsPrincipal
    {
        private IPrincipal UserPrincipal;

        public MOOClaimsPrincipal(IPrincipal principal) : base(principal)
        {
            UserPrincipal = principal;
        }

        public override bool IsInRole(string role)
        {
            if(UserPrincipal != null && UserPrincipal.Identity != null)
            {
                //if we want to add back checking active directory groups this can be done with the following line
                //HOWEVER we must get passed the "The trust relationship between the primary domain and the trusted domain failed." issue noted in the UserInfoClaim.cs comments
                //var adHasAccess = UserPrincipal.IsInRole(role);
                MOO.DAL.ToLive.Models.Sec_Users usr = MOO.DAL.ToLive.Services.Sec_UserSvc.Get(UserPrincipal!.Identity!.Name);
                if (usr != null)
                {
                    //we will also compare role to the user name and if that matches, then return true
                    if (string.Equals(UserPrincipal!.Identity!.Name, role, StringComparison.CurrentCultureIgnoreCase))
                        return true;

                    var usrRole = usr.Roles.FirstOrDefault(x => x.Role_Name == role);
                    return usrRole != null;
                }
            }
            //if we get here then we know the user has no access
            return false;

        }
    }
}
