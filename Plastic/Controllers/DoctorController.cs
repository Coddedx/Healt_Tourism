using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Plastic.IRepository;
using Plastic.Models;
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

        // GET: DoctorController
        public ActionResult Index()
        {
            return View();
        }

        // GET: DoctorController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: DoctorController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: DoctorController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create( ClinicModalViewModel doctorMVM) //Doctor model,
        {
            try 
            {
                var doctorVM = new DoctorViewModel // bul olmadığında doctor dan obje üretemiyorum???
                {
                    Doctor = new Doctor()
                };

                if (ModelState.IsValid)
                {
                    //franchiseıd ??????????????????????
                    var doctor = new Doctor()
                    {
                        FirstName = doctorMVM.Doctor.FirstName,
                        LastName = doctorMVM.Doctor.LastName,
                        Gender = doctorMVM.Doctor.Gender,
                        Phone = doctorMVM.Doctor.Phone,
                        Title = doctorMVM.Doctor.Title,
                        Country = doctorMVM.Doctor.Country,
                        CertificationNumber = doctorMVM.Doctor.CertificationNumber,
                        Email = doctorMVM.Doctor.Email,  
                        Password = doctorMVM.Doctor.Password,
                        FranchiseId = doctorMVM.Doctor.FranchiseId,
                        
                        Status = true,
                        CreatedDate = DateTime.Now,
                        CreatedBy = 0,
                        Deleted = false,
                    };                  
                    db.Doctors.Add(doctor);
                    db.SaveChanges();

                    doctorVM.ClinicId = doctorMVM.Clinic.Id; //tekrardan clinic id yi gönderelim ki aynı sayfadaki clinicten işlem yapmaya devam edelim.                   
                    //clinic den gelen veriler doğruysa buraya yönlendirme yapılcak
                    return RedirectToAction("Details", "Clinic", doctorVM);
                }
               
                //Doctor özelliğinin null gidiyor clinic controllera. modelin RedirectToAction yöntemiyle gönderilirken Doctor nesnesinin seri hale getirilip geri yüklenmemesi
                //RedirectToAction yöntemi, genellikle URL parametreleri aracılığıyla veri gönderir ve karmaşık nesneleri(örneğin Doctor gibi) düzgün bir şekilde seri hale getiremez.
                //Bu sorunu çözmek için TempData veya Session kullanarak karmaşık veri nesnelerini geçici olarak depolayabiliriz.
                // DoctorViewModel'i TempData'ya serialize edip sakla                   
                TempData["ClinicModalViewModel"] = JsonSerializer.Serialize(doctorMVM); 

                
                //clinic den gelen veriler yanlışsa buraya yönlendirme yapılcak
                return RedirectToAction("Details", "Clinic");  //, clinicId

            }
            catch
            {
                return View(doctorMVM);
            }
        }

        // GET: DoctorController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: DoctorController/Edit/5
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

        // GET: DoctorController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: DoctorController/Delete/5
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
