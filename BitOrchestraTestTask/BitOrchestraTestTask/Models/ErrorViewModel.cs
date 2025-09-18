namespace BitOrchestraTestTask.Models
{
	public class ErrorViewModel
	{
		public ErrorViewModel(int errorCode, string errorMessage)
		{
			ErrorCode = errorCode;
			ErrorMessage = errorMessage;
		}
		public int ErrorCode { get; set; }
		public string? ErrorMessage { get; set; }
	}
}
