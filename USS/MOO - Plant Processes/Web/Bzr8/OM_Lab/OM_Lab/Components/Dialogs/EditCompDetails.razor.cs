using Microsoft.AspNetCore.Components;
using MOO.DAL.ToLive.Models;
using Radzen;
using OM_Lab.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace OM_Lab.Components.Dialogs
{
    public class EditCompDetailsBase : ComponentBase
    {
        [Parameter]
        public Lab_Compression CompTest { get; set; } = null!;

        [Inject]
        protected Radzen.DialogService DialogService { get; set; } = null!;

        [Inject]
        protected ICompTestService CompTestService { get; set; } = null!;

        [Inject]
        protected NotificationService NotificationService { get; set; } = null!;

        protected List<Lab_Compression_Dtl> CompressionDetails { get; set; } = new();
        protected List<List<Lab_Compression_Dtl>> CompressionBuckets { get; set; } = new();
        protected double? MinCompLbs { get; set; }
        protected double? MaxCompLbs { get; set; }
        protected string? StartTime { get; set; }
        protected string? EndTime { get; set; }

        protected override async Task OnInitializedAsync()
        {
            if (CompTest != null)
            {
                if (CompTest.Line_Nbr == null)
                {
                    CompTest.Line_Nbr = 3;
                }
                CompressionDetails = await CompTestService.GetCompTestDetails(CompTest.Comp_Id);
                if (CompressionDetails.Any())
                {
                    MinCompLbs = CompressionDetails.Min(x => x.Comp_Lbs);
                    MaxCompLbs = CompressionDetails.Max(x => x.Comp_Lbs);
                    StartTime = CompressionDetails.Min(x => x.Comp_Date).ToString("MM/dd/yyyy HH:mm");
                    EndTime = CompressionDetails.Max(x => x.Comp_Date).ToString("MM/dd/yyyy HH:mm");
                }
                // Divide into up to 7 buckets
                CompressionBuckets = new List<List<Lab_Compression_Dtl>>();
                int bucketCount = 7;
                int bucketSize = (int)System.Math.Ceiling(CompressionDetails.Count / (double)bucketCount);
                for (int i = 0; i < bucketCount; i++)
                {
                    var bucket = CompressionDetails.Skip(i * bucketSize).Take(bucketSize).ToList();
                    if (bucket.Any())
                        CompressionBuckets.Add(bucket);
                }
            }
        }

        protected void OnCancel()
        {
            DialogService.Close(null);
        }

        protected async Task OnValidSubmit(Lab_Compression compTest)
        {
            // Validate required fields: SHIFT_DATE, SHIFT, SHIFT_HALF, LINE_NBR
            if (compTest.Shift_Date == null || compTest.Shift == null || compTest.Shift_Half == null || compTest.Line_Nbr == null)
            {
                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = "Validation Error",
                    Detail = "Shift Date, Shift, Shift Half and Line # are required.",
                    Duration = 4000
                });
                return;
            }

            await CompTestService.UpdateCompTestAsync(compTest);
            DialogService.Close(compTest);
        }

        protected string GetCompLbsStyle(double compLbs)
        {
            const double epsilon = 0.000001;
            if (MinCompLbs.HasValue && System.Math.Abs(compLbs - MinCompLbs.Value) < epsilon)
                return "color: #1565c0; font-weight: bold;"; // blue
            if (MaxCompLbs.HasValue && System.Math.Abs(compLbs - MaxCompLbs.Value) < epsilon)
                return "color: #c62828; font-weight: bold;"; // red
            return "";
        }
    }
}
