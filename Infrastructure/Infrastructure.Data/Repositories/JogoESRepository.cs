using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.Nodes;
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
                        DataLancamento = doc.DataLancamento,
                        qtdPesquisas = doc.qtdPesquisas,
                        tipoJogo=doc.tipoJogo
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

                await AtualizaPesquisa(id);
                JogoESDocumentoModel jogo = new JogoESDocumentoModel
                {
                    Id = IdList,
                    DataLancamento = response.Source.DataLancamento,
                    Descricao = response.Source.Descricao,
                    Preco = response.Source.Preco,
                    Nome = response.Source.Nome,
                    qtdPesquisas=response.Source.qtdPesquisas,
                    tipoJogo = response.Source.tipoJogo 
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

        public async Task<bool> AtualizaPesquisa(string id)
        {
            try
            {
                var response = await _client.UpdateAsync<JogoESModel, object>("games", id, u => u
                    .Script(s => s
                        .Source("ctx._source.qtdPesquisas = (ctx._source.qtdPesquisas ?: 0) + 1")
                    )
                );
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

        [Obsolete]
        public async Task<List<JogoESDocumentoModel>> BuscarPopularesAsync(int qtd, int? tipoJogo)
        {
            try
            {
                SearchResponse<JogoESModel> response;
                if (tipoJogo>0)
                {
                    response = await _client.SearchAsync<JogoESModel>(s => s
                        .Index("games")
                        .Size(qtd)
                        .Query(q => q
                            .Bool(b => b
                                .Filter(f => f
                                    .Term(t => t
                                        .Field(p => p.tipoJogo)
                                        .Value((FieldValue)tipoJogo)
                                    )
                                )
                            )
                        )
                        .Sort(sort => sort
                            .Field(f => f
                                .Field(p => p.qtdPesquisas)
                                .Order(SortOrder.Desc)
                            )
                        )
                    );
                }
                else
                {
                     response = await _client.SearchAsync<JogoESModel>(s => s
                        .Index("games")
                        .Size(qtd)
                        .Sort(sort => sort
                            .Field(f => f
                                .Field(p => p.qtdPesquisas)
                                .Order(SortOrder.Desc)
                            )
                        )
                    );
                }

                if (!response.IsValidResponse)
                {
                    Console.WriteLine($"❌ Erro:  {response.ApiCallDetails?.OriginalException?.Message}");
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
                        DataLancamento = doc.DataLancamento,
                        qtdPesquisas = doc.qtdPesquisas,
                        tipoJogo=doc.tipoJogo
                    };
                }).ToList();

                return jogos;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Exceção: {ex.Message}");
                return new List<JogoESDocumentoModel>();
            }
        }
        [Obsolete]
        public async Task<List<JogoESDocumentoModel>> BuscaSugestoesUserAsync(string idUser)
        {
            var resultado = new Dictionary<string, int>();
            try
            {
                var response = await _client.SearchAsync<JogosHistoricoModel>(s => s
                    .Index("user_game_history")
                    .Size(0)
                    .Query(q => q
                    .Term(t => t
                        .Field("userId.keyword")
                        .Value(idUser.ToString())
                    )
                ).Aggregations(aggs => aggs
                    .Add("group_por_tipo", a => a
                        .Terms(t => t
                            .Field("TipoJogo.keyword")
                        )
                    )                
                )
                );

                if (!response.IsValidResponse)
                {
                    Console.WriteLine($"Erro:  {response.ApiCallDetails.OriginalException?.Message}");
                    return new List<JogoESDocumentoModel>();
                }
                var buckets = response.Aggregations
                        .GetStringTerms("group_por_tipo")
                        .Buckets;
                              //SE N RETORNAR NADA, VOLTA VAZIO
                if (buckets.Count == 0)
                {
                    return new List<JogoESDocumentoModel>();
                }
                foreach (var bucket in buckets)
                {
                    resultado[bucket.Key.ToString()] = Convert.ToInt16(bucket.DocCount);
                }
                //Oredena o historico daquele user por tipo de jogo mais acessado
                var tiposOrdenados = resultado.OrderByDescending(x => x.Value);

                // Check if we have any ordered results
                if (!tiposOrdenados.Any())
                {
                    return new List<JogoESDocumentoModel>();
                }

                TipoJogosEnum? tipoJogo = null;
                if (Enum.TryParse<TipoJogosEnum>(tiposOrdenados.First().Key, out var tipoEnum))
                {
                    tipoJogo = tipoEnum;
                }
                if (tipoJogo.HasValue)
                {
                    //Busca 5 sugestoes baseado no tipo de jogo mais acessado pelo usuario
                    List<JogoESDocumentoModel> jogosSugeridos = await BuscarPopularesAsync(5, Convert.ToInt16(tipoJogo.Value));
                    return jogosSugeridos;
                }

                return new List<JogoESDocumentoModel>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao buscar games: {ex.Message}");
                return new List<JogoESDocumentoModel>();
            }
        }
    }
}
