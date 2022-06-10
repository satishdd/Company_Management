using Company_Management.Context;
using Company_Management.Modules;
using Company_Management.Services;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Company_Management.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private IEmployeeService _employeeService;
        private readonly DataContext _context;

        public EmployeesController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        [HttpPost]
        [SwaggerOperation
        (Summary = "Upload employees data from .xlsx")]
        [Route("UploadExcelFile")]
        public async Task<IActionResult> UploadExcelFile([FromForm] UploadXMLFileRequest request)
        {
            UploadXMLFileResponse response = new UploadXMLFileResponse();

            string path = "UploadFileFolder/" + request.File.FileName;
            try
            {
                using (FileStream stream = new FileStream(path, FileMode.CreateNew))
                {
                    await request.File.CopyToAsync(stream);
                }

                response = await _employeeService.UploadXMLFile(request, path);

            }
            catch (Exception ex)
            {
                response.Message = ex.Message;

            }
            finally
            {
                string[] files = Directory.GetFiles("UploadFileFolder/");
                foreach (string file in files)
                {
                    System.IO.File.Delete(file);
                    Console.WriteLine($"{file} is deleted.");
                }
            }

            return Ok(response);
        }

        [HttpPost]
        [SwaggerOperation
        (Summary = "Upload employees data .csv file")]
        [Route("UploadCSVFile")]
        public async Task<IActionResult> UploadCSVFile([FromForm] UploadCSVFileRequest request)
        {
            UploadCSVFileResponse response = new UploadCSVFileResponse();

            string path = "UploadFileFolder/" + request.File.FileName;
            try
            {
                using (FileStream stream = new FileStream(path, FileMode.CreateNew))
                {
                    await request.File.CopyToAsync(stream);
                }

                response = await _employeeService.UploadCSVFile(request, path);

                string[] files = Directory.GetFiles("UploadFileFolder/");
                foreach (string file in files)
                {
                    System.IO.File.Delete(file);
                    Console.WriteLine($"{file} is deleted.");
                }

            }
            catch (Exception ex)
            {
                response.Message = ex.Message;

            }
            finally
            {
                string[] files = Directory.GetFiles("UploadFileFolder/");
                foreach (string file in files)
                {
                    System.IO.File.Delete(file);
                    Console.WriteLine($"{file} is deleted.");
                }
            }

            return Ok(response);
        }


        [SwaggerOperation
        (Summary = "Get all the Employees")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var employees = await _employeeService.GetAllEmployees();
            return Ok(employees);
        }

        [SwaggerOperation
        (Summary = "Get Employee by Employee Email")]
        [HttpGet("{email}")]
        public async Task<IActionResult> GetEmployeeByEmail(string email)
        {
            var employee = await _employeeService.GetEmployeeByEmail(email);
            return Ok(employee);
        }

        [SwaggerOperation
        (Summary = "Get Employee by Employee ID")]
        [HttpGet("employee/{empId}")]
        public async Task<IActionResult> GetEmployeeById(string empId)
        {
            var employee = await _employeeService.GetEmployeeById(empId);
            return Ok(employee);
        }
    }
}
