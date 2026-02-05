using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Project2IdentityEmail.Dtos;
using Project2IdentityEmail.Entities;

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
            Random rnd = new Random();
            int x = rnd.Next(100000, 1000000);
            AppUser appUser = new AppUser()
            {
                Name = createUserRegisterDto.Name,
                Email = createUserRegisterDto.Email,
                Surname = createUserRegisterDto.Surname,
                UserName = createUserRegisterDto.Username,
                ConfirmCode = x.ToString()
            };
            var result= await _userManager.CreateAsync(appUser, createUserRegisterDto.Password);


            //Email Gönderme Kodu


            if(result.Succeeded)
            {
                return RedirectToAction("UserLogin","Login");
            }
            else
            {
                foreach(var item in result.Errors)
                {
                    ModelState.AddModelError("",item.Description);
                }
            }
            return View();
            
        }
    }
}
