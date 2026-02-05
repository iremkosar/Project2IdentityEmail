using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using Project2IdentityEmail.Dtos;

namespace Project2IdentityEmail.Controllers
{
    public class EmailController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Index(MailRequestDto mailRequestDto)
        {
            MimeMessage mimeMessage = new MimeMessage();

            MailboxAddress mailboxAddressForm = new MailboxAddress("Admin Identity", "iremkosar97@gmail.com");
            mimeMessage.From.Add(mailboxAddressForm);

            MailboxAddress mailboxAddressTo = new MailboxAddress("User", mailRequestDto.ReceiverEmail);
            mimeMessage.To.Add(mailboxAddressTo);

            mimeMessage.Subject = mailRequestDto.Subject;

            var bodyBuilder = new BodyBuilder();
            bodyBuilder.TextBody = mailRequestDto.MessageDetail;
            mimeMessage.Body = bodyBuilder.ToMessageBody();

            SmtpClient client = new SmtpClient();
            client.Connect("smtp.gmail.com", 587, false);
            client.Authenticate("iremkosar97@gmail.com", "ukky vjjj jofu hefr");
            client.Send(mimeMessage);
            client.Disconnect(true);

            return RedirectToAction("Sendbox");
        }
    }
}
