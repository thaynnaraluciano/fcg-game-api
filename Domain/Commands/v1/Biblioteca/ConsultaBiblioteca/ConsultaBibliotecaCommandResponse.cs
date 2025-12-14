namespace Domain.Commands.v1.Biblioteca.ConsultaBiblioteca
{
    public class ConsultaBibliotecaCommandResponse
    {
        public Guid Id { get; set; } // Id do jogo
        public string? Nome { get; set; }
        public string? Descricao { get; set; }
        public DateTime DataLancamento { get; set; }

        // Dados da compra
        public DateTime DtAdquirido { get; set; }
        public decimal PrecoOriginal { get; set; }
        public decimal PrecoFinal { get; set; }
    }
}
