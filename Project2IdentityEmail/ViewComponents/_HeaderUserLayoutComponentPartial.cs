using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Project2IdentityEmail.Context;
using Project2IdentityEmail.Entities;

namespace Project2IdentityEmail.ViewComponents
{
    public class _HeaderUserLayoutComponentPartial : ViewComponent
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly EmailContext _context;

        public _HeaderUserLayoutComponentPartial(
            UserManager<AppUser> userManager,
            EmailContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            if (user == null) return View();

            var messages = _context.Messages
                .Where(x => x.ReceiverEmail == user.Email)
                .OrderByDescending(x => x.SendDate)
                .Take(5)
                .ToList();

            // Inbox ile aynı mantık: okunmamış sayısı
            var unreadCount = _context.Messages
                .Count(x => x.ReceiverEmail == user.Email && x.IsStatus == false);

            var senderEmails = messages
                .Select(x => x.SenderEmail)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct()
                .ToList();

            var senderImages = _userManager.Users
                .Where(u => senderEmails.Contains(u.Email))
                .ToDictionary(u => u.Email, u => u.ImageUrl);

            ViewBag.Messages = messages;
            ViewBag.SenderImages = senderImages;
            ViewBag.UnreadCount = unreadCount;

            return View();
        }
    }
}
