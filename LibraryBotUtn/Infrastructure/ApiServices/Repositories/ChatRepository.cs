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
    public class ChatRepository : IChatRepositori
    {
        private readonly string baseUrl;

        public ChatRepository(string baseUrl)
        {
            this.baseUrl = baseUrl;
        }

        public async Task<ChatEntity> Interaction(InteractionEntity interaction, string token)
        {

            var url = $"{baseUrl}/chat/crear";

            var chat = new ChatEntity();

            using (var client = new HttpClient())
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(interaction), Encoding.UTF8, "application/json");

                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

                var resp = await client.PostAsync(url, content);

                if (resp.IsSuccessStatusCode)
                {
                    var apiResponse = resp.Content.ReadAsStringAsync().Result;

                    ResultadoEntity result = JsonConvert.DeserializeObject<ResultadoEntity>(apiResponse);

                    chat = ChatEntity.fromJson(result.data.ToString());

                }
            }

            return chat;
        }
    }
}
