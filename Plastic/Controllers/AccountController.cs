using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Plastic.Data;
using Plastic.Models;
using Plastic.ViewModels;
using System.Numerics;

namespace Plastic.Controllers
{
    public class AccountController : Controller
    {
        private readonly PlasticDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public AccountController(PlasticDbContext context, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _context = context;
            _signInManager = signInManager;
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Login()
        {
            var result = new LoginViewModel();
            return View(result);
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel loginVM)
        {
            if (!ModelState.IsValid) { return View(loginVM); }

            var user = await _userManager.FindByEmailAsync(loginVM.EmailAddress);
            if (user != null)
            {
                await _signInManager.SignOutAsync();

                var passwordCheck = await _userManager.CheckPasswordAsync(user, loginVM.Password);
                if (passwordCheck)
                {
                    var result = await _signInManager.PasswordSignInAsync(user, loginVM.Password, false, false);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Index", "Clinic");
                    }
                }
                else
                {
                    TempData["Error"] = "Wrong credentials. Please try again.";  //uyarılar çalışmıyor!!!!!!!!!!!!!!!
                    return View(loginVM);
                }
            }
            else
            {
                TempData["Error"] = "Wrong credentials. Please try again.";
            }
            return View(loginVM);

        }

        public IActionResult Register()
        {
            var result = new RegisterViewModel();
            return View(result);
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel registerVM)
        {
            ModelState.Remove("Clinic");
            ModelState.Remove("Franchise");

            if (!ModelState.IsValid) return View(registerVM);

            var user = await _userManager.FindByEmailAsync(registerVM.EmailAddress);
            if (user != null)
            {
                TempData["Error"] = "This email address is already in use.";
                return View(registerVM);
            }

            var normalizedEmail = _userManager.NormalizeEmail(registerVM.EmailAddress);
            var newAppUser = new AppUser()
            {
                Email = registerVM.EmailAddress,
                UserName = registerVM.EmailAddress,
                NormalizedEmail = normalizedEmail,
                PhoneNumber = registerVM.Phone ?? null,
                
                User = new User()
                {                   
                    FirstName = registerVM.FirstName,
                    LastName = registerVM.LastName,
                    Gender = registerVM.Gender,
                    IdentityNumber = registerVM.IdentityNumber ?? null,
                    Country = registerVM.Country ?? null,

                    Status = true,
                    Deleted = false,
                    CreatedDate = DateTime.Now,
                    UpdatedDate = DateTime.Now,
                    CreatedBy = 0,
                    UpdatedBy = 0
                },

            };
            var newUserResponse = await _userManager.CreateAsync(newAppUser, registerVM.Password);
            await _context.SaveChangesAsync();

            if (newUserResponse.Succeeded)
            {
                newAppUser.UserId = newAppUser.Id;
                await _userManager.AddToRoleAsync(newAppUser, UserRoles.User);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index", "Clinic");
        }

        [HttpPost]
        public async Task<IActionResult> LogOut()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Clinic");
        }

    }
}
