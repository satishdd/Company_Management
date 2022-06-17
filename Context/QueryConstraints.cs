using Company_Management.Exceptions;
using Company_Management.Modules;

namespace Company_Management.Context
{
    public class QueryConstraints
    {
        private readonly DataContext db;

        public QueryConstraints(DataContext dbContext)
        {
            db = dbContext;
        }

        public async Task<List<Employee>> PicEmployees(string date, int numberOfEmployees)
        {
            var checkMonth = db.FeedbackDetails.Where(e => e.Month == date);
            if (checkMonth.Any())
            {
                var _date = Convert.ToDateTime(date);
                throw new AppException("Records found for the month of " + _date.ToString("MM/yyyy") + ". Please delete the records that are available in this month and try again");
            }
            else
            {
                return (from emp in db.Employees
                        where !db.FeedbackDetails.Any(f => f.EmployeeID == emp.EmployeeID)
                        select emp).Take(numberOfEmployees).ToList();
            }
        }
    }
}
