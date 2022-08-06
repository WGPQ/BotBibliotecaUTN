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


        public async Task<LoginResponse> Auth(string email)
        {


            var url = $"{baseUrl}/auth/login/chatbot";


            LoginResponse result = new LoginResponse();
            var emailRequest = new AuthRequest();
            var bot = new UserVerificadoEntity();
            emailRequest.correo = email;
            using (var client = new HttpClient())
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(emailRequest), Encoding.UTF8, "application/json");


                var resp = await client.PostAsync(url, content);

                if (resp.IsSuccessStatusCode)
                {
                    var apiResponse = resp.Content.ReadAsStringAsync().Result;

                    result = JsonConvert.DeserializeObject<LoginResponse>(apiResponse);

                }
            }

            return result;
        }

        public async Task<UserVerificadoEntity> VerificationToken(string token)
        {
            var url = $"{baseUrl}/auth/verificar/token";

            UserVerificadoEntity userVerificado = new UserVerificadoEntity();
            ResultadoEntity result = new ResultadoEntity();

            using (var client = new HttpClient())
            {

                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

                var resp = await client.GetAsync(url);

                if (resp.IsSuccessStatusCode)
                {
                    var apiResponse = resp.Content.ReadAsStringAsync().Result;

                     result = ResultadoEntity.fromJson(apiResponse);

                    if (result.exito)
                    {
                        userVerificado = UserVerificadoEntity.fromJson(result.data.ToString());
                    }
                    



                }
            }

            return userVerificado;
        }
    }
}
