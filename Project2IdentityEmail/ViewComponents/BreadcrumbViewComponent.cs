using Microsoft.AspNetCore.Mvc;

namespace Project2IdentityEmail.ViewComponents
{
    public class BreadcrumbViewComponent:ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
