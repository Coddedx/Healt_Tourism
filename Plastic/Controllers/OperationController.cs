using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Plastic.IRepository;

namespace Plastic.Controllers
{
    public class OperationController : Controller
    {

        private readonly IFranchiseRepository _franchiseRepository;

        public OperationController(IFranchiseRepository franchiseRepository)
        {
            _franchiseRepository = franchiseRepository;
        }


        // GET: OperationController
        public async Task<IActionResult> Index(int id)
        {
            //var clinic = await _franchiseRepository.GetByIdClinicAsync(id);

            //if (clinic == null) { return RedirectToAction("Index", "Franchise"); }

            return View(); //clinic
        }

        // GET: OperationController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: OperationController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: OperationController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
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

        // GET: OperationController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: OperationController/Edit/5
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

        // GET: OperationController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: OperationController/Delete/5
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
