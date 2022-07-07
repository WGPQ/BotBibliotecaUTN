using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryBotUtn.Common.Models
{
    public class BotEntity
    {
        // [Column("id")]
        public string id { get; set; }
        // [Column("nombres")]
        public string nombres { get; set; }
        // [Required("")]
        public string apellidos { get; set; }
        public string correo { get; set; }
        public string telefono { get; set; }

        public string rol { get; set; }

        public static BotEntity fromJson(string json) => JsonConvert.DeserializeObject<BotEntity>(json);

    }


}
