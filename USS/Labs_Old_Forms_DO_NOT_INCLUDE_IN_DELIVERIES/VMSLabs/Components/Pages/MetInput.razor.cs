using Microsoft.AspNetCore.Components;
using Radzen;
using System;

namespace VMSLabs.Components.Pages
{
    public partial class MetInputPage : ComponentBase
    {
        [Inject] protected DialogService DialogService { get; set; } = default!;
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

        protected async void ShowTransmitDialog()
        {
            var confirmed = await DialogService.Confirm("Are you sure you want to transmit?", "Confirm Transmit");
            if (confirmed == true)
            {
                // TODO: Add transmit logic here
            }
        }
    }
}
