using Microsoft.AspNetCore.Mvc;

namespace Project2IdentityEmail.ViewComponents
{
    public class SidebarViewComponent:ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
