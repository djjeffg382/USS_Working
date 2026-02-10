using Microsoft.AspNetCore.Components;

namespace VMSLabs.Components.Pages
{
    public partial class HomePage : ComponentBase
    {
        [Inject] NavigationManager Navigation { get; set; } = default!;

        protected void NavigateTo(string url)
        {
            Navigation.NavigateTo(url);
        }
    }
}
