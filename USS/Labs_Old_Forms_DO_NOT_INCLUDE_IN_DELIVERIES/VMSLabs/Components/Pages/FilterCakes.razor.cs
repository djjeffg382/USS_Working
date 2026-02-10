using Microsoft.AspNetCore.Components;
using Radzen;
using System;

namespace VMSLabs.Components.Pages
{
    public partial class FilterCakesPage : ComponentBase
    {
        [Inject] protected DialogService DialogService { get; set; } = default!;
        protected DateTime SelectedDate { get; set; } = DateTime.Today;
        protected string Step { get; set; } = "Step 2";
        protected string[] Steps { get; set; } = new[] { "Step 2", "Step 3" };
        protected string SampleTime { get; set; } = "";
        protected string TimeDateOk { get; set; } = "Y";
        protected string[] YesNoOptions { get; set; } = new[] { "Y", "N" };
        protected string SIO2 { get; set; } = "";
        protected string AL203 { get; set; } = "";
        protected string CAO { get; set; } = "";
        protected string MGO { get; set; } = "";
        protected string Basicity { get; set; } = "";
        protected string EstRatio { get; set; } = "";
        protected string LookGood { get; set; } = "Y";
        protected string[] LookGoodOptions { get; set; } = new[] { "Y", "N", "A" };

        protected async void ShowDateDialog()
        {
            var result = await DialogService.OpenAsync<DatePickerDialog>(
                "Select Date",
                new System.Collections.Generic.Dictionary<string, object>
                {
                    { "SelectedDate", SelectedDate }
                },
                new DialogOptions { Width = "400px", Height = "250px" }
            );
            if (result is DateTime date)
            {
                SelectedDate = date;
                StateHasChanged();
            }
        }

        protected async void SubmitValues()
        {
            var confirmed = await DialogService.Confirm("Are you sure you want to submit these values?", "Confirm Submission");
            if (confirmed == true)
            {
                // TODO: Add logic to handle submission
            }
        }
    }
}
