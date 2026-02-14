using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using Project2IdentityEmail.Context;
using Project2IdentityEmail.Dtos;
using Project2IdentityEmail.Entities;

namespace Project2IdentityEmail.Controllers
{
    public class MessageController1 : Controller
    {
        private readonly EmailContext _context;
        private readonly UserManager<AppUser> _userManager;

        public MessageController1(EmailContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Inbox(int? categoryId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("UserLogin", "Login");
            await SetSidebarCounts();

            var query = _context.Messages.Where(x => x.ReceiverEmail == user.Email
                                                  && !x.IsDraft     
                                                    && !x.IsDeleted);  

            if (categoryId.HasValue)
                query = query.Where(x => x.CategoryId == categoryId.Value);

            var values = query.ToList();
            return View(values); // Inbox.cshtml aynı kalır ✅
        }

        public async Task<IActionResult> Sendbox()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("UserLogin", "Login");

            var values = _context.Messages
                .Where(x => x.SenderEmail == user.Email)
                .ToList();

            return View(values);
        }
        public async Task<IActionResult> MessageDetail(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("UserLogin", "Login");

            var message = _context.Messages
                .FirstOrDefault(x => x.MessageId == id &&
                                    (x.ReceiverEmail == user.Email || x.SenderEmail == user.Email));

            if (message == null) return NotFound();

            // Sadece alıcı okudu sayalım (Inbox için mantıklı)
            if (message.ReceiverEmail == user.Email && message.IsStatus == false)
            {
                message.IsStatus = true;
                _context.SaveChanges();
            }

            return View(message);
        }


        [HttpGet]
        public IActionResult ComposeMessage()
        {
            ViewBag.Categories = _context.Categories.ToList();
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ComposeMessage(MailRequestDto mailRequestDto)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("UserLogin", "Login");

          
            var mimeMessage = new MimeMessage();
            mimeMessage.From.Add(new MailboxAddress("Admin Identity", user.Email));
            mimeMessage.To.Add(new MailboxAddress("User", mailRequestDto.ReceiverEmail));
            mimeMessage.Subject = mailRequestDto.Subject;

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = mailRequestDto.MessageDetail
            };

            mimeMessage.Body = bodyBuilder.ToMessageBody();

            using var client = new SmtpClient();
            client.Connect("smtp.gmail.com", 587, false);
            client.Authenticate("iremkosar97@gmail.com", "ukky vjjj jofu hefr");
            client.Send(mimeMessage);
            client.Disconnect(true);

            
            var message = new Message
            {
                SenderEmail = user.Email,
                ReceiverEmail = mailRequestDto.ReceiverEmail,
                Subject = mailRequestDto.Subject,
                MessageDetail = mailRequestDto.MessageDetail,
                SendDate = DateTime.Now,
                CategoryId = mailRequestDto.CategoryId,
                IsStatus = false
            };

            _context.Messages.Add(message);
            _context.SaveChanges();

            return RedirectToAction("Sendbox", "MessageController1");
        }
        public async Task<IActionResult> Starred()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("UserLogin", "Login");
            await SetSidebarCounts();

            var values = _context.Messages
                .Include(x => x.Category)
                .Where(x => x.ReceiverEmail == user.Email && x.IsStarred && !x.IsDeleted)
                .OrderByDescending(x => x.SendDate)
                .ToList();

            return View(values);
        }

        public async Task<IActionResult> Drafts()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("UserLogin", "Login");
            await SetSidebarCounts();

            var values = _context.Messages
                .Include(x => x.Category)
                .Where(x => x.ReceiverEmail == user.Email && x.IsDraft && !x.IsDeleted)
                .OrderByDescending(x => x.SendDate)
                .ToList();

            return View(values);
        }

        public async Task<IActionResult> Trash()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("UserLogin", "Login");
            await SetSidebarCounts();

            var values = _context.Messages
                .Include(x => x.Category)
                .Where(x => x.ReceiverEmail == user.Email && x.IsDeleted)
                .OrderByDescending(x => x.SendDate)
                .ToList();

            return View(values);
        }


        // ✅ Yıldız toggle
        [HttpPost]
        public IActionResult ToggleStar(int id)
        {
            var msg = _context.Messages.FirstOrDefault(x => x.MessageId == id);
            if (msg == null) return NotFound();

            msg.IsStarred = !msg.IsStarred;
            _context.SaveChanges();

            return Json(new { ok = true, starred = msg.IsStarred });
        }

        // ✅ Taslak toggle
        [HttpPost]
        public IActionResult ToggleDraft(int id)
        {
            var msg = _context.Messages.FirstOrDefault(x => x.MessageId == id);
            if (msg == null) return NotFound();

            msg.IsDraft = !msg.IsDraft;

            // Taslak yapınca Inbox’tan çıkması için (istersen) okunma durumunu da resetleyebilirsin
            // msg.IsStatus = false;

            _context.SaveChanges();
            return Json(new { ok = true, draft = msg.IsDraft });
        }

        // ✅ Çöpe at / geri al
        [HttpPost]
        public IActionResult ToggleTrash(int id)
        {
            var msg = _context.Messages.FirstOrDefault(x => x.MessageId == id);
            if (msg == null) return NotFound();

            msg.IsDeleted = !msg.IsDeleted;
            _context.SaveChanges();

            return Json(new { ok = true, deleted = msg.IsDeleted });
        }

        // (Opsiyonel) Çöpten kalıcı sil
        [HttpPost]
        public IActionResult DeleteForever(int id)
        {
            var msg = _context.Messages.FirstOrDefault(x => x.MessageId == id);
            if (msg == null) return NotFound();

            _context.Messages.Remove(msg);
            _context.SaveChanges();

            return Json(new { ok = true });
        }
        private async Task SetSidebarCounts()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return;

            var baseQuery = _context.Messages.Where(x => x.ReceiverEmail == user.Email);

            ViewBag.InboxCount = baseQuery.Count(x => !x.IsDeleted && !x.IsDraft);
            ViewBag.StarredCount = baseQuery.Count(x => x.IsStarred && !x.IsDeleted);
            ViewBag.DraftCount = baseQuery.Count(x => x.IsDraft && !x.IsDeleted);
            ViewBag.TrashCount = baseQuery.Count(x => x.IsDeleted);

            // (opsiyonel) giden kutusu
            ViewBag.SendboxCount = _context.Messages.Count(x => x.SenderEmail == user.Email);
        }

    }
}
