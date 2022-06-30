using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryBotUtn.Common.Models
{
    public class TokenEntity
    {
        public string token { get; set; }
       public static TokenEntity formJson(string json) => JsonConvert.DeserializeObject<TokenEntity>(json);
    }


}
