using Domain.Enums;

namespace Domain.Commands.v1.Jogos.BuscarJogoPorId
{
    public class BuscarJogoPorIdCommandResponse
    {
        public string? Id { get; set; }
        public string? Nome { get; set; }
        public string? Descricao { get; set; }
        public decimal Preco { get; set; }
        public DateTime DataLancamento { get; set; }
        public int qtdPesquisas { get; set; }
        public TipoJogosEnum tipoJogo { get; set; }
    }
}
