using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using PlanetaryDocs.Domain;
using PlanetaryDocs.Services;

namespace PlanetaryDocs.Shared
{
    public class TagSearchBase : ComponentBase
    {
        protected string _tag;

        [Inject]
        public IDocumentService DocumentService { get; set; }

        [CascadingParameter]
        public LoadingService LoadingService { get; set; }

        [Parameter]
        public string TabIndex { get; set; }

        [Parameter]
        public string Tag
        {
            get => _tag;
            set
            {
                if (value != _tag)
                {
                    _tag = value;
                    InvokeAsync(async () => await TagChanged.InvokeAsync(_tag));
                }
            }
        }

        [Parameter]
        public EventCallback<string> TagChanged { get; set; }

        public async Task<List<string>> SearchAsync(string searchText)
        {
            List<string> results = null;

            if (string.IsNullOrWhiteSpace(searchText))
            {
                return results;
            }

            await LoadingService.WrapExecutionAsync(
                async () => results =
                await DocumentService.SearchTagsAsync(searchText));

            return results;
        }

    }
}
