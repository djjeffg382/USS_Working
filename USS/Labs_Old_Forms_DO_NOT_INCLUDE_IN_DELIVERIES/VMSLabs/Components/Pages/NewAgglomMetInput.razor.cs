using Microsoft.AspNetCore.Components;
using Radzen;
using System;
using System.Collections.Generic;

namespace VMSLabs.Components.Pages
{
    public partial class NewAgglomMetInputPage : ComponentBase
    {
        [Inject] protected DialogService DialogService { get; set; } = default!;
        protected DateTime SelectedDate { get; set; } = DateTime.Today;
        protected string Shift { get; set; } = "";
        protected string Half { get; set; } = "";
        protected string CorrectTimeFrame { get; set; } = "Y";
        protected List<string> TimeFrameOptions { get; set; } = new() { "Y", "N", "A" };

        protected List<string> SieveSizes { get; set; } = new() { "+9/16", "+1/2", "+7/16", "+3/8", "+1/4", "+28M", "-28M" };
        protected Dictionary<int, Dictionary<string, SieveEntry>> SieveValues { get; set; } = new();
        protected Dictionary<int, SummaryEntry> SummaryValues { get; set; } = new();

        protected override void OnInitialized()
        {
            for (int line = 3; line <= 7; line++)
            {
                SieveValues[line] = new();
                foreach (var sieve in SieveSizes)
                {
                    SieveValues[line][sieve] = new SieveEntry();
                }
                SummaryValues[line] = new SummaryEntry();
            }
        }

        protected async void ShowDateDialog()
        {
            var result = await DialogService.OpenAsync<DatePickerDialog>(
                "Select Date",
                new Dictionary<string, object> { { "SelectedDate", SelectedDate } },
                new DialogOptions { Width = "400px", Height = "250px" }
            );
            if (result is DateTime date)
            {
                SelectedDate = date;
                StateHasChanged();
            }
        }

        public class SieveEntry
        {
            public string Before { get; set; } = "";
            public string After { get; set; } = "";
        }
        public class SummaryEntry
        {
            public string CompMean { get; set; } = "";
            public string Pct200 { get; set; } = "";
            public string SiO2 { get; set; } = "";
            public string Al2O3 { get; set; } = "";
            public string CaO { get; set; } = "";
            public string MgO { get; set; } = "";
        }
    }
}
