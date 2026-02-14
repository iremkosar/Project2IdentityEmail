using Microsoft.AspNetCore.Mvc;

namespace Project2IdentityEmail.ViewComponents
{
    public class _BreadCrumbUserLayoutComponentPartial:ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            var controller = ViewContext.RouteData.Values["controller"]?.ToString();
            var action = ViewContext.RouteData.Values["action"]?.ToString();

            string page = "Dashboard";

            
            if (controller == "MessageController1" && action == "Inbox") page = "Inbox";
            else if (controller == "MessageController1" && action == "Starred") page = "Yıldızlananlar";
            else if (controller == "MessageController1" && action == "Drafts") page = "Taslaklar";
            else if (controller == "MessageController1" && action == "Trash") page = "Çöp Kutusu";
            else if (controller == "MessageController1" && action == "Sendbox") page = "Giden Kutusu";
            else if (controller == "Dashboard" && action == "Index") page = "Dashboard";
            else if (controller == "MessageController1" && action == "ComposeMessage") page = "Mesaj Oluştur";
            else if (controller == "Profile" && action == "Index") page = "Profilim";
            else if (controller == "ProfileController1" && action == "EditProfile") page = "Profilim";

            ViewBag.Breadcrumb = page;
            ViewBag.PageTitle = page;

            return View();
        }
    }
}
