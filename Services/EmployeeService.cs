using Company_Management.Context;
using Company_Management.Exceptions;
using Company_Management.Modules;
using ExcelDataReader;
using LumenWorks.Framework.IO.Csv;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Text;

namespace Company_Management.Services
{
    public class EmployeeService : IEmployeeService
    {
        private DataContext _context;
        public readonly SqlConnection _sqlConnection;

        public EmployeeService(DataContext context, IConfiguration _configuration)
        {
            try
            {
                _context = context;
                _sqlConnection = new SqlConnection(_configuration["ConnectionStrings:DefaultConnection"]);
            }
            catch (Exception ex)
            {
                throw new AppException(ex.Message);
                ErrorHandlerMiddleware.ErrorLogging(ex);
            }
        }

        public async Task<IEnumerable<Employee>> GetAllEmployees()
        {
            try
            {
                return _context.Employees;
            }
            catch (Exception ex)
            {
                throw new AppException(ex.Message);
                ErrorHandlerMiddleware.ErrorLogging(ex);
            }
        }

        public async Task<Employee> GetEmployeeByEmail(string email)
        {
            try
            {
                var response = _context.Employees.FirstOrDefault(e => e.EmailID == email);
                if (response == null) throw new KeyNotFoundException("Email '" + email + "' not found");
                return response;
            }
            catch (Exception ex)
            {
                throw new AppException(ex.Message);
                ErrorHandlerMiddleware.ErrorLogging(ex);
            }
        }

        public async Task<Employee> GetEmployeeById(string empId)
        {
            try
            {
                var emp = _context.Employees.FirstOrDefault(e => e.EmployeeID == empId);
                if (emp == null) throw new KeyNotFoundException("EmployeeID '" + empId + "' not found");
                return emp;
            }
            catch (Exception ex)
            {
                throw new AppException(ex.Message);
                ErrorHandlerMiddleware.ErrorLogging(ex);
            }
        }

        public async Task<FileResponse> UploadFileData(UploadDataFromFile request, string path)
        {
            FileResponse response = new FileResponse();
            List<BulkUploadParameter> Parameters = new List<BulkUploadParameter>();
            DataSet dataSet;

            try
            {
                if (request.File.FileName.ToLower().Contains(".xlsx"))
                {
                    FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
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
                    if ((dataSet.Tables[0].Columns.Count != 4) ||
                            dataSet.Tables[0].Columns[0].ColumnName != "EmployeeID" ||
                            dataSet.Tables[0].Columns[1].ColumnName != "Name" ||
                            dataSet.Tables[0].Columns[2].ColumnName != "Phone" ||
                            dataSet.Tables[0].Columns[3].ColumnName != "EmailID")
                    {
                        response.Message = "File columns did not match. Ensure that there are only 4 columns, and added as following : |EmployeeID|Name|Phone|EmailID|";
                        stream.Close();
                        return response;
                    }
                    // Check duplicate records in excel file
                    if (dataSet.Tables[0].AsEnumerable().GroupBy(r => r["EmployeeID"]).Where(gr => gr.Count() > 1).ToList().Count > 0)
                    {
                        response.Message = "Looks like duplicate Employee ID's added in the file, please verify and try again";
                        stream.Close();
                        return response;
                    }
                    try
                    {
                        string empExist; // Check employee existance in the database before inserting duplicate records to db.
                        for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
                        {
                            BulkUploadParameter rows = new BulkUploadParameter();
                            rows.EmployeeID = dataSet.Tables[0].Rows[i].ItemArray[0] != null ? Convert.ToString(dataSet.Tables[0].Rows[i].ItemArray[0]) : "-1";

                            if (!string.IsNullOrEmpty(rows.EmployeeID))
                            {
                                var exist = _context.Employees.Any(e => e.EmployeeID == rows.EmployeeID);
                                if (exist)
                                {
                                    empExist = rows.EmployeeID;
                                    response.Message = "Employee ID " + empExist + " is already exist in the database. Duplicate Employee ID cannot be added.";
                                    stream.Close();
                                    return response;
                                }
                            }

                            rows.Name = dataSet.Tables[0].Rows[i].ItemArray[1] != null ? Convert.ToString(dataSet.Tables[0].Rows[i].ItemArray[1]) : "-1";

                            rows.Phone = dataSet.Tables[0].Rows[i].ItemArray[2] != null ? Convert.ToString(dataSet.Tables[0].Rows[i].ItemArray[2]) : "-1";
                            if (!Validator.IsPhoneNumber(rows.Phone))
                            {
                                response.Message = "Phone number column contains invalid entries in file  : " + rows.Phone + ". Phone column must contain only Numeric values";
                                return response;
                            }

                            rows.EmailID = dataSet.Tables[0].Rows[i].ItemArray[3] != null ? Convert.ToString(dataSet.Tables[0].Rows[i].ItemArray[3]) : "-1";
                            if (!Validator.ValidateEmail(rows.EmailID))
                            {
                                response.Message = "Email format for Employee ID : " + rows.EmployeeID + "is not in the correct format";
                            }

                            Parameters.Add(rows);
                        }
                    }
                    catch (Exception ex)
                    {
                        response.Message = ex.Message;
                        ErrorHandlerMiddleware.ErrorLogging(ex);
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

                        foreach (BulkUploadParameter rows in Parameters)
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
                            response.Message = "Data insurted successfully";
                        }
                    }
                }
                else if (request.File.FileName.ToLower().Contains(".csv"))
                {
                    DataTable value = new DataTable();
                    try
                    {
                        var csvReader = new CsvReader(new StreamReader(File.OpenRead(path)), true);
                        if (csvReader != null)
                        {
                            value.Load(csvReader);
                        }

                        // We need one more condition, because reader columns are not immediatelly affected
                        if (csvReader != null)
                        {
                            if ((csvReader.Columns.Count != 4) ||
                                csvReader.Columns[0].Name != "EmployeeID" ||
                                csvReader.Columns[1].Name != "Name" ||
                                csvReader.Columns[2].Name != "Phone" ||
                                csvReader.Columns[3].Name != "EmailID")
                            {
                                response.Message = "File columns did not match. Ensure that there are only 4 columns, and added as following : |EmployeeID|Name|Phone|EmailID|";
                                csvReader.Dispose();
                                return response;
                            }
                        }
                        if (csvReader != null) { csvReader.Dispose(); }
                        // Check duplicate records in .csv file
                        if (value.AsEnumerable().GroupBy(r => r["EmployeeID"]).Where(gr => gr.Count() > 1).ToList().Count > 0)
                        {
                            response.Message = "Looks like duplicate Employee ID's added in the file, please verify and try again";
                            return response;
                        }

                        try
                        {
                            string empExist; // Check employee existance in the database before inserting duplicate records.
                            for (int i = 0; i < value.Rows.Count; i++)
                            {
                                BulkUploadParameter readData = new BulkUploadParameter();
                                readData.EmployeeID = value.Rows[i][0] != null ? Convert.ToString(value.Rows[i]["EmployeeID"]) : "-1";

                                if (!string.IsNullOrEmpty(readData.EmployeeID))
                                {
                                    if (_context.Employees.Any(e => e.EmployeeID == readData.EmployeeID))
                                    {
                                        empExist = readData.EmployeeID;
                                        response.Message = "Employee ID " + empExist + " is already exist in the database. Duplicate Employee ID cannot be added.";
                                        return response;
                                    }
                                }

                                readData.Name = value.Rows[i][1] != null ? Convert.ToString(value.Rows[i]["Name"]) : "-1";

                                readData.Phone = value.Rows[i][2] != null ? Convert.ToString(value.Rows[i]["Phone"]) : "-1";
                                if (!Validator.IsPhoneNumber(readData.Phone))
                                {
                                    response.Message = "Phone number contains invalid entries for Employee ID  : " + readData.EmployeeID + ". Phone column must contain only Numeric values";
                                    return response;
                                }

                                readData.EmailID = value.Rows[i][3] != null ? Convert.ToString(value.Rows[i]["EmailID"]) : "-1";
                                if (!Validator.ValidateEmail(readData.EmailID))
                                {
                                    response.Message = "Email format for Employee ID : " + readData.EmployeeID + "is not in the correct format";
                                }

                                Parameters.Add(readData);
                            }
                        }
                        catch (Exception ex)
                        {
                            response.Message = ex.Message;
                            ErrorHandlerMiddleware.ErrorLogging(ex);
                        }

                        if (Parameters.Count > 0)
                        {
                            if (ConnectionState.Open != _sqlConnection.State)
                            {
                                await _sqlConnection.OpenAsync();
                            }

                            foreach (BulkUploadParameter rows in Parameters)
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
                                response.Message = "Data insurted successfully";
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        response.Message = ex.Message;
                        ErrorHandlerMiddleware.ErrorLogging(ex);
                    }
                }
                else
                {
                    throw new AppException("Invalid File, Please select proper file format(Supports only : .xlsx or .csv).");
                }
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
                ErrorHandlerMiddleware.ErrorLogging(ex);
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
