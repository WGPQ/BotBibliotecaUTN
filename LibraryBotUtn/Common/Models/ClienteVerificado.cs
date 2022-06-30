using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryBotUtn.Common.Models
{
    public class ClienteVerificado
    {
        public string newToken { get; set; }
        public ClienteEntity cliente { get; set; }
        public static ClienteVerificado fromJson(string json) => JsonConvert.DeserializeObject<ClienteVerificado>(json);
    }
}
