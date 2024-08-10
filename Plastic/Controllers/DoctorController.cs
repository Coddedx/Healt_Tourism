using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Plastic.IRepository;
using Plastic.Models;
using Plastic.Services;
using Plastic.ViewModels;
using System;
using System.Numerics;
using System.Reflection;
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
            _clinicRepository = clinicRepository;
            _doctorRepository = doctorRepository;
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
        public async Task<IActionResult> Create( ClinicModalViewModel clinicMVM) //FRANCHİSEID DÜZELT!!!!!!!!!!!1
        {
            try 
            {
                var doctorVM = new DoctorViewModel // bul olmadığında doctor dan obje üretemiyorum???
                {
                    Doctor = new Doctor()
                };

                //clinicMVM içinde doctor,clinic,franchise tabloları da olduğu için sadece doctor için model state kontrolü yapmalıyız !!!!!!
                var doctorModelState = ModelState
                            .Where(ms => ms.Key.StartsWith("Doctor."))
                            .ToDictionary(ms => ms.Key, ms => ms.Value);

                // TABLOLAR GÜNCELLENDİKTEN SONRA KALMALI MI????????????????!!!!!!!!!!!!!!!
                ModelState.Remove("Doctor.Operations"); //bunu manuel olarak model state den çıkarıyorum çünkü form için kotnrol etmeme gerek yok
               
                var _idClinic = 0;
                var _idFranchise = 0;
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
                    //    doctor.FranchiseId = _id;
                    //}
                    //else if (clinicMVM.Doctor.ClinicId.HasValue)
                    //{
                    //    ModelState.Remove("Doctor.Franchise");
                    //}
                }

                if (doctorModelState.Values.All(v => v.Errors.Count == 0))
                {
                    var result = await _photoService.AddPhotoAsync(clinicMVM.Image);

                    var doctor = new Doctor()
                    {
                        FirstName = clinicMVM.Doctor.FirstName,
                        LastName = clinicMVM.Doctor.LastName,
                        Gender = clinicMVM.Doctor.Gender,
                        Phone = clinicMVM.Doctor.Phone,
                        Title = clinicMVM.Doctor.Title,
                        Country = clinicMVM.Doctor.Country,
                        CertificationNumber = clinicMVM.Doctor.CertificationNumber,
                        Email = clinicMVM.Doctor.Email,
                        Password = clinicMVM.Doctor.Password,
                        ImageUrl = result.Url.ToString(),

                        Status = true,
                        CreatedDate = DateTime.Now,
                        CreatedBy = 0,
                        Deleted = false,
                    };
                    if (_idFranchise == 0)
                    {
                        doctor.FranchiseId = null;  //0 a falan eşitleyemem çünkü tabloları oluştururuken ya franchise ya clinic ıd null olmalı diye ayarladım
                        doctor.ClinicId = _idClinic; // clinicMVM.Doctor.ClinicId;//, // clinicMVM.Doctor.FranchiseId,
                    }
                    else if (_idClinic == 0)
                    {
                        doctor.ClinicId = null;
                        doctor.FranchiseId = _idFranchise; // clinicMVM.Doctor.FranchiseId;//, // clinicMVM.Doctor.FranchiseId,
                    }

                    _context.Doctors.Add(doctor);
                    _context.SaveChanges();

                    doctorVM.ClinicId = _idClinic;//clinicMVM.Clinic.Id; //tekrardan clinic id yi gönderelim ki aynı sayfadaki clinicten işlem yapmaya devam edelim.                   
                                                  //clinic den gelen veriler doğruysa buraya yönlendirme yapılcak
                    doctorVM.FranchiseId = _idFranchise;
                    
                    //TempData["ClinicId"] = doctorVM.ClinicId;
                    //int id = doctorVM.ClinicId;

                    return RedirectToAction("Details", "Clinic", doctorVM);  //, clinicId
                }

                //Doctor özelliğinin null gidiyor clinic controllera. modelin RedirectToAction yöntemiyle gönderilirken Doctor nesnesinin seri hale getirilip geri yüklenmemesi
                //RedirectToAction yöntemi, genellikle URL parametreleri aracılığıyla veri gönderir ve karmaşık nesneleri(örneğin Doctor gibi) düzgün bir şekilde seri hale getiremez.
                //Bu sorunu çözmek için TempData veya Session kullanarak karmaşık veri nesnelerini geçici olarak depolayabiliriz.
                // DoctorViewModel'i TempData'ya serialize edip sakla                   
                TempData["ClinicModalViewModel"] = JsonSerializer.Serialize(clinicMVM); 

                
                //clinic den gelen veriler yanlışsa buraya yönlendirme yapılcak
                return RedirectToAction("Details", "Clinic");  
            }
            catch
            {
                return View(clinicMVM);
            }
        }

        public ActionResult Edit(int id)
        {
            return View();
        }

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
