using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using Project2IdentityEmail.Context;
using Project2IdentityEmail.Dtos;
using Project2IdentityEmail.Entities;

namespace Project2IdentityEmail.Controllers
{
    public class EmailController : Controller
    {
        private readonly EmailContext _context;
        private readonly UserManager<AppUser> _userManager;

        public EmailController(EmailContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Index(MailRequestDto mailRequestDto)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("UserLogin", "Login");

            // 1) Önce maili gönder
            var mimeMessage = new MimeMessage();
            mimeMessage.From.Add(new MailboxAddress("Admin Identity", user.Email));
            mimeMessage.To.Add(new MailboxAddress("User", mailRequestDto.ReceiverEmail));
            mimeMessage.Subject = mailRequestDto.Subject;

            var bodyBuilder = new BodyBuilder { TextBody = mailRequestDto.MessageDetail };
            mimeMessage.Body = bodyBuilder.ToMessageBody();

            using var client = new SmtpClient();
            client.Connect("smtp.gmail.com", 587, false);
            client.Authenticate("iremkosar97@gmail.com", "ukky vjjj jofu hefr");
            client.Send(mimeMessage);
            client.Disconnect(true);

            // 2) Sonra DB’ye kaydet (Sendbox için)
            var message = new Message
            {
                SenderEmail = user.Email,
                ReceiverEmail = mailRequestDto.ReceiverEmail,
                Subject = mailRequestDto.Subject,
                MessageDetail = mailRequestDto.MessageDetail,
                SendDate = DateTime.Now,
                CategoryId = 1,
                IsStatus = false
            };

            _context.Messages.Add(message);
            _context.SaveChanges();

            return RedirectToAction("Sendbox", "MessageController1");
        }
        //[HttpPost]
        //public IActionResult Index(MailRequestDto mailRequestDto)
        //{
        //    MimeMessage mimeMessage = new MimeMessage();

        //    MailboxAddress mailboxAddressForm = new MailboxAddress("Admin Identity", "iremkosar97@gmail.com");
        //    mimeMessage.From.Add(mailboxAddressForm);

        //    MailboxAddress mailboxAddressTo = new MailboxAddress("User", mailRequestDto.ReceiverEmail);
        //    mimeMessage.To.Add(mailboxAddressTo);

        //    mimeMessage.Subject = mailRequestDto.Subject;

        //    var bodyBuilder = new BodyBuilder();
        //    bodyBuilder.TextBody = mailRequestDto.MessageDetail;
        //    mimeMessage.Body = bodyBuilder.ToMessageBody();

        //    SmtpClient client = new SmtpClient();
        //    client.Connect("smtp.gmail.com", 587, false);
        //    client.Authenticate("iremkosar97@gmail.com", "ukky vjjj jofu hefr");
        //    client.Send(mimeMessage);
        //    client.Disconnect(true);

        //    return RedirectToAction("Sendbox");
        //}
    }
}
