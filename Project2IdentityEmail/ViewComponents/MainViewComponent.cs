using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project2IdentityEmail.Context;
using Project2IdentityEmail.Entities;

namespace Project2IdentityEmail.ViewComponents
{
    public class MainViewComponent : ViewComponent
    {
        private readonly EmailContext _context;
        private readonly UserManager<AppUser> _userManager;

        // Bu kısım (Constructor) olmazsa _context ve _userManager nesnelerini kullanamazsın, altı çizilir.
        public MainViewComponent(EmailContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync(string mode = "inbox", int messageId = 0)
        {
            var user = await _userManager.FindByNameAsync(User.Identity?.Name);

            // DİKKAT: Controller'da "details" (s ile) dedik, burada da öyle olmalı
            if (mode == "details" && messageId > 0)
            {
                var message = await _context.Messages.FirstOrDefaultAsync(x => x.MessageId == messageId);
                if (message != null)
                {
                    // Views/Shared/Components/Main/ içindeki dosyanın adı "Detail" ise:
                    return View("Detail", message);
                }
            }

            // 2. Durum: Listeleme (Eski kodun)
            List<Message> values;
            if (mode == "sendbox")
                values = _context.Messages.Where(x => x.SenderEmail == user.Email).ToList();
            else
                values = _context.Messages.Where(x => x.ReceiverEmail == user.Email).ToList();

            ViewBag.Mode = mode;
            return View(values); // Default.cshtml'e (listeye) gider
        }
    }
}