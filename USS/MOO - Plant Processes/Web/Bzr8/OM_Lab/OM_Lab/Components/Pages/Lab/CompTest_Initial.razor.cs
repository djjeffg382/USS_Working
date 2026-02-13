using Microsoft.AspNetCore.Components;
using Radzen;
using MOO.DAL.ToLive.Models;
using Radzen.Blazor;
using OM_Lab.Services;
using OM_Lab.Components.Dialogs;

namespace OM_Lab.Components.Pages.Lab
{
    public partial class CompTest_Initial : ComponentBase
    {
        protected bool GridLoading { get; set; } = false;
        protected RadzenDataGrid<Lab_Compression>? CompGrid { get; set; } = null!;
        protected List<Lab_Compression> CompTestList { get; set; } = new();
        protected bool ShowAll { get; set; } = false;
        protected Lab_Compression? SelectedCompTest { get; set; }
        protected bool CanEditSelected => SelectedCompTest != null;

        [Inject]
        protected ICompTestService CompTestService { get; set; } = null!;
        [Inject]
        protected DialogService DialogService { get; set; } = null!;

        protected override async Task OnInitializedAsync()
        {
            await LoadGridData();
        }

        private async Task LoadGridData()
        {
            GridLoading = true;
            var allData = await CompTestService.GetCompTestData(DateTime.MinValue, DateTime.MaxValue);
            if (ShowAll)
            {
                CompTestList = allData;
            }
            else
            {
                CompTestList = allData
                    .Where(x => x.Shift_Date == null && x.Shift == null && x.Shift_Half == null && x.Line_Nbr == null)
                    .ToList();
            }
            GridLoading = false;
        }

        protected void OnRowSelect(Lab_Compression comp)
        {
            SelectedCompTest = comp;
        }

        protected async Task EditSelectedAsync()
        {
            if (SelectedCompTest != null)
            {
                await DialogService.OpenAsync<EditCompDetails>("Edit Compression Details",
                    new Dictionary<string, object> { { "CompTest", SelectedCompTest } },
                    new DialogOptions() { Width = "80vw", Height = "80vh", Resizable = true, Draggable = true, Style = "overflow:hidden;" });
            }
        }

        protected async Task ShowAllChanged(bool value)
        {
            ShowAll = value;
            await LoadGridData();
        }
    }
}
