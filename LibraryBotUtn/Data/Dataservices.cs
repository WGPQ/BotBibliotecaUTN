using LibraryBotUtn.Common.Models;
using LibraryBotUtn.Services.BotConfig;
using LibraryBotUtn.Services.BotConfig.Repositories;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace LibraryBotUtn.RecursosBot.Services
{
    public class Dataservices:IDataservices
    {
        private readonly string baseUrl = "https://bibliochatservice02.azurewebsites.net/api";

        //private readonly string baseUrl = "https://localhost:44399/api";
        public IFracesRepositori FracesRepositori => new FracesRepository(baseUrl);

        public IAutenticationRepositori AuthRepositori =>  new AuthRepository(baseUrl);

        public IChatRepositori ChatRepositori => new ChatRepository(baseUrl);

        public IClienteRepositori ClienteRepositori => new ClienteRepository(baseUrl);

        public IStoreRepositori StoreRepositori => new StoreRepository(baseUrl);
        ////static string url = "http://localhost:38906/api/frace/Listar";

        ////public Dataservices()
        ////{

        ////}


        ////public async static Task<T> getFraces()
        ////{
        ////    Random myObject = new Random();
        ////    var fraces = new List<FracesEntiti>();
        ////    var frace = new FracesEntiti();
        ////    var lista = new Listar();

        ////    lista.columna = "";
        ////    lista.nombre = "";
        ////    lista.offset = 0;
        ////    lista.limit = 100;
        ////    lista.sort = "";
        ////    using (var client = new HttpClient())
        ////    {
        ////        StringContent content = new StringContent(JsonConvert.SerializeObject(lista), Encoding.UTF8, "application/json");

        ////        var resp = await client.PostAsync(url, content);

        ////        if (resp.IsSuccessStatusCode)
        ////        {
        ////            var apiResponse = resp.Content.ReadAsStringAsync().Result;

        ////            var valor = JsonConvert.DeserializeObject<ResultadoEntity>(apiResponse);

        ////            fraces = JsonConvert.DeserializeObject<List<FracesEntiti>>(valor.data.ToString()).Where(x => x.activo == true && x.intencion == 9).ToList();
        ////            int num1 = myObject.Next(0, fraces.Count);
        ////            frace = fraces[num1];
        ////            var uuu = jObject["data"].selec;
        ////            List<SetsModels> sets = (from item in doc.Descendants("set")

        ////                                     select new SetsModels
        ////                                     {
        ////                                         setName = item.Element("setName").Value,
        ////                                         setSpec = item.Element("setSpec").Value
        ////                                     }).ToList();

        ////            list = sets;
        ////        }
        ////    }

        ////    return frace;

        ////}
    }
}
