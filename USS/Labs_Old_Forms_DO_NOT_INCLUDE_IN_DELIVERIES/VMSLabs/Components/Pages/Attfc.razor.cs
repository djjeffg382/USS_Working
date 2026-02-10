using Microsoft.AspNetCore.Components;
using Radzen;
using System;
using Microsoft.JSInterop;

namespace VMSLabs.Components.Pages
{
    public partial class AttfcPage : ComponentBase
    {
        [Inject] protected DialogService DialogService { get; set; } = default!;
        [Inject] protected IJSRuntime JSRuntime { get; set; } = default!;
        protected DateTime SelectedDate { get; set; } = DateTime.Today;

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

        protected async void ExportPdf()
        {
            await JSRuntime.InvokeVoidAsync("attfcExportPdf", "attfc-report-content");
        }
    }
}
