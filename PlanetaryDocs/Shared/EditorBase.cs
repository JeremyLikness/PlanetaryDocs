using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using PlanetaryDocs.Domain;
using PlanetaryDocs.Services;

namespace PlanetaryDocs.Shared
{
    public class EditorBase : ComponentBase
    {
        [Inject]
        public IDocumentService DocumentService { get; set; }

        [CascadingParameter]
        public LoadingService LoadingService { get; set; }

        [Parameter]
        public Document DocumentToEdit { get; set; }

        protected enum FocusElement
        {
            None,
            Alias,
            Tag
        }

        protected FocusElement elementToFocus = FocusElement.None;

        protected HtmlPreview preview;
        protected AliasSearch aliasSearch;
        protected ValidatedInput newAliasInput;

        protected List<string> tagList
        {
            get => DocumentToEdit.Tags;
            set
            {
                DocumentToEdit.Tags.Clear();
                DocumentToEdit.Tags.AddRange(value);
                ChangeCount++;
                StateHasChanged();
            }
        }

        protected string title
        {
            get => DocumentToEdit.Title;
            set
            {
                if (value != DocumentToEdit.Title)
                {
                    DocumentToEdit.Title = value;
                    ChangeCount++;
                }
            }
        }

        protected string uid
        {
            get => DocumentToEdit.Uid;
            set
            {
                if (value != DocumentToEdit.Uid)
                {
                    DocumentToEdit.Uid = value;
                    ChangeCount++;
                }
            }
        }

        protected string description
        {
            get => DocumentToEdit.Description;
            set
            {
                if (value != DocumentToEdit.Description)
                {
                    DocumentToEdit.Description = value;
                    ChangeCount++;
                }
            }
        }

        [Parameter]
        public bool Insert { get; set; }

        [Parameter]
        public bool IsValid { get; set; }

        [Parameter]
        public int ChangeCount
        {
            get => changeCount;
            set
            {
                if (value != changeCount)
                {
                    changeCount = value;
                    InvokeAsync(async () => await ChangeCountChanged.InvokeAsync(changeCount));
                }
            }
        }

        [Parameter]
        public EventCallback<int> ChangeCountChanged { get; set; }

        [Parameter]
        public EventCallback<bool> IsValidChanged { get; set; }

        public bool ValidateAll(Document document)
        {
            var results = ValidationRules.ValidateDocument(document);
            var isValid = results.All(r => r.IsValid);
            if (isValid != IsValid)
            {
                ValidationStates.Clear();
                ValidationStates.AddRange(results.Where(r => !r.IsValid));
                ignoreParameterSet = true;
                OnValidationChange(false);
            }

            return isValid;
        }

        protected List<ValidationState> ValidationStates =
            new List<ValidationState>();

        protected int changeCount;

        protected bool existingAlias = true;
        protected string aliasButton => existingAlias ? "Add New Alias"
            : "Choose Existing Alias";

        protected ValidationState _aliasValidation =
            ValidationRules.ValidResult();

        protected ValidationState _uidValidation =
            ValidationRules.ValidResult();

        protected ValidationState _titleValidation =
            ValidationRules.ValidResult();

        protected ValidationState _descriptionValidation =
            ValidationRules.ValidResult();

        protected ValidationState _markdownValidation =
            ValidationRules.ValidResult();

        protected string html;

        protected string markdown
        {
            get => DocumentToEdit.Markdown;
            set
            {
                if (value != DocumentToEdit.Markdown)
                {
                    DocumentToEdit.Markdown = value;
                    MarkdownUpdated();
                }
            }
        }

        protected string alias
        {
            get => DocumentToEdit.AuthorAlias;
            set
            {
                if (value != DocumentToEdit.AuthorAlias)
                {
                    DocumentToEdit.AuthorAlias = value;
                    ChangeCount++;
                    InvokeAsync(ValidateAliasAsync);
                }
            }
        }

        protected ValidationState titleValidation
        {
            get => _titleValidation;
            set
            {
                _titleValidation = value;
                OnValidationChange();
            }
        }

        protected ValidationState uidValidation
        {
            get => _uidValidation;
            set
            {
                _uidValidation = value;
                OnValidationChange();
            }
        }

        protected ValidationState markdownValidation
        {
            get => _markdownValidation;
            set
            {
                _markdownValidation = value;
                OnValidationChange();
            }
        }

        protected ValidationState descriptionValidation
        {
            get => _descriptionValidation;
            set
            {
                _descriptionValidation = value;
                OnValidationChange();
            }
        }

        protected ValidationState aliasValidation
        {
            get => _aliasValidation;
            set
            {
                _aliasValidation = value;
                OnValidationChange();
            }
        }

        protected bool ignoreParameterSet = false;

        protected override void OnParametersSet()
        {
            if (ignoreParameterSet)
            {
                ignoreParameterSet = false;
                return;
            }

            html = DocumentToEdit.Html;
            OnValidationChange();

            base.OnParametersSet();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (elementToFocus == FocusElement.Alias)
            {
                if (existingAlias)
                {
                    await aliasSearch.FocusAsync();
                }
                else
                {
                    await newAliasInput.FocusAsync();
                }

                elementToFocus = FocusElement.None;
            }

            await base.OnAfterRenderAsync(firstRender);
        }

        protected void OnValidationChange(bool resetList = true)
        {
            if (resetList)
            {
                ValidationStates.Clear();
                ValidationStates.AddRange(
                    new[]
                    {
                _titleValidation,
                _aliasValidation,
                _descriptionValidation,
                _markdownValidation
                                        });
                if (Insert)
                {
                    ValidationStates.Add(_uidValidation);
                }
            }

            var isValid = ValidationStates.All(vr => vr.IsValid);

            if (isValid != IsValid)
            {
                IsValid = isValid;
                InvokeAsync(
                    async () =>
                    await IsValidChanged.InvokeAsync(isValid));
            }
        }

        protected async Task ToggleAliasAsync()
        {
            DocumentToEdit.AuthorAlias = string.Empty;
            existingAlias = !existingAlias;
            elementToFocus = FocusElement.Alias;
            await ValidateAliasAsync();
        }

        protected async Task ValidateAliasAsync()
        {
            _aliasValidation = ValidationRules.ValidateProperty(
                nameof(Document.AuthorAlias),
                existingAlias ? alias : DocumentToEdit.AuthorAlias);

            if (!existingAlias && _aliasValidation.IsValid)
            {
                var aliasToCheck = DocumentToEdit.AuthorAlias;
                List<string> aliases = null;

                await LoadingService.WrapExecutionAsync(
                    async () =>
                        aliases = await DocumentService.SearchAuthorsAsync(
                    aliasToCheck));
                if (aliases.Any(a => a == aliasToCheck))
                {
                    _aliasValidation = new ValidationState
                    {
                        IsValid = false,
                        Message = $"Alias '{aliasToCheck}' already exists."
                    };
                }
            }
            OnValidationChange();
        }

        protected void MarkdownUpdated()
        {
            ChangeCount++;

            var checkRequired = ValidationRules.ValidateProperty(
                nameof(DocumentToEdit.Markdown), DocumentToEdit.Markdown);

            if (checkRequired.IsValid)
            {
                markdownValidation = ValidationRules.InvalidResult(
                        "Markdown has changed. Preview before saving.");
            }
            else if (_markdownValidation.IsValid != checkRequired.IsValid ||
                    _markdownValidation.Message != checkRequired.Message)
            {
                markdownValidation = checkRequired;
            }
            OnValidationChange();
        }

        protected void MarkdownPreview()
        {
            if (string.IsNullOrWhiteSpace(markdown))
            {
                html = string.Empty;
                DocumentToEdit.Html = string.Empty;
                InvokeAsync(preview.OnUpdateAsync);
                return;
            }

            html = Markdig.Markdown.ToHtml(DocumentToEdit.Markdown);
            DocumentToEdit.Html = html;

            InvokeAsync(preview.OnUpdateAsync);

            markdownValidation = ValidationRules.ValidResult();

            OnValidationChange();
        }
    }
}
