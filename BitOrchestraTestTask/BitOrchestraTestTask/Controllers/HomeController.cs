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
		//[Route("/user")]
		public async Task<IActionResult> CreateUsers([FromForm] IFormFile inputFile)
		{
			Result result = new Result();
			try
			{
				if (inputFile == null || inputFile.Length == 0)
				{
					return View("Error", new ErrorViewModel("File is empty"));
				}

				ILogger<UserService> loggerUserService = new Logger<UserService>(new LoggerFactory());
				UserService userService = new UserService(loggerUserService);

				await using (Stream stream = inputFile.OpenReadStream())
				{
					result = await userService.CreateUser(stream);
				}
				if (result.ErrorCode != 0)
				{
					_logger.LogError($"CreateUsers failed: {result.ErrorMessage}");
					return View("Error", new ErrorViewModel($"Upload file failed: {result.ErrorMessage}"));
				}
				return RedirectToAction("GetAllUsers");
			}
			catch (Exception ex)
			{
				int errorCode = ex.HResult;
				_logger.LogError(ex, $"Error uploading file, code: {errorCode}");
				return View("Error", new ErrorViewModel($"Error uploading file, code: {errorCode}"));
			}
		}

		[HttpGet]
		//[Route("/uploadFile")]
		public IActionResult GetUploadFileForm()
		{
			return View("Views/Form/UploadFileForm.cshtml");
		}

		[HttpGet]
		//[Route("/error")]
		public IActionResult GetErrorPage(string message)
		{
			ErrorViewModel errorModel = new ErrorViewModel(message);
			return View("/Views/Shared/Error.cshtml", errorModel);
		}

		[HttpGet]
		//[Route("/user")]
		public async Task<IActionResult> GetAllUsers()
		{
			Result<List<User>> result = new Result<List<User>>();
			ILogger<UserService> loggerUserService = new Logger<UserService>(new LoggerFactory());
			UserService userService = new UserService(loggerUserService);
			result = await userService.GetAllUsers();
			if (result.ErrorCode != 0)
			{
				_logger.LogError(result.ErrorMessage);
				ErrorViewModel errorViewModel = new ErrorViewModel(result.ErrorMessage);
				return View("Error", errorViewModel);
			}
			return View("Main", result.Data);
		}

		[HttpPost]
		//[Route("/user/{id}")]
		public async Task<IActionResult> UpdateUser([FromBody] User user)
		{
			Result result = new Result();
			ILogger<UserService> loggerUserService = new Logger<UserService>(new LoggerFactory());
			UserService userService = new UserService(loggerUserService);
			result = await userService.UpdateUser(user);
			if (result.ErrorCode != 0)
			{
				_logger.LogError(result.ErrorMessage);
				ErrorViewModel errorViewModel = new ErrorViewModel(result.ErrorMessage);
				return RedirectToAction("GetErrorPage", errorViewModel.ErrorMessage);
				//return View("Error", errorViewModel);
			}
			return RedirectToAction("GetAllUsers");
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
				ErrorViewModel errorViewModel = new ErrorViewModel(result.ErrorMessage);
				return View("Error", errorViewModel);
			}
			return RedirectToAction("GetAllUsers");
		}
	}
}
