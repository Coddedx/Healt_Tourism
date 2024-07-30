using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Plastic.Models;
using Plastic.ViewModels;
using System.Numerics;

namespace Plastic.Controllers
{
    public class DoctorController : Controller
    {
        PlasticDbContext db = new PlasticDbContext();
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
                var doctorVM = new DoctorViewModel();

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
               
                doctorVM.ClinicId =doctorMVM.Clinic.Id;
                //formdan gelen veriler yanlış geld için gelen verilerle tekrardan işlem yapab. için clinic e bunları da gönderiyorum 
                {
                    //sadece null olmayan verileri tekrar gönderelim ki formu yeniden kaldıkları yerden döndürebilsinler
                    //foreach (var item in doctorMVM)
                    //{

                    //}
                doctorVM.Doctor.FirstName = doctorMVM.Doctor.FirstName;
                doctorVM.Doctor.LastName = doctorMVM.Doctor.LastName;
                doctorVM.Doctor.Gender = doctorMVM.Doctor.Gender;
                doctorVM.Doctor.Phone = doctorMVM.Doctor.Phone;
                doctorVM.Doctor.Title = doctorMVM.Doctor.Title;
                doctorVM.Doctor.Country = doctorMVM.Doctor.Country;
                doctorVM.Doctor.CertificationNumber = doctorMVM.Doctor.CertificationNumber;
                doctorVM.Doctor.Email = doctorMVM.Doctor.Email;
                doctorVM.Doctor.Password = doctorMVM.Doctor.Password;
                doctorVM.Doctor.FranchiseId = doctorMVM.Doctor.FranchiseId;
                }
                //clinic den gelen veriler yanlışsa buraya yönlendirme yapılcak
                return RedirectToAction("Details", "Clinic", doctorVM);

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
