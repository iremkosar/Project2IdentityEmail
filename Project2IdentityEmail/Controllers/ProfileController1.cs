using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Project2IdentityEmail.Dtos;
using Project2IdentityEmail.Entities;
using Project2IdentityEmail.Context;   // ✅ eklendi

namespace Project2IdentityEmail.Controllers
{
    public class ProfileController1 : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly EmailContext _context; // ✅ eklendi

        public ProfileController1(UserManager<AppUser> userManager, EmailContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> EditProfile()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            if (user == null) return RedirectToAction("UserLogin", "Login");

            // Okunan / Okunmamış sayıları (Mevcut kodunuz)
            ViewBag.UnreadCount = _context.Messages.Count(x => x.ReceiverEmail == user.Email && x.IsStatus == false);
            ViewBag.ReadCount = _context.Messages.Count(x => x.ReceiverEmail == user.Email && x.IsStatus == true);

            // DİKKAT: Telefonu manuel olarak UserManager'dan talep edelim
            var currentPhone = await _userManager.GetPhoneNumberAsync(user);

            var userEditDto = new UserEditDto
            {
                Email = user.Email,
                Name = user.Name,
                Surname = user.Surname,
                UserName = user.UserName,
                ImageUrl = user.ImageUrl,
                About = user.About,
                PhoneNumber = currentPhone // user.PhoneNumber yerine bu değişkeni verin
            };

            return View(userEditDto);
        }

        [HttpPost]
        public async Task<IActionResult> EditProfile(UserEditDto userEditDto)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            if (user == null) return RedirectToAction("UserLogin", "Login");

            // ✅ alan güncellemeleri
            user.Name = userEditDto.Name;
            user.Surname = userEditDto.Surname;
            user.UserName = userEditDto.UserName;
            user.About = userEditDto.About;
            user.Email = userEditDto.Email;
            user.PhoneNumber = userEditDto.PhoneNumber;

            // ✅ Fotoğraf seçildiyse upload et, seçilmediyse dokunma
            if (userEditDto.Image != null && userEditDto.Image.Length > 0)
            {
                var resource = Directory.GetCurrentDirectory();
                var extension = Path.GetExtension(userEditDto.Image.FileName);
                var imageName = Guid.NewGuid() + extension;
                var saveLocation = Path.Combine(resource, "wwwroot/images", imageName);

                using var stream = new FileStream(saveLocation, FileMode.Create);
                await userEditDto.Image.CopyToAsync(stream);

                user.ImageUrl = imageName;
            }

            // ✅ Şifre boş değilse değiştir
            if (!string.IsNullOrWhiteSpace(userEditDto.Password))
            {
                user.PasswordHash = _userManager.PasswordHasher.HashPassword(user, userEditDto.Password);
            }

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                // İstersen profilde kalsın:
                return RedirectToAction("EditProfile");
                // ya da login'e atmak istersen:
                // return RedirectToAction("UserLogin", "Login");
            }

            // hata varsa tekrar sayıları da yollayalım ki view boş kalmasın
            ViewBag.UnreadCount = _context.Messages.Count(x => x.ReceiverEmail == user.Email && x.IsStatus == false);
            ViewBag.ReadCount = _context.Messages.Count(x => x.ReceiverEmail == user.Email && x.IsStatus == true);

            // ModelState'e identity hatalarını bas (istersen)
            foreach (var err in result.Errors)
                ModelState.AddModelError("", err.Description);

            // tekrar aynı view'e dto ile dön
            userEditDto.ImageUrl = user.ImageUrl;
            return View(userEditDto);
        }
    }
}
