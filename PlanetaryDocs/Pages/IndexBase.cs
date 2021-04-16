using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using PlanetaryDocs.Domain;
using PlanetaryDocs.Services;

namespace PlanetaryDocs.Pages
{
    public class IndexBase : ComponentBase
    {
        protected bool loading = true;
        protected bool navigatingToThisPage = true;
        protected bool searchQueued = false;
        protected string alias = string.Empty;
        protected string tag = string.Empty;
        protected string text = string.Empty;
        protected List<DocumentSummary> docsList = null;
        protected ElementReference inputElem;

        [Inject]
        protected NavigationManager NavigationService { get; set; }

        [Inject]
        protected IDocumentService DocumentService { get; set; }

        [CascadingParameter]
        protected LoadingService LoadingService { get; set; }

        protected bool CanSearch =>
            !loading && (
                (!string.IsNullOrWhiteSpace(text) &&
                    text.Trim().Length > 2)
                || !string.IsNullOrWhiteSpace(alias)
                || !string.IsNullOrWhiteSpace(tag));

        protected override void OnAfterRender(bool firstRender)
        {
            var stateHasChanged = false;
            if (navigatingToThisPage)
            {
                loading = false;
                stateHasChanged = true;
            }

            if (navigatingToThisPage && NavigationService.Uri.IndexOf('?') > 0)
            {
                var queryString = NavigationService.Uri.Split('?');
                var keyValuePairs = queryString[1].Split('&');
                foreach (var keyValuePair in keyValuePairs)
                {
                    if (keyValuePair.IndexOf('=') > 0)
                    {
                        var pair = keyValuePair.Split('=');
                        switch (pair[0])
                        {
                            case nameof(text):
                                text = WebUtility.UrlDecode(pair[1]);
                                break;
                            case nameof(alias):
                                alias = WebUtility.UrlDecode(pair[1]);
                                break;
                            case nameof(tag):
                                tag = WebUtility.UrlDecode(pair[1]);
                                break;
                        }
                    }
                }
                navigatingToThisPage = false;
                InvokeAsync(async () => await SearchAsync());
            }

            if (stateHasChanged)
            {
                StateHasChanged();
            }

            base.OnAfterRender(firstRender);
        }

        protected void HandleKeyPress(KeyboardEventArgs key)
        {
            if (key.Key == "Enter")
            {
                InvokeAsync(SearchAsync);
            }
        }

        protected async Task SearchAsync()
        {
            if (loading)
            {
                searchQueued = true;
                return;
            }

            searchQueued = false;

            alias = alias.Trim();
            tag = tag.Trim();
            text = text.Trim();

            if (CanSearch)
            {
                loading = true;

                do
                {
                    searchQueued = false;
                    await LoadingService.WrapExecutionAsync(
                        async () =>
                    docsList = await DocumentService.QueryDocumentsAsync(
                        text,
                        alias,
                        tag));
                }
                while (searchQueued);

                loading = false;
            }
            else
            {
                docsList = null;
            }

            StateHasChanged();

            var searchParameters = new[]
            {
                (nameof(text), WebUtility.UrlEncode(text)),
                (nameof(alias), WebUtility.UrlEncode(alias)),
                (nameof(tag), WebUtility.UrlEncode(tag))
            };

            var queryString =
                string.Join('&',
                searchParameters.Select(p => $"{p.Item1}={p.Item2}"));

            navigatingToThisPage = false;
            NavigationService.NavigateTo($"/?{queryString}");

            await inputElem.FocusAsync();
        }

        protected string SafeUid(string uid) =>
            WebUtility.UrlEncode(uid);

        protected void Navigate(string uid)
        {
            navigatingToThisPage = false;
            NavigationService.NavigateTo($"/View/{SafeUid(uid)}");
        }

    }
}
