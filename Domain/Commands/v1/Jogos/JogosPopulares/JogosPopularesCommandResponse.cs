using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Commands.v1.Jogos.JogosPopulares
{
    public class JogosPopularesCommandResponse
    {
        public Guid Id { get; set; }
        public string? nome { get; set; }
        public string? descricao { get; set; }
        public decimal preco { get; set; }
        public DateTime dataLancamento { get; set; }
        public int qtdPesquisas { get; set; }
        public TipoJogosEnum tipoJogo { get; set; }

    }
}
