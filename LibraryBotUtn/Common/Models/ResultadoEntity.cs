using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryBotUtn.Common.Models
{
    public partial class ResultadoEntity
    {
        //public int Id { get; set; }

        public bool exito { get; set; }

        public string message { get; set; }

        public object data { get; set; }
        public static ResultadoEntity fromJson(string json)=> JsonConvert.DeserializeObject<ResultadoEntity>(json);

    }
}
