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
        public PartialViewResult OperationDoctor(string clinicId, string franchiseId)
        {
            var _idClinic = clinicId;
            var _idFranchise = franchiseId;

            List<OperationDoctor> operations = new List<OperationDoctor>();
            try
            {
                if (_idFranchise == null)
                {
                    operations = _operationdoctorRepository.GetAllOperationDoctorByClinicId(_idClinic).ToList();
                }
                else if (_idClinic == null)
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

        public ActionResult Create(string clinicId, string franchiseId)
        {
            var _idClinic = clinicId;
            var _idFranchise = franchiseId;
            //_idClinic = Convert.ToInt32(HttpContext.Session.GetInt32("_ClinicId"));
            //_idFranchise = Convert.ToInt32(HttpContext.Session.GetInt32("_FranchiseId"));

            var operationDoctorVM = new OperationDoctorViewModel()
            {
                ClinicId = clinicId,
                FranchiseId = franchiseId,
            };

            //Select doctor ve operations işlemleri
            {
                var operations = _context.Operations.Include(a => a.Category).ToList();
                operationDoctorVM.Operations = operations;
                operationDoctorVM.OperationIds = operations.Select(a => a.Id).ToList();

                if (_idFranchise == null)
                {
                    var doctors = _context.Doctors.Include(a => a.Clinic).Where(b => b.ClinicId == _idClinic && b.Status == true && b.Deleted == false).ToList();
                    operationDoctorVM.Doctors = doctors;
                    operationDoctorVM.DoctorIds = doctors.Select(a => a.Id).ToList();
                }
                else if (_idClinic == null)
                {
                    var doctors = _context.Doctors.Include(a => a.Clinic).Where(b => b.FranchiseId == _idFranchise && b.Status == true && b.Deleted == false).ToList();
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
            var _idClinic = operationDoctorVM.ClinicId;
            var _idFranchise = operationDoctorVM.FranchiseId;
            try
            {
                var operationdoctorModelState = ModelState
                                    .Where(ms => ms.Key.StartsWith("OperationDoctor."))
                                    .ToDictionary(ms => ms.Key, ms => ms.Value);

                var operationDoctor = new OperationDoctor();

                int selectedDoctorId = operationDoctorVM.DoctorIds?.FirstOrDefault() ?? 0;
                int selectedOperationId = operationDoctorVM.OperationIds?.FirstOrDefault() ?? 0;

                //operationdoctor.DoctorId = SelectedDoctorId; //operationDoctorVM.DoctorIds;
                //operationdoctor.OperationId = SelectedOperationId;

                ModelState.Remove("OperationDoctor");
                ModelState.Remove("OperationDoctor.Doctor");
                ModelState.Remove("OperationDoctor.Operation");

                // MODEL SEÇİMİ YÖNETİMİ İLE BU SORUNU ÇÖZ !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                // DoctorId ve OperationId hiç seçilmese de her türlü model stateyi geçiyor DÜZELT!!!!!!!!!!!!!!!!!!
                if (operationDoctorVM.DoctorIds.Any() && operationDoctorVM.OperationIds.Any())
                {
                    //ModelState.AddModelError("", "Doktor ve işlem seçmek zorunludur.");
                    //}
                    //else if (ModelState.IsValid) // operationdoctorModelState.Values.All(v => v.Errors.Count == 0)
                    //{
                    operationDoctor = operationDoctorVM.OperationDoctor;

                    operationDoctor.Doctor = _context.Doctors.Find(selectedDoctorId);
                    operationDoctor.Operation = _context.Operations.Find(selectedOperationId);

                    // OperationDoctor ıd sinin db de 0 lma çözümü. MODELİ TEKRARDAN YAP KÖKTEN ÇÖZ!!!!!!!!!!!!!!!!!!!????????????????????????? 
                    //{
                    //    var lastOperationDoctor = _context.OperationDoctors.OrderByDescending(o => o.Id).FirstOrDefault();
                    //    int lastOperationDoctorId = lastOperationDoctor != null ? lastOperationDoctor.Id : 0;
                    //    lastOperationDoctorId += 1;
                    //    operationDoctor.Id = lastOperationDoctorId;
                    //}

                    //  BaseEntity 
                    {
                        operationDoctor.Status = true;
                        operationDoctor.Deleted = false;
                        operationDoctor.CreatedDate = DateTime.Now;
                        operationDoctor.UpdatedDate = DateTime.Now;
                        operationDoctor.CreatedBy = 0;
                        operationDoctor.UpdatedBy = 0;
                    }

                    if (operationDoctorVM.Image1 != null)
                    {
                        var result1 = await _photoService.AddPhotoAsync(operationDoctorVM.Image1);
                        operationDoctor.ImageUrl1 = result1.Url.ToString();
                    }
                    if (operationDoctorVM.Image2 != null)
                    {
                        var result2 = await _photoService.AddPhotoAsync(operationDoctorVM.Image2);
                        operationDoctor.ImageUrl2 = result2.Url.ToString();
                    }
                    if (operationDoctorVM.Image3 != null)
                    {
                        var result3 = await _photoService.AddPhotoAsync(operationDoctorVM.Image3);
                        operationDoctor.ImageUrl3 = result3.Url.ToString();
                    }

                    operationDoctor.DoctorId = selectedDoctorId;
                    operationDoctor.OperationId = selectedOperationId;

                    _context.Add(operationDoctor);
                    _context.SaveChanges();

                    if (_idFranchise == null) //form verileri clinicten geldiyse
                    {
                        return RedirectToAction("Details", "Clinic", new { id = _idClinic });
                    }
                    else if (_idClinic == null) //form verileri franchisedan geldiyse
                    {
                        return RedirectToAction("Details", "Franchise", new { id = _idFranchise });
                    }

                }

                //TempData["ClinicModalViewModel"] = JsonSerializer.Serialize(clinicMVM);
                //TempData["FranchiseModalViewModel"] = JsonSerializer.Serialize(franchiseMVM);

                if (_idClinic == null) //from verileri franchise dan gelmiştir
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
                if (_idFranchise == null)
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

        public ActionResult Edit(int id, string clinicId, string franchiseId)
        {
            var operationDoctorVM = new OperationDoctorViewModel();

            var _idClinic = clinicId;
            operationDoctorVM.ClinicId = clinicId;
            var _idFranchise = franchiseId;
            operationDoctorVM.FranchiseId = franchiseId;

            //_idClinic = Convert.ToInt32(HttpContext.Session.GetInt32("_ClinicId"));
            //_idFranchise = Convert.ToInt32(HttpContext.Session.GetInt32("_FranchiseId"));

            //var operationDoctor = _operationdoctorRepository.GetOperationDoctorByIdAsync(id);
            //if (operationDoctor == null) { return RedirectToAction("~/Views/OperationDoctor/_PartialOperationDoctor.cshtml"); } //????????????????????????*

            var selectedOperationDoctor = _context.OperationDoctors.FirstOrDefault(a => a.Id == id);

            { operationDoctorVM.OperationDoctor = selectedOperationDoctor; }

            //Select doctor ve operations işlemleri
            {
                var operations = _context.Operations.Include(a => a.Category).ToList();
                operationDoctorVM.Operations = operations;
                operationDoctorVM.OperationIds = operations.Select(a => a.Id).ToList();

                if (_idFranchise == null)
                {
                    var doctors = _context.Doctors.Include(a => a.Clinic).Where(b => b.ClinicId == _idClinic && b.Status == true && b.Deleted == false).ToList();
                    operationDoctorVM.Doctors = doctors;
                    operationDoctorVM.DoctorIds = doctors.Select(a => a.Id).ToList();
                }
                else if (_idClinic == null)
                {
                    var doctors = _context.Doctors.Include(a => a.Clinic).Where(b => b.FranchiseId == _idFranchise && b.Status == true && b.Deleted == false).ToList();
                    operationDoctorVM.Doctors = doctors;
                    operationDoctorVM.DoctorIds = doctors.Select(a => a.Id).ToList();
                }
            }
            return PartialView("~/Views/OperationDoctor/_PartialEditOperationDoctor.cshtml", operationDoctorVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, OperationDoctorViewModel operationDoctorVM)
        {
            var _idClinic = operationDoctorVM.ClinicId;
            var _idFranchise = operationDoctorVM.FranchiseId;
            try
            {
                OperationDoctor? operationDoctor = await _operationdoctorRepository.GetOperationDoctorByIdAsync(id);

                if (operationDoctor != null)
                {
                    try
                    {
                        if (operationDoctorVM.Image1 != null)
                        {
                            await _photoService.DeletePhotoAsync(Convert.ToString(operationDoctorVM.Image1));
                        }
                        else if (operationDoctorVM.Image2 != null)
                        {
                            await _photoService.DeletePhotoAsync(Convert.ToString(operationDoctorVM.Image2));
                        }
                        else if (operationDoctorVM.Image3 != null)
                        {
                            await _photoService.DeletePhotoAsync(Convert.ToString(operationDoctorVM.Image3));
                        }
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("", "Fotoğaf silinemedi.");
                        return PartialView("~/Views/OperationDoctor/_PartialEditOperationDoctor.cshtml", operationDoctorVM);
                    }

                    operationDoctor = operationDoctorVM.OperationDoctor;      

                    if (operationDoctorVM.Image1 != null)
                    {
                        var photoResult1 = await _photoService.AddPhotoAsync(operationDoctorVM.Image1);
                        operationDoctor.ImageUrl1 = photoResult1.Url.ToString();
                    }
                    if (operationDoctorVM.Image2 != null)
                    {
                        var photoResult2 = await _photoService.AddPhotoAsync(operationDoctorVM.Image2);
                        operationDoctor.ImageUrl2 = photoResult2.Url.ToString();
                    }
                    if (operationDoctorVM.Image3 != null)
                    {
                    var photoResult3 = await _photoService.AddPhotoAsync(operationDoctorVM.Image3);
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

                    if (_idClinic == null) //from verileri franchise dan gelmiştir
                    {
                        return RedirectToAction("Details", "Franchise", new { id = _idFranchise });
                    }
                    else //from verileri clinic den gelmiştir
                    {
                        return RedirectToAction("Details", "Clinic", new { id = _idClinic });
                    }
                }
                else
                {
                    return PartialView("~/Views/OperationDoctor/_PartialEditOperationDoctor.cshtml", operationDoctorVM);
                }

            }
            catch
            {
                if (_idClinic == null) //from verileri franchise dan gelmiştir
                {
                    return RedirectToAction("Details", "Franchise", new { id = _idFranchise });
                }
                else //from verileri clinic den gelmiştir
                {
                    return RedirectToAction("Details", "Clinic", new { id = _idClinic });
                }
            }
        }

        public async Task<IActionResult> Delete(int id, string clinicId, string franchiseId)
        {
            var operationDoctorVM = new OperationDoctorViewModel()
            {
                OperationDoctor = new OperationDoctor(),
                ClinicId = clinicId,
                FranchiseId = franchiseId
            };

            //OperationDoctor? operationDoctor = await _operationdoctorRepository.GetOperationDoctorByIdAsync(id);
            var operationDoctor = _context.OperationDoctors.Include(a => a.Doctor).Include(a => a.Operation).FirstOrDefault(o => o.Id == id);
            operationDoctorVM.OperationDoctor = operationDoctor;

            return PartialView("~/Views/OperationDoctor/_PartialDeleteOperationDoctor.cshtml", operationDoctorVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection, OperationDoctorViewModel operationDoctorVM)
        {
            var _idClinic = "";
            var _idFranchise = "";
            try
            {
                _idClinic = operationDoctorVM.ClinicId;
                _idFranchise = operationDoctorVM.FranchiseId;
                //_idClinic = Convert.ToInt32(HttpContext.Session.GetInt32("_ClinicId"));
                //_idFranchise = Convert.ToInt32(HttpContext.Session.GetInt32("_FranchiseId"));

                var operationDoctor = _context.OperationDoctors.FirstOrDefault(a => a.Id == id);
                if (operationDoctor != null)
                {
                    operationDoctor.Status = false;
                    operationDoctor.Deleted = true;
                    _context.OperationDoctors.Update(operationDoctor);
                    _context.SaveChanges();
                }

                if (_idFranchise == null)
                {
                    return RedirectToAction("Details", "Clinic", new { id = _idClinic });
                }
                else
                {
                    return RedirectToAction("Details", "Franchise", new { id = _idFranchise });
                }

            }
            catch
            {
                if (_idFranchise == null)
                {
                    return RedirectToAction("Details", "Clinic", new { id = _idClinic });
                }
                else
                {
                    return RedirectToAction("Details", "Franchise", new { id = _idFranchise });
                }
            }
        }
    }
}
