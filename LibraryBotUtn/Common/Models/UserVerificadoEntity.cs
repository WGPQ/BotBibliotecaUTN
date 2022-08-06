using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryBotUtn.Common.Models
{
    public class UserVerificadoEntity
    {
        public string token { get; set; }
        public UsuarioEntity usuario { get; set; }
        public static UserVerificadoEntity fromJson(string json) => JsonConvert.DeserializeObject<UserVerificadoEntity>(json);

    }
}
