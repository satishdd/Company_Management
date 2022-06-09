using Company_Management.Context;
using Company_Management.Modules;

namespace Company_Management.Services
{
    public class EmployeeService : IEmployeeService
    {
        private DataContext _context;

        public EmployeeService(DataContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Employee>> GetAllEmployees()
        {
            return _context.Employees;
        }

        public async Task<Employee> GetEmployeeByEmail(string email)
        {
            var response = _context.Employees.FirstOrDefault(e => e.EmailID == email);
            if (response == null) throw new KeyNotFoundException("Employee not found in this Employee EmailID");
            return response;
        }

        public async Task<Employee> GetEmployeeById(string empId)
        {
            var emp = _context.Employees.FirstOrDefault(e => e.EmployeeID == empId);
            if (emp == null) throw new KeyNotFoundException("Employee not found Employee ID");
            return emp;
        }
    }
}
