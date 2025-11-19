using MediatR;

namespace Domain.Commands.v1.Biblioteca.ConsultaBiblioteca
{
    public class ConsultaBibliotecaCommand : IRequest<IEnumerable<ConsultaBibliotecaCommandResponse>>
    {
        public Guid IdUsuario { get; set; }
        public ConsultaBibliotecaCommand() { }
        public ConsultaBibliotecaCommand(Guid idUser)
        {
            IdUsuario = idUser;
        }
    }
}
