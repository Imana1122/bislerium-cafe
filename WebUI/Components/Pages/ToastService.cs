using Microsoft.JSInterop;
using System.Threading.Tasks;
namespace WebUI.Components.Pages
{  

    public interface IToastService
    {
        Task ShowSuccess(string message);
        Task ShowError(string message);
    }

    public class ToastService : IToastService
    {
        private readonly IJSRuntime _jsRuntime;

        public ToastService(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public async Task ShowSuccess(string message)
        {
            await _jsRuntime.InvokeVoidAsync("showToast", "success", message);
        }

        public async Task ShowError(string message)
        {
            await _jsRuntime.InvokeVoidAsync("showToast", "error", message);
        }
    }


}
