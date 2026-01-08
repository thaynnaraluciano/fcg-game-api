

using AutoMapper;
using Infrastructure.Data.Interfaces;
using Infrastructure.Data.Models.Jogos;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Domain.Commands.v1.Jogos.BuscarJogoeSugestoes
{
    public class BuscaJogoeSugestoesCommandHandler : IRequestHandler<BuscaJogoeSugestoesCommand, IEnumerable<BuscaJogoeSugestoesCommandResponse>>
    {
        private readonly IJogoESRepository _jogoRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<BuscaJogoeSugestoesCommandHandler> _logger;
        public BuscaJogoeSugestoesCommandHandler(IJogoESRepository jogoRepository, IMapper mapper, ILogger<BuscaJogoeSugestoesCommandHandler> logger)
        {
            _jogoRepository = jogoRepository; 
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<BuscaJogoeSugestoesCommandResponse>> Handle(BuscaJogoeSugestoesCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Buscando sugestões de jogos para o jogo {JogoId}", 
                request.Id
            );

            var jogoPesquisado = await _jogoRepository.BuscarPorIdAsync(request.Id.ToString());

            if (jogoPesquisado is null)
            {
                _logger.LogWarning(
                    "Jogo com Id {JogoId} não encontrado", 
                    request.Id
                );

                return Enumerable.Empty<BuscaJogoeSugestoesCommandResponse>();
            }

            var jogosPopulares = await _jogoRepository.BuscarPopularesAsync(
                qtd: 4,
                tipoJogo: jogoPesquisado.tipoJogo
            );
            // Remove o jogo pesquisado da lista de populares (caso exista)
            var jogosPopularesFiltrados = jogosPopulares?
                .Where(j => j.Id != jogoPesquisado.Id)
                .ToList() 
                ?? new List<JogoESDocumentoModel>();
            var sugestoes = new List<JogoESDocumentoModel>
            {
                jogoPesquisado
            };

            if (jogosPopularesFiltrados?.Any() == true)
                sugestoes.AddRange(jogosPopularesFiltrados);

            return _mapper.Map<IEnumerable<BuscaJogoeSugestoesCommandResponse>>(sugestoes);
        }

    }
    
}