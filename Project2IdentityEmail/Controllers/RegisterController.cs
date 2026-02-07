using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Project2IdentityEmail.Dtos;
using Project2IdentityEmail.Entities;
using MailKit.Net.Smtp;
using MimeKit;
using Humanizer;

namespace Project2IdentityEmail.Controllers
{
    public class RegisterController : Controller
    {
        private readonly UserManager<AppUser> _userManager;

        public RegisterController(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

       [HttpGet]
       public IActionResult CreateUser()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateUser(CreateUserRegisterDto createUserRegisterDto)
        {
            if (!ModelState.IsValid) return View(createUserRegisterDto);

            string code = new Random().Next(100000, 1000000).ToString();
            AppUser appUser = new AppUser()
            {
                Name = createUserRegisterDto.Name,
                Email = createUserRegisterDto.Email,
                Surname = createUserRegisterDto.Surname,
                UserName = createUserRegisterDto.Username,
                ConfirmCode = code
            };

            var result = await _userManager.CreateAsync(appUser, createUserRegisterDto.Password);

            if (result.Succeeded)
            {
                // Mail gönderme işlemini asenkron çağırmak daha iyidir
                SendConfirmMail(appUser.Email, code);

                TempData["ShowConfirmModal"] = "1";
                TempData["ConfirmEmail"] = appUser.Email;

                // Modeli temizleyip gönderiyoruz ki form boşalsın ama modal açılsın
                return View();
            }

            // Identity hatalarını (şifre çok kısa vb.) ModelState'e ekle
            foreach (var e in result.Errors)
            {
                ModelState.AddModelError("", e.Description);
            }

            return View(createUserRegisterDto);
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmCode(ConfirmCodeDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
            {
                TempData["ShowConfirmModal"] = "1";
                TempData["ConfirmEmail"] = dto.Email;
                TempData["ConfirmError"] = "Kullanıcı bulunamadı";
                return RedirectToAction("CreateUser");
            }

            if (user.ConfirmCode == dto.ConfirmCode)
            {
                user.ConfirmCode = null;
                user.EmailConfirmed = true;
                user.TwoFactorEnabled = true;
                await _userManager.UpdateAsync(user);

                return RedirectToAction("UserLogin", "Login");
            }

            TempData["ShowConfirmModal"] = "1";
            TempData["ConfirmEmail"] = dto.Email;
            TempData["ConfirmError"] = "Doğrulama kodu hatalı";
            return RedirectToAction("CreateUser");
        }

        public void SendConfirmMail(string toEmail, string code)
        {
            var mimeMessage = new MimeMessage();
            mimeMessage.From.Add(new MailboxAddress("Project2IdentityEmail", "iremkosar97@gmail.com"));
            mimeMessage.To.Add(new MailboxAddress("User", toEmail));
            mimeMessage.Subject = "Doğrulama Kodu";
            mimeMessage.Body = new TextPart("plain") { Text = $"Doğrulama kodunuz: {code}" };

            using var client = new SmtpClient();
            client.Connect("smtp.gmail.com", 587, false);
            client.Authenticate("iremkosar97@gmail.com", "ukky vjjj jofu hefr");
            client.Send(mimeMessage);
            client.Disconnect(true);
        }
    }
}
