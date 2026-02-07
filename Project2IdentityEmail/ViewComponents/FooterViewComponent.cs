using Microsoft.AspNetCore.Mvc;

namespace Project2IdentityEmail.ViewComponents
{
    public class FooterViewComponent:ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
