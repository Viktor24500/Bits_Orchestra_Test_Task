using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BitOrchestraTestTask.BL
{
	public class User
	{
		public User(int id, string name, Date dateOfBirth, bool married, string phone, decimal salary)
		{
			Id = id;
			Name = name;
			DateOfBirth = dateOfBirth;
			Married = married;
			Phone = phone;
			Salary = salary;
		}
		public int Id { get; set; }
		public string Name { get; set; }
		public Date DateOfBirth { get; set; }
		public bool Married { get; set; }

		public string Phone { get; set; }

		public decimal Salary { get; set; }
	}
}
