using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Threading.Tasks;

namespace PlanetaryDocs
{
    public class TitleService
    {
        private const string DefaultTitle = "Planetary Docs";
        private readonly NavigationManager navigationManager;
        private readonly IJSRuntime jsRuntime;

        public TitleService(
            NavigationManager manager,
            IJSRuntime jsRuntime)
        {
            navigationManager = manager;
            navigationManager.LocationChanged += async (o, e) =>
                await SetTitleAsync(DefaultTitle);
            this.jsRuntime = jsRuntime;
        }

        public string Title { get; set; }
        
        public async Task SetTitleAsync(string title)
        {
            Title = title;
            await jsRuntime.InvokeVoidAsync("titleService.setTitle", title);
        }
    }
}
