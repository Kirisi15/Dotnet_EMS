using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Employee.Api.Model;

namespace Employee.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DesignationController : ControllerBase
    {
        private readonly EmployeeDbContext _context;

        public DesignationController(EmployeeDbContext context)
        {
            _context = context;
        }

        // ✅ GET: api/designation
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var data = await _context.Designations.ToListAsync();
                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }

        // ✅ GET: api/designation/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var designation = await _context.Designations.FindAsync(id);

                if (designation == null)
                    return NotFound("Designation not found");

                return Ok(designation);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }

        // ✅ FILTER: api/designation/by-department/2
        [HttpGet("by-department/{departmentId}")]
        public async Task<IActionResult> GetByDepartment(int departmentId)
        {
            try
            {
                var data = await _context.Designations
                    .Where(d => d.departmentId == departmentId)
                    .ToListAsync();

                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }

        // ✅ POST: api/designation
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Designation model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                await _context.Designations.AddAsync(model);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetById), new { id = model.designationId }, model);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }

        // ✅ PUT: api/designation/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Designation model)
        {
            try
            {
                if (id != model.designationId)
                    return BadRequest("ID mismatch");

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var existing = await _context.Designations.FindAsync(id);

                if (existing == null)
                    return NotFound("Designation not found");

                // Update fields
                existing.designationName = model.designationName;
                existing.departmentId = model.departmentId;

                _context.Designations.Update(existing);
                await _context.SaveChangesAsync();

                return Ok(existing);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }

        // ✅ DELETE: api/designation/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var designation = await _context.Designations.FindAsync(id);

                if (designation == null)
                    return NotFound("Designation not found");

                _context.Designations.Remove(designation);
                await _context.SaveChangesAsync();

                return Ok("Deleted successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }
    }
}