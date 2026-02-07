using Microsoft.AspNetCore.Mvc;

namespace Project2IdentityEmail.Controllers
{
    public class LayoutController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
