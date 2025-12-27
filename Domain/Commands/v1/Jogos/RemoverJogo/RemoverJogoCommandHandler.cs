using Infrastructure.Data.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Domain.Commands.v1.Jogos.RemoverJogo
{
    public class RemoverJogoCommandHandler : IRequestHandler<RemoverJogoCommand, Unit>
    {
        private readonly IJogoESRepository _jogoRepository;
        private readonly ILogger<RemoverJogoCommandHandler> _logger;

        public RemoverJogoCommandHandler(IJogoESRepository jogoRepository, ILogger<RemoverJogoCommandHandler> logger)
        {
            _jogoRepository = jogoRepository;
            _logger = logger;
        }

        public async Task<Unit> Handle(RemoverJogoCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Removendo o jogo {request.Id}");
            var jogo = await _jogoRepository.BuscarPorIdAsync(request.Id.ToString());

            if (jogo == null)
            {
                _logger.LogError($"Jogo com ID {request.Id} não encontrado.");
                throw new Exception($"Jogo com ID {request.Id} não encontrado.");
            }

            if(await _jogoRepository.RemoverAsync(jogo.Id))
            {
                _logger.LogInformation($"Jogo {request.Id} removido");
                return Unit.Value;
            }
            else
            {
                _logger.LogInformation($"Erro ao excluir game id: {request.Id}");
                return Unit.Value;
            }

        }
    }
}
