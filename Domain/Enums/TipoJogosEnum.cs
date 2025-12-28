using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enums
{
    public enum TipoJogosEnum
    {
        [Description("Ação")]
        Acao = 1,
        [Description("Aventura")]
        Aventura = 2,
        [Description("RPG")]
        RPG = 3,
        [Description("Estratégia")]
        Estrategia = 4,
        [Description("Simulação")]
        Simulacao = 5,
        [Description("Esporte")]
        Esporte = 6,
        [Description("Corrida")]
        Corrida = 7,
        [Description("Luta")]
        Luta = 8,
        [Description("Terror")]
        Terror = 9,
        [Description("Plataforma")]
        Plataforma = 10,
        [Description("Puzzle")]
        Puzzle = 11,
        [Description("MMO")]
        MMO = 12
    }
}
