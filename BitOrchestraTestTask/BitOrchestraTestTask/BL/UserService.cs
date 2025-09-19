using BitOrchestraTestTask.DAL;
using System.Globalization;

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
							if (result.ErrorCode != 200)
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
						if (rowResult.ErrorCode != 200)
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
				if (result.ErrorCode != 200)
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
		private Result ValidateHeader(string[] headers)
		{
			Result result = new Result();
			if (headers.Length != 5)
			{
				result.ErrorCode = 102;
				result.ErrorMessage = "Invalid number of columns";
				return result;
			}
			for (int i = 0; i < headers.Length; i++)
			{
				string headerName = headers[i];
				switch (i)
				{
					case 0:
						if (headerName != "Name")
						{
							result.ErrorCode = 103;
							result.ErrorMessage = $"Invalid column name at position 1 {headerName}";
							return result;
						}
						break;
					case 1:
						if (headerName != "Date of birth")
						{
							result.ErrorCode = 103;
							result.ErrorMessage = $"Invalid column name at position 2 {headerName}";
							return result;
						}
						break;
					case 2:
						if (headerName != "Married")
						{
							result.ErrorCode = 103;
							result.ErrorMessage = $"Invalid column name at position 3 {headerName}";
							return result;
						}
						break;
					case 3:
						if (headerName != "Phone")
						{
							result.ErrorCode = 103;
							result.ErrorMessage = $"Invalid column name at position 4 {headerName}";
							return result;
						}
						break;
					case 4:
						if (headerName != "Salary")
						{
							result.ErrorCode = 103;
							result.ErrorMessage = $"Invalid column name at position 5 {headerName}";
							return result;
						}
						break;
					default:
						result.ErrorCode = 104;
						result.ErrorMessage = "Unexpected error during header validation";
						return result;
				}
			}
			result.ErrorCode = 200;
			return result;
		}

		private Result<User> ParseFileRow(string[] row, int rowIndex)
		{
			Result<User> result = new Result<User>();
			User user = new User();
			if (row.Length != 5)
			{
				result.ErrorCode = 301;
				result.ErrorMessage = "Invalid number of columns";
				return result;
			}
			for (int i = 0; i < row.Length; i++)
			{
				string rowValue = row[i];
				if (string.IsNullOrEmpty(rowValue))
				{
					result.ErrorCode = 302;
					result.ErrorMessage = $"Invalid data at position {i + 1} {rowValue}. Row {rowIndex}";
					return result;
				}
				switch (i)
				{
					case 0:
						user.Name = rowValue;
						break;
					case 1:
						DateTime dateOfBirth;
						if (!DateTime.TryParseExact(rowValue, "yyyy-mm-dd",
							CultureInfo.InvariantCulture, //IFormatProvider
							DateTimeStyles.None,
							out dateOfBirth))
						{
							result.ErrorCode = 303;
							result.ErrorMessage = $"Invalid data at position {i + 1} {rowValue}. Row {rowIndex}";
							return result;
						}
						user.DateOfBirth = dateOfBirth;
						break;
					case 2:
						bool maried;
						if (!bool.TryParse(rowValue, out maried))
						{
							result.ErrorCode = 304;
							result.ErrorMessage = $"Invalid data at position {i + 1} {rowValue}. Row {rowIndex}";
							return result;
						}
						user.Married = maried;
						break;
					case 3:
						user.Phone = rowValue;
						break;
					case 4:
						decimal salary;
						if (!decimal.TryParse(rowValue, out salary))
						{
							result.ErrorCode = 305;
							result.ErrorMessage = $"Invalid data at position {i + 1} {rowValue}. Row {rowIndex}";
							return result;
						}
						if (salary < 0)
						{
							result.ErrorCode = 306;
							result.ErrorMessage = $"Invalid data at position {i + 1} {rowValue}. Row {rowIndex}";
							return result;
						}
						user.Salary = salary;
						break;
					default:
						result.ErrorCode = 307;
						result.ErrorMessage = "Unexpected error during header validation";
						return result;
				}
			}
			result.ErrorCode = 200;
			result.Data = user;
			return result;
		}
	}
}
