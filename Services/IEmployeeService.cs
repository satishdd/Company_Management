using Company_Management.Modules;

namespace Company_Management.Services
{
    public interface IEmployeeService
    {
        public Task<IEnumerable<Employee>> GetAllEmployees();
        public Task<Employee> GetEmployeeByEmail(string emailID);
        public Task<Employee> GetEmployeeById(string empID);
    }
}
