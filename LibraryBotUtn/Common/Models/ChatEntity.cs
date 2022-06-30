using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryBotUtn.Common.Models
{
    public class ChatEntity
    {
        public string chat { get; set; }
        public static ChatEntity fromJson(string json) => JsonConvert.DeserializeObject<ChatEntity>(json);

    }
}
