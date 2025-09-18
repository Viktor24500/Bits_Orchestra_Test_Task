using BitOrchestraTestTask.BL;
using BitOrchestraTestTask.Models;
using Microsoft.AspNetCore.Mvc;

namespace BitOrchestraTestTask.Controllers
{
	public class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger;

		public HomeController(ILogger<HomeController> logger)
		{
			_logger = logger;
		}

		[HttpPost]
		[Route("/user")]
		public IActionResult CreateUser()
		{
			return View();
		}

		[HttpGet]
		[Route("/user")]
		public async Task<IActionResult> GetAllUsers()
		{
			Result<List<User>> result = new Result<List<User>>();
			ILogger<UserService> loggerUserService = new Logger<UserService>(new LoggerFactory());
			UserService userService = new UserService(loggerUserService);
			result = await userService.GetAllUsers();
			if (result.ErrorCode != 0)
			{
				_logger.LogError(result.ErrorMessage);
				ErrorViewModel errorViewModel = new ErrorViewModel(result.ErrorCode, result.ErrorMessage);
				return View("Error", errorViewModel);
			}
			return View("Main", result.Data);
		}

		//[HttpPut]
		//[Route("/user/{id}")]
		public IActionResult UpdateUser(int id)
		{
			return View();
		}

		//[HttpDelete]
		//[Route("/user/{id}")]
		public async Task<IActionResult> DeleteUser(int id)
		{
			Result result = new Result();
			ILogger<UserService> loggerUserService = new Logger<UserService>(new LoggerFactory());
			UserService userService = new UserService(loggerUserService);
			result = await userService.DeleteUser(id);
			if (result.ErrorCode != 0)
			{
				_logger.LogError(result.ErrorMessage);
				ErrorViewModel errorViewModel = new ErrorViewModel(result.ErrorCode, result.ErrorMessage);
				return View("Error", errorViewModel);
			}
			return RedirectToAction("GetAllUsers");
		}
	}
}
