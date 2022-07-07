using LibraryBotUtn.Common.Models;
using Microsoft.Bot.Schema;
using Microsoft.BotBuilderSamples;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace LibraryBotUtn.Services.BotConfig.Repositories
{
    public class AuthRepository : IAutenticationRepositori
    {
        private readonly string baseUrl;
        public AuthRepository(string baseUrl)
        {
            this.baseUrl = baseUrl;
        }


        public async Task<BotVerificadoEntity> Auth(string email)
        {


            var url = $"{baseUrl}/login/auth/chatbot";


            ResultadoEntity result = new ResultadoEntity();
            var emailRequest = new AuthRequest();
            var bot = new BotVerificadoEntity();
            emailRequest.correo = email;
            using (var client = new HttpClient())
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(emailRequest), Encoding.UTF8, "application/json");


                var resp = await client.PostAsync(url, content);

                if (resp.IsSuccessStatusCode)
                {
                    var apiResponse = resp.Content.ReadAsStringAsync().Result;

                    result = JsonConvert.DeserializeObject<ResultadoEntity>(apiResponse);
                    if (result.data != null)
                    {
                        bot = BotVerificadoEntity.fromJson(result.data.ToString());
                    }

                }
            }

            return bot;
        }


    }
}
