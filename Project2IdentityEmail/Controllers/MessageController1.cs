using Microsoft.AspNetCore.Mvc;
using Project2IdentityEmail.Context;

namespace Project2IdentityEmail.Controllers
{
    public class MessageController1 : Controller
    {
        private readonly EmailContext _context;

        public MessageController1(EmailContext context)
        {
            _context = context;
        }

        public IActionResult Inbox()
        {
            var values = _context.Messages.Where(x=>x.ReceiverEmail=="ali@gmail.com").ToList();
            //var values = _context.Messages.ToList();

            return View(values);
        }
        public IActionResult Sendbox()
        {
            var values = _context.Messages.Where(x => x.SenderEmail == "ali@gmail.com").ToList();
            //var values = _context.Messages.ToList();

            return View(values);
        }
        public IActionResult MessageDetail()
        {
            var value = _context.Messages.Where(x => x.MessageId == 1).FirstOrDefault();
            return View(value);
        }
        [HttpGet]
        public IActionResult ComposeMessage()
        {
            return View();
        }
    }
}
