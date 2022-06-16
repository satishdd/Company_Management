using Company_Management.Controllers.Params;
using Company_Management.Modules;
using System.Collections;

namespace Company_Management.Services
{
    public interface IEmployeeService
    {
        // Upload data from Excel/.csv file.
        public Task<Response> UploadFileData(UploadDataFromFile request, string Path);
        public Task<IEnumerable<Employee>> GetAllEmployees();
        public Task<Employee> GetEmployeeByEmail(string emailID);
        public Task<Employee> GetEmployeeById(string empID);

        public List<FeedbackDetails> GetEmployeeByMonth(GetMonthwiseEmployees model);
    }
}
