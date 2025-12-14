using FluentValidation;

namespace Domain.Commands.v1.Biblioteca.ConsultaBiblioteca
{
    public class ConsultaBibliotecaCommandValidator : AbstractValidator<ConsultaBibliotecaCommand>
    {
        public ConsultaBibliotecaCommandValidator()
        {
            RuleFor(command => command.IdUsuario)
            .NotNull().WithMessage("Usuário não pode ser nulo")
            .Must(id => id != Guid.Empty).WithMessage("O ID do usuário não pode ser um GUID vazio.");
        }
    }
}
