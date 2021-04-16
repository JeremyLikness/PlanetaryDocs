using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace PlanetaryDocs.Shared
{
    public class AutoCompleteBase : ComponentBase
    {
        protected int index = -1;
        protected ElementReference inputElem;
        protected bool selected = false;
        protected bool loading = false;
        protected bool queued = false;
        protected string val = string.Empty;
        protected string tabIndex;
        protected int baseIndex;
        protected List<string> values = new();

        [Parameter]
        public string TabIndex
        {
            get => tabIndex;
            set
            {
                tabIndex = value;
                if (int.TryParse(value, out int numIndex))
                {
                    baseIndex = numIndex;
                }
            }
        }


        public string Value
        {
            get => val;
            set
            {
                if (val != value)
                {
                    val = value;
                    InvokeAsync(OnValueChangedAsync);
                }
            }
        }

        [Parameter]
        public string LabelText { get; set; } = string.Empty;

        [Parameter]
        public string PlaceHolderText { get; set; } = string.Empty;

        [Parameter]
        public string SelectedValue { get; set; }

        [Parameter]
        public EventCallback<string> SelectedValueChanged { get; set; }

        [Parameter]
        public Func<string, Task<List<string>>> SearchFn { get; set; }

        public async Task FocusAsync() => await inputElem.FocusAsync();

        protected string GetClass(string item) =>
            index >= 0 && item == values[index] ?
                "active" : string.Empty;

        protected string GetIndex(string item) =>
            values.IndexOf(item) >= 0 ?
            (values.IndexOf(item) + baseIndex).ToString() :
            string.Empty;

        protected override async Task OnParametersSetAsync()
        {
            if (!string.IsNullOrWhiteSpace(SelectedValue))
            {
                await SetSelectionAsync(SelectedValue);
            }

            await base.OnParametersSetAsync();
        }

        protected void HandleKeyDown(KeyboardEventArgs e)
        {
            var maxIndex = values != null ?
                values.Count - 1 : -1;

            switch (e.Key)
            {
                case "ArrowDown":
                    if (index < maxIndex)
                    {
                        index++;
                    }
                    break;

                case "ArrowUp":
                    if (index > 0)
                    {
                        index--;
                    }
                    break;

                case "Enter":
                    if (selected)
                    {
                        InvokeAsync(
                            async () =>
                            await SetSelectionAsync(string.Empty, true));
                    }
                    else if (index >= 0)
                    {
                        InvokeAsync(async () =>
                        await SetSelectionAsync(values[index]));
                    }
                    break;
            }
        }

        protected async Task OnValueChangedAsync()
        {
            if (loading)
            {
                queued = true;
                return;
            }

            loading = true;

            do
            {
                queued = false;
                values = await SearchFn(val);
            }
            while (queued);
            loading = false;
            await SetSelectionAsync(string.Empty);
            index = -1;
            StateHasChanged();
        }

        protected async Task SetSelectionAsync(string selection, bool reset = false)
        {
            if (string.IsNullOrWhiteSpace(selection))
            {
                if (selected)
                {
                    selected = false;
                    SelectedValue = string.Empty;
                    await SelectedValueChanged.InvokeAsync(string.Empty);
                    return;
                }
            }
            else
            {
                selected = true;
                values = null;
                if (SelectedValue != selection)
                {
                    SelectedValue = selection;
                    await SelectedValueChanged.InvokeAsync(selection);
                }
            }

            if (reset)
            {
                Value = string.Empty;
                await inputElem.FocusAsync();
            }
        }

    }
}
