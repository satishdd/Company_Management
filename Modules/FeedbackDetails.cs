using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Company_Management.Modules
{
    public class FeedbackDetails
    {
        [Key]
        public int Id { get; set; }

        //[ForeignKey("FK_Employees")]
        //public int EmpTable_Id { get; set; }
        public string EmployeeID { get; set; }

        public string Month { get; set; }

        public string? FeedbackFromEmployee { get; set; } = "";

        public string? FeedbackToEmployee { get; set; } = "";

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? CreatedDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? UpdatedDate { get; set; }
    }
}
