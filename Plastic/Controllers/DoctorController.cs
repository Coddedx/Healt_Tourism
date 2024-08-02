using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        PlasticDbContext db = new PlasticDbContext();
        private readonly IDoctorRepository _doctorRepository;
        private readonly IPhotoService _photoService;

        public DoctorController(IPhotoService photoService) 
        {
            _photoService = photoService;

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
        public async Task<IActionResult> Create( ClinicModalViewModel clinicMVM) 
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

                ModelState.Remove("Doctor.Operations"); //bunu manuel olarak model state den çıkarıyorum çünkü form için kotnrol etmeme gerek yok
                if (doctorModelState.Values.All(v => v.Errors.Count == 0))
                {
                    var result = await _photoService.AddPhotoAsync(clinicMVM.Image);

                    //franchiseıd ??????????????????????
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
                        FranchiseId = 2, // clinicMVM.Doctor.FranchiseId, //düzelt!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                        ImageUrl = result.Url.ToString(),

                        Status = true,
                        CreatedDate = DateTime.Now,
                        CreatedBy = 0,
                        Deleted = false,
                    };
                    db.Doctors.Add(doctor);
                    db.SaveChanges();

                    doctorVM.ClinicId = clinicMVM.Clinic.Id; //tekrardan clinic id yi gönderelim ki aynı sayfadaki clinicten işlem yapmaya devam edelim.                   
                                                             //clinic den gelen veriler doğruysa buraya yönlendirme yapılcak
                    //TempData["ClinicId"] = doctorVM.ClinicId;
                    int id = doctorVM.ClinicId;
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
