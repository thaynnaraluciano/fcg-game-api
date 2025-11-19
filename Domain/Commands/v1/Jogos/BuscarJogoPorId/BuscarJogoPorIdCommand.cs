using MediatR;

namespace Domain.Commands.v1.Jogos.BuscarJogoPorId
{
    public class BuscarJogoPorIdCommand : IRequest<BuscarJogoPorIdCommandResponse>
    {
        public Guid Id { get; set; }

        public BuscarJogoPorIdCommand(Guid id)
        {
            Id = id;
        }
    }
}
