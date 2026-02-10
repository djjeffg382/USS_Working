using System;
using System.Text;
using Xunit;
using DAL = MOO.DAL.ToLive;

namespace DAL_UnitTest.Tolive
{
    public class PeopleTest : BaseTestClass
    {
        [Fact]
        public void RunTest()
        {
            
            //var z = DAL.Services.PeopleSvc.GetAll("", "", 263550, 0);
            //var ppl = DAL.Services.PeopleSvc.GetAll("Evan", "Shefik", 0, 0);
            //var x = DAL.Services.PeopleSvc.Get(70029308);
            //x.Home_Number = "N/A";
            //DAL.Services.PeopleSvc.Update(x);
            var v = DAL.Services.PeopleSvc.GetAll();
            StringBuilder sb = new();
            foreach (DAL.Models.People ppl in v)
            {
                sb.AppendLine(ppl.Full_Name_WithID);
                if(ppl.Person_Id== 772350)
                {
                    Console.WriteLine(ppl);
                }
            }
            //var missingMiddleName = DAL.Services.PeopleSvc.Get(47823);
            //var a = 0;
        }
    }
}
