using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Xunit;
using DAL = MOO.DAL.ToLive;
using LMEnum = MOO.DAL.ToLive.Enums.LineupMeeting;

namespace DAL_UnitTest.Tolive
{
    public class LineupMeetingTest
    {
        [Fact]
        public void TestAggLineupMeeting()
        {

            var aggMeet = DAL.Services.Lineup_MeetingSvc.GetDictionary<DAL.Enums.LineupMeeting.AggLineupMeeting>(DateTime.Today);
            Assert.NotEmpty(aggMeet);
            aggMeet[LMEnum.AggLineupMeeting.Safety].Value = $"Safety Test {DateTime.Now:MM/dd/yyyy HH:mm:ss}";
            DAL.Services.Lineup_MeetingSvc.Upsert(aggMeet[LMEnum.AggLineupMeeting.Safety]);
            
        }
    }
}
