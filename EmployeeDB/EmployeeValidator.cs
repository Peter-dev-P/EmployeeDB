using FluentValidation;

namespace EmployeeDB
{
    public class EmployeeValidator : AbstractValidator<Employee>
    {
        public EmployeeValidator()
        {
            RuleFor(e => e.RFC)
                .Matches("^[A-Z]{4}[0-9]{6}[A-Z0-9]{3}$") //Expresión regular para validar el formato del rfc
                .WithMessage("El formato del RFC es inválido.");

            RuleFor(e => e.RFC)
                .Length(13)
                .WithMessage("El RFC debe tener 13 caracteres."); //Valida cantidad de caracteres del rfc
        }
    }
}
