using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Plastic.IRepository;
using Plastic.Models;
using Plastic.ViewModels;
using System.Diagnostics;

namespace Plastic.Controllers
{
	public class ClinicController : Controller
	{
		PlasticDbContext db = new PlasticDbContext();
        private readonly IFranchiseRepository _franchiseRepository;

        public ClinicController(IFranchiseRepository franchiseRepository)
        {
            _franchiseRepository = franchiseRepository;
        }
        //private readonly PlasticDbContext _context;

        //    public ClinicController(PlasticDbContext context)
        //    {
        //_context = context;
        //    }

        // GET: ClinicController
        public async Task<IActionResult> Index(int pageNumber)  //	, IFormCollection fc,int id
        {
			var clinics = db.Clinics
				.Where(c => c.Deleted == false)
				.ToList();
            return View(clinics);
        }

        // GET: ClinicController/Details/5
        public ActionResult Details(int id)
		{
			return View();
		}

		// GET: ClinicController/Create
		public ActionResult Create()
		{
			return View();
		}

		// POST: ClinicController/Create
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

		// GET: ClinicController/Edit/5
		public ActionResult Edit(int id)
		{
			return View();
		}

		// POST: ClinicController/Edit/5
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

		// GET: ClinicController/Delete/5
		public ActionResult Delete(int id)
		{
			return View();
		}

		// POST: ClinicController/Delete/5
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
