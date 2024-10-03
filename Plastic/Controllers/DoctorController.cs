using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Plastic.IRepository;
using Plastic.Models;
using Plastic.Repository;
using Plastic.Services;
using Plastic.ViewModels;
using System;
using System.Numerics;
using System.Reflection;
using System.Security.Cryptography;
using System.Text.Json;

namespace Plastic.Controllers
{
    public class DoctorController : Controller
    {
        private readonly PlasticDbContext _context;
        private readonly IDoctorRepository _doctorRepository;
        private readonly IClinicRepository _clinicRepository;
        private readonly IPhotoService _photoService;

        public DoctorController(IPhotoService photoService, PlasticDbContext context, IClinicRepository clinicRepository, IDoctorRepository doctorRepository)
        {
            _photoService = photoService;
            _doctorRepository = doctorRepository;
            _clinicRepository = clinicRepository;
            _context = context;
        }
        public ActionResult Index()
        {
            return View();
        }
        public PartialViewResult Doctor(string clinicId, string franchiseId)
        {
            ////var _idClinic = Convert.ToInt32(HttpContext.Session.GetInt32("_ClinicId"));
            ////var _idFranchise = Convert.ToInt32(HttpContext.Session.GetInt32("_FranchiseId"));
            var _idClinic = clinicId;
            var _idFranchise = franchiseId;

            try
            {
                var PartialDoctorVm = new _PartialDoctorViewModel()
                {
                    ClinicId = clinicId,
                    FranchiseId = franchiseId,
                };

                if (_idFranchise == null)
                {
                    PartialDoctorVm.Doctors = _doctorRepository.GetAllDoctorByClinicId(_idClinic);
                }
                else if (_idClinic == null)
                {
                    PartialDoctorVm.Doctors = _doctorRepository.GetAllDoctorByFranchiseId(_idFranchise);
                }
                //var doctor = _clinicRepository.GetDoctorByClinicId(_id);
                return PartialView("~/Views/Doctor/_PartialDoctor.cshtml", PartialDoctorVm);  //doctor  _PartialView.cshtml Views/Operation/Index.cshtml  
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return PartialView("~/Views/Doctor/_PartialDoctor.cshtml");  //doctor döndürmeyince hata verir!!!!!!!!!!!!!!!!!!1
            }
        }
        public ActionResult Details(string id)
        {
            return View();
        }

        public ActionResult Create(string clinicId, string franchiseId)
        {
            var doctorVM = new DoctorViewModel()
            {
                Doctor = new Doctor(),
                ClinicId = clinicId,
                FranchiseId = franchiseId
            };
            return PartialView("~/Views/Doctor/_PartialCreateDoctor.cshtml", doctorVM); //, doctorVM
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DoctorViewModel doctorVM)
        {
            try
            {
                //clinicMVM ya da franchiseMVM içinde doctor,clinic,franchise tabloları da olduğu için sadece doctor için model state kontrolü yapmalıyız !!!!!!
                var doctorModelState = ModelState
                            .Where(ms => ms.Key.StartsWith("Doctor."))
                            .ToDictionary(ms => ms.Key, ms => ms.Value); // bir koleksiyonun her bir öğesini bir anahtar-değer çiftine dönüştürmektir,doctorModelState her bir kişinin Key özelliğini anahtar, Value özelliğini ise değer olarak kullanır.


                //foreign key le bağlı tabloların formdan idlerinin null gelmesi sorunu çözümü  
                {
                    //formdan FranchiseId ve ClinicId valid ama null geldiği için. Hangi kontrollerdan form geldiğini bu sayede anlarız (0 olanın contr. dan gelmiyo yani)
                    //_idClinic = Convert.ToInt32(HttpContext.Session.GetInt32("_ClinicId"));
                    //_idFranchise = Convert.ToInt32(HttpContext.Session.GetInt32("_FranchiseId"));

                    //doctorModelState["Doctor.Clinic"].ValidationState = ModelValidationState.Valid; //Valid yapıyor ama yine de giremedim ondan direk remove diyorum
                    ModelState.Remove("Doctor.Clinic");
                    ModelState.Remove("Doctor.Franchise");
                }

                if (doctorModelState.Values.All(v => v.Errors.Count == 0))
                {
                    var doctor = new Doctor();
                    doctor = doctorVM.Doctor;

                    //null a falan eşitli kalmamsı lazım çünkü tabloları oluştururuken ya franchise ya clinic ıd null olmalı diye ayarladım
                    if (doctorVM.ClinicId == null)
                    {
                        doctor.ClinicId = null;
                        doctor.FranchiseId = doctorVM.FranchiseId;
                    }
                    else if (doctorVM.FranchiseId == null)
                    {
                        doctor.FranchiseId = null;
                        doctor.ClinicId = doctorVM.ClinicId;
                    }

                    if (doctorVM.Image != null)  // clinicMVM.Image1
                    {
                        var result = await _photoService.AddPhotoAsync(doctorVM.Image); // clinicMVM.Image1
                        doctor.ImageUrl = result.Url.ToString();
                    }

                    //  BaseEntity 
                    {
                        doctor.Status = true;
                        doctor.Deleted = false;
                        doctor.CreatedDate = DateTime.Now;
                        doctor.UpdatedDate = DateTime.Now;
                        doctor.CreatedBy = 0;
                        doctor.UpdatedBy = 0;
                    }

                    _context.Doctors.Add(doctor);
                    _context.SaveChanges();
                }

                //clinic/franchise den gelen veriler yanlışsa buraya yönlendirme yapılcak
                if (doctorVM.ClinicId == null) //from verileri franchise dan gelmiştir
                {
                    return RedirectToAction("Details", "Franchise", new { id = doctorVM.FranchiseId });
                }
                else //from verileri clinic den gelmiştir
                {
                    return RedirectToAction("Details", "Clinic", new { id = doctorVM.ClinicId });
                }
            }
            catch  //burası çalışmıyor klinikler sayfasına atıyor direk düzelt
            {
                if (doctorVM.FranchiseId == null)
                {
                    return RedirectToAction("Details", "Clinic", new { id = doctorVM.ClinicId });
                }
                else
                {
                    return RedirectToAction("Details", "Franchise", new { id = doctorVM.FranchiseId });
                }
            }
        }

        public async Task<IActionResult> Edit(string clinicId, string franchiseId, int id)
        {
            var doctor = await _doctorRepository.GetDoctorByIdAsync(id);
            if (doctor == null)
            {
                return RedirectToAction("~/Views/Clinic/_PartialDoctor.cshtml"); //????????????????????????*
            }

            var selectedDoctor = _context.Doctors.FirstOrDefault(c => c.Id == id);
            var doctorVM = new _PartialDoctorViewModel();
            {
                doctorVM.EditDoctor = selectedDoctor;
                doctorVM.ClinicId = clinicId;
                doctorVM.FranchiseId = franchiseId;
            };

            return PartialView("~/Views/Doctor/_PartialEditDoctor.cshtml", doctorVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, _PartialDoctorViewModel doctorVM)
        {
            var _idClinic = "";
            var _idFranchise = "";
            try
            {
                _idClinic = doctorVM.ClinicId;
                _idFranchise = doctorVM.FranchiseId;

                var doctorModelState = ModelState
                                            .Where(ms => ms.Key.StartsWith("EditDoctor."))
                                            .ToDictionary(ms => ms.Key, ms => ms.Value);
                ModelState.Remove("EditDoctor.Clinic");
                ModelState.Remove("EditDoctor.Franchise");
                if (!doctorModelState.Values.All(v => v.Errors.Count == 0))
                {
                    ModelState.AddModelError("", "Bir şeyler ters gitti. Tekrar deneyiniz.");
                    return PartialView("~/Views/Doctor/_PartialEditDoctor.cshtml", doctorVM);  //_PartialEditDoctor sayfasını açıyor direk clinic/franchise sayfasında açılmıyo??????????????      
                }
                Doctor? doctor = await _doctorRepository.GetDoctorByIdAsync(id);

                if (doctorModelState.Values.All(v => v.Errors.Count == 0))
                {
                    if (doctor != null)
                    {
                        try
                        {
                            if (doctorVM.Image != null)
                            {
                                await _photoService.DeletePhotoAsync(Convert.ToString(doctorVM.Image)); // boş değilse önceki fotoğrafı silicek
                            }
                        }
                        catch (Exception ex)
                        {
                            ModelState.AddModelError("", "Fotoğaf silinemedi.");
                            if (_idFranchise == null) //form verileri clinicten geldiyse
                            {
                                return RedirectToAction("Details", "Clinic", new { id = _idClinic });
                            }
                            else if (_idClinic == null) //form verileri franchisedan geldiyse
                            {
                                return RedirectToAction("Details", "Franchise", new { id = _idFranchise });
                            }

                            //return PartialView("~/Views/Doctor/_PartialEditDoctor.cshtml", doctorVM);  //sadece partial view ı açıyor böyle
                        }

                        {
                            doctor = doctorVM.EditDoctor;
                            doctor.ClinicId = _idClinic;
                            doctor.FranchiseId = _idFranchise;
                        }

                        // RESİM YÜKLÜYOR
                        if (doctorVM.Image != null)
                        {
                            var photoResult = await _photoService.AddPhotoAsync(doctorVM.Image);
                            doctor.ImageUrl = photoResult.Url.ToString();
                        }

                        //  BaseEntity 
                        {
                            doctor.Status = true;
                            doctor.Deleted = false;
                            doctor.CreatedDate = DateTime.Now;
                            doctor.UpdatedDate = DateTime.Now;
                            doctor.CreatedBy = 0;
                            doctor.UpdatedBy = 0;
                        }

                        _context.ChangeTracker.Clear(); // birden fazla aynı anahtar değeri (ID) ile ilişkilendirilmiş Doctor nesnesinin aynı DbContext içinde birden fazla kez izlenmeye çalışıldığında ortaya çıkar
                        _context.Doctors.Update(doctor);
                        _context.SaveChanges();

                        //return PartialView("~/Views/Doctor/_PartialEditDoctor.cshtml", doctorVM);
                        if (_idFranchise == null) //form verileri clinicten geldiyse
                        {
                            return RedirectToAction("Details", "Clinic", new { id = _idClinic });
                        }
                        else if (_idClinic == null) //form verileri franchisedan geldiyse
                        {
                            return RedirectToAction("Details", "Franchise", new { id = _idFranchise });
                        }

                    }
                    else
                    {
                        return PartialView("~/Views/Doctor/_PartialEditDoctor.cshtml", doctorVM);
                    }
                }

                if (_idFranchise == null) //form verileri clinicten geldiyse
                {
                    return RedirectToAction("Details", "Clinic", new { id = _idClinic });
                }
                else //if (_idClinic == 0) //form verileri franchisedan geldiyse
                {
                    return RedirectToAction("Details", "Franchise", new { id = _idFranchise });
                }

            }
            catch
            {
                if (_idFranchise == null) //form verileri clinicten geldiyse
                {
                    return RedirectToAction("Details", "Clinic", new { id = _idClinic });
                }
                else //if (_idClinic == 0) //form verileri franchisedan geldiyse
                {
                    return RedirectToAction("Details", "Franchise", new { id = _idFranchise });
                }
            }
        }

        public ActionResult Delete(int id, string clinicId, string franchiseId)
        {
            var DoctorVM = new DoctorViewModel()
            {
                Doctor = new Doctor(),
                ClinicId = clinicId,
                FranchiseId = franchiseId
            };

            //OperationDoctor? operationDoctor = await _operationdoctorRepository.GetOperationDoctorByIdAsync(id);
            var Doctor = _context.Doctors.FirstOrDefault(o => o.Id == id);
            DoctorVM.Doctor = Doctor;

            return PartialView("~/Views/Doctor/_PartialDeleteDoctor.cshtml", DoctorVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection, DoctorViewModel doctorVM)
        {
            var _idClinic = "";
            var _idFranchise = "";
            try
            {
                _idClinic = doctorVM.ClinicId;
                _idFranchise = doctorVM.FranchiseId;

                //_idClinic = Convert.ToInt32(HttpContext.Session.GetInt32("_ClinicId"));
                //_idFranchise = Convert.ToInt32(HttpContext.Session.GetInt32("_FranchiseId"));

                var Doctor = _context.Doctors.FirstOrDefault(o => o.Id == id);
                if (Doctor != null)
                {
                    Doctor.Status = false;
                    Doctor.Deleted = true;
                    _context.Doctors.Update(Doctor);
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
