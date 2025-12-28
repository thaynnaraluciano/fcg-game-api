using AutoMapper;
using Infrastructure.Data.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Domain.Commands.v1.Jogos.JogosPopulares
{
    public class JogosPopularesCommandHandler : IRequestHandler<JogosPopularesCommand, IEnumerable<JogosPopularesCommandResponse>>
    {
        private readonly IJogoESRepository _jogoRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<JogosPopularesCommandHandler> _logger;
        public JogosPopularesCommandHandler(IJogoESRepository jogoRepository, IMapper mapper, ILogger<JogosPopularesCommandHandler> logger)
        {
            _jogoRepository = jogoRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<JogosPopularesCommandResponse>> Handle(JogosPopularesCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Buscando jogos top  {request.quantidade} populares");

            var jogos = await _jogoRepository.BuscarPopularesAsync(request.quantidade, request.tipoJogo);

            return _mapper.Map<IEnumerable<JogosPopularesCommandResponse>>(jogos); ;
        }
    }
}
