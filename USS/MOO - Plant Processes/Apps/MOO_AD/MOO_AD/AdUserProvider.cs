using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.DirectoryServices.AccountManagement;
using System.Security.Principal;
using MOO.AD;
//https://www.c-sharpcorner.com/article/how-to-get-domain-users-search-users-and-user-from-active-directory-using-net/
namespace MOO.AD
{
    /// <summary>
    /// Class used to fill the AdUser class with information from Active Directory
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "This will be windows only")]
    public class AdUserProvider
    {

        private const string WEB_PASS = "v2=rDRc6n8F?Ed6";

        /// <summary>
        /// Gets the user from the username searching the mno domain
        /// </summary>
        /// <param name="samAccountName"></param>
        /// <returns></returns>
        public static AdUser GetAdUser(string samAccountName)
        {
            if (samAccountName.Contains('\\'))
            {
                //the account contains the domain, split it up
                string[] act = samAccountName.Split("\\");
                return GetAdUser(act[1], act[0]);
            }
            else
                return GetAdUser(samAccountName, "mno");  //default to use the MNO domain
        }

        /// <summary>
        /// Gets the user from the username
        /// </summary>
        /// <param name="samAccountName"></param>
        /// <param name="Domain">The domain to search (example: hdq, mno glw etc.)</param>
        /// <returns></returns>
        public static AdUser GetAdUser(string samAccountName, string Domain)
        {

            try
            {
                PrincipalContext context = new(ContextType.Domain, Domain, "srv_mnoweb", WEB_PASS);
                UserPrincipal principal = new(context);
                if (context != null)
                {
                    principal = UserPrincipal.FindByIdentity(context, IdentityType.SamAccountName, samAccountName);
                }
                if (principal != null)
                    return AdUser.CastToAdUser(principal);
                else
                    return null;
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving AD User", ex);
            }
        }

        /// <summary>
        /// Searches the mno domain for a given last name
        /// </summary>
        /// <param name="search">Last name to search for</param>
        /// <returns></returns>
        public static List<AdUser> SearchByLastName(string search)
        {
            return SearchByLastName(search, "mno");
        }

        /// <summary>
        /// Searches a domain for the given last name
        /// </summary>
        /// <param name="search">Last name to search for</param>
        /// <param name="Domain">The domain to search (example: hdq, mno glw etc.)</param>
        /// <returns></returns>
        public static List<AdUser> SearchByLastName(string search, string Domain)
        {
            List<AdUser> ReturnVal = [];

            PrincipalContext context = new(ContextType.Domain, Domain, "srv_mnoweb", WEB_PASS);
            UserPrincipal principal = new(context)
            {
                Surname = $"*{search}*"
            };
            PrincipalSearcher searcher = new(principal);
            foreach (UserPrincipal found in searcher.FindAll().Cast<UserPrincipal>())
            {
                ReturnVal.Add(AdUser.CastToAdUser(found));
            }

            return ReturnVal;
        }

        /// <summary>
        /// Gets the list of AD Groups the user is in
        /// </summary>
        /// <param name="Domain">The domain of the user</param>
        /// <param name="UserName">The user name</param>
        /// <returns></returns>
        public static async Task<List<GroupPrincipal>> GetGroupsAsync(string Domain, string UserName)
        {
            return await Task.Run(() => GetGroups(Domain, UserName));
        }

        /// <summary>
        /// Gets the list of AD Groups the user is in
        /// </summary>
        /// <param name="UserName">the user name, can be provided as domain\user.  If domain is not provided, then mno is used.</param>
        /// <returns></returns>
        public static async Task<List<GroupPrincipal>> GetGroupsAsync(string UserName)
        {

            string[] usr = SplitUserGrpDomain(UserName);
            return await Task.Run(() => GetGroups(usr[0], usr[1]));
        }


        private static List<GroupPrincipal> GetGroups(string Domain, string UserName)
        {
            List<GroupPrincipal> result = [];

            // establish domain context
            PrincipalContext ctx = new(ContextType.Domain, Domain, "srv_mnoweb", WEB_PASS);

            // find your user
            UserPrincipal user = UserPrincipal.FindByIdentity(ctx, UserName);

            // if found - grab its groups
            if (user != null)
            {
                PrincipalSearchResult<Principal> groups = user.GetAuthorizationGroups();

                // iterate over all groups
                foreach (Principal p in groups)
                {
                    // make sure to add only group principals
                    if (p is GroupPrincipal)
                    {
                        var g = (GroupPrincipal)p;
                        result.Add(g);
                    }
                }
            }

            return result;
        }


        /// <summary>
        /// Gets a list of users in the group
        /// </summary>
        /// <param name="GroupName">use domain\group format or if domain is not provided, mno will be used</param>
        /// <returns></returns>
        public static async Task<List<AdUser>> GetUsersInGroupAsync(string GroupName)
        {
            string[] grp = SplitUserGrpDomain(GroupName);
            return await Task.Run(() => GetUsersInGroupAsync(grp[0], grp[1]));
        }
        /// <summary>
        /// Gets a list of users in the group
        /// </summary>
        /// <param name="Domain"></param>
        /// <param name="GroupName"></param>
        /// <returns></returns>
        public static async Task<List<AdUser>> GetUsersInGroupAsync(string Domain, string GroupName)
        {

            return await Task.Run(() => GetUsersInGroup(Domain, GroupName));
        }



        private static List<AdUser> GetUsersInGroup(string Domain, string GroupName)
        {
            List<AdUser> retVal = [];
            // set up domain context
            PrincipalContext ctx = new(ContextType.Domain, Domain, "srv_mnoweb", WEB_PASS);

            // find the group in question
            GroupPrincipal group = GroupPrincipal.FindByIdentity(ctx, GroupName);

            // if found....
            if (group != null)
            {
                // iterate over members
                foreach (Principal p in group.GetMembers())
                {
                    if(p is UserPrincipal)
                        retVal.Add(AdUser.CastToAdUser((UserPrincipal)p));
                }
            }
            return retVal;
        }


        public static async Task<bool> IsUserInGroupAsync(string GroupDomain, string GroupName, string UserDomain, string UserName)
        {
            return await Task.Run(() => IsUserInGroup(GroupDomain, GroupName, UserDomain, UserName));
        }


        public static async Task<bool> IsUserInGroupAsync(string GroupName, string UserName)
        {
            string[] grp = SplitUserGrpDomain(GroupName);
            string[] usr = SplitUserGrpDomain(UserName);

            return await Task.Run(() => IsUserInGroup(grp[0], grp[1], usr[0], usr[1]));
        }

        /// <summary>
        /// Returns if the specified user is in the AD group
        /// </summary>
        /// <param name="GroupDomain">Group domain</param>
        /// <param name="GroupName">Group Name</param>
        /// <param name="UserDomain">User Domain</param>
        /// <param name="UserName">User Name</param>
        /// <returns></returns>
        private static bool IsUserInGroup(string GroupDomain, string GroupName, string UserDomain, string UserName)
        {
            
            // set up domain context
            PrincipalContext ctx = new(ContextType.Domain, GroupDomain, "srv_mnoweb", WEB_PASS);

            // find the group in question
            GroupPrincipal group = GroupPrincipal.FindByIdentity(ctx, GroupName);

            // if found....
            if (group != null)
            {
                // iterate over members
                foreach (Principal p in group.GetMembers())
                {
                    if (p is UserPrincipal)
                    {
                        if (p.SamAccountName.Equals(UserName, StringComparison.CurrentCultureIgnoreCase))
                        {
                            //username matches, now check the domain matches
                            string[] vals = p.DistinguishedName.Split(",");
                            string dmn = "";
                            foreach (var val in vals)
                            {
                                if (val.Contains("DC="))
                                {
                                    dmn = val[3..].ToLower();
                                    break;
                                }
                            }
                            if(UserDomain.Equals(dmn, StringComparison.CurrentCultureIgnoreCase))
                                return true;
                        }                            
                    }                       
                }
            }
            return false;
        }

        /// <summary>
        /// Splits a domain\username or domain\group into string array
        /// </summary>
        /// <param name="UserGrpDomainString">The string to split</param>
        /// <param name="Domain">Domain to use if domain is not in the string</param>
        /// <returns></returns>
        private static string[] SplitUserGrpDomain(string UserGrpDomainString, string Domain = "mno") {
            string domain = Domain;
            string name = UserGrpDomainString;
            if (UserGrpDomainString.Contains('\\'))
            {
                var split = UserGrpDomainString.Split('\\');
                domain = split[0];
                name = split[1];
            }
            return [domain, name];
        }

    }
}
