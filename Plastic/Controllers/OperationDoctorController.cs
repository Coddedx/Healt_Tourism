using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Plastic.IRepository;
using Plastic.Models;
using Plastic.ViewModels;
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

        public ActionResult Details(int id)
        {
            return View();
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ClinicModalViewModel clinicMVM, FranchiseModalViewModel franchiseMVM, int SelectedDoctorId, int SelectedOperationId)
        {
            var _idClinic = 0;
            var _idFranchise = 0;
            try
            {
                var operationdoctorVM = new OperationDoctorViewModel
                {
                    OperationDoctor = new OperationDoctor()
                };

                //var operationdoctorModelState = ModelState
                //                    .Where(ms => ms.Key.StartsWith("OperationDoctor."))
                //                    .ToDictionary(ms => ms.Key, ms =>  ms.Value);

                _idClinic = Convert.ToInt32(HttpContext.Session.GetInt32("_ClinicId"));
                _idFranchise = Convert.ToInt32(HttpContext.Session.GetInt32("_FranchiseId"));
                
                var operationdoctor = new OperationDoctor();
                operationdoctor.DoctorId = SelectedDoctorId;
                operationdoctor.OperationId = SelectedOperationId;

                // ChatGpt de MODEL SEÇİMİ YÖNETİMİ İLE BU SORUNU ÇÖZ !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                // DoctorId ve OperationId hiç seçilmese de her türlü model stateyi geçiyor DÜZELT!!!!!!!!!!!!!!!!!!
                if (ModelState.IsValid) //operationdoctorModelState.Values.All(v => v.Errors.Count == 0)
                {
                    //var operationdoctor = new OperationDoctor();

                    if (_idFranchise == 0) //form verileri clinicten geldiyse
                    {
                        operationdoctor = clinicMVM.OperationDoctor;
                        if (clinicMVM.Image1 != null)
                        {
                            var result = await _photoService.AddPhotoAsync(clinicMVM.Image1);
                        }
                        operationdoctor.DoctorId = SelectedDoctorId;  //yukarıda old operationdoctor = clinicMVM.OperationDoctor dolayı doctor ve operasyon ıd ler 0 lanıyor
                        operationdoctor.OperationId = SelectedOperationId;

                        operationdoctorVM.ClinicId = _idClinic;
                    }
                    else if (_idClinic == 0) //form verileri franchisedan geldiyse
                    {
                        operationdoctor = franchiseMVM.OperationDoctor;
                        if (franchiseMVM.Image != null)
                        {
                            var result = await _photoService.AddPhotoAsync(franchiseMVM.Image);
                        }
                        operationdoctor.DoctorId = SelectedDoctorId;
                        operationdoctor.OperationId = SelectedOperationId;

                        operationdoctorVM.FranchiseId = _idFranchise;
                    }

                    _context.Add(operationdoctor);
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

                TempData["ClinicModalViewModel"] = JsonSerializer.Serialize(clinicMVM);
                TempData["FranchiseModalViewModel"] = JsonSerializer.Serialize(franchiseMVM);

                // DÜZGÜN ÇALIŞMIYOR !!!!!!!!!!!!!!!!!!!!!!! SİYAH EKRANDA İNTERNAL ERROR 404 VERİYORRR!!!!!!!!!!!!!!!!!!
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
                    return RedirectToAction("Details", "Clinic", _idClinic);
                   // return View(clinicMVM);

                }
                else
                {
                    return RedirectToAction("Details", "Franchise", _idFranchise);

                    // return View(franchiseMVM);
                }
            }
        }

        // GET: OperationDoctorController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: OperationDoctorController/Edit/5
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

        // GET: OperationDoctorController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: OperationDoctorController/Delete/5
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
