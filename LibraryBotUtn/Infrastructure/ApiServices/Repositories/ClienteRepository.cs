using LibraryBotUtn.Common.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace LibraryBotUtn.Services.BotConfig.Repositories
{
    public class ClienteRepository: IClienteRepositori
    {
        private readonly string baseUrl;

        public ClienteRepository(string baseUrl)
        {
            this.baseUrl = baseUrl;
        }

        public async Task<ClienteVerificado> Auth(string email)
        {


            var url = $"{baseUrl}/login/auth/cliente";


            //var tokenResult = new TokenResult();
            var clienteVerificado = new ClienteVerificado();
            var emailRequest = new AuthRequest();
            emailRequest.correo = email;
            using (var client = new HttpClient())
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(emailRequest), Encoding.UTF8, "application/json");


                var resp = await client.PostAsync(url, content);

                if (resp.IsSuccessStatusCode)
                {
                    var apiResponse = resp.Content.ReadAsStringAsync().Result;

                    ResultadoEntity result = JsonConvert.DeserializeObject<ResultadoEntity>(apiResponse);
                    if (result.data != null)
                    {
                        clienteVerificado = ClienteVerificado.fromJson(result.data.ToString());
                    }

                }
            }

            return clienteVerificado;
        }

        public async Task<ClienteEntity> GetUser(TokenEntity entity)
        {

            var url = $"{baseUrl}/login/verificar/token/cliente";
            var cliente = new ClienteEntity();
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
                        cliente = ClienteEntity.fromJson(result.data.ToString());

                    }
                }
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
            }
            return cliente;
        }

       
    }
}
