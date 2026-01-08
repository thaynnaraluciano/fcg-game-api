using AutoMapper;
using Infrastructure.Data.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Domain.Commands.v1.Jogos.BuscarJogoPorId
{
    public class BuscarJogoPorIdCommandHandler : IRequestHandler<BuscarJogoPorIdCommand, BuscarJogoPorIdCommandResponse>
    {
        private readonly IJogoESRepository _jogoRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<BuscarJogoPorIdCommandHandler> _logger;

        public BuscarJogoPorIdCommandHandler(IJogoESRepository jogoRepository, IMapper mapper, ILogger<BuscarJogoPorIdCommandHandler> logger)
        {
            _jogoRepository = jogoRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<BuscarJogoPorIdCommandResponse> Handle(BuscarJogoPorIdCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Consultando jogo {request.Id}");

            var jogo = await _jogoRepository.BuscarPorIdAsync(request.Id.ToString());

            if (jogo == null)
            {
                _logger.LogError($"Jogo com ID {request.Id} não encontrado.");
                throw new KeyNotFoundException($"Jogo com ID {request.Id} não encontrado.");
            }
            _logger.LogInformation($"Jogo {request.Id} encontrado");
            return _mapper.Map<BuscarJogoPorIdCommandResponse>(jogo);
        }
    }
}
