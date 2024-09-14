using Microsoft.AspNetCore.Http;
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
        public PartialViewResult Doctor()
        {
            var _idClinic = Convert.ToInt32(HttpContext.Session.GetInt32("_ClinicId"));
            var _idFranchise = Convert.ToInt32(HttpContext.Session.GetInt32("_FranchiseId"));

            try
            {
                var PartialDoctorVm = new _PartialDoctorViewModel();

                if (_idFranchise == 0)
                {
                    PartialDoctorVm.Doctors = _doctorRepository.GetAllDoctorByClinicId(_idClinic);
                }
                else if (_idClinic == 0)
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
        public ActionResult Details(int id)
        {
            return View();
        }

        public ActionResult Create()
        {
            var doctorVM = new DoctorViewModel()
            {
                Doctor = new Doctor()
            };
            return PartialView("~/Views/Doctor/_PartialCreateDoctor.cshtml", doctorVM); //, doctorVM
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DoctorViewModel doctorVM) //, ClinicModalViewModel clinicMVM, FranchiseModalViewModel franchiseMVM
        {
            var _idClinic = 0;
            var _idFranchise = 0;
            try
            {
                //var doctorVM = new DoctorViewModel // bu olmadığında doctor dan obje üretemiyorum???
                //{
                //    Doctor = new Doctor()
                //};

                //clinicMVM ya da franchiseMVM içinde doctor,clinic,franchise tabloları da olduğu için sadece doctor için model state kontrolü yapmalıyız !!!!!!
                var doctorModelState = ModelState
                            .Where(ms => ms.Key.StartsWith("Doctor."))
                            .ToDictionary(ms => ms.Key, ms => ms.Value); // bir koleksiyonun her bir öğesini bir anahtar-değer çiftine dönüştürmektir,doctorModelState her bir kişinin Key özelliğini anahtar, Value özelliğini ise değer olarak kullanır.


                //foreign key le bağlı tabloların formdan idlerinin null gelmesi sorunu çözümü  
                {
                    //formdan FranchiseId ve ClinicId valid ama null geldiği için. Hangi kontrollerdan form geldiğini bu sayede anlarız (0 olanın contr. dan gelmiyo yani)
                    _idClinic = Convert.ToInt32(HttpContext.Session.GetInt32("_ClinicId"));
                    _idFranchise = Convert.ToInt32(HttpContext.Session.GetInt32("_FranchiseId"));

                    //doctorModelState["Doctor.Clinic"].ValidationState = ModelValidationState.Valid; //Valid yapıyor ama yine de giremedim ondan direk remove diyorum
                    ModelState.Remove("Doctor.Clinic");
                    ModelState.Remove("Doctor.Franchise");

                    //if (clinicMVM.Doctor.FranchiseId.HasValue)
                    //{
                    //    ModelState.Remove("Doctor.Clinic");
                    //}
                    //else if (clinicMVM.Doctor.ClinicId.HasValue)
                    //{
                    //    ModelState.Remove("Doctor.Franchise");
                    //}
                }

                if (doctorModelState.Values.All(v => v.Errors.Count == 0))
                {
                    var doctor = new Doctor();

                    //verilerin clinic/franchise dan gelmesine göre gerekli işlemler 
                    if (_idFranchise == 0) //form verileri clinicten geldiyse
                    {
                        doctor = doctorVM.Doctor; //clinicMVM
                        if (doctorVM.Image != null)  // clinicMVM.Image1
                        {
                            var result = await _photoService.AddPhotoAsync(doctorVM.Image); // clinicMVM.Image1
                        }

                        //0 a falan eşitli kalmamsı lazım çünkü tabloları oluştururuken ya franchise ya clinic ıd null olmalı diye ayarladım
                        if (_idClinic == 0) { doctor.ClinicId = null; } else if (_idFranchise == 0) { doctor.FranchiseId = null; }

                        doctorVM.ClinicId = _idClinic;//clinicMVM.Clinic.Id; //tekrardan clinic id yi gönderelim ki aynı sayfadaki clinicten işlem yapmaya devam edelim.clinic den gelen veriler doğruysa buraya yönlendirme yapılcak
                    }
                    else if (_idClinic == 0) //form verileri franchisedan geldiyse
                    {
                        doctor = doctorVM.Doctor;  //franchiseMVM.Doctor
                        if (doctorVM.Image != null)  //franchiseMVM.Image
                        {
                            var result = await _photoService.AddPhotoAsync(doctorVM.Image);// franchiseMVM.Image
                        }

                        doctor.ClinicId = null;
                        doctor.FranchiseId = _idFranchise;

                        doctorVM.FranchiseId = _idFranchise;
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

                    if (_idFranchise == 0) //form verileri clinicten geldiyse
                    {
                        return RedirectToAction("Details", "Clinic", new { id = _idClinic });
                    }
                    else if (_idClinic == 0) //form verileri franchisedan geldiyse
                    {
                        return RedirectToAction("Details", "Franchise", new { id = _idFranchise });
                    }
                }

                //Doctor özelliğinin null gidiyor clinic controllera. modelin RedirectToAction yöntemiyle gönderilirken Doctor nesnesinin seri hale getirilip geri yüklenmemesi
                //RedirectToAction yöntemi, genellikle URL parametreleri aracılığıyla veri gönderir ve karmaşık nesneleri(örneğin Doctor gibi) düzgün bir şekilde seri hale getiremez.
                //Bu sorunu çözmek için TempData veya Session kullanarak karmaşık veri nesnelerini geçici olarak depolayabiliriz.
                // DoctorViewModel'i TempData'ya serialize edip sakla                   
                //TempData["ClinicModalViewModel"] = JsonSerializer.Serialize(clinicMVM);
                //TempData["FranchiseModalViewModel"] = JsonSerializer.Serialize(franchiseMVM);


                //clinic/franchise den gelen veriler yanlışsa buraya yönlendirme yapılcak
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
                }
                else
                {
                    return RedirectToAction("Details", "Franchise", new { id = _idFranchise });
                }
            }
        }

        public async Task<IActionResult> Edit(int id)
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
            };

            return PartialView("~/Views/Doctor/_PartialEditDoctor.cshtml", doctorVM);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, _PartialDoctorViewModel doctorVM)
        {
            var _idClinic = 0;
            var _idFranchise = 0;
            try
            {
                _idClinic = Convert.ToInt32(HttpContext.Session.GetInt32("_ClinicId"));
                _idFranchise = Convert.ToInt32(HttpContext.Session.GetInt32("_FranchiseId"));

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
                            await _photoService.DeletePhotoAsync(Convert.ToString(doctorVM.Image)); // boş değilse önceki fotoğrafı silicek
                        }
                        catch (Exception ex)
                        {
                            ModelState.AddModelError("", "Fotoğaf silinemedi.");
                            return PartialView("~/Views/Doctor/_PartialEditDoctor.cshtml", doctorVM);
                        }
                        // RESİM YÜKLÜYOR
                        var photoResult = await _photoService.AddPhotoAsync(doctorVM.Image);

                        {
                            doctor = doctorVM.EditDoctor;
                            doctor.ClinicId = _idClinic;
                            doctor.FranchiseId = _idFranchise;
                            doctor.ImageUrl = photoResult.Url.ToString();
                        }

                        //var doctorEdit = new Doctor
                        //{
                        //    ClinicId = _idClinic,
                        //    FranchiseId = _idFranchise,
                        //    ImageUrl = photoResult.Url.ToString(), 
                        //};

                        if (_idClinic == 0) { doctor.ClinicId = null; } else if (_idFranchise == 0) { doctor.FranchiseId = null; }

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
                        if (_idFranchise == 0) //form verileri clinicten geldiyse
                        {
                            return RedirectToAction("Details", "Clinic", new { id = _idClinic });
                        }
                        else if (_idClinic == 0) //form verileri franchisedan geldiyse
                        {
                            return RedirectToAction("Details", "Franchise", new { id = _idFranchise });
                        }

                    }
                    else
                    {
                        return PartialView("~/Views/Doctor/_PartialEditDoctor.cshtml", doctorVM);
                    }
                }

                if (_idFranchise == 0) //form verileri clinicten geldiyse
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
                if (_idFranchise == 0) //form verileri clinicten geldiyse
                {
                    return RedirectToAction("Details", "Clinic", new { id = _idClinic });
                }
                else //if (_idClinic == 0) //form verileri franchisedan geldiyse
                {
                    return RedirectToAction("Details", "Franchise", new { id = _idFranchise });
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
