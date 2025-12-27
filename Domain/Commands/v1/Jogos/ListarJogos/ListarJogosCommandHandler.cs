using AutoMapper;
using Infrastructure.Data.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Domain.Commands.v1.Jogos.ListarJogos
{
    public class ListarJogosCommandHandler : IRequestHandler<ListarJogosCommand, IEnumerable<ListarJogoCommandResponse>>
    {
        private readonly IJogoESRepository _jogoRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<ListarJogosCommandHandler> _logger;

        public ListarJogosCommandHandler(IJogoESRepository jogoRepository, IMapper mapper, ILogger<ListarJogosCommandHandler> logger)
        {
            _jogoRepository = jogoRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<ListarJogoCommandResponse>> Handle(ListarJogosCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Listando jogos");
            var jogos = await _jogoRepository.BuscarTodosAsync();
            return _mapper.Map<IEnumerable<ListarJogoCommandResponse>>(jogos);
        }
    }
}
