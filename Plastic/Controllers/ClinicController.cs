using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using Plastic.Data;
using Plastic.IRepository;
using Plastic.Models;
using Plastic.Repository;
using Plastic.ViewModels;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using System.Security.Claims;
using System.Security.Principal;
using System.Text.Json;
using System.Xml.Linq;


namespace Plastic.Controllers
{
    public class ClinicController : Controller
    {
        private readonly PlasticDbContext _context;
        private readonly IClinicRepository _clinicRepository;
        private readonly IFranchiseRepository _franchiseRepository;
        private readonly ILogger<ClinicController> _logger;  //hataları yakalayıp loglamak için 
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;


        public ClinicController(IClinicRepository clinicRepository, IFranchiseRepository franchiseRepository, ILogger<ClinicController> logger, PlasticDbContext context, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _clinicRepository = clinicRepository;
            _franchiseRepository = franchiseRepository;
            _logger = logger;
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<IActionResult> Index(IFormCollection fc, int pageNumber = 1, int pageSize = 3)
        {
            //Object reference not set to an instance of an object hatası almamak için new List ile başlatırız (ClinicModalViewModel deki gibi de yapılabilir)
            var ClinicFranchiseVM = new ClinicViewModel()
            {
                Clinics = await _clinicRepository.GetAllClinicsAsync() ?? new List<Clinic>(),
                Franchises = await _franchiseRepository.GetAllFranchisesAsync() ?? new List<Franchise>()

            };

            //Arama butonu için
            {
                //ŞEHİR arama butonu için 
                {
                    var cities = _context.Cities.Select(c => new { c.Id, c.Name }).ToList();
                    ViewBag.Cities = new SelectList(cities, "Id", "Name");
                }
                //BÖLGE arama butonu için
                {
                    var districts = _context.Districts.Select(d => new { d.Id, d.Name }).ToList();
                    ViewBag.Districts = new SelectList(districts, "Id", "Name");

                }
                //İŞLEM arama butonu için 
                {
                    var operations = _context.Operations.Include(a => a.Category).ToList();
                    ViewBag.Operations = operations;
                    //operationDoctorVM.Operations = operations;
                    var OperationIds = operations.Select(a => a.Id).ToList();
                    ViewBag.OperationIds = OperationIds;
                }
                //KATEGORİ arama butonu için
                {
                    var categories = _context.Categories.ToList();
                    ViewBag.Categories = categories;
                    var categoryIds = categories.Select(a => a.Id).ToList();
                    ViewBag.CategoryIds = categoryIds;
                }

            }

            //OpenWeather api
            {
                string api = "ac8d0bd8b6affb7c9873f4d2fcea43b0";
                string city = "Antalya";
                ViewBag.City = city;
                string connection = "https://api.openweathermap.org/data/2.5/weather?q=" + city + "&mode=xml&lang=tr&units=metric&appid=" + api;
                try
                {
                    XDocument document = XDocument.Load(connection);
                    ViewBag.v4 = document.Descendants("temperature").ElementAt(0).Attribute("value").Value;//Descendants içine xml deki çekmek istediğimiz alan,Attribute Descendants içindeki hangi değeri alıcaz(xml deki val,min,max...)
                }
                catch (HttpRequestException ex)
                {
                    ViewBag.Error = "Hava durumu verilerine ulaşılamadı. Lütfen daha sonra tekrar deneyiniz.";
                    // Log ex for debugging purposes
                }
                catch (Exception ex)
                {
                    ViewBag.Error = "Bilinmeyen bir hata oluştu.";
                    // Log ex for debugging purposes
                }
            }

            // Search Clinics and Franchises
            var action = fc["action"];
            if (action == "search")
            {
                // Arama parametrelerini al
                var cityId = fc["cityS"];
                var districtId = fc["district"];
                var doctorName = fc["doctor"];
                var categoryId = fc["category"];
                var operationId = fc["operation"];

                ClinicFranchiseVM = await _clinicRepository.SearchClinicsAndFranchises(cityId, districtId, doctorName, categoryId, operationId);
            }

            //Pagination
            {
                // Toplam Clinic ve Franchise sayıları
                int totalClinics = ClinicFranchiseVM.Clinics.Count();
                int totalFranchises = ClinicFranchiseVM.Franchises.Count();

                // Kaç kayıt atlanacak
                int recSkip = (pageNumber - 1) * pageSize;

                // List ile sayfalama işlemi
                var pagedClinics = ClinicFranchiseVM.Clinics.Skip(recSkip).Take(pageSize).ToList();
                var pagedFranchises = ClinicFranchiseVM.Franchises.Skip(recSkip).Take(pageSize).ToList();

                // Pager sınıfı ile sayfalama bilgilerini hesapla
                var pager = new Pager(Math.Max(totalClinics, totalFranchises), pageNumber, pageSize);

                // ViewModel'e sayfalı verileri ve sayfalama bilgisini ekle
                ClinicFranchiseVM.Clinics = pagedClinics;
                ClinicFranchiseVM.Franchises = pagedFranchises;
                ClinicFranchiseVM.Pager = pager;
            }

            return View(ClinicFranchiseVM);
        }

        public async Task<IActionResult> Details(string id)
        {
            try
            {
                if (User.Identity.IsAuthenticated)
                {
                    if (User.IsInRole("clinic"))
                    {
                        var clinicId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                        id = clinicId;
                    }
                }

                var clinicVM = new ClinicViewModel();
                {
                    clinicVM.Clinic = new Clinic();
                    clinicVM.Clinic.District = new District();
                    clinicVM.Clinic.District.City = new City();
                };

                if (id == "") { id = clinicVM.Clinic.Id; }

                //Form yanlış doldurulduktan sonra doldurulan yerlerin aynen gelmesi için verileri taşıyorum.
                //var clinicModalViewModelJson = TempData["ClinicModalViewModel"] as string;
                //if (!string.IsNullOrEmpty(clinicModalViewModelJson))
                //{
                //    clinicVM = JsonSerializer.Deserialize<ClinicModalViewModel>(clinicModalViewModelJson);
                //}
                //formlarda işlem yaptıktan sonra id yi tutabilmek için ???????????????
                //HttpContext.Session.SetInt32("_ClinicId", id);

                var clinic = await _clinicRepository.GetByIdClinicAsync(id);
                if (clinic == null) { return RedirectToAction("Index", "Clinic"); }

                if (clinic != null)
                {
                    clinicVM.Clinic.Id = id; //id klinik id idi zaten
                    clinicVM.Clinic.Name = clinic.Name;
                    clinicVM.Clinic.District.Name = clinic.District.Name;
                    clinicVM.Clinic.District.City.Name = clinic.District.City.Name;
                    clinicVM.Clinic.Adress = clinic.Adress;
                    clinicVM.Clinic.Email = clinic.Email;
                    clinicVM.Clinic.Phone = clinic.Phone;
                }
                return View(clinicVM);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred.");
                return StatusCode(500, "Internal server error");
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

            var result = new RegisterClinicViewModel();
            return View(result);
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterClinicViewModel registerCVM)
        {

            ModelState.Remove("Clinic.District");
            ModelState.Remove("Clinic.Email");
            ModelState.Remove("Clinic.Id");

            if (!ModelState.IsValid)
            {
                var cities = _context.Cities.Select(c => new { c.Id, c.Name }).ToList();
                ViewBag.Cities = new SelectList(cities, "Id", "Name");

                var districts = _context.Districts.Select(d => new { d.Id, d.Name }).ToList();
                ViewBag.Districts = new SelectList(districts, "Id", "Name");
                return View(registerCVM);

            }

            var user = await _userManager.FindByEmailAsync(registerCVM.EmailAddress);
            if (user != null)
            {
                TempData["Error"] = "This email address is already in use.";
                return View(registerCVM);
            }

            var normalizedEmail = _userManager.NormalizeEmail(registerCVM.EmailAddress);
            var newAppUser = new AppUser()
            {
                Email = registerCVM.EmailAddress,
                UserName = registerCVM.EmailAddress,
                NormalizedEmail = normalizedEmail,
                PhoneNumber = registerCVM.Clinic.Phone ?? null,

                Clinic = new Clinic()
                {
                    Name = registerCVM.Clinic.Name,
                    CertificationNumber = registerCVM.Clinic.CertificationNumber,
                    Adress = registerCVM.Clinic.Adress,
                    Email = registerCVM.EmailAddress, // CLİNİC/FRANCHİSE ID DÜZELTİNCE BUNU DA DÜZELT!!!!!!!!!!!1
                    Phone = registerCVM.Clinic.Phone,
                    DistrictId = registerCVM.DistrictId,


                    Status = true,
                    Deleted = false,
                    CreatedDate = DateTime.Now,
                    UpdatedDate = DateTime.Now,
                    CreatedBy = 0,
                    UpdatedBy = 0
                },
            };

            var newUserResponse = await _userManager.CreateAsync(newAppUser, registerCVM.Password);
            _context.SaveChanges();

            if (newUserResponse.Succeeded)
            {
                newAppUser.ClinicId = newAppUser.Id;
                await _userManager.AddToRoleAsync(newAppUser, UserRoles.Clinic);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index", "Clinic");
        }

    }
}
