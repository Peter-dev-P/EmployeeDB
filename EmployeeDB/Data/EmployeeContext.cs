using Microsoft.EntityFrameworkCore;

namespace EmployeeDB.Data
{
    public class EmployeeContext : DbContext
    {
        // Constructor que recibe las opciones de configuración del contexto
        public EmployeeContext(DbContextOptions<EmployeeContext> options) : base(options)
        {
        }

       

        // Tabla de empleados
        public DbSet<Employee> Employees { get; set; }

        // Configuración de las reglas del modelo
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Restricción para rfc unico
            modelBuilder.Entity<Employee>()
                .HasIndex(e => e.RFC)
                .IsUnique(); // RFC unico en base datos no debe repetirse
        }
    }
}
