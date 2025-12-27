using Elastic.Clients.Elasticsearch;
using Infrastructure.Data.Interfaces;
using Infrastructure.Data.Models.Jogos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data.Repositories
{
    public class JogoESRepository : IJogoESRepository
    {

        private readonly ElasticsearchClient _client;

        public JogoESRepository(ElasticsearchClient client)
        {
            _client = client;
        }

        public async Task<List<JogoESDocumentoModel>> BuscarTodosAsync()
        
        {
            try
            {
                var response = await _client.SearchAsync<JogoESModel>(s => s
                    .Index("games")
                    .Query(q => q.MatchAll())
                    .Size(100)
                );

                if (!response.IsValidResponse)
                {
                    Console.WriteLine($"Erro:  {response.ApiCallDetails.OriginalException?.Message}");
                    return new List<JogoESDocumentoModel>();
                }
                var hitsList = response.Hits.ToList(); // Converter para lista

                var jogos = response.Documents.Select((doc, index) =>
                {
                    var hit = hitsList[index];
                    return new JogoESDocumentoModel
                    {
                        Id = hit.Id,
                        Nome = doc.Nome,
                        Descricao = doc.Descricao,
                        Preco = doc.Preco,
                        DataLancamento = doc.DataLancamento
                    };
                }).ToList();

                return jogos;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao buscar games: {ex.Message}");
                return new List<JogoESDocumentoModel>();
            }
        }

        public async Task<JogoESDocumentoModel?> BuscarPorIdAsync(string id)
        {
            try
            {
                var response = await _client.GetAsync<JogoESModel>(id);

                if (!response.IsValidResponse)
                {
                    Console.WriteLine($"Erro:  {response.ApiCallDetails.OriginalException?.Message}");
                    return new JogoESDocumentoModel();
                }
                var IdList = response.Id;

                JogoESDocumentoModel jogo = new JogoESDocumentoModel
                {
                    Id = IdList,
                    DataLancamento = response.Source.DataLancamento,
                    Descricao = response.Source.Descricao,
                    Preco = response.Source.Preco,
                    Nome = response.Source.Nome
                };
                return jogo;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao buscar games: {ex.Message}");
                return new JogoESDocumentoModel();
            }

        }

        public async Task<bool> CriarAsync(JogoESDocumentoModel game)
        {
            try
            {
                var response = await _client.IndexAsync(game, i => i
                    .Index("games")
                    .Id(game.Id)
                    .Refresh(Refresh.WaitFor)
                );

                if (!response.IsValidResponse)
                {
                    Console.WriteLine($"Erro:  {response.ApiCallDetails?.OriginalException?.Message}");
                    return false;
                }

                Console.WriteLine($"Jogo criado com ID: {response.Id}");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error:  {ex.Message}");
                return false;
            }
        }

        public async Task<bool> AtualizarAsync(string id, JogoESModel game)
        {
            try
            {
                var updateRequest = new UpdateRequest<JogoESModel, JogoESModel>("games", id)
                {
                    Doc = game,
                    Refresh = Refresh.WaitFor
                };

                var response = await _client.UpdateAsync(updateRequest); 
                if (!response.IsValidResponse)
                {
                    Console.WriteLine($"Erro ao atualizar:  {response.ApiCallDetails?.OriginalException?.Message}");
                    return false;
                }

                Console.WriteLine($"Jogo atualizado:  {id}");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error:  {ex.Message}");
                return false;
            }
        }

        public async Task<bool> RemoverAsync(string id)
        {
            try
            {
                var delete = new UpdateRequest<JogoESModel, JogoESModel>("games", id);
                var response = await _client.DeleteAsync(delete);
                if (!response.IsValidResponse)
                {
                    Console.WriteLine($"Erro ao excluir jogo:  {response.ApiCallDetails?.OriginalException?.Message}");
                    return false;
                }

                Console.WriteLine($"Jogo excluido com sucesso:  {id}");
                return true;

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error:  {ex.Message}");
                return false;
            }
        }
    }
}
