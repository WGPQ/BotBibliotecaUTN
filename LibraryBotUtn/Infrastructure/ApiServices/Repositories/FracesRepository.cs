using LibraryBotUtn.Common.Models;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace LibraryBotUtn.Services.BotConfig.Repositories
{
    public class FracesRepository : IFracesRepositori
    {
        private readonly string baseUrl;

        public FracesRepository(string baseUrl)
        {
            this.baseUrl = baseUrl;
        }

        public async Task<FracesEntiti> Frace(string intencion, string token)
        {
            var url = $"{baseUrl}/frace/bot?intencion={intencion}";

            var frace = new FracesEntiti();

            using (var client = new HttpClient())
            {

                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

                var resp = await client.GetAsync(url);

                if (resp.IsSuccessStatusCode)
                {
                    var apiResponse = resp.Content.ReadAsStringAsync().Result;

                    var result = ResultadoEntity.fromJson(apiResponse);
                    if (result.exito)
                    {
                        frace = FracesEntiti.fromJson(result.data.ToString());
                    }

                 

                }
            }

            return frace;

        }


    }
}
