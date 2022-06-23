using Newtonsoft.Json;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Company_Management.Controllers.Params
{
    public class PicRandomEmployees
    {
        [Required]
        [DataType(DataType.DateTime)]
        public DateTime Date { get; set; }

        [DefaultValue(5)]
        public int NumberOfEmployees { get; set; }
    }
}
