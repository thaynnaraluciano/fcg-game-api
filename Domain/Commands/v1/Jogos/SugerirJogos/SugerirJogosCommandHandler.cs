using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Infrastructure.Data.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Domain.Commands.v1.Jogos.SugerirJogos
{
    public class SugerirJogosCommandHandler: IRequestHandler<SugerirJogosCommand, IEnumerable<SugerirJogosCommandResponse>>
    {
        private readonly IJogoESRepository _jogoRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<SugerirJogosCommandHandler> _logger;

        public SugerirJogosCommandHandler(IJogoESRepository jogoRepository, IMapper mapper, ILogger<SugerirJogosCommandHandler> logger)
        {
            _jogoRepository = jogoRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<SugerirJogosCommandResponse>> Handle(SugerirJogosCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Listando jogos");
            var jogos = await _jogoRepository.BuscaSugestoesUserAsync(request.GuidUsuario);
            return _mapper.Map<IEnumerable<SugerirJogosCommandResponse>>(jogos);
        }
    }
 
}