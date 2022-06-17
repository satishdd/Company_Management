using Company_Management.Controllers.Params;
using Company_Management.Exceptions;
using Company_Management.Modules;
using Company_Management.Services;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Text.Json;

namespace Company_Management.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private IEmployeeService _employeeService;

        public EmployeesController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        [HttpPost]
        [SwaggerOperation
        (Summary = "Bulk upload employees data to db. (Supported file formats : .XLSX and .CSV)")]
        [Route("UploadFile")]
        public async Task<IActionResult> UploadFileData([FromForm] UploadDataFromFile request)
        {
            Response response = new Response();
            string path = "UploadFileFolder/" + request.File.FileName;
            try
            {
                using (FileStream stream = new FileStream(path, FileMode.CreateNew))
                {
                    await request.File.CopyToAsync(stream);
                }

                response = await _employeeService.UploadFileData(request, path);
            }
            finally
            {
                string[] files = Directory.GetFiles("UploadFileFolder/");
                foreach (string file in files)
                {
                    System.IO.File.Delete(file);
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
            if (!Validator.ValidateEmail(email))
                throw new AppException("The specified string is not in the form required for an e-mail address.");

            var employee = await _employeeService.GetEmployeeByEmail(email);
            return Ok(employee);
        }

        [SwaggerOperation
        (Summary = "Get Employee by Employee ID")]
        [HttpGet("employee/{empId}")]
        public async Task<IActionResult> GetEmployeeById(string empId)
        {
            if (!empId.StartsWith("VD"))
            {
                throw new AppException("Enter valid employee ID : i.e. VD123");
            }
            var employee = await _employeeService.GetEmployeeById(empId);
            return Ok(employee);
        }

        [HttpPost]
        [SwaggerOperation
        (Summary = "Check available employees for feedback")]
        [Route("FetchRandomEmployees")]
        public async Task<ActionResult> GetEmployeeByMonth([FromForm] PicRandomEmployees model)
        {
            if (model == null) { throw new AppException("Please enter the date"); }
            if (model.NumberOfEmployees == 0) { model.NumberOfEmployees = 5; }
            var employees = _employeeService.PicEmployeeForFeedback(model);
            if (employees.Exception != null)
            {
                throw new AppException(employees.Exception.InnerException.Message);
            }
            return Ok(employees);
        }

        [SwaggerOperation
        (Summary = "Save random employees for feedback")]
        [HttpPost("employee/{url}")]
        public async Task<ActionResult> SaveEmployeeForFeedback(string url = "https://localhost:44300/api/Employees/FetchRandomEmployees")
        {
            //var currentData = await HttpClient.GetAsync(url);
            var jsonSeriolizerOption = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
            using (var httpClient = new HttpClient())
            {
                var employeesFeedback = new FeedbackDetails();
                var content = new  StringContent()
            }
            if (ModelState.IsValid)
            {
                return Ok(pickedEmployees);
            }
            else
            {
                return null;
            }
            //return Ok(employees);
        }
    }
}
