using Microsoft.AspNetCore.Authentication;
using System;
using System.Security.Claims;

namespace MOO.Blazor.Security
{

    /// <summary>
    /// Class will be used to add roles from the sec_roles table
    /// </summary>
    public class UserInfoClaims : IClaimsTransformation        
    {

        public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            var customPrincipal = new MOOClaimsPrincipal(principal) as ClaimsPrincipal;
            return Task.FromResult(customPrincipal);
        }


        //the following code was used until 2/2/2023.  This code allowed us to use AD Groups as well as SEC_USER groups.
        //on that fateful date we received an error "The trust relationship between the primary domain and the trusted domain failed."
        //we were unable to figure out the problem and thus this function was changed to use a customPrincipal that bypasses checking
        //for groups in AD.

        //public Task<ClaimsPrincipal> TransformAsyncOld(ClaimsPrincipal principal)
        //{
        //    //if (principal !=  null && principal.Identity != null && !principal.IsInRole("SEC_USER"))
        //    if (principal !=  null && principal.Identity != null )
        //        {
        //        //if the role SEC_USER has been added already, then do not add it again
        //        //we will use the check for SEC_USER to indicate whether this code has run already
        //        MOO.DAL.ToLive.Models.Sec_Users usr = MOO.DAL.ToLive.Services.Sec_UserSvc.Get(principal.Identity.Name);
        //        if(usr == null)
        //        {
        //            //try to get the ad user
        //            MOO.AD.AdUser ad = MOO.AD.AdUserProvider.GetAdUser(principal.Identity.Name);
        //            if(ad != null)
        //            {
        //                string fName = ad.GivenName;
        //                if (string.IsNullOrEmpty(fName))
        //                    fName = ad.Name;
        //                //user is not in sec_user yet, add them now
        //                usr = new()
        //                {
        //                    First_Name = fName,
        //                    Last_Name = ad.Surname,
        //                    Email = ad.EmailAddress,
        //                    Active = true,
        //                    Created_By = "AUTO",
        //                    Created_Date = DateTime.Now,
        //                    Domain = ad.Domain,
        //                    Username = ad.SamAccountName,
        //                    Last_Edited_Date = DateTime.Now
        //                };
        //                MOO.DAL.ToLive.Services.Sec_UserSvc.Insert(usr);
        //            }
        //        }
        //        ClaimsIdentity id = new ();
        //        id.AddClaim(new Claim(ClaimTypes.Role, "SEC_USER"));
        //        principal.AddIdentity(id);
        //        if(usr != null)
        //        {
        //            foreach(MOO.DAL.ToLive.Models.Sec_Role sr in usr.Roles)
        //            {
        //                id = new ClaimsIdentity();
        //                id.AddClaim(new Claim(ClaimTypes.Role, sr.Role_Name));
        //                principal.AddIdentity(id);
        //            }
        //        }
        //    }
        //    return Task.FromResult(principal);
        //}
    }
}
