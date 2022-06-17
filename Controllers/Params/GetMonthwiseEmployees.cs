using System.ComponentModel.DataAnnotations;

namespace Company_Management.Controllers.Params
{
    public class PicRandomEmployees
    {
        [Required]
        [DataType(DataType.DateTime)]
        public DateTime Date { get; set; }

        public int NumberOfEmployees { get; set; }
    }
}
