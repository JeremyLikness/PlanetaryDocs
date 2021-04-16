using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace PlanetaryDocs.Shared
{
    public class TagPickerBase : ComponentBase
    {
        protected bool pickNew = false;
        protected string newTag = string.Empty;
        protected string addTag = string.Empty;

        [Parameter]
        public List<string> Tags { get; set; }

        [Parameter]
        public EventCallback<List<string>> TagsChanged { get; set; }

        public string NewTag
        {
            get => newTag;
            set
            {
                if (!string.IsNullOrWhiteSpace(value) &&
                    !Tags.Contains(value))
                {
                    Tags.Add(value);
                    newTag = string.Empty;
                    addTag = string.Empty;
                    pickNew = false;
                    InvokeAsync(async () => await TagsChanged.InvokeAsync(
                        Tags.ToList()));
                }
            }
        }

        public async Task RemoveAsync(string tag)
        {
            Tags.Remove(tag);
            await TagsChanged.InvokeAsync(Tags.ToList());
        }

    }
}
