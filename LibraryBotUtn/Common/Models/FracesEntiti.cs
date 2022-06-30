using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace LibraryBotUtn.Common.Models
{
    public class FracesEntiti
    {
        public static Random ram = new Random();
        [Key]
        public string Id { get; set; }

        [Required(ErrorMessage = "La intencion es requerida")]
        public string intencion { get; set; }

        [Required(ErrorMessage = "La frace es requerida")]
        public string frace { get; set; }

        public bool activo { get; set; }

        public static FracesEntiti fromJson(string json) => JsonConvert.DeserializeObject<FracesEntiti>(json);

    }
}
