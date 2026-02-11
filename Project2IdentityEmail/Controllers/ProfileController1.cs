using Microsoft.AspNetCore.Mvc;

namespace Project2IdentityEmail.Controllers
{
    public class ProfileController1 : Controller
    {
        public IActionResult EditProfile()
        {
            return View();
        }
    }
}
