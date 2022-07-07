using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryBotUtn.Common.Models
{
    public class BotVerificadoEntity
    {
        public string token { get; set; }
        public BotEntity bot { get; set; }
        public static BotVerificadoEntity fromJson(string json) => JsonConvert.DeserializeObject<BotVerificadoEntity>(json);

    }
}
