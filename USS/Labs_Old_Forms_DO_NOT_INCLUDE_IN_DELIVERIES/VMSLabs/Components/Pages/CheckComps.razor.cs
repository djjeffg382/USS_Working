using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace VMSLabs.Components.Pages
{
    public partial class CheckCompsPage : ComponentBase
    {
        [Inject] protected IJSRuntime JSRuntime { get; set; } = default!;

        protected async void ExportPdf()
        {
            await JSRuntime.InvokeVoidAsync("attfcExportPdf", "checkcomps-report-content");
        }
    }
}
