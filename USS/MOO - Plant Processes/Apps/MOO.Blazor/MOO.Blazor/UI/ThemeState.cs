using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using System.Threading.Tasks;


namespace MOO.Blazor
{
    
    public class ThemeState
    {
        /// <summary>
        /// Name of the setting in the user options
        /// </summary>
        public const string USER_OPTION_NAME = "Theme";
        private readonly AuthenticationStateProvider authenticationStateProvider;

        private string UserName = "";
        public ThemeState(AuthenticationStateProvider authenticationStateProvider)
        {
            this.authenticationStateProvider = authenticationStateProvider;
            var task = Task.Run(async () => await GetUserName());
            task.Wait();
            CurrentTheme = MOO.DAL.ToLive.Services.Sec_UserSvc.GetUserOption<string>(UserName, USER_OPTION_NAME, false);
        }

        public string CurrentTheme { get; set; } = "default";

        public string ThemeCSS
        {
            get { return CurrentTheme + ".css"; }
        }

        public void SetTheme(string theme)
        {
            CurrentTheme = theme;
            MOO.DAL.ToLive.Services.Sec_UserSvc.UpdateUserOption(UserName, USER_OPTION_NAME, CurrentTheme);
        }
        
        /// <summary>
        /// Toggles between dark and default, use SetTheme instead
        /// </summary>
        public void ToggleTheme()
        {
            if (CurrentTheme == "default")
            {
                CurrentTheme = "dark";
            }
            else
            {
                CurrentTheme = "default";
            }
            MOO.DAL.ToLive.Services.Sec_UserSvc.UpdateUserOption(UserName, "DarkMode", CurrentTheme == "dark");
            MOO.DAL.ToLive.Services.Sec_UserSvc.UpdateUserOption(UserName, USER_OPTION_NAME, CurrentTheme);

        }

        public async Task GetUserName()
        {
            var authState = await authenticationStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;
            UserName = user?.Identity?.Name ?? "Unknown";
        }

    }
}
