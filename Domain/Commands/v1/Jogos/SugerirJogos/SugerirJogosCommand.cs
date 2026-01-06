
using MediatR;

namespace Domain.Commands.v1.Jogos.SugerirJogos
{
    public class SugerirJogosCommand: IRequest<IEnumerable<SugerirJogosCommandResponse>>
    {
        public SugerirJogosCommand(string guidUsuario)
        {
            GuidUsuario = guidUsuario;
        }
        public string GuidUsuario { get; set; }= string.Empty;
    }
}