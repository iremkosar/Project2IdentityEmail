using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Project2IdentityEmail.Context;
using Project2IdentityEmail.Dtos;
using Project2IdentityEmail.Entities;

namespace Project2IdentityEmail.Controllers
{
    public class DashboardController : Controller
    {
        private readonly EmailContext _context;
        private readonly UserManager<AppUser> _userManager;

        public DashboardController(EmailContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("UserLogin", "Login");

            var inboxQuery = _context.Messages.Where(x => x.ReceiverEmail == user.Email);
            var sendboxQuery = _context.Messages.Where(x => x.SenderEmail == user.Email);

            // ✅ Son Mesajlar (SADECE GELEN - Inbox)
            var lastMessages = inboxQuery
                .OrderByDescending(x => x.SendDate)
                .Take(8)
                .Select(x => new LastMessageDto
                {
                    MessageId = x.MessageId,
                    SenderEmail = x.SenderEmail,
                    ReceiverEmail = x.ReceiverEmail,
                    Subject = x.Subject,
                    SendDate = x.SendDate,
                    IsStatus = x.IsStatus,
                    MessageDetail = x.MessageDetail,
                    SenderImageUrl = null
                })
                .ToList();

            // ✅ Gönderen emailleri al
            var senderEmails = lastMessages
                .Select(x => x.SenderEmail)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct()
                .ToList();

            // ✅ AspNetUsers'tan Email -> ImageUrl map
            var senderImages = _userManager.Users
                .Where(u => senderEmails.Contains(u.Email))
                .Select(u => new { u.Email, u.ImageUrl })
                .ToList()
                .ToDictionary(x => x.Email, x => x.ImageUrl);

            // ✅ Mesajlara SenderImageUrl bas
            foreach (var m in lastMessages)
            {
                if (m.SenderEmail != null && senderImages.TryGetValue(m.SenderEmail, out var img))
                    m.SenderImageUrl = img;
            }

            var dto = new DashboardDto
            {
                // Profil
                UserName = user.UserName,
                Email = user.Email,
                ImageUrl = user.ImageUrl,
                About = user.About,

                // KPI
                TotalUsers = _userManager.Users.Count(),
                TotalCategories = _context.Categories.Count(),

                InboxCount = inboxQuery.Count(),
                SendboxCount = sendboxQuery.Count(),
                TotalMessages = inboxQuery.Count() + sendboxQuery.Count(),

                UnreadMessages = inboxQuery.Count(x => x.IsStatus == false),
                ReadMessages = inboxQuery.Count(x => x.IsStatus == true),

                // ✅ Son Mesajlar
                LastMessages = lastMessages,

                // Kategori Dağılımı (Inbox'a göre)
                CategoryStats = inboxQuery
                    .GroupBy(x => x.CategoryId)
                    .Select(g => new CategoryStatDto
                    {
                        CategoryId = g.Key,
                        CategoryName = _context.Categories
                            .Where(c => c.CategoryId == g.Key)
                            .Select(c => c.CategoryName)
                            .FirstOrDefault(),
                        Count = g.Count()
                    })
                    .OrderByDescending(x => x.Count)
                    .ToList()
            };

            return View(dto);
        }
    }
}
