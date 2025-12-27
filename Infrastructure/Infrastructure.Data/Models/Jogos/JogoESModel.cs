using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data.Models.Jogos
{
    public class JogoESModel
    {
        public string? Nome { get; set; }
        public string? Descricao { get; set; }
        public decimal Preco { get; set; }
        public DateTime? DataLancamento { get; set; }
        public void Atualizar(string? nome, string? descricao, decimal? preco, DateTime? dataLancamento)
        {
            if (!string.IsNullOrEmpty(nome))
                Nome = nome!;

            if (!string.IsNullOrEmpty(descricao))
                Descricao = descricao!;

            if (preco.HasValue && preco.Value > 0)
                Preco = preco.Value;

            if (dataLancamento != null)
                DataLancamento = dataLancamento.Value;
        }

    }
}
