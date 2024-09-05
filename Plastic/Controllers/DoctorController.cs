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

        public ActionResult Details(int id)
        {
            return View();
        }

        public ActionResult Create()
        {
            //var clinicMVM = new ClinicModalViewModel
            //{
            //    Doctor = new Doctor(),
            //    Clinic = new Clinic()
            //};
            return View(); //clinicMVM
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create( ClinicModalViewModel clinicMVM,FranchiseModalViewModel franchiseMVM) 
        {
            var _idClinic = 0;
            var _idFranchise = 0;
            try
            {
                var doctorVM = new DoctorViewModel // bu olmadığında doctor dan obje üretemiyorum???
                {
                    Doctor = new Doctor()
                };

                //clinicMVM ya da franchiseMVM içinde doctor,clinic,franchise tabloları da olduğu için sadece doctor için model state kontrolü yapmalıyız !!!!!!
                var doctorModelState = ModelState
                            .Where(ms => ms.Key.StartsWith("Doctor."))
                            .ToDictionary(ms => ms.Key, ms => ms.Value); // bir koleksiyonun her bir öğesini bir anahtar-değer çiftine dönüştürmektir,doctorModelState her bir kişinin Key özelliğini anahtar, Value özelliğini ise değer olarak kullanır.

                // TABLOLAR GÜNCELLENDİKTEN SONRA KALMALI MI????????????????!!!!!!!!!!!!!!!
                ModelState.Remove("Doctor.Operations"); //bunu manuel olarak model state den çıkarıyorum çünkü form için kotnrol etmeme gerek yok
               
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
                    var doctor = new Doctor()
                    {
                        //FirstName = clinicMVM.Doctor.FirstName,
                        //LastName = clinicMVM.Doctor.LastName,
                        //Gender = clinicMVM.Doctor.Gender,
                        //Phone = clinicMVM.Doctor.Phone,
                        //Title = clinicMVM.Doctor.Title,
                        //Country = clinicMVM.Doctor.Country,
                        //CertificationNumber = clinicMVM.Doctor.CertificationNumber,
                        //Email = clinicMVM.Doctor.Email,
                        //Password = clinicMVM.Doctor.Password,
                        //ImageUrl = result.Url.ToString(),

                        //Status = true,
                        //CreatedDate = DateTime.Now,
                        //CreatedBy = 0,
                        //Deleted = false,
                    };
                    //verilerin clinic/franchise dan gelmesine göre gerekli işlemler 
                    if (_idFranchise == 0) //form verileri clinicten geldiyse
                    {
                        doctor = clinicMVM.Doctor;
                        if (clinicMVM.Image1 != null)
                        {
                            var result = await _photoService.AddPhotoAsync(clinicMVM.Image1);
                        }

                        doctor.FranchiseId = null;  //0 a falan eşitli kalmamsı lazım çünkü tabloları oluştururuken ya franchise ya clinic ıd null olmalı diye ayarladım
                        doctor.ClinicId = _idClinic;

                        doctorVM.ClinicId = _idClinic;//clinicMVM.Clinic.Id; //tekrardan clinic id yi gönderelim ki aynı sayfadaki clinicten işlem yapmaya devam edelim.clinic den gelen veriler doğruysa buraya yönlendirme yapılcak
                    }
                    else if (_idClinic == 0) //form verileri franchisedan geldiyse
                    {
                        doctor = franchiseMVM.Doctor;
                        if (franchiseMVM.Image != null)
                        {
                            var result = await _photoService.AddPhotoAsync(franchiseMVM.Image);
                        }

                        doctor.ClinicId = null;
                        doctor.FranchiseId = _idFranchise;

                        doctorVM.FranchiseId = _idFranchise;
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
                TempData["ClinicModalViewModel"] = JsonSerializer.Serialize(clinicMVM); 
                TempData["FranchiseModalViewModel"] = JsonSerializer.Serialize(franchiseMVM);


                //clinic/franchise den gelen veriler yanlışsa buraya yönlendirme yapılcak
                if (_idClinic == 0) //from verileri franchise dan gelmiştir
                {
                    return RedirectToAction("Details", "Franchise");
                }
                else //from verileri clinic den gelmiştir
                {
                    return RedirectToAction("Details", "Clinic");
                }
            }
            catch  //burası çalışmıyor klinikler sayfasına atıyor direk düzelt
            {
                if (_idFranchise == 0)
                {
                    return View(clinicMVM);

                }
                else
                {
                    return View(franchiseMVM);
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

            var selectedDoctor = _context.Doctors.FirstOrDefault(c=>c.Id==id); 
            var doctorVM = new _PartialDoctorViewModel();
            {
                doctorVM.EditDoctor = selectedDoctor;
            };            
           
            return PartialView("~/Views/Doctor/_PartialEditDoctor.cshtml", doctorVM);  
            
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, DoctorViewModel doctorVM)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Doctor? doctor = await _doctorRepository.GetDoctorByIdAsync(id);
                    if (doctor == null)
                    {
                        return PartialView("~/Views/Clinic/_PartialDoctor.cshtml", doctor);
                    }                   
                }
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
