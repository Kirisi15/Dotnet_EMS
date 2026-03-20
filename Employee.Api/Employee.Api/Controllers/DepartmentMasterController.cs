using Employee.Api.Model;
using Microsoft.AspNetCore.Mvc;

namespace Employee.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentMasterController : ControllerBase
    {
        private readonly EmployeeDbContext _context;

        public DepartmentMasterController(EmployeeDbContext context)
        {
            _context = context;
        }

        [HttpGet("GetAllDepartments")]
        public IActionResult GetAllDepartments()
        {
            var depList = _context.Departments.ToList();
            return Ok(depList);
        }

        [HttpPost("AddDepartment")]
        public IActionResult AddDepartment([FromBody] Department department)
        {
            _context.Departments.Add(department);
            _context.SaveChanges();
            return Ok("Department added successfully");
        }

        [HttpPut("UpdateDepartment")]
        public IActionResult UpdateDepartment([FromBody] Department department)
        {
            var dep = _context.Departments.FirstOrDefault(x => x.departmentId == department.departmentId);
            if (dep != null)
            {
                dep.departmentName = department.departmentName;
                dep.isActive = department.isActive;
                _context.SaveChanges();
                return Ok("Department updated successfully");
            }
            else
            {
                return NotFound("Department not found");
            }
        }
        [HttpDelete("DeleteDepartment/{id}")]
        public IActionResult DeleteDepartment(int id)
        {
            var dep = _context.Departments.FirstOrDefault(x => x.departmentId == id);
            if (dep != null)
            {
                _context.Departments.Remove(dep);
                _context.SaveChanges();
                return Ok("Department deleted successfully");
            }
            else
            {
                return NotFound("Department not found");
            }

        }
    }
}
