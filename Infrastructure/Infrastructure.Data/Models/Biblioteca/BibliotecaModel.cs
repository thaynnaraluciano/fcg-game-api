using Infrastructure.Data.Models.Jogos;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.Data.Models.Biblioteca
{
    public class BibliotecaModel
    {
        public Guid Id { get; set; }
        public Guid IdJogo { get; set; }
        public Guid IdUsuario { get; set; }
        public DateTime DtAdquirido { get; set; }
        public decimal PrecoOriginal { get; set; }
        public decimal PrecoFinal { get; set; }

        [ForeignKey(nameof(IdJogo))]
        public JogoModel Jogo { get; set; }

    }
}
