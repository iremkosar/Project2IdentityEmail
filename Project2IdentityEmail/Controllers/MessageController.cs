using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using Project2IdentityEmail.Context;
using Project2IdentityEmail.Entities;

namespace Project2IdentityEmail.Controllers
{
    public class MessageController : Controller
    {
        private readonly EmailContext _context;
        private readonly UserManager<AppUser> _userManager;

        public MessageController(EmailContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // ✅ GELEN KUTUSU
        public async Task<IActionResult> Inbox()
        {
            var user = await _userManager.FindByNameAsync(User.Identity!.Name);
            if (user == null) return RedirectToAction("UserLogin", "Login");

            ViewBag.SelectedMode = "inbox";

            var messages = _context.Messages
                .Where(x => x.ReceiverEmail == user.Email && x.IsStatus == false)
                .OrderByDescending(x => x.SendDate)
                .ToList();

            return View(messages); // Views/Message/Inbox.cshtml
        }


        // ✅ GİDEN KUTUSU
        public async Task<IActionResult> Sendbox()
        {
            var user = await _userManager.FindByNameAsync(User.Identity!.Name);
            if (user == null) return RedirectToAction("UserLogin", "Login");

            ViewBag.SelectedMode = "sendbox";

            // Kullanıcının gönderdiği mailler
            var sendboxMessages = await _context.Messages
                .Where(x => x.SenderEmail == user.Email && x.IsStatus == false)
                .OrderByDescending(x => x.SendDate)
                .ToListAsync();

            return View(sendboxMessages); // Views/Message/Sendbox.cshtml
        }

        // ✅ DETAY (tek action yeterli)
        public async Task<IActionResult> MessageDetail(int id)
        {
            var user = await _userManager.FindByNameAsync(User.Identity!.Name);
            if (user == null) return RedirectToAction("UserLogin", "Login");

            // güvenlik: sadece kendi mailine ait detayı görebilsin
            var message = await _context.Messages.FirstOrDefaultAsync(x =>
                x.MessageId == id &&
                (x.ReceiverEmail == user.Email || x.SenderEmail == user.Email));

            if (message == null) return NotFound();

            ViewBag.SelectedMode = "details";

            return View(message); // Views/Message/MessageDetail.cshtml
        }

        // ✅ MESAJ OLUŞTUR (GET)
        [HttpGet]
        public IActionResult SendMessage()
        {
            ViewBag.SelectedMode = "sendmessage";
            return View(); // Views/Message/SendMessage.cshtml
        }

        // ✅ MESAJ OLUŞTUR (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendMessage(Message message)
        {
            var user = await _userManager.FindByNameAsync(User.Identity!.Name);
            if (user == null) return RedirectToAction("UserLogin", "Login");

            // DB kaydı
            message.SenderEmail = user.Email;
            message.SendDate = DateTime.Now;
            message.IsStatus = false;

            _context.Messages.Add(message);
            await _context.SaveChangesAsync();

            // Gerçek mail gönderme (hata olursa DB kaydı kalsın)
            try
            {
                var mimeMessage = new MimeMessage();
                mimeMessage.From.Add(new MailboxAddress("Admin Identity", "iremkosar97@gmail.com"));
                mimeMessage.To.Add(new MailboxAddress("User", message.ReceiverEmail));
                mimeMessage.Subject = message.Subject;

                var bodyBuilder = new BodyBuilder
                {
                    // Summernote HTML üretiyor -> HTML olarak göndermek daha doğru
                    HtmlBody = message.MessageDetail
                };
                mimeMessage.Body = bodyBuilder.ToMessageBody();

                using var client = new MailKit.Net.Smtp.SmtpClient();
                client.Connect("smtp.gmail.com", 587, false);
                client.Authenticate("iremkosar97@gmail.com", "ukky vjjj jofu hefr");
                client.Send(mimeMessage);
                client.Disconnect(true);
            }
            catch
            {
                // istersen log ekleriz
            }

            return RedirectToAction(nameof(Sendbox));
        }
    }
}
