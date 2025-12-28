using Domain.Commands.v1.Jogos.AtualizarJogo;
using Domain.Enums;
using Elastic.Clients.Elasticsearch;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Commands.v1.Jogos.JogosPopulares
{
    public class JogosPopularesCommand: IRequest<IEnumerable<JogosPopularesCommandResponse>>
    {
        public JogosPopularesCommand(int qtd, TipoJogosEnum? tipo)
        {
            quantidade = qtd;
            if (tipo != null)
            {
                tipoJogo = (int)tipo;
            }
        }

        public int quantidade{ get; set; }
        public int tipoJogo { get; set; }
    }
}
