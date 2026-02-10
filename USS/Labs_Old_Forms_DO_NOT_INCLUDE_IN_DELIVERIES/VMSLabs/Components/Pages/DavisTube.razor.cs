using Microsoft.AspNetCore.Components;
using Radzen;

namespace VMSLabs.Components.Pages
{
    public partial class DavisTube : ComponentBase
    {
        [Inject]
        protected DialogService DialogService { get; set; } = default!;

        protected string? FirstShiftWt { get; set; }
        protected string? FirstShiftWtPct { get; set; }
        protected string? FirstShiftConcSiO2 { get; set; }

        protected string? SecondShiftWt { get; set; }
        protected string? SecondShiftWtPct { get; set; }
        protected string? SecondShiftConcSiO2 { get; set; }

        protected string? ThirdShiftWt { get; set; }
        protected string? ThirdShiftWtPct { get; set; }
        protected string? ThirdShiftConcSiO2 { get; set; }

        protected DateTime SelectedDate { get; set; } = DateTime.Today;

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
    }
}
