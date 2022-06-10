namespace Company_Management.Modules
{
    public class UploadCSVFileRequest
    {
        public IFormFile File { get; set; }
    }

    public class UploadCSVFileResponse
    {
        public string Message { get; set; }
    }
}
