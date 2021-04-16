using Microsoft.AspNetCore.Components;
using PlanetaryDocs.Domain;
using PlanetaryDocs.Services;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace PlanetaryDocs.Pages
{
    public class ViewBase : ComponentBase
    {
        [Inject]
        public IDocumentService DocumentService { get; set; }

        [CascadingParameter]
        public LoadingService LoadingService { get; set; }

        [Inject]
        public NavigationManager NavigationService { get; set; }

        [Inject]
        public TitleService TitleService { get; set; }

        protected bool showHistory = false;
        protected bool previewHtml = false;
        protected bool showMarkdown = true;
        protected string _uid = string.Empty;
        protected bool loading = true;
        protected bool notFound = false;
        protected bool audit = false;

        [Parameter]
        public string Uid { get; set; }

        protected Document document = null;

        protected string toggleText => showMarkdown ?
            "Show HTML" : "Show Markdown";

        protected string previewText => previewHtml ?
            "Show Source" : "Show Preview";

        protected string title => audit ? $"[ARCHIVE] {document?.Title}"
            : document?.Title;

        protected string SafeUid(string uid) =>
            WebUtility.UrlEncode(uid);

        protected override async Task OnParametersSetAsync()
        {
            var newUid = WebUtility.UrlDecode(Uid);
            if (newUid != _uid)
            {
                var history = string.Empty;
                if (NavigationService.Uri.IndexOf('?') > 0)
                {
                    var parts = NavigationService.Uri.Split('?');
                    var keyValues = parts[1].Split('&');
                    var historyPair = keyValues.FirstOrDefault(kv =>
                        kv.StartsWith("history="));
                    if (!string.IsNullOrWhiteSpace(historyPair))
                    {
                        history = historyPair.Split('=')[1];
                    }
                }
                loading = false;
                notFound = false;
                _uid = newUid;
                try
                {
                    loading = true;
                    if (string.IsNullOrWhiteSpace(history))
                    {
                        await LoadingService.WrapExecutionAsync(
                            async () => document = await
                                DocumentService.LoadDocumentAsync(_uid));
                        notFound = document == null;
                        audit = false;
                    }
                    else
                    {
                        await LoadingService.WrapExecutionAsync(
                            async () => document = await
                                DocumentService.LoadDocumentSnapshotAsync(Guid.Parse(history), _uid));
                        audit = true;
                        notFound = document == null;
                    }
                    loading = false;
                    await TitleService.SetTitleAsync($"Viewing {title}");
                }
                catch
                {
                    notFound = true;
                }
            }
            await base.OnParametersSetAsync();
        }

        protected void BackToMain()
        {
            NavigationService.NavigateTo(
                $"/View/{WebUtility.UrlEncode(Uid)}",
                true);
        }
    }
}
