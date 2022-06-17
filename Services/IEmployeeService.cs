using Company_Management.Controllers.Params;
using Company_Management.Modules;

namespace Company_Management.Services
{
    public interface IEmployeeService
    {
        // Upload data from Excel/.csv file.
        public Task<Response> UploadFileData(UploadDataFromFile request, string Path);
        public Task<IEnumerable<Employee>> GetAllEmployees();
        public Task<Employee> GetEmployeeByEmail(string emailID);
        public Task<Employee> GetEmployeeById(string empID);
        public Task<IEnumerable<Employee>> PicEmployeeForFeedback(PicRandomEmployees model);
        public Task<IEnumerable<FeedbackDetails>> SaveEmployeeForFeedback(PicRandomEmployees model);
    }
}
