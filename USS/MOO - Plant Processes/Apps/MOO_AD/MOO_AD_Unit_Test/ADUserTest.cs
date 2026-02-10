using System;
using System.Collections.Generic;
using System.Diagnostics;
using Xunit;

namespace MOO_AD_Unit_Test
{
    public class ADUserTest
    {
        [Fact]
        public void Test1()
        {
           // MOO.AD.AdUser usr = MOO.AD.AdUserProvider.GetAdUser("mno\\alt7747");
            List<MOO.AD.AdUser> usrs = MOO.AD.AdUserProvider.SearchByLastName("altman","gry");
            var a = 0;
        }
        [Fact]
        public void Test2()
        {
            var sw = new Stopwatch();
            sw.Start();
            //bool inGrp = await MOO.AD.AdUserProvider.IsUserInGroupAsync("mno\\cad_user_g", "mno\\alt7747");


            bool inGrp = MOO.AD.AdUserProvider.IsUserInGroupAsync("mno", "cad_read_g", "mno", "alt7747").Result;
            Assert.False(inGrp);


            inGrp = MOO.AD.AdUserProvider.IsUserInGroupAsync("mno", "systems", "mno", "alt7747").Result;
            Assert.True(inGrp);

            var elapsed = sw.Elapsed;

            var a = 0;
        }


        [Fact]
        public void TestGetUsersInGroup()
        {
            //Test MNO domain
            var usrs = MOO.AD.AdUserProvider.GetUsersInGroupAsync("cad_read_g").Result;
            Assert.NotEmpty(usrs);
            //test non MNO domain
            usrs = MOO.AD.AdUserProvider.GetUsersInGroupAsync("gry","cad_users").Result;
            Assert.NotEmpty(usrs);
        }
        [Fact]
        public void TestUserGroups()
        {
            var grps = MOO.AD.AdUserProvider.GetGroupsAsync("alt7747").Result;
            Assert.NotEmpty(grps);

        }
    }
}
