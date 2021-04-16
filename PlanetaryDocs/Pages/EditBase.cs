using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using System.Net;
using Microsoft.EntityFrameworkCore;
using PlanetaryDocs.Domain;
using PlanetaryDocs.Shared;
using PlanetaryDocs.Services;

namespace PlanetaryDocs.Pages
{
    public class EditBase : ComponentBase
    {
        protected bool notfound = false;
        protected bool concurrency = false;
        protected bool loading = true;
        protected bool saving = false;
        protected Document document = null;
        protected string _uid = string.Empty;
        protected bool isValid = false;
        protected int changeCount = 0;
        protected Editor editor;

        [Inject]
        public NavigationManager NavigationService { get; set; }

        [Inject]
        public IDocumentService DocumentService { get; set; }

        [CascadingParameter]
        public LoadingService LoadingService { get; set; }

        [Inject]
        public TitleService TitleService { get; set; }

        [Parameter]
        public string Uid { get; set; }

        public bool IsValid
        {
            get => isValid;
            set
            {
                if (value != isValid)
                {
                    isValid = value;
                    InvokeAsync(StateHasChanged);
                }
            }
        }

        protected bool isDirty => changeCount > 0;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender && !string.IsNullOrWhiteSpace(_uid))
            {
                await TitleService.SetTitleAsync($"Editing '{_uid}'");
            }
            await base.OnAfterRenderAsync(firstRender);
        }

        protected override async Task OnParametersSetAsync()
        {
            if (Uid != _uid)
            {
                _uid = Uid;
                loading = true;
                document = null;
                concurrency = false;
                await LoadingService.WrapExecutionAsync(
                    async () => document =
                    await DocumentService.LoadDocumentAsync(_uid));
                notfound = document == null;
                changeCount = 0;
                loading = false;
            }
            await base.OnParametersSetAsync();
        }

        public async Task SaveAsync()
        {
            if (!isDirty || !IsValid || !editor.ValidateAll(document))
            {
                return;
            }

            saving = true;
            if (concurrency)
            {
                concurrency = false;
                Document original = null;
                await LoadingService.WrapExecutionAsync(async () =>
                    original =
                    await DocumentService.LoadDocumentAsync(document.Uid));
                document.ETag = original.ETag;
            }
            try
            {
                await LoadingService.WrapExecutionAsync(async () =>
                    await DocumentService.UpdateDocumentAsync(document));
            }
            catch (DbUpdateConcurrencyException)
            {
                concurrency = true;
            }

            if (!concurrency)
            {
                NavigationService.NavigateTo($"/View/{WebUtility.UrlEncode(document.Uid)}", true);
            }
            else
            {
                saving = false;
            }
        }
    }
}
