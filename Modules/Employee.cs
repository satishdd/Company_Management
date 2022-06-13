using System.ComponentModel.DataAnnotations;

namespace Company_Management.Modules
{
    public class Employee
    {
        public int Id { get; set; }

        public string EmployeeID { get; set; }
        public string Name { get; set; }

        [DataType(DataType.PhoneNumber)]
        public string Phone { get; set; }

        [DataType(DataType.EmailAddress)]
        public string EmailID { get; set; }
    }
}
