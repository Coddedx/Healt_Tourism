using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Plastic.IRepository;
using Plastic.Models;
using System.Diagnostics;

namespace Plastic.Controllers
{
    public class FranchiseController : Controller
    {
        PlasticDbContext db = new PlasticDbContext();
        private readonly IFranchiseRepository _franchiseRepository;

        public FranchiseController(IFranchiseRepository franchiseRepository)
        {
            _franchiseRepository = franchiseRepository; 
        }

        // GET: FranchiseController
        public async Task<IActionResult> Index(int id) //bu gelen ıd clinic ıd!!!!!!!1
        {
            //tıklanan sayfanın hastaneye mi cliniğe mi bağlı olduğu ayrıştırılıp id si mi çekilmeli (şimdilik hepsinin clinic old bild için @Model.Clinic.Name ile yazdım) ????????????????????????????????

            var clinic = await _franchiseRepository.GetByIdClinicAsync(id);

            if (clinic == null) { return RedirectToAction("Index", "Clinic"); }

                return View(clinic);
        }

        public PartialViewResult Operation(int id)
        {
            //var operation = db.OperationDoctors.ToList();
            var operation = _franchiseRepository.GetOperationDoctorAsync(id);
            return PartialView("_PartialOperation",operation);  //_PartialView.cshtml Views/Operation/Index.cshtml  
        }
        public PartialViewResult Doctor() //int id
        {
            //var doctor = db.Doctors.FirstOrDefault(c => c.FranchiseId == id);
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
