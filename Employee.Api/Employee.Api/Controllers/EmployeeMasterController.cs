using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Employee.Api.Model;

namespace Employee.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly EmployeeDbContext _context;

        public EmployeeController(EmployeeDbContext context)
        {
            _context = context;
        }

        // ✅ NORMAL GET: api/employee
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var data = await _context.Employees.ToListAsync();
                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // ✅ GET BY ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var emp = await _context.Employees.FindAsync(id);

                if (emp == null)
                    return NotFound("Employee not found");

                return Ok(emp);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // ✅ FILTER + SORT + PAGINATION
        // api/employee/search?name=abc&page=1&pageSize=5&sortBy=name&sortDir=asc
        [HttpGet("search")]
        public async Task<IActionResult> Search(
            string? name,
            string? city,
            int page = 1,
            int pageSize = 5,
            string sortBy = "employeeId",
            string sortDir = "asc"
        )
        {
            try
            {
                var query = _context.Employees.AsQueryable();

                // 🔍 FILTER
                if (!string.IsNullOrEmpty(name))
                    query = query.Where(e => e.name.Contains(name));

                if (!string.IsNullOrEmpty(city))
                    query = query.Where(e => e.city.Contains(city));

                // 🔃 SORT
                switch (sortBy.ToLower())
                {
                    case "name":
                        query = sortDir == "asc"
                            ? query.OrderBy(e => e.name)
                            : query.OrderByDescending(e => e.name);
                        break;

                    case "createddate":
                        query = sortDir == "asc"
                            ? query.OrderBy(e => e.createdDate)
                            : query.OrderByDescending(e => e.createdDate);
                        break;

                    default:
                        query = query.OrderBy(e => e.employeeId);
                        break;
                }

                // 📄 PAGINATION
                var totalRecords = await query.CountAsync();

                var data = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                return Ok(new
                {
                    totalRecords,
                    page,
                    pageSize,
                    data
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // ✅ CREATE
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] EmployeeModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                // 🔒 UNIQUE CHECK
                var exists = await _context.Employees.AnyAsync(e =>
                    e.email == model.email || e.contactNo == model.contactNo);

                if (exists)
                    return BadRequest("Email or Contact Number already exists");

                model.createdDate = DateTime.Now;
                model.modifiedDate = DateTime.Now;

                await _context.Employees.AddAsync(model);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetById), new { id = model.employeeId }, model);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // ✅ UPDATE
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] EmployeeModel model)
        {
            try
            {
                if (id != model.employeeId)
                    return BadRequest("ID mismatch");

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var existing = await _context.Employees.FindAsync(id);

                if (existing == null)
                    return NotFound("Employee not found");

                // 🔒 UNIQUE CHECK (exclude current record)
                var exists = await _context.Employees.AnyAsync(e =>
                    (e.email == model.email || e.contactNo == model.contactNo)
                    && e.employeeId != id);

                if (exists)
                    return BadRequest("Email or Contact Number already exists");

                // ✏️ UPDATE FIELDS
                existing.name = model.name;
                existing.contactNo = model.contactNo;
                existing.email = model.email;
                existing.city = model.city;
                existing.state = model.state;
                existing.pincode = model.pincode;
                existing.altContactNo = model.altContactNo;
                existing.desinationName = model.desinationName;
                existing.address = model.address;
                existing.designationId = model.designationId;
                existing.modifiedDate = DateTime.Now;

                _context.Employees.Update(existing);
                await _context.SaveChangesAsync();

                return Ok(existing);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // ✅ DELETE
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var emp = await _context.Employees.FindAsync(id);

                if (emp == null)
                    return NotFound("Employee not found");

                _context.Employees.Remove(emp);
                await _context.SaveChangesAsync();

                return Ok("Deleted successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var user = await _context.Employees
                    .FirstOrDefaultAsync(e => e.email == model.email && e.contactNo == model.contactNo);

                if (user == null)
                    return Unauthorized("Invalid email or password");

                if (user.contactNo != model.contactNo)
                    return Unauthorized("Invalid email or password");

                return Ok(new
                {
                    message = "Login successful",
                    data = new
                    {
                        user.employeeId,
                        user.name,
                        user.email,
                        user.contactNo,
                        user.designationId,
                        user.desinationName,
                        user.role
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}