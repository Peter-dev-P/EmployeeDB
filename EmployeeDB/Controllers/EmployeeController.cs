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

        // Crea a un empleado
        [HttpPost]
        public async Task<IActionResult> AddEmployee([FromBody] Employee employee)
        {
            // Validación del RFC
            var validator = new EmployeeValidator();
            var validationResult = validator.Validate(employee);
            if (!validationResult.IsValid) // RFC no válido
            {
                return BadRequest(validationResult.Errors);
            }

            employee.BornDate = employee.BornDate.Date;

            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetEmployee), new { id = employee.ID }, employee);
        }

        // Obtener todos los empleados ordenados por fecha de nacimiento, filtrados opcionalmente por nombre
        [HttpGet]
        public async Task<IActionResult> GetEmployees(string name = "")
        {
            var employees = await _context.Employees
                .Where(e => string.IsNullOrEmpty(name) || e.Name.Contains(name))
                .OrderBy(e => e.BornDate)
                .ToListAsync();

            return Ok(employees);
        }

        // Obtener a un empleado por ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetEmployee(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                return NotFound();
            }
            return Ok(employee);
        }

        // Actualizar a un empleado
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEmployee(int id, [FromBody] Employee employee)
        {
            if (id != employee.ID) // ID proporcionado no coincide con el de la BD
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
        [HttpDelete("{id}")]
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
