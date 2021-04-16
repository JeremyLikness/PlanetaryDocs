using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using PlanetaryDocs.Domain;
using PlanetaryDocs.Services;

namespace PlanetaryDocs.Shared
{
    public class DocHistoryBase : ComponentBase
    {
        protected List<DocumentAuditSummary> history = null;

        protected string _uid = string.Empty;

        [CascadingParameter]
        public LoadingService LoadingService { get; set; }

        [Inject]
        public IDocumentService DocumentService { get; set; }

        [Inject]
        public NavigationManager NavigationService { get; set; }

        [Parameter]
        public string Uid { get; set; }

        protected override async Task OnParametersSetAsync()
        {
            if (Uid != _uid)
            {
                _uid = Uid;
                await LoadingService.WrapExecutionAsync(
                    async () =>
                        history =
                        await DocumentService.LoadDocumentHistoryAsync(_uid));
            }
            await base.OnParametersSetAsync();
        }

        protected void Navigate(DocumentAuditSummary audit)
        {
            NavigationService.NavigateTo($"/View/{WebUtility.UrlEncode(audit.Uid)}?history={audit.Id}", true);
        }
    }
}
