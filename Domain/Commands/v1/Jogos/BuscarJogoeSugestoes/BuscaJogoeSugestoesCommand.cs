using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;

namespace Domain.Commands.v1.Jogos.BuscarJogoeSugestoes
{
    public class BuscaJogoeSugestoesCommand : IRequest<IEnumerable<BuscaJogoeSugestoesCommandResponse>>
    {
        public Guid Id { get; set; }
        public BuscaJogoeSugestoesCommand(Guid id)
        {
            Id = id;
        }
    }
}