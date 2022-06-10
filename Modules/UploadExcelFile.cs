namespace Company_Management.Modules
{
    public class UploadXMLFileRequest
    {
        public IFormFile File { get; set; }
    }

    public class UploadXMLFileResponse
    {
        public string Message { get; set; }
    }

    public class ExcelBulkUploadParameter
    {
        public string EmployeeID { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string EmailID { get; set; }
    }
}
