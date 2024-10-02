using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Plastic.IRepository;
using Plastic.Models;
using Plastic.Repository;
using Plastic.ViewModels;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text.Json;


namespace Plastic.Controllers
{
    public class FranchiseController : Controller
    {
        private readonly PlasticDbContext _context;
        private readonly IFranchiseRepository _franchiseRepository;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;


        public FranchiseController(IFranchiseRepository franchiseRepository, PlasticDbContext context, SignInManager<AppUser> signInManager, UserManager<AppUser> userManager)
        {
            _franchiseRepository = franchiseRepository;
            _context = context;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index() //int id
        {
            //tıklanan sayfanın hastaneye mi cliniğe mi bağlı olduğu ayrıştırılıp id si mi çekilmeli (şimdilik hepsinin clinic old bild için @Model.Franchise.Name ile yazdım) ????????????????????????????????

            //HttpContext.Session.SetInt32("_FranchiseId", id);  

            //var clinic = await _franchiseRepository.GetByIdFranchiseAsync(id);
            //if (clinic == null) { return RedirectToAction("Index", "Franchise"); }

            //var franchise = _context.Franchises
            //        //.AsNoTracking()
            //        .Include(c => c.Franchise)
            //        .Where(d => d.FranchiseId == 1).ToList();

            //if (franchise == null || !franchise.Any())
            //{
            //    // Veri bulunamadı, loglama veya hata yönetimi.
            //}

            //var franchise = _context.Franchises.Where(_context => _context.Id == 2).FirstOrDefault(); 

            //var doctor = _context.Doctors.Where(a => a.FranchiseId == 2).ToList();

            return View();
        }

        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var franchiseVM = new FranchiseViewModel();
                {
                    franchiseVM.Franchise = new Franchise();
                    franchiseVM.Franchise.Clinic = new Clinic();
                    franchiseVM.Franchise.District = new District();
                    franchiseVM.Franchise.District.City = new City();
                }

                //var franchiseModalViewModelJson = TempData["FranchiseModalViewModel"] as string;
                //if (!string.IsNullOrEmpty(franchiseModalViewModelJson))
                //{
                //    franchiseMVM = JsonSerializer.Deserialize<FranchiseModalViewModel>(franchiseModalViewModelJson);
                //}

                if (id == 0) { id = franchiseVM.Franchise.Id; }

                //HttpContext.Session.SetInt32("_FranchiseId", id);

                var franchise = await _franchiseRepository.GetByIdFranchiseAsync(id);
                if (franchise == null) { return RedirectToAction("Index", "Franchise"); }

                if (franchise != null)
                {
                    franchiseVM.Franchise.Id = id;
                    franchiseVM.Franchise.Title = franchise.Title;
                    franchiseVM.Franchise.District.Name = franchise.District.Name;
                    franchiseVM.Franchise.District.City.Name = franchise.District.City.Name;
                    franchiseVM.Franchise.Adress = franchise.Adress;
                    franchiseVM.Franchise.Email = franchise.Email;
                    franchiseVM.Franchise.Phone = franchise.Phone;
                }
                return View(franchiseVM);
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "An error occurred.");
                return StatusCode(500, "Internal server error");

                //return RedirectToAction(nameof(Index));
            }
        }

        public ActionResult Edit(int id)
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
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
            //Şehir arama butonu için
            {
                var cities = _context.Cities.Select(c => new { c.Id, c.Name }).ToList();
                ViewBag.Cities = new SelectList(cities, "Id", "Name");
            }
            //BÖLGE arama butonu için
            {
                var districts = _context.Districts.Select(d => new { d.Id, d.Name }).ToList();
                ViewBag.Districts = new SelectList(districts, "Id", "Name");
            }
            {
                var clinics = _context.Clinics.Select(d => new { d.Id, d.Name }).ToList();
                ViewBag.Clinics = new SelectList(clinics, "Id", "Name");
            }

            var result = new RegisterFranchiseViewModel();
            return View(result);
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterFranchiseViewModel registerFVM)
        {

            ModelState.Remove("Franchise.District");
            ModelState.Remove("Franchise.Clinic");
            ModelState.Remove("Franchise.Email");

            if (!ModelState.IsValid)
            {
                var cities = _context.Cities.Select(c => new { c.Id, c.Name }).ToList();
                ViewBag.Cities = new SelectList(cities, "Id", "Name");

                var districts = _context.Districts.Select(d => new { d.Id, d.Name }).ToList();
                ViewBag.Districts = new SelectList(districts, "Id", "Name");
                return View(registerFVM);
            }

            var user = await _userManager.FindByEmailAsync(registerFVM.EmailAddress);
            if (user != null)
            {
                TempData["Error"] = "This email address is already in use.";
                return View(registerFVM);
            }

            var normalizedEmail = _userManager.NormalizeEmail(registerFVM.EmailAddress);
            var newAppUser = new AppUser()
            {
                Email = registerFVM.EmailAddress,
                UserName = registerFVM.EmailAddress,
                NormalizedEmail = normalizedEmail,
                PhoneNumber = registerFVM.Franchise.Phone ?? null,

                Franchise = new Franchise()
                {
                    Title = registerFVM.Franchise.Title,
                    CertificationNumber = registerFVM.Franchise.CertificationNumber,
                    Adress = registerFVM.Franchise.Adress,
                    Email = registerFVM.EmailAddress, // CLİNİC/FRANCHİSE ID DÜZELTİNCE BUNU DA DÜZELT!!!!!!!!!!!1
                    Phone = registerFVM.Franchise.Phone,
                    DistrictId = registerFVM.DistrictId,
                    ClinicId = registerFVM.ClinicId,
                    Description = registerFVM.Franchise.Description,


                    Status = true,
                    Deleted = false,
                    CreatedDate = DateTime.Now,
                    UpdatedDate = DateTime.Now,
                    CreatedBy = 0,
                    UpdatedBy = 0
                },
            };

            var newUserResponse = await _userManager.CreateAsync(newAppUser, registerFVM.Password);
            _context.SaveChanges();

            if (newUserResponse.Succeeded)
            {
                newAppUser.FranchiseId = newAppUser.Id;
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index", "Clinic");
        }

    }
}
