using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Shop.Gateway.Web.Components
{
    public class UserProfileHeader : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            await Task.CompletedTask;
            return View();
        }
    }
}
