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

        //private int _clinicId;

        public FranchiseController(IFranchiseRepository franchiseRepository, PlasticDbContext context)
        {
            _franchiseRepository = franchiseRepository;
            _context = context;
        }

        // GET: FranchiseController
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
                var franchiseMVM = new FranchiseModalViewModel();
                var doctorVM = new DoctorViewModel(); //// KONTROL ET???????????????????????????

                var franchiseModalViewModelJson = TempData["FranchiseModalViewModel"] as string;
                if (!string.IsNullOrEmpty(franchiseModalViewModelJson))
                {
                    franchiseMVM = JsonSerializer.Deserialize<FranchiseModalViewModel>(franchiseModalViewModelJson);
                }

                //formlarda işlem yaptıktan sonra id yi tutabilmek için 
                if (id == 0) { id = franchiseMVM.Franchise.Id; }
                if (id == 0) { id = doctorVM.FranchiseId; }  //// KONTROL ET???????????????????????????

                HttpContext.Session.SetInt32("_FranchiseId", id);

                var franchise = await _franchiseRepository.GetByIdFranchiseAsync(id);
                if (franchise == null) { return RedirectToAction("Index", "Clinic"); }



                if (franchise != null)
                {
                    franchiseMVM.Franchise.Id = id; //id franchise id idi zaten
                    franchiseMVM.Franchise.Adress = franchise.Adress;
                    franchiseMVM.Franchise.Email = franchise.Email;
                    franchiseMVM.Franchise.Phone = franchise.Phone;
                }

                return View(franchiseMVM);
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "An error occurred.");
                return StatusCode(500, "Internal server error");

                //return RedirectToAction(nameof(Index));
            }
        }

        public PartialViewResult Operation()
        {
            var _id = Convert.ToInt32(HttpContext.Session.GetInt32("_FranchiseId"));
            try
            {
                var operation = _franchiseRepository.GetOperationDoctor(_id).ToList();  
                return PartialView("_PartialOperation", operation);  //_PartialView.cshtml Views/Operation/Index.cshtml  
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return PartialView("_PartialDoctor");  //veri döndürmeyince hata verir!!!!!!!!!!!!!!!!!!1
            }
        }
        public PartialViewResult Doctor()
        {
            var _id = Convert.ToInt32(HttpContext.Session.GetInt32("_FranchiseId"));
            try
            {
                var doctor = _franchiseRepository.GetDoctorByFranchiseId(_id);
                return PartialView("_PartialDoctor", doctor);  //_PartialView.cshtml Views/Operation/Index.cshtml  
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return PartialView("_PartialDoctor");  //doctor döndürmeyince hata verir!!!!!!!!!!!!!!!!!!1
            }

        }


        // GET: FranchiseController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: FranchiseController/Create
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

        // GET: FranchiseController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: FranchiseController/Edit/5
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

        // GET: FranchiseController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: FranchiseController/Delete/5
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
