using AutoMapper;
using Infrastructure.Data.Interfaces;
using Infrastructure.Data.Models.Jogos;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Domain.Commands.v1.Jogos.CriarJogo
{
    public class CriarJogoCommandHandler : IRequestHandler<CriarJogoCommand, CriarJogoCommandResponse>
    {
        private readonly IJogoESRepository _jogoRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<CriarJogoCommandHandler> _logger;

        public CriarJogoCommandHandler(IJogoESRepository jogoRepository, IMapper mapper, ILogger<CriarJogoCommandHandler> logger)
        {
            _jogoRepository = jogoRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<CriarJogoCommandResponse> Handle(CriarJogoCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Criando novo jogo");
            var jogo = _mapper.Map<JogoESDocumentoModel>(request);

            if (await _jogoRepository.CriarAsync(jogo)){
                _logger.LogInformation($"Jogo criado com sucesso");
                return _mapper.Map<CriarJogoCommandResponse>(jogo);
            }
            else
            {
                _logger.LogInformation($"Erro ao criar jogo");
                return _mapper.Map<CriarJogoCommandResponse>(jogo);
            }
        }
    }
}
