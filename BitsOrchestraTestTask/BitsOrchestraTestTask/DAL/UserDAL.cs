using BitsOrchestraTestTask.BL;
using System.Data;
using System.Data.SqlClient;

namespace BitsOrchestraTestTask.DAL
{
	public class UserDAL
	{
		private ILogger<UserDAL> _logger;
		private IConfiguration _configuration;
		public UserDAL(ILogger<UserDAL> logger, IConfiguration configuration)
		{
			_logger = logger;
			_configuration = configuration;
		}

		public async Task<Result<List<User>>> GetAllUsers()
		{
			await using (SqlConnection connection = new SqlConnection(_configuration["ConnectionStrings:MainConnection"]))
			{
				connection.Open();
				string sql = "SELECT UserId, UserName, DateOfBirth, Married, Phone, Salary FROM UserInfo";
				SqlCommand command = new SqlCommand(sql, connection);
				command.CommandType = CommandType.Text;
				await using (SqlDataReader reader = command.ExecuteReader())
				{
					Result<List<User>> result = new Result<List<User>>();
					result.Data = new List<User>();
					while (reader.Read())
					{
						User user = new User
						{
							Id = reader.GetInt32(reader.GetOrdinal("UserId")),
							Name = reader.GetString(reader.GetOrdinal("UserName")),
							DateOfBirth = reader.GetDateTime(reader.GetOrdinal("DateOfBirth")),
							Married = reader.GetBoolean(reader.GetOrdinal("Married")),
							Phone = reader.GetString(reader.GetOrdinal("Phone")),
							Salary = reader.GetDecimal(reader.GetOrdinal("Salary"))
						};
						result.Data.Add(user);
					}
					result.ErrorCode = 0;
					return result;
				}
			}
		}

		public async Task<Result> DeleteUser(int id)
		{
			await using (SqlConnection connection = new SqlConnection(_configuration["ConnectionStrings:MainConnection"]))
			{
				Result result = new Result();
				connection.Open();
				string sql = "DELETE FROM UserInfo Where UserId=@id";
				SqlCommand command = new SqlCommand(sql, connection);
				command.CommandType = CommandType.Text;
				command.Parameters.AddWithValue("@id", id);
				int rowsAffected = await command.ExecuteNonQueryAsync();
				if (rowsAffected == 0)
				{
					result.ErrorCode = 404;
					result.ErrorMessage = "User not found";
				}
				result.ErrorCode = 0;
				return result;
			}
		}

		public async Task<Result> CreateUsers(List<User> users)
		{
			Result result = new Result();
			await using (SqlConnection connection = new SqlConnection(_configuration["ConnectionStrings:MainConnection"]))
			{
				connection.Open();
				SqlTransaction transaction = connection.BeginTransaction();
				SqlCommand command = connection.CreateCommand();
				command.Connection = connection;
				command.Transaction = transaction;
				try
				{
					string sql = "INSERT INTO UserInfo (UserName, DateOfBirth, Married, Phone, Salary)" +
						"VALUES(@name, @dateOfBirth, @married, @phone, @salary)";
					for (int i = 0; i < users.Count; i++)
					{
						command.CommandText = sql;
						command.Parameters.AddWithValue("@name", users[i].Name);
						command.Parameters.AddWithValue("@dateOfBirth", users[i].DateOfBirth);
						command.Parameters.AddWithValue("@married", users[i].Married);
						command.Parameters.AddWithValue("@phone", users[i].Phone);
						command.Parameters.AddWithValue("@salary", users[i].Salary);
						int rowsAffected = await command.ExecuteNonQueryAsync();
						if (rowsAffected <= 0)
						{
							transaction.Rollback();
							result.ErrorCode = 500;
							result.ErrorMessage = $"Failed to insert user int DB. User {i}";
							_logger.LogError(result.ErrorMessage);
							return result;
						}
						command.Parameters.Clear();
					}
					await transaction.CommitAsync();
				}
				catch (Exception ex)
				{
					transaction.Rollback();
					_logger.LogError(ex, "Error creating users");
					result.ErrorCode = ex.HResult;
					result.ErrorMessage = ex.Message;
					return result;
				}
			}
			result.ErrorCode = 0;
			return result;
		}

		public async Task<Result> UpdateUser(User user)
		{
			Result result = new Result();
			await using (SqlConnection connection = new SqlConnection(_configuration["ConnectionStrings:MainConnection"]))
			{
				connection.Open();
				string sql = "UPDATE UserInfo SET UserName=@name, DateOfBirth=@dateOfBirth," +
					"Married=@married, Phone=@phone, Salary=@salary Where UserId=@id";
				SqlCommand command = new SqlCommand(sql, connection);
				command.CommandType = CommandType.Text;
				command.Parameters.AddWithValue("@id", user.Id);
				command.Parameters.AddWithValue("@name", user.Name);
				command.Parameters.AddWithValue("@dateOfBirth", user.DateOfBirth);
				command.Parameters.AddWithValue("@married", user.Married);
				command.Parameters.AddWithValue("@phone", user.Phone);
				command.Parameters.AddWithValue("@salary", user.Salary);
				int rowsAffected = await command.ExecuteNonQueryAsync();
				if (rowsAffected == 0)
				{
					result.ErrorCode = 404;
					result.ErrorMessage = "User not updated";
				}
				result.ErrorCode = 0;
				return result;
			}
		}
	}
}
