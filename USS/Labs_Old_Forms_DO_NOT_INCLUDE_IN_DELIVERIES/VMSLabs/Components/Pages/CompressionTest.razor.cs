using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Linq;

namespace VMSLabs.Components.Pages
{
    public partial class CompressionTestPage : ComponentBase
    {
        protected TestResult SelectedTest { get; set; }
        protected List<TestResult> TestResults { get; set; } = new()
        {
            new TestResult { SeqNo = "156", Rpt = "01", StartTime = "14-MAY 09:32", StopTime = "14-MAY 10:22" },
            new TestResult { SeqNo = "157", Rpt = "01", StartTime = "14-MAY 10:24", StopTime = "14-MAY 11:27" },
            new TestResult { SeqNo = "274", Rpt = "01", StartTime = "14-MAY 09:34", StopTime = "14-MAY 10:29" },
            new TestResult { SeqNo = "301", Rpt = "01", StartTime = "14-MAY 09:30", StopTime = "14-MAY 10:17" }
        };

        protected override void OnInitialized()
        {
            SelectedTest = TestResults.First();
        }

        protected void OnRowSelect(TestResult test)
        {
            SelectedTest = test;
        }

        public class TestResult
        {
            public string SeqNo { get; set; }
            public string Rpt { get; set; }
            public string StartTime { get; set; }
            public string StopTime { get; set; }
        }
    }
}
