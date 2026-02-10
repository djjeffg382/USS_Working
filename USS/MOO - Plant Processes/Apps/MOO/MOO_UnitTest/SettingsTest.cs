using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
namespace MOO_UnitTest
{
    public class SettingsTest
    {
        [Fact]
        public void TestGlobalSettingsRead()
        {
            //try to get a setting that doesn't exist, should return the default value
            string testSetting = MOO.Settings.GetSettingValByName<string>("DoesNotExist","abc");
            Assert.Equal("abc", testSetting);


            //Test obtaining the value of all the settings
            _ = MOO.Settings.SMTP_Server;

            _ = MOO.Settings.ServerType;

            _ = MOO.Settings.PC_Level2_Email;

            _ = MOO.Settings.MailFromAddress;

            _ = MOO.Settings.PriorityMailFromAddress;

        }
    }
}
