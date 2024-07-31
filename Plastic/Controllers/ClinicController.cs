using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Plastic.IRepository;
using Plastic.Models;
using Plastic.Repository;
using Plastic.ViewModels;
using System.Diagnostics;
using System.Text.Json;

namespace Plastic.Controllers
{
	public class ClinicController : Controller
	{
		PlasticDbContext db = new PlasticDbContext();
        private readonly IClinicRepository _clinicRepository;
        private readonly ILogger<ClinicController> _logger;  //hataları yakalayıp loglamak için 

        public ClinicController(IClinicRepository clinicRepository, ILogger<ClinicController> logger)
        {
            _clinicRepository = clinicRepository;
            _logger = logger;
        }

        // GET: ClinicController
        public async Task<IActionResult> Index(int pageNumber)  //	, IFormCollection fc,int id
        {
			var clinics = db.Clinics
				.Where(c => c.Deleted == false)
				.ToList();
            return View(clinics);
        }

        // GET: ClinicController/Details/5
        public async Task<IActionResult> Details(int id) //clinic  ,DoctorViewModel _doctorVM
        {
			try
			{
                var clinicVM = new ClinicModalViewModel();

                var clinicModalViewModelJson = TempData["ClinicModalViewModel"] as string;
                if (!string.IsNullOrEmpty(clinicModalViewModelJson))
                {
                    clinicVM = JsonSerializer.Deserialize<ClinicModalViewModel>(clinicModalViewModelJson);
                }

                //formlarda işlem yaptıktan sonra id yi tutabilmek için 
                if (id == 0) { id = clinicVM.Clinic.Id; }

                HttpContext.Session.SetInt32("_ClinicId", id);

                var clinic = await _clinicRepository.GetByIdClinic(id);
                if (clinic == null) { return RedirectToAction("Index", "Clinic"); }

                if (clinic != null)
                {
					clinicVM.Clinic.Id = id; //id klinik id idi zaten
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

                //return RedirectToAction(nameof(Index));
            }
        }

        public PartialViewResult Operation() //TÜM VERİLERİ GETİYOR DÜZELT!!!!!!!!!!!!!!!1
        {
            var _id = Convert.ToInt32(HttpContext.Session.GetInt32("_ClinicId"));

            var operation = _clinicRepository.GetOperationDoctor(_id).ToList();  //Async
            return PartialView("_PartialOperation", operation);  //_PartialView.cshtml Views/Operation/Index.cshtml  
        }
        public PartialViewResult Doctor()
        {
            //var doctor = db.Doctors.FirstOrDefault(c => c.FranchiseId == id);
            var _id = Convert.ToInt32(HttpContext.Session.GetInt32("_ClinicId"));

            var doctor = db.Doctors.ToList();  //doctor modelini ıenumerable dan list/ıcollection a çevirdim 
            return PartialView("_PartialDoctor", doctor);  //_PartialView.cshtml Views/Operation/Index.cshtml  
        }


		// GET: ClinicController/Edit/5
		public ActionResult Edit(int id)
		{
			return View();
		}

		// POST: ClinicController/Edit/5
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

		// GET: ClinicController/Delete/5
		public ActionResult Delete(int id)
		{
			return View();
		}

		// POST: ClinicController/Delete/5
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
