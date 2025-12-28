using AutoMapper;
using Domain.Commands.v1.Jogos.AtualizarJogo;
using Domain.Commands.v1.Jogos.BuscarJogoPorId;
using Domain.Commands.v1.Jogos.CriarJogo;
using Domain.Commands.v1.Jogos.JogosPopulares;
using Domain.Commands.v1.Jogos.ListarJogos;
using Infrastructure.Data.Models.Jogos;

namespace Domain.MapperProfiles
{
    public class JogoProfile : Profile
    {
        public JogoProfile()
        {
            CreateMap<JogoESDocumentoModel, ListarJogoCommandResponse>();
            CreateMap<JogoESDocumentoModel, AtualizarJogoCommandResponse>();
            CreateMap<JogoESDocumentoModel, CriarJogoCommandResponse>();
            CreateMap<JogoModel, BuscarJogoPorIdCommandResponse>();
            CreateMap<CriarJogoCommand, JogoESDocumentoModel>();
            CreateMap<JogoESDocumentoModel, BuscarJogoPorIdCommandResponse>();
            CreateMap<JogoESDocumentoModel, JogosPopularesCommandResponse>();

        }
    }
}
