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

        public List<FeedbackDetails> PicEmployees(string date, int numberOfEmployees)
        {
            //var query = (from emp in db.Employees
            //             join feedback in db.FeedbackDetails on emp.EmployeeID equals feedback.EmployeeID
            //             where feedback.FeedbackToEmployee == null &&
            //                     feedback.FeedbackFromEmployee == null &&
            //                     feedback.UpdatedDate == null &&
            //                     feedback.Month == null
            //             select new FeedbackDetails
            //             {
            //                 EmployeeID = emp.EmployeeID,
            //                 Month = date,
            //                 FeedbackFromEmployee = null,
            //                 FeedbackToEmployee = null,
            //                 CreatedDate = DateTime.Today,
            //                 UpdatedDate = null,
            //             }).Take(numberOfEmployees).ToList();

            //return query;
            Response response = new Response();
            var checkMonth = db.FeedbackDetails.Where(e => e.Month == date);
            if (checkMonth.Any())
            {
                throw new AppException("Records found for this" + date + "month. Please delete the records that are available in this month and try again");
            }
            else
            {
                //var feedbackData = db.FeedbackDetails.ToList();

                //var l = feedbackData.Select(l => l.EmployeeID).ToList();


                //var name = from emp in db.Employees
                //           where r.ClientCode == Convert.ToString(feedbackData.[l].Value)
                //           select r.IsInternalClient;



                //var randomEmployees = db.Employees.Where(e => e.EmployeeID != l);

                var query = (from emp in db.Employees
                             select new FeedbackDetails
                             {
                                 EmployeeID = emp.EmployeeID,
                                 Month = date,
                                 FeedbackFromEmployee = null,
                                 FeedbackToEmployee = null,
                                 CreatedDate = DateTime.Today,
                                 UpdatedDate = null,
                             }).Take(numberOfEmployees).ToList();

                return query;
            }
        }
    }
}
