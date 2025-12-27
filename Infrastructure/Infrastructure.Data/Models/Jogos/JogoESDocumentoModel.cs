using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data.Models.Jogos
{
    public class JogoESDocumentoModel:JogoESModel
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
    }
}
