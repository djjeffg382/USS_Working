using Microsoft.AspNetCore.Components;
using Radzen.Blazor;

namespace VMSLabs.Components.Layout
{
    public class MainLayoutBase : LayoutComponentBase
    {
        protected RadzenSidebar? sidebar;
        protected bool sidebarExpanded = false;

        protected void ToggleSidebar()
        {
            sidebar?.Toggle();
        }
    }
}
