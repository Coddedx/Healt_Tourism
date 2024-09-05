using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using Plastic.IRepository;
using Plastic.Models;
using Plastic.Repository;
using Plastic.ViewModels;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using System.Security.Principal;
using System.Text.Json;


namespace Plastic.Controllers
{
	public class ClinicController : Controller
	{
        private readonly PlasticDbContext _context;
        private readonly IClinicRepository _clinicRepository;
        private readonly IFranchiseRepository _franchiseRepository;
        private readonly ILogger<ClinicController> _logger;  //hataları yakalayıp loglamak için 

        public ClinicController(IClinicRepository clinicRepository, IFranchiseRepository franchiseRepository, ILogger<ClinicController> logger, PlasticDbContext context)
        {
            _clinicRepository = clinicRepository;
            _franchiseRepository = franchiseRepository;
            _logger = logger;
            _context = context;
        }

        // GET: ClinicController
        public async Task<IActionResult> Index()  //	int pageNumber, IFormCollection fc,int id
        {
            //        var clinics = _context.Clinics
            //.Where(c => c.Deleted == false)
            //.ToList();

            //Object reference not set to an instance of an object hatası almamak için new List ile başlatırız (ClinicModalViewModel deki gibi de yapılabilir)
            var ClinicFranchiseVM = new ClinicViewModel();
            ClinicFranchiseVM.Clinics = await _clinicRepository.GetAllClinicsAsync() ?? new List<Clinic>();
            ClinicFranchiseVM.Franchises = await _franchiseRepository.GetAllFranchisesAsync() ?? new List<Franchise>();

            //var clinics = await _clinicRepository.GetAllClinicsAsync();
            //var franchises = await _franchiseRepository.GetAllFranchisesAsync();
            //var ClinicFranchiseVM = new ClinicViewModel //list old için böyle yapabiliyorum
            //{
            //    Clinics = clinics,
            //    Franchises = franchises
            //};

            return View(ClinicFranchiseVM); //clinics
        }

        // GET: ClinicController/Details/5
        public async Task<IActionResult> Details(int id) //clinic  ,DoctorViewModel _doctorVM
            {
			try
			{               
                var clinicVM = new ClinicModalViewModel();
                var doctorVM = new DoctorViewModel(); //// KONTROL ET???????????????????????????

                //var PdoctorVM = new _PartialDoctorViewModel();//edit doctor da object set nul.. hatası için
                ////{
                ////    //Clinic = clinic,
                ////    Doctors = _context.Doctors?.ToList() ?? new List<Doctor>() // Eğer null ise boş liste ata
                ////};
                //ViewBag._PartialDoctorClinicId = PdoctorVM.ClinicId;

                if (id == 0) { id = clinicVM.Clinic.Id; }
                if (id == 0) { id = doctorVM.ClinicId; }  //// KONTROL ET???????????????????????????
               // if (id == 0) { id = PdoctorVM.ClinicId; }  

                //Operation Doctor için operasyonlar ve doktorların listelenemesi   ???????????????      
                {
                    var doctors = _clinicRepository.GetDoctorByClinicId(id);  //operation doctor da doktor seçmeyi seçeneklendiricem
                    ViewData["Doctors"] = doctors;

                    var categories = _clinicRepository.GetAllCategories();
                    ViewData["Categories"] = categories;

                    var categoryIds = _context.Categories.Select(c => c.Id).ToList();
                    var operations = _clinicRepository.GetAllOperationByCategoryId(categoryIds);
                    ViewData["Operations"] = operations;
                }

                //Form yanlış doldurulduktan sonra doldurulan yerlerin aynen gelmesi için verileri taşıyorum.
                var clinicModalViewModelJson = TempData["ClinicModalViewModel"] as string;
                if (!string.IsNullOrEmpty(clinicModalViewModelJson))
                {
                    clinicVM = JsonSerializer.Deserialize<ClinicModalViewModel>(clinicModalViewModelJson);
                }
                
                //formlarda işlem yaptıktan sonra id yi tutabilmek için ???????????????

                HttpContext.Session.SetInt32("_ClinicId", id);

                var clinic = await _clinicRepository.GetByIdClinicAsync(id);
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
            }
        }


        //break point siz çalışmayı çözebilirsen bunu kullan !!!!!!!!!!!!!!!!!!!!!!!!
        //[HttpGet]
        //public JsonResult GetOperationsByCategory(int categoryId)  
        //{
        //    var operations = _clinicRepository.GetAllOperationByCategoryId(categoryId);  // List<
        //    return Json(operations);
        //}

        public PartialViewResult OperationDoctor() 
        {
            var _id = Convert.ToInt32(HttpContext.Session.GetInt32("_ClinicId"));
            try
            {

                var operation = _clinicRepository.GetOperationDoctor(_id).ToList();  //Async
                return PartialView("_PartialOperationDoctor", operation);  //_PartialView.cshtml Views/Operation/Index.cshtml  
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return PartialView("_PartialOperatinDoctor");  //veri döndürmeyince hata verir!!!!!!!!!!!!!!!!!!1
            }
        }
        public PartialViewResult Doctor() 
        {
            var _id =Convert.ToInt32(HttpContext.Session.GetInt32("_ClinicId"));
            try
            {
                var PartialDoctorVm = new _PartialDoctorViewModel();
                PartialDoctorVm.Doctors = _clinicRepository.GetDoctorByClinicId(_id);
                //var doctor = _clinicRepository.GetDoctorByClinicId(_id);
                return PartialView("~/Views/Doctor/_PartialDoctor.cshtml", PartialDoctorVm);  //doctor  _PartialView.cshtml Views/Operation/Index.cshtml  
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return PartialView("~/Views/Doctor/_PartialDoctor.cshtml");  //doctor döndürmeyince hata verir!!!!!!!!!!!!!!!!!!1
            }
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
