using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryBotUtn.Common.Models
{
    public class ClienteEntity
    {
        [Key]
        public string Id { get; set; }

        [Required(ErrorMessage = "El nombre son requerido")]
        public string nombre { get; set; }

        [Required(ErrorMessage = "El correo es requerido")]
        public string correo { get; set; }
        public string rol { get; set; }

        public bool conectado { get; set; }
        public DateTime conectedAt { get; set; }
        public static ClienteEntity fromJson(string json) => JsonConvert.DeserializeObject<ClienteEntity>(json);

    }

  
}
