using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using PlanetaryDocs.Domain;

namespace PlanetaryDocs.Shared
{
    public class ValidatedInputBase : ComponentBase
    {
        protected bool focused = false;
        protected ElementReference textAreaControl;
        protected ElementReference inputControl;
        protected ElementReference activeControl =>
            UseTextArea ? textAreaControl : inputControl;
        protected string _originalValue = string.Empty;
        protected string _value = string.Empty;
        protected string error => Validation != null && Validation.IsValid == false
            ? "error" : string.Empty;

        [Parameter]
        public string PlaceHolder { get; set; }

        [Parameter]
        public string Value { get; set; }

        [Parameter]
        public bool AutoFocus { get; set; }

        [Parameter]
        public EventCallback<string> ValueChanged { get; set; }

        [Parameter]
        public Func<string, ValidationState> Validate { get; set; }

        [Parameter]
        public ValidationState Validation { get; set; }

        [Parameter]
        public EventCallback<ValidationState> ValidationChanged { get; set; }

        [Parameter]
        public bool UseTextArea { get; set; }

        protected string value
        {
            get => _value;
            set
            {
                if (value != _value)
                {
                    _value = value;
                    Value = _value;
                    InvokeAsync(async () =>
                        await ValueChanged.InvokeAsync(Value));
                    OnValidate();
                }
            }
        }

        protected ValidationState ValidationState;

        protected override void OnParametersSet()
        {
            ValidationState = null;
            _originalValue = Value;
            _value = Value;
            OnValidate();
            base.OnParametersSet();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (AutoFocus && !focused)
            {
                focused = true;
                await activeControl.FocusAsync();
            }

            await base.OnAfterRenderAsync(firstRender);
        }

        public void OnValidate()
        {
            ValidationState = Validate(_value);

            if (Validation == null ||
                ValidationState.IsValid != Validation.IsValid ||
                ValidationState.Message != Validation.Message)
            {
                Validation = ValidationState;
                InvokeAsync(async () =>
                    await ValidationChanged.InvokeAsync(Validation));
            }

            if (!ValidationState.IsValid && AutoFocus)
            {
                InvokeAsync(async () => await activeControl.FocusAsync());
            }
        }

        public async Task FocusAsync() => await activeControl.FocusAsync();

    }
}
