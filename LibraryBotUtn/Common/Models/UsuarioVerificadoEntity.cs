using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryBotUtn.Common.Models
{
    public class UsuarioVerificadoEntity
    {
        public TokenEntity newToken { get; set; }
        public UsuarioEntity usuario { get; set; }
        public static UsuarioVerificadoEntity fromJson(string json) => JsonConvert.DeserializeObject<UsuarioVerificadoEntity>(json);

    }
}
