using Azure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Plastic.IRepository;
using Plastic.Models;
using Plastic.Repository;
using Plastic.ViewModels;
using System.Linq;
using System.Numerics;
using System.Text.Json;


namespace Plastic.Controllers
{
    public class OperationDoctorController : Controller
    {
        
        private readonly PlasticDbContext _context;
        private readonly IOperationDoctorRepository _operationdoctorRepository;
        private readonly IPhotoService _photoService;

        public OperationDoctorController(IPhotoService photoService, IOperationDoctorRepository operationdoctorRepository, PlasticDbContext context)
        {
            _context = context;
            _photoService = photoService;
            _operationdoctorRepository = operationdoctorRepository;
        }

        public ActionResult Index()
        {
            return View();
        }
        public PartialViewResult OperationDoctor()
        {
            var _idClinic = Convert.ToInt32(HttpContext.Session.GetInt32("_ClinicId"));
            var _idFranchise = Convert.ToInt32(HttpContext.Session.GetInt32("_FranchiseId"));
            List<OperationDoctor> operations = new List<OperationDoctor>();
            try
            {
                if (_idFranchise == 0)
                {
                    operations = _operationdoctorRepository.GetAllOperationDoctorByClinicId(_idClinic).ToList();
                }
                else if (_idClinic == 0)
                {
                    operations = _operationdoctorRepository.GetAllOperationDoctorByFranchiseId(_idFranchise).ToList();
                }
                return PartialView("~/Views/OperationDoctor/_PartialOperationDoctor.cshtml", operations);  //_PartialView.cshtml Views/Operation/Index.cshtml  
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return PartialView("~/Views/OperationDoctor/_PartialOperationDoctor.cshtml", operations);  //veri döndürmeyince hata verir!!!!!!!!!!!!!!!!!!1
            }
        }

        public ActionResult Details(int id)
        {
            return View();
        }

        public ActionResult Create()
        {
            var _idClinic = 0;
            var _idFranchise = 0;
            _idClinic = Convert.ToInt32(HttpContext.Session.GetInt32("_ClinicId"));
            _idFranchise = Convert.ToInt32(HttpContext.Session.GetInt32("_FranchiseId"));

            var operationDoctorVM = new OperationDoctorViewModel();

            //Select doctor ve operations işlemleri
            {
                var operations = _context.Operations.Include(a => a.Category).ToList();
                operationDoctorVM.Operations = operations;
                operationDoctorVM.OperationIds = operations.Select(a => a.Id).ToList();

                if (_idFranchise == 0)
                {
                    var doctors = _context.Doctors.Include(a => a.Clinic).Where(b => b.ClinicId == _idClinic).ToList();
                    operationDoctorVM.Doctors = doctors;
                    operationDoctorVM.DoctorIds = doctors.Select(a => a.Id).ToList();
                }
                else if (_idClinic == 0)
                {
                    var doctors = _context.Doctors.Include(a => a.Clinic).Where(b => b.FranchiseId == _idFranchise).ToList();
                    operationDoctorVM.Doctors = doctors;
                    operationDoctorVM.DoctorIds = doctors.Select(a => a.Id).ToList();
                }
            }
            return PartialView("~/Views/OperationDoctor/_PartialCreateOperationDoctor.cshtml", operationDoctorVM);
            // return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(OperationDoctorViewModel operationDoctorVM)
        {
            var _idClinic = 0;
            var _idFranchise = 0;
            try
            {
                var operationdoctorModelState = ModelState
                                    .Where(ms => ms.Key.StartsWith("OperationDoctor."))
                                    .ToDictionary(ms => ms.Key, ms => ms.Value);

                _idClinic = Convert.ToInt32(HttpContext.Session.GetInt32("_ClinicId"));
                _idFranchise = Convert.ToInt32(HttpContext.Session.GetInt32("_FranchiseId"));

                var operationDoctor = new OperationDoctor();

                int selectedDoctorId = operationDoctorVM.DoctorIds?.FirstOrDefault() ?? 0;
                int selectedOperationId = operationDoctorVM.OperationIds?.FirstOrDefault() ?? 0;

                //operationdoctor.DoctorId = SelectedDoctorId; //operationDoctorVM.DoctorIds;
                //operationdoctor.OperationId = SelectedOperationId;

                ModelState.Remove("OperationDoctor.Doctor");
                ModelState.Remove("OperationDoctor.Operation");

                // MODEL SEÇİMİ YÖNETİMİ İLE BU SORUNU ÇÖZ !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                // DoctorId ve OperationId hiç seçilmese de her türlü model stateyi geçiyor DÜZELT!!!!!!!!!!!!!!!!!!
                //if (operationdoctorModelState.Values.All(v => v.Errors.Count == 0)) // ModelState.IsValid
                //{
                operationDoctor = operationDoctorVM.OperationDoctor;

                operationDoctor.Doctor = _context.Doctors.Find(selectedDoctorId);
                operationDoctor.Operation = _context.Operations.Find(selectedOperationId);

                // OperationDoctor ıd sinin db de 0 lma çözümü. MODELİ TEKRARDAN YAP KÖKTEN ÇÖZ!!!!!!!!!!!!!!!!!!!????????????????????????? 
                {
                    var lastOperationDoctor = _context.OperationDoctors.OrderByDescending(o => o.Id).FirstOrDefault();
                    int lastOperationDoctorId = lastOperationDoctor != null ? lastOperationDoctor.Id : 0;
                    lastOperationDoctorId += 1;
                    operationDoctor.Id = lastOperationDoctorId;
                }

                //  BaseEntity 
                {
                    operationDoctor.Status = true;
                    operationDoctor.Deleted = false;
                    operationDoctor.CreatedDate = DateTime.Now;
                    operationDoctor.UpdatedDate = DateTime.Now;
                    operationDoctor.CreatedBy = 0;
                    operationDoctor.UpdatedBy = 0;
                }

                if (operationDoctorVM.Image1 != null || operationDoctorVM.Image2 != null || operationDoctorVM.Image3 != null)
                {
                    var result1 = await _photoService.AddPhotoAsync(operationDoctorVM.Image1);
                    var result2 = await _photoService.AddPhotoAsync(operationDoctorVM.Image2);
                    var result3 = await _photoService.AddPhotoAsync(operationDoctorVM.Image3);

                    operationDoctor.ImageUrl1 = result1.Url.ToString();
                    operationDoctor.ImageUrl2 = result2.Url.ToString();
                    operationDoctor.ImageUrl3 = result3.Url.ToString();
                }

                operationDoctor.DoctorId = selectedDoctorId;
                operationDoctor.OperationId = selectedOperationId;

                _context.Add(operationDoctor);
                _context.SaveChanges();

                if (_idFranchise == 0) //form verileri clinicten geldiyse
                {
                    return RedirectToAction("Details", "Clinic", new { id = _idClinic });
                }
                else if (_idClinic == 0) //form verileri franchisedan geldiyse
                {
                    return RedirectToAction("Details", "Franchise", new { id = _idFranchise });
                }

                //}

                //TempData["ClinicModalViewModel"] = JsonSerializer.Serialize(clinicMVM);
                //TempData["FranchiseModalViewModel"] = JsonSerializer.Serialize(franchiseMVM);

                if (_idClinic == 0) //from verileri franchise dan gelmiştir
                {
                    return RedirectToAction("Details", "Franchise", new { id = _idFranchise });
                }
                else //from verileri clinic den gelmiştir
                {
                    return RedirectToAction("Details", "Clinic", new { id = _idClinic });
                }
            }
            catch  //burası çalışmıyor klinikler sayfasına atıyor direk düzelt
            {
                if (_idFranchise == 0)
                {
                    return RedirectToAction("Details", "Clinic", new { id = _idClinic });
                    // return View(clinicMVM);

                }
                else
                {
                    return RedirectToAction("Details", "Franchise", new { id = _idFranchise });

                    // return View(franchiseMVM);
                }
            }
        }

        public ActionResult Edit(int id)
        {
            var _idClinic = 0;
            var _idFranchise = 0;

            _idClinic = Convert.ToInt32(HttpContext.Session.GetInt32("_ClinicId"));
            _idFranchise = Convert.ToInt32(HttpContext.Session.GetInt32("_FranchiseId"));

            var operationDoctor = _operationdoctorRepository.GetOperationDoctorByIdAsync(id);
            if (operationDoctor == null) { return RedirectToAction("~/Views/OperationDoctor/_PartialOperationDoctor.cshtml"); } //????????????????????????*

            var selectedOperationDoctor = _context.OperationDoctors.FirstOrDefault(a => a.Id == id);

            var operationDoctorVM = new OperationDoctorViewModel();
            { operationDoctorVM.OperationDoctor = selectedOperationDoctor; }

            //Select doctor ve operations işlemleri
            {
                var operations = _context.Operations.Include(a => a.Category).ToList();
                operationDoctorVM.Operations = operations;
                operationDoctorVM.OperationIds = operations.Select(a => a.Id).ToList();

                if (_idFranchise == 0)
                {
                    var doctors = _context.Doctors.Include(a => a.Clinic).Where(b => b.ClinicId == _idClinic).ToList();
                    operationDoctorVM.Doctors = doctors;
                    operationDoctorVM.DoctorIds = doctors.Select(a => a.Id).ToList();
                }
                else if (_idClinic == 0)
                {
                    var doctors = _context.Doctors.Include(a => a.Clinic).Where(b => b.FranchiseId == _idFranchise).ToList();
                    operationDoctorVM.Doctors = doctors;
                    operationDoctorVM.DoctorIds = doctors.Select(a => a.Id).ToList();
                }
            }

            return PartialView("~/Views/OperationDoctor/_PartialEditOperationDoctor.cshtml", operationDoctorVM);  //View olması lazım ama details de @html kısmıçözemediğim içn böyle şimdilik
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, OperationDoctorViewModel operationDoctorVM)
        {
            var _idClinic = 0;
            var _idFranchise = 0;
            try
            {
                _idClinic = Convert.ToInt32(HttpContext.Session.GetInt32("_ClinicId"));
                _idFranchise = Convert.ToInt32(HttpContext.Session.GetInt32("_FranchiseId"));

                OperationDoctor? operationDoctor = await _operationdoctorRepository.GetOperationDoctorByIdAsync(id);

                if (operationDoctor != null)
                {
                    try
                    {
                        await _photoService.DeletePhotoAsync(Convert.ToString(operationDoctorVM.Image1));
                        await _photoService.DeletePhotoAsync(Convert.ToString(operationDoctorVM.Image2));
                        await _photoService.DeletePhotoAsync(Convert.ToString(operationDoctorVM.Image3));
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("", "Fotoğaf silinemedi.");
                        return PartialView("~/Views/Doctor/_PartialEditOperationDoctor.cshtml", operationDoctorVM);
                    }
                    var photoResult1 = await _photoService.AddPhotoAsync(operationDoctorVM.Image1);
                    var photoResult2 = await _photoService.AddPhotoAsync(operationDoctorVM.Image2);
                    var photoResult3 = await _photoService.AddPhotoAsync(operationDoctorVM.Image3);
                    {
                        operationDoctor = operationDoctorVM.OperationDoctor;
                        operationDoctor.ImageUrl1 = photoResult1.Url.ToString();
                        operationDoctor.ImageUrl2 = photoResult2.Url.ToString();
                        operationDoctor.ImageUrl3 = photoResult3.Url.ToString();
                    }

                    //  BaseEntity 
                    {
                        operationDoctor.Status = true;
                        operationDoctor.Deleted = false;
                        operationDoctor.CreatedDate = DateTime.Now;
                        operationDoctor.UpdatedDate = DateTime.Now;
                        operationDoctor.CreatedBy = 0;
                        operationDoctor.UpdatedBy = 0;
                    }

                    _context.OperationDoctors.Update(operationDoctor);
                    _context.SaveChanges();

                    if (_idClinic == 0) //from verileri franchise dan gelmiştir
                    {
                        return RedirectToAction("Details", "Franchise", _idFranchise);
                    }
                    else //from verileri clinic den gelmiştir
                    {
                        return RedirectToAction("Details", "Clinic", _idClinic);
                    }
                }
                else
                {
                    return PartialView("~/Views/Doctor/_PartialEditOperationDoctor.cshtml", operationDoctorVM);
                }

            }
            catch
            {
                if (_idClinic == 0) //from verileri franchise dan gelmiştir
                {
                    return RedirectToAction("Details", "Franchise", _idFranchise);
                }
                else //from verileri clinic den gelmiştir
                {
                    return RedirectToAction("Details", "Clinic", _idClinic);
                }
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
