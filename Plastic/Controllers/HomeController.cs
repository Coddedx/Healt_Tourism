using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Plastic.Models;
using System.Diagnostics;
using System.Security.Claims;

namespace Plastic.Controllers
{
	public class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger;
        private readonly SignInManager<AppUser> _signInManager;
		private readonly UserManager<AppUser> _userManager;


        public HomeController(ILogger<HomeController> logger, SignInManager<AppUser> signInManager, UserManager<AppUser> userManager)
		{
			_logger = logger;
			_signInManager = signInManager;
			_userManager = userManager;
		}

		public IActionResult Index()
		{
			return View();
		}

		//public async Task<IActionResult> Navbar()
		//{
		//	var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

		//}

		public IActionResult Privacy() 
		{
			return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}
