using System.ComponentModel.DataAnnotations;

namespace Company_Management.Modules
{
    /// <summary>
    /// Helps to store data from the file format of .xlsx file or .CSV file
    /// </summary>
    public class UploadDataFromFile
    {
        public IFormFile File { get; set; }
    }
    public class FileResponse
    {
        public string Message { get; set; }
    }

    public class BulkUploadParameter
    {
        public string EmployeeID { get; set; }
        public string Name { get; set; }

        public string Phone { get; set; }
        public string EmailID { get; set; }
    }
}
