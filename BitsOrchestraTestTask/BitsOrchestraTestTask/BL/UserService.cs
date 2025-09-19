using BitsOrchestraTestTask.DAL;

namespace BitsOrchestraTestTask.BL
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
		public async Task<Result> CreateUser(Stream csvStream)
		{
			Result result = new Result();
			List<User> users = new List<User>();
			try
			{
				int rowNumber = 0;

				using (StreamReader reader = new StreamReader(csvStream))
				{
					while (!reader.EndOfStream)
					{
						string line = reader.ReadLine();
						string[] lineArray = line.Split(',');
						if (rowNumber == 0)
						{
							result = ValidateHeader(lineArray);
							if (result.ErrorCode != 0)
							{
								_logger.LogError(result.ErrorMessage);
								return result;
							}
						}
						if (rowNumber == 0)
						{
							rowNumber++;
							continue;
						}
						rowNumber++;
						Result<User> rowResult = ParseFileRow(lineArray, rowNumber);
						if (rowResult.ErrorCode != 0)
						{
							result.ErrorCode = rowResult.ErrorCode;
							result.ErrorMessage = rowResult.ErrorMessage;
							_logger.LogError(result.ErrorMessage);
							return result;
						}
						users.Add(rowResult.Data);
					}
				}
				ILogger<UserDAL> loggerDAL = new Logger<UserDAL>(new LoggerFactory());
				IConfiguration configuration = new ConfigurationBuilder()
		.AddJsonFile("appsettings.json")
		.Build();
				UserDAL userDAL = new UserDAL(loggerDAL, configuration);
				result = await userDAL.CreateUsers(users);
				if (result.ErrorCode != 0)
				{
					_logger.LogError(result.ErrorMessage);
					return result;
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error parsing CSV");
				result.ErrorCode = ex.HResult;
				result.ErrorMessage = ex.Message;
				return result;
			}
			return result;
		}

		public async Task<Result> UpdateUser(User user)
		{
			Result result = new Result();
			result = ValidateUser(user);
			if (result.ErrorCode != 0)
			{
				_logger.LogError(result.ErrorMessage);
				return result;
			}
			ILogger<UserDAL> loggerDAL = new Logger<UserDAL>(new LoggerFactory());
			IConfiguration configuration = new ConfigurationBuilder()
	.AddJsonFile("appsettings.json")
	.Build();
			UserDAL userDAL = new UserDAL(loggerDAL, configuration);
			result = await userDAL.UpdateUser(user);
			if (result.ErrorCode != 0)
			{
				_logger.LogError(result.ErrorMessage);
				return result;
			}
			return result;
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
		private Result ValidateHeader(string[] headers)
		{
			Result result = new Result();
			if (!Validations.isCollumnNumberCountEqualsFive(headers))
			{
				result.ErrorCode = 400;
				result.ErrorMessage = "Invalid number of columns";
				return result;
			}
			for (int i = 0; i < headers.Length; i++)
			{
				if (!string.Equals(headers[i], "Name") && i == 0)
				{
					result.ErrorCode = 400;
					result.ErrorMessage = $"HEADERS. Invalid column name. Your value '{headers[i]}'. Valid value 'Name'";
					return result;
				}
				if (!string.Equals(headers[i], "Date of birth") && i == 1)
				{
					result.ErrorCode = 400;
					result.ErrorMessage = $"HEADERS. Invalid column name. Your value '{headers[i]}'. Valid value 'Date of birth'";
					return result;
				}
				if (!string.Equals(headers[i], "Married") && i == 2)
				{
					result.ErrorCode = 400;
					result.ErrorMessage = $"HEADERS. Invalid column name. Your value '{headers[i]}'. Valid value 'Married'";
					return result;
				}
				if (!string.Equals(headers[i], "Phone") && i == 3)
				{
					result.ErrorCode = 400;
					result.ErrorMessage = $"HEADERS. Invalid column name. Your value '{headers[i]}'. Valid value 'Phone'";
					return result;
				}
				if (!string.Equals(headers[i], "Salary") && i == 4)
				{
					result.ErrorCode = 400;
					result.ErrorMessage = $"HEADERS. Invalid column name. Your value '{headers[i]}'. Valid value 'Salary'";
					return result;
				}
			}
			result.ErrorCode = 0;
			return result;
		}

		private Result<User> ParseFileRow(string[] row, int rowIndex)
		{
			Result<User> result = new Result<User>();
			User user = new User();
			if (!Validations.isCollumnNumberCountEqualsFive(row))
			{
				result.ErrorCode = 400;
				result.ErrorMessage = "Invalid number of columns";
				return result;
			}
			for (int i = 0; i < row.Length; i++)
			{
				if (string.IsNullOrEmpty(row[i]))
				{
					result.ErrorCode = 400;
					result.ErrorMessage = $"Invalid data at position {i + 1}. Your value '{row[i]}'. Row {rowIndex}";
					return result;
				}

				if (i == 0)
				{
					user.Name = row[i];
				}

				if (i == 1)
				{
					Result<DateTime> dateResult = Validations.DateValidation(row[i]);
					if (dateResult.ErrorCode != 0)
					{
						result.ErrorCode = dateResult.ErrorCode;
						result.ErrorMessage = $"Invalid data at position {i + 1}. Your value '{row[i]}'. Row {rowIndex}";
						return result;
					}
					user.DateOfBirth = dateResult.Data;
				}

				if (i == 2)
				{
					Result<bool> boolResult = Validations.BoolValidation(row[i]);
					if (boolResult.ErrorCode != 0)
					{
						result.ErrorCode = boolResult.ErrorCode;
						result.ErrorMessage = $"Invalid data at position {i + 1}. Your value ' {row[i]} '. Row {rowIndex}";
						return result;
					}
					user.Married = boolResult.Data;
				}

				if (i == 3)
				{
					user.Phone = row[i];
				}

				if (i == 4)
				{
					Result<decimal> decimalResult = Validations.DecimalValidation(row[i]);
					if (decimalResult.ErrorCode != 0)
					{
						result.ErrorCode = decimalResult.ErrorCode;
						result.ErrorMessage = $"Invalid data at position {i + 1}. Your value ' {row[i]} '. Row {rowIndex}";
						return result;
					}
					user.Salary = decimalResult.Data;
				}
			}
			result.ErrorCode = 0;
			result.Data = user;
			return result;
		}

		private Result ValidateUser(User user)
		{
			Result result = new Result();
			if (user.Id < 0)
			{
				result.ErrorCode = 400;
				result.ErrorMessage = "Id must be greater than zero";
				_logger.LogError(result.ErrorMessage);
				return result;
			}
			if (string.IsNullOrEmpty(user.Name))
			{
				result.ErrorCode = 400;
				result.ErrorMessage = "Name can't be empty";
				_logger.LogError(result.ErrorMessage);
				return result;
			}
			if (string.IsNullOrEmpty(user.Phone))
			{
				result.ErrorCode = 400;
				result.ErrorMessage = "Phone can't be empty";
				_logger.LogError(result.ErrorMessage);
				return result;
			}
			if (user.Salary < 0)
			{
				result.ErrorCode = 400;
				result.ErrorMessage = "Salary must be greater than zero";
				_logger.LogError(result.ErrorMessage);
				return result;
			}
			result.ErrorCode = 0;
			return result;
		}
	}
}
