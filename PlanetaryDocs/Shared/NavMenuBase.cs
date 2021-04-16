using Microsoft.AspNetCore.Components;

namespace PlanetaryDocs.Shared
{
    public class NavMenuBase : ComponentBase
    {
        protected class NavItem
        {
            public bool Disabled { get; set; }
            public string Text { get; set; }
            public string Icon { get; set; }
            public string Href { get; set; }
        }

        protected readonly NavItem[] navItems = new[]
        {
        new NavItem { Disabled = false, Text = "Home", Icon = "home", Href = "" },
        new NavItem { Disabled = true, Text="View", Icon = "eye", Href = "/View" },
        new NavItem { Disabled = false, Text="Add New", Icon = "plus", Href = "/Add" },
        new NavItem { Disabled = true, Text="Edit", Icon = "pencil", Href = "/Edit"},
    };

        protected bool collapseNavMenu = true;
        protected string version = "?";

        protected string NavMenuCssClass => collapseNavMenu ? "collapse" : null;

        protected void ToggleNavMenu()
        {
            collapseNavMenu = !collapseNavMenu;
        }

        protected override void OnInitialized()
        {
            if (version == "?")
            {
                version = System.Diagnostics.FileVersionInfo.GetVersionInfo(
                    GetType().Assembly.Location).ProductVersion;
            }

            base.OnInitialized();
        }
    }
}
