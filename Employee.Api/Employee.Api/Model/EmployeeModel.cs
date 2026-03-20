using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Employee.Api.Model
{
    [Table("employeeTbl")]
    public class EmployeeModel
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int employeeId { get; set; }
        [Required, MaxLength(50)]
        public string name { get; set; } = string.Empty;
        public string contactNo { get; set; } = string.Empty;
        public string email { get; set; } = string.Empty;
        public string city { get; set; } = string.Empty;
        public string state { get; set; } = string.Empty;
        public string pincode { get; set; } = string.Empty;
        public string altContactNo { get; set; } = string.Empty;
        public string desinationName { get; set; } = string.Empty;
        public string address { get; set; } = string.Empty;
        public int designationId { get; set; } 
        public DateTime createdDate { get; set; }
        public DateTime modifiedDate { get; set; }
    }
}
