using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

namespace PlanetaryDocs.Services
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
