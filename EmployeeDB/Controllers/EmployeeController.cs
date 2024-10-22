namespace EmployeeDB.Controllers
{
    using EmployeeDB.Data;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    [ApiController]
    [Route("api/[controller]")]
    public class EmployeeController : ControllerBase
    {
        private readonly EmployeeContext _context;

        public EmployeeController(EmployeeContext context)
        {
            _context = context;
        }

        // Crea un empleado
        [HttpPost("Add-an-employee")]
        public async Task<IActionResult> AddEmployee(Employee employee)
        {
            // Validación de rfc
            var validator = new EmployeeValidator();
            var validationResult = validator.Validate(employee);
            if (!validationResult.IsValid)//rfc no valido
            {
                return BadRequest(validationResult.Errors);
            }

            employee.BornDate = employee.BornDate.Date;

            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetEmployee), new { id = employee.ID }, employee);
        }

        // Obtener empleados ordenados por fecha de nacimiento y filtrados opcionalmente por nombre
        [HttpGet("Employees ordered by birth date while also having the option to search by name")]
        public async Task<IActionResult> GetEmployees(string name = "")
        {
            var employees = _context.Employees
                .Where(e => string.IsNullOrEmpty(name) || e.Name.Contains(name))
                .OrderBy(e => e.BornDate);

            return Ok(await employees.ToListAsync());
        }

        // Obtener un empleado usando el ID
        [HttpGet("Search-employee using/{id}")]
        public async Task<IActionResult> GetEmployee(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                return NotFound();
            }
            return Ok(employee);
        }

        // Actualizar un empleado
        [HttpPut("update-an-employee/{id}")]
        public async Task<IActionResult> UpdateEmployee(int id, Employee employee)
        {
            if (id != employee.ID) //id proporcionado no coincide con BD
            {
                return BadRequest();
            }

            _context.Entry(employee).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EmployeeExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // Eliminar a un empleado
        [HttpDelete("delete-an-employee from the database/{id}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                return NotFound();
            }

            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool EmployeeExists(int id)
        {
            return _context.Employees.Any(e => e.ID == id);
        }
    }

}
