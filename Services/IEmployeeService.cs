using Company_Management.Modules;

namespace Company_Management.Services
{
    public interface IEmployeeService
    {
        public Task<IEnumerable<Employee>> GetAllEmployees();
        public Task<Employee> GetEmployeeByEmail(string emailID);
        public Task<Employee> GetEmployeeById(string empID);

        // Upload data from Excel/.csv file.
        public Task<UploadXMLFileResponse> UploadXMLFile(UploadXMLFileRequest request, string Path);

        public Task<UploadCSVFileResponse> UploadCSVFile(UploadCSVFileRequest request, string Path);
    }
}
