// Copyright (c) Jeremy Likness. All rights reserved.
// Licensed under the MIT License. See LICENSE in the repository root for license information.

using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace PlanetaryDocs.Shared
{
    /// <summary>
    /// Code for the HTML preview component.
    /// </summary>
    public class HtmlPreviewBase : ComponentBase
    {
        private bool rendered;
        private string html;

        /// <summary>
        /// Gets or sets the reference to the JavaScript runtime.
        /// </summary>
        [Inject]
        public IJSRuntime JsRuntime { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the preview is shown in an
        /// edit context.
        /// </summary>
        [Parameter]
        public bool IsEdit { get; set; }

        /// <summary>
        /// Gets or sets the HTML to preview.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the reference to the internal container for the preview.
        /// </summary>
        protected ElementReference HtmlContainer { get; set; }

        /// <summary>
        /// Gets the CSS class to use based on the mode.
        /// </summary>
        protected string WebClass => IsEdit ? "webedit" : "web";

        /// <summary>
        /// Method to update the preview.
        /// </summary>
        /// <remarks>
        /// This component uses a "trick" to render HTML by inserting it into a
        /// <c>textarea</c> element then moving the value over. The code is in
        /// <c>wwwroot/js/markdownExtensions.js</c>.
        /// </remarks>
        /// <returns>The asynchronous task.</returns>
        public async Task OnUpdateAsync()
        {
            if (rendered)
            {
                await JsRuntime.InvokeVoidAsync(
                    "markdownExtensions.toHtml",
                    Html,
                    HtmlContainer);
            }
        }

        /// <summary>
        /// Called after rendering.
        /// </summary>
        /// <param name="firstRender">A value indicating whether it is the first render.</param>
        /// <returns>The asynchronous task.</returns>
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                rendered = true;
                await OnUpdateAsync();
            }

            await base.OnAfterRenderAsync(firstRender);
        }
    }
}
