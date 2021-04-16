using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using System.Net;
using PlanetaryDocs.Domain;
using PlanetaryDocs.Shared;
using PlanetaryDocs.Services;

namespace PlanetaryDocs.Pages
{
    public class AddBase : ComponentBase
    {
        [Inject]
        public NavigationManager NavigationService { get; set; }

        [Inject]
        public IDocumentService DocumentService { get; set; }

        [CascadingParameter]
        public LoadingService LoadingService { get; set; }

        [Inject]
        public TitleService TitleService { get; set; }


        protected bool loading = true;
        protected bool saving = false;
        protected Document document = new ();
        protected bool isValid = false;
        protected int changeCount = 0;
        protected Editor editor;

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
            if (firstRender)
            {
                await TitleService.SetTitleAsync($"Adding new document");
            }
            await base.OnAfterRenderAsync(firstRender);
        }

        protected override void OnInitialized()
        {
            document = new Document();
            changeCount = 0;
            loading = false;
            base.OnInitialized();
        }

        protected override void OnAfterRender(bool firstRender)
        {
            if (firstRender)
            {
                editor.ValidateAll(document);
            }
            base.OnAfterRender(firstRender);
        }

        public async Task SaveAsync()
        {
            if (!isDirty || !IsValid || !editor.ValidateAll(document))
            {
                return;
            }

            saving = true;

            await LoadingService.WrapExecutionAsync(async () =>
                await DocumentService.InsertDocumentAsync(document));

            NavigationService.NavigateTo($"/View/{WebUtility.UrlEncode(document.Uid)}", true);
        }
    }
}
