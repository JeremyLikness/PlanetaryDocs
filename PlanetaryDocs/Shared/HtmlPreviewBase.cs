using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace PlanetaryDocs.Shared
{
    public class HtmlPreviewBase : ComponentBase
    {
        [Inject]
        public IJSRuntime JsRuntime { get; set; }

        private bool rendered;

        private string html;

        protected string webClass => IsEdit ? "webedit" : "web";

        [Parameter]
        public bool IsEdit { get; set; }

        [Parameter]
        public string Html
        {
            get => html;
            set
            {
                if (value != html)
                {
                    html = value;
                    InvokeAsync(OnUpdateAsync);
                }
            }
        }

        public ElementReference htmlContainer;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                rendered = true;
                await OnUpdateAsync();
            }

            await base.OnAfterRenderAsync(firstRender);
        }

        public async Task OnUpdateAsync()
        {
            if (rendered)
            {
                await JsRuntime.InvokeVoidAsync(
                    "markdownExtensions.toHtml",
                    Html,
                    htmlContainer);
            }
        }

    }
}
