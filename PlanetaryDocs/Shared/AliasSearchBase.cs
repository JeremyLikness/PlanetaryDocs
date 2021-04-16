using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using PlanetaryDocs.Domain;
using PlanetaryDocs.Services;

namespace PlanetaryDocs.Shared
{
    public class AliasSearchBase : ComponentBase
    {
        protected string _alias;
        protected AutoComplete autoComplete;

        [CascadingParameter]
        public LoadingService LoadingService { get; set; }

        [Inject]
        public IDocumentService DocumentService { get; set; }

        [Parameter]
        public string TabIndex { get; set; }

        [Parameter]
        public string Alias
        {
            get => _alias;
            set
            {
                if (value != _alias)
                {
                    _alias = value;
                    InvokeAsync(async () => await AliasChanged.InvokeAsync(_alias));
                }
            }
        }

        [Parameter]
        public EventCallback<string> AliasChanged { get; set; }

        public async Task<List<string>> SearchAsync(string searchText)
        {
            List<string> results = null;

            if (string.IsNullOrWhiteSpace(searchText))
            {
                return results;
            }

            await LoadingService.WrapExecutionAsync(
                async () => results =
                await DocumentService.SearchAuthorsAsync(searchText));

            return results;
        }

        public async Task FocusAsync() => await autoComplete.FocusAsync();
    }
}
