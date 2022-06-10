using Company_Management.Context;
using Company_Management.Modules;
using ExcelDataReader;
using LumenWorks.Framework.IO.Csv;
using Microsoft.Data.SqlClient;
using System.Data;
using Microsoft.Extensions.Configuration;

namespace Company_Management.Services
{
    public class EmployeeService : IEmployeeService
    {
        private DataContext _context;
        private IConfiguration _configuration;
        public readonly SqlConnection _sqlConnection;

        public EmployeeService(DataContext context, IConfiguration _configuration)
        {
            _context = context;
            _configuration = _configuration;
            _sqlConnection = new SqlConnection(_configuration["ConnectionStrings:DefaultConnection"]);
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

        public async Task<UploadCSVFileResponse> UploadCSVFile(UploadCSVFileRequest request, string Path)
        {
            UploadCSVFileResponse response = new UploadCSVFileResponse();
            List<ExcelBulkUploadParameter> Parameters = new List<ExcelBulkUploadParameter>();
            response.Message = "Data insurted successfully";
            try
            {
                if (request.File.FileName.ToLower().Contains(".csv"))
                {
                    DataTable value = new DataTable();
                    using (var csvReader = new CsvReader(new StreamReader(File.OpenRead(Path)), true))
                    {
                        value.Load(csvReader);
                    };

                    for (int i = 0; i < value.Rows.Count; i++)
                    {
                        ExcelBulkUploadParameter readData = new ExcelBulkUploadParameter();
                        readData.EmployeeID = value.Rows[i][0] != null ? Convert.ToString(value.Rows[i][0]) : "-1";
                        readData.Name = value.Rows[i][1] != null ? Convert.ToString(value.Rows[i][1]) : "-1";
                        readData.Phone = value.Rows[i][2] != null ? Convert.ToString(value.Rows[i][2]) : "-1";
                        readData.EmailID = value.Rows[i][3] != null ? Convert.ToString(value.Rows[i][3]) : "-1";
                        Parameters.Add(readData);
                    }

                    if (Parameters.Count > 0)
                    {
                        if (ConnectionState.Open != _sqlConnection.State)
                        {
                            await _sqlConnection.OpenAsync();
                        }

                        foreach (ExcelBulkUploadParameter rows in Parameters)
                        {
                            var query = "INSERT INTO Employees (EmployeeID, Name, Phone, EmailID) VALUES (@EmployeeID, @Name, @Phone, @EmailID)";
                            using (SqlCommand sqlCommand = new SqlCommand(query, _sqlConnection))
                            {
                                sqlCommand.CommandType = CommandType.Text;
                                sqlCommand.CommandTimeout = 180;
                                sqlCommand.Parameters.AddWithValue("@EmployeeID", rows.EmployeeID);
                                sqlCommand.Parameters.AddWithValue("@Name", rows.Name);
                                sqlCommand.Parameters.AddWithValue("@Phone", rows.Phone);
                                sqlCommand.Parameters.AddWithValue("@EmailID", rows.EmailID);
                                int Status = await sqlCommand.ExecuteNonQueryAsync();
                                if (Status <= 0)
                                {
                                    response.Message = "Query Not Executed";
                                    return response;
                                }
                            }
                        }
                    }
                }
                else
                {
                    response.Message = "InValid File";
                }
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            finally
            {
                await _sqlConnection.CloseAsync();
                await _sqlConnection.DisposeAsync();
            }
            return response;
        }

        public async Task<UploadXMLFileResponse> UploadXMLFile(UploadXMLFileRequest request, string path)
        {
            UploadXMLFileResponse response = new UploadXMLFileResponse();
            List<ExcelBulkUploadParameter> Parameters = new List<ExcelBulkUploadParameter>();
            DataSet dataSet;
            response.Message = "Data insurted successfully";

            try
            {
                if (request.File.FileName.ToLower().Contains(".xlsx"))
                {
                    FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
                    IExcelDataReader reader = ExcelReaderFactory.CreateReader(stream);
                    dataSet = reader.AsDataSet(
                        new ExcelDataSetConfiguration()
                        {
                            UseColumnDataType = false,
                            ConfigureDataTable = (tableReader) => new ExcelDataTableConfiguration()
                            {
                                UseHeaderRow = true
                            }
                        });
                    try
                    {
                        for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
                        {
                            ExcelBulkUploadParameter rows = new ExcelBulkUploadParameter();
                            rows.EmployeeID = dataSet.Tables[0].Rows[i].ItemArray[0] != null ? Convert.ToString(dataSet.Tables[0].Rows[i].ItemArray[0]) : "-1";
                            rows.Name = dataSet.Tables[0].Rows[i].ItemArray[1] != null ? Convert.ToString(dataSet.Tables[0].Rows[i].ItemArray[1]) : "-1";
                            rows.Phone = dataSet.Tables[0].Rows[i].ItemArray[2] != null ? Convert.ToString(dataSet.Tables[0].Rows[i].ItemArray[2]) : "-1";
                            rows.EmailID = dataSet.Tables[0].Rows[i].ItemArray[3] != null ? Convert.ToString(dataSet.Tables[0].Rows[i].ItemArray[3]) : "-1";
                            Parameters.Add(rows);
                        }
                    }
                    finally
                    {
                        stream.Close();
                    }

                    if (Parameters.Count > 0)
                    {
                        if (ConnectionState.Open != _sqlConnection.State)
                        {
                            await _sqlConnection.OpenAsync();
                        }

                        foreach (ExcelBulkUploadParameter rows in Parameters)
                        {
                            var query = "INSERT INTO Employees (EmployeeID, Name, Phone, EmailID) VALUES (@EmployeeID, @Name, @Phone, @EmailID)";
                            using (SqlCommand sqlCommand = new SqlCommand(query, _sqlConnection))
                            {
                                sqlCommand.CommandType = CommandType.Text;
                                sqlCommand.CommandTimeout = 180;
                                sqlCommand.Parameters.AddWithValue("@EmployeeID", rows.EmployeeID);
                                sqlCommand.Parameters.AddWithValue("@Name", rows.Name);
                                sqlCommand.Parameters.AddWithValue("@Phone", rows.Phone);
                                sqlCommand.Parameters.AddWithValue("@EmailID", rows.EmailID);
                                int Status = await sqlCommand.ExecuteNonQueryAsync();
                                if (Status <= 0)
                                {
                                    response.Message = "Query Not Executed";
                                    return response;
                                }
                            }
                        }
                    }
                }
                else
                {
                    response.Message = "Invalid File, Please select proper file format.";
                }
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            finally
            {
                await _sqlConnection.CloseAsync();
                await _sqlConnection.DisposeAsync();
            }

            return response;
        }
    }
}
