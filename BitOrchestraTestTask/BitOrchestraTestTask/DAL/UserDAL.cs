using BitOrchestraTestTask.BL;
using System.Data;
using System.Data.SqlClient;

namespace BitOrchestraTestTask.DAL
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
						User user = new User(
							reader.GetInt32(reader.GetOrdinal("UserId")),
							reader.GetString(reader.GetOrdinal("UserName")),
							reader.GetDateTime(reader.GetOrdinal("DateOfBirth")),
							reader.GetBoolean(reader.GetOrdinal("Married")),
							reader.GetString(reader.GetOrdinal("Phone")),
							reader.GetDecimal(reader.GetOrdinal("Salary"))
							);
						result.Data.Add(user);
					}
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
				return result;
			}
		}
	}
}
