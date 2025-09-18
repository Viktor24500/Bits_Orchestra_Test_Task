using BitOrchestraTestTask.DAL;

namespace BitOrchestraTestTask.BL
{
	public class UserService
	{
		private ILogger<UserService> _logger;
		public UserService(ILogger<UserService> logger)
		{
			_logger = logger;
		}
		public async Task<Result<List<User>>> GetAllUsers()
		{
			Result<List<User>> users = new Result<List<User>>();
			ILogger<UserDAL> loggerDAL = new Logger<UserDAL>(new LoggerFactory());
			IConfiguration configuration = new ConfigurationBuilder()
				.AddJsonFile("appsettings.json")
				.Build();
			UserDAL userDAL = new UserDAL(loggerDAL, configuration);
			users = await userDAL.GetAllUsers();
			if (users.ErrorCode != 0)
			{
				_logger.LogError(users.ErrorMessage);
				return users;
			}
			return users;
		}
		public Result<User> CreateUser(Stream csvStream)
		{
			return null;
		}

		public Result<User> UpdateUser()
		{
			return null;
		}
		public async Task<Result> DeleteUser(int id)
		{
			Result result = new Result();
			if (id < 0)
			{
				result.ErrorCode = 400;
				result.ErrorMessage = "Id must be greater than zero";
				_logger.LogError(result.ErrorMessage);
				return result;
			}
			ILogger<UserDAL> loggerDAL = new Logger<UserDAL>(new LoggerFactory());
			IConfiguration configuration = new ConfigurationBuilder()
	.AddJsonFile("appsettings.json")
	.Build();
			UserDAL userDAL = new UserDAL(loggerDAL, configuration);
			result = await userDAL.DeleteUser(id);
			if (result.ErrorCode != 0)
			{
				_logger.LogError(result.ErrorMessage);
				return result;
			}
			return result;
		}
	}
}
