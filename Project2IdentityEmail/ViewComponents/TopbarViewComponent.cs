using Microsoft.AspNetCore.Mvc;

namespace Project2IdentityEmail.ViewComponents
{
    public class TopbarViewComponent:ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
