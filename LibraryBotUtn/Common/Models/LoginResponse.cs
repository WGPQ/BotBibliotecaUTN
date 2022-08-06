using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryBotUtn.Common.Models
{
    public class LoginResponse
    {
        public bool exito { get; set; }

        public string message { get; set; }
        public string token { get; set; }

        public string id_usuario { get; set; }
        public static LoginResponse fromJson(string json) => JsonConvert.DeserializeObject<LoginResponse>(json);
    }
}
