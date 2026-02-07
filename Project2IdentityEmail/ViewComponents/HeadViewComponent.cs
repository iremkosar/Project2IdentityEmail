using Microsoft.AspNetCore.Mvc;

namespace Project2IdentityEmail.ViewComponents
{
    public class HeadViewComponent:ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
