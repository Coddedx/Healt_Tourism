using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Plastic.IRepository;
using Plastic.Models;
using Plastic.ViewModels;
using System.Diagnostics;
using System.Security.Cryptography;

namespace Plastic.Controllers
{
    public class FranchiseController : Controller
    {
        PlasticDbContext db = new PlasticDbContext();
        private readonly IFranchiseRepository _franchiseRepository;

        //private int _clinicId;

        public FranchiseController(IFranchiseRepository franchiseRepository)
        {
            _franchiseRepository = franchiseRepository; 
        }

        // GET: FranchiseController
        public async Task<IActionResult> Index(int id) //bu gelen ıd clinic ıd!!!!!!!1
        {
            //tıklanan sayfanın hastaneye mi cliniğe mi bağlı olduğu ayrıştırılıp id si mi çekilmeli (şimdilik hepsinin clinic old bild için @Model.Clinic.Name ile yazdım) ????????????????????????????????

            HttpContext.Session.SetInt32("_ClinicId", id);  
          
            var clinic = await _franchiseRepository.GetByIdClinicAsync(id);
            if (clinic == null) { return RedirectToAction("Index", "Clinic"); }

                return View(clinic);
        }

        public PartialViewResult Operation() //TÜM VERİLERİ GETİYOR DÜZELT!!!!!!!!!!!!!!!1
        {
            var _id = Convert.ToInt32(HttpContext.Session.GetInt32("_ClinicId"));  

            var operation =  _franchiseRepository.GetOperationDoctor(_id).ToList();  //Async
            return PartialView("_PartialOperation",operation);  //_PartialView.cshtml Views/Operation/Index.cshtml  
        }
        public PartialViewResult Doctor() 
        {
            //var doctor = db.Doctors.FirstOrDefault(c => c.FranchiseId == id);
            var _id = Convert.ToInt32(HttpContext.Session.GetInt32("_ClinicId"));

            var doctor = db.Doctors.ToList();
            return PartialView("_PartialDoctor",doctor);  //_PartialView.cshtml Views/Operation/Index.cshtml  
        }

        // GET: FranchiseController/Details/5
        public ActionResult Details(int id)
        {
            return View();
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
