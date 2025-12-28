using Elastic.Clients.Elasticsearch;
using Infrastructure.Data.Models.Jogos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data.Interfaces
{
    public interface IJogoESRepository
    {
        Task<List<JogoESDocumentoModel>> BuscarTodosAsync();
        Task<JogoESDocumentoModel?> BuscarPorIdAsync(string id);
        Task<bool> CriarAsync(JogoESDocumentoModel game);
        Task<bool> AtualizarAsync(string id, JogoESModel game);
        Task<bool> RemoverAsync(string id);
        Task<List<JogoESDocumentoModel>> BuscarPopularesAsync(int qtd, int? tipoJogo);
    }
}
