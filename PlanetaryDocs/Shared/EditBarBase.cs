using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using PlanetaryDocs.Services;

namespace PlanetaryDocs.Shared
{
    public class EditBarBase : ComponentBase
    {
        [Inject]
        public NavigationManager NavigationService { get; set; }

        [Inject]
        public HistoryService HistoryService { get; set; }

        [Parameter]
        public bool IsDirty { get; set; }

        [Parameter]
        public bool IsValid { get; set; }

        [Parameter]
        public int ChangeCount { get; set; }

        [Parameter]
        public Func<Task> SaveAsync { get; set; }

        protected void Reset() => NavigationService.NavigateTo(
            NavigationService.Uri,
            true);

        protected void Cancel() => InvokeAsync(async () =>
            await HistoryService.GoBackAsync());

    }
}
