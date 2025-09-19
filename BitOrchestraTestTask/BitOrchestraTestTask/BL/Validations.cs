using System.Globalization;

namespace BitOrchestraTestTask.BL
{
	public static class Validations
	{
		public static Result<DateTime> DateValidation(string date)
		{
			Result<DateTime> result = new Result<DateTime>();
			DateTime dateOut;
			if (!DateTime.TryParseExact(date, "yyyy-mm-dd",
				CultureInfo.InvariantCulture, //IFormatProvider
				DateTimeStyles.None,
				out dateOut))
			{
				result.ErrorCode = 400;
				result.ErrorMessage = $"Invalid datatime. Can't parse 'Date of Birth' from string to DateTime";
				return result;
			}
			result.Data = dateOut;
			return result;
		}

		public static Result<bool> BoolValidation(string data)
		{
			Result<bool> result = new Result<bool>();
			bool validBool;
			if (!bool.TryParse(data, out validBool))
			{
				result.ErrorCode = 400;
				result.ErrorMessage = "Invalid data. Can't parse 'Married' from string to bool";
				return result;
			}
			result.Data = validBool;
			return result;
		}

		public static Result<decimal> DecimalValidation(string data)
		{
			Result<decimal> result = new Result<decimal>();
			decimal validDecimal;
			if (!decimal.TryParse(data, out validDecimal))
			{
				result.ErrorCode = 400;
				result.ErrorMessage = "Invalid data. Can't parse 'Salary' from string to decimal";
				return result;
			}
			result.Data = validDecimal;
			if (result.Data < 0)
			{
				result.ErrorCode = 400;
				result.ErrorMessage = "Invalid data. Salary must be positive";
				return result;
			}
			return result;
		}

		public static bool isCollumnNumberCountEqualsFive(string[] data)
		{
			if (data.Length != 5)
			{
				return false;
			}
			else
			{
				return true;
			}
		}
	}
}
