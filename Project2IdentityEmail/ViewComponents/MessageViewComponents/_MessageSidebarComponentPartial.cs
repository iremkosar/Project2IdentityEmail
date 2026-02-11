using Microsoft.AspNetCore.Mvc;

namespace Project2IdentityEmail.ViewComponents.MessageViewComponents
{
    public class _MessageSidebarComponentPartial:ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
