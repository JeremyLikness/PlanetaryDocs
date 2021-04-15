using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlanetaryDocs
{
    public class HistoryService
    {
        private readonly Func<ValueTask> goBack;

        public HistoryService(IJSRuntime jsRuntime)
        {
            goBack = () => jsRuntime.InvokeVoidAsync("history.go", "-1");
        }

        public ValueTask GoBackAsync() => goBack();
        
    }
}
