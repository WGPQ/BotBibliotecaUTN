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


        public async Task<ResultadoEntity> Auth(string email)
        {


            var url = $"{baseUrl}/login/auth/chatbot";


            //var tokenResult = new TokenResult();
            var result = new ResultadoEntity();
            var emailRequest = new AuthRequest();
            emailRequest.correo = email;
            using (var client = new HttpClient())
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(emailRequest), Encoding.UTF8, "application/json");


                var resp = await client.PostAsync(url, content);

                if (resp.IsSuccessStatusCode)
                {
                    var apiResponse = resp.Content.ReadAsStringAsync().Result;

                    result = JsonConvert.DeserializeObject<ResultadoEntity>(apiResponse);

                }
            }

            return result;
        }

        public async Task<UsuarioEntity> GetUser(TokenEntity entity)
        {

            var url = $"{baseUrl}/login/verificar/token";
            var usuario = new UsuarioEntity();
            try
            {
                var emailRequest = new AuthRequest();
                using (var client = new HttpClient())
                {

                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {entity.token}");
                    var resp = await client.GetAsync(url);

                    if (resp.IsSuccessStatusCode)
                    {
                        var apiResponse = resp.Content.ReadAsStringAsync().Result;

                        ResultadoEntity result = ResultadoEntity.fromJson(apiResponse);
                        var usuarioVerificado = UsuarioVerificadoEntity.fromJson(result.data.ToString());
                        usuario = usuarioVerificado.usuario;

                    }
                }
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
            }
            return usuario;
        }

        public async Task<ResultadoEntity> NewUser(ClienteEntity cliente, string token)
        {
            var url = $"{baseUrl}/cliente/Insertar";
            var result = new ResultadoEntity();
           
            if (cliente != null)
            {
                
                cliente.rol = "5";

                using (var client = new HttpClient())
                {
                    StringContent content = new StringContent(JsonConvert.SerializeObject(cliente), Encoding.UTF8, "application/json");
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

                    var resp = await client.PostAsync(url, content);

                    if (resp.IsSuccessStatusCode)
                    {
                        var apiResponse = resp.Content.ReadAsStringAsync().Result;

                        result = JsonConvert.DeserializeObject<ResultadoEntity>(apiResponse);

                    }
                }
            }


            return result;
        }
    }
}
