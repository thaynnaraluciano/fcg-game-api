namespace Domain.Commands.v1.Jogos.ListarJogos
{
    public class ListarJogoCommandResponse
    {
        public Guid Id { get; set; }
        public string? nome { get; set; }
        public string? descricao { get; set; }
        public decimal preco { get; set; }
        public DateTime dataLancamento { get; set; }
    }
}
