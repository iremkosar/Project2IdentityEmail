using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Project2IdentityEmail.Context;
using Project2IdentityEmail.Entities;

namespace Project2IdentityEmail.ViewComponents.MessageViewComponents
{
    public class _MessageSidebarComponentPartial:ViewComponent
    {
        private readonly EmailContext _context;
        private readonly UserManager<AppUser> _userManager;

        public _MessageSidebarComponentPartial(EmailContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var user = await _userManager.GetUserAsync(UserClaimsPrincipal);
            var email = user?.Email;

            int inboxCount = 0;
            int sendboxCount = 0;

            if (!string.IsNullOrEmpty(email))
            {
                inboxCount = _context.Messages.Count(x => x.ReceiverEmail == email);
                sendboxCount = _context.Messages.Count(y => y.SenderEmail == email);
            }

            ViewBag.InboxCount = inboxCount;
            ViewBag.SendboxCount = sendboxCount;
            return View();
        }
    }
}
