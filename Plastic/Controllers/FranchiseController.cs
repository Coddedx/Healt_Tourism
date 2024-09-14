using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        public FranchiseController(IFranchiseRepository franchiseRepository, PlasticDbContext context)
        {
            _franchiseRepository = franchiseRepository;
            _context = context;
        }

        public async Task<IActionResult> Index() //int id
        {
            //tıklanan sayfanın hastaneye mi cliniğe mi bağlı olduğu ayrıştırılıp id si mi çekilmeli (şimdilik hepsinin clinic old bild için @Model.Clinic.Name ile yazdım) ????????????????????????????????

            //HttpContext.Session.SetInt32("_ClinicId", id);  

            //var clinic = await _franchiseRepository.GetByIdClinicAsync(id);
            //if (clinic == null) { return RedirectToAction("Index", "Clinic"); }

            //var franchise = _context.Franchises
            //        //.AsNoTracking()
            //        .Include(c => c.Clinic)
            //        .Where(d => d.ClinicId == 1).ToList();

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
                var doctorVM = new DoctorViewModel(); 

                //var franchiseModalViewModelJson = TempData["FranchiseModalViewModel"] as string;
                //if (!string.IsNullOrEmpty(franchiseModalViewModelJson))
                //{
                //    franchiseMVM = JsonSerializer.Deserialize<FranchiseModalViewModel>(franchiseModalViewModelJson);
                //}

                if (id == 0) { id = franchiseVM.Franchise.Id; }
                if (id == 0) { id = doctorVM.FranchiseId; }

                HttpContext.Session.SetInt32("_FranchiseId", id);

                var franchise = await _franchiseRepository.GetByIdFranchiseAsync(id);
                if (franchise == null) { return RedirectToAction("Index", "Clinic"); }

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

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
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

        public ActionResult Delete(int id)
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
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
    }
}
