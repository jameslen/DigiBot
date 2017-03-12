using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DigiBot.Discord.Internal
{
    public class DiscordHttpClient
    {
        private HttpClient _client = new HttpClient();
        private ILogger Logger;

        public DiscordHttpClient(ILogger logger)
        {
            Logger = logger;

            _client.BaseAddress = new Uri(APIs.Base.API);
            _client.DefaultRequestHeaders.Add("User-Agent", $"DigiBot.Discord/{typeof(DiscordHttpClient).GetTypeInfo().Assembly.ImageRuntimeVersion}");
        }

        public void AddBotAuth(string token)
        {
            _client.DefaultRequestHeaders.Add("Authorization", $"Bot {token}");
        }

        public async Task<string> GetBotGateway()
        {
            var result = await _client.GetAsync(DiscordAPI.Gateway.Bot);

            if (result.IsSuccessStatusCode)
            {
                return await result.Content.ReadAsStringAsync();
            }
            else
            {
                Logger?.Error($"Error returning gateway: {result.ReasonPhrase}");
                return null;
            }
        }

        public async Task<T> Post<T>(string api, string payload)
        {
            var result = await _client.PostAsync(api, new StringContent(payload, Encoding.UTF8, "application/json"));

            if(result.IsSuccessStatusCode)
            {
                Logger?.Debug(await result.Content.ReadAsStringAsync());

                using (var stream = await result.Content.ReadAsStreamAsync())
                {
                    using (var streamReader = new StreamReader(stream))
                    {
                        using (var reader = new JsonTextReader(streamReader))
                        {
                            JsonSerializer s = new JsonSerializer();

                            return s.Deserialize<T>(reader);
                        }
                    }
                }
            }

            return default(T);
        }

        public async Task<string> Get(string api)
        {
            return await (await _client.GetAsync(api)).Content.ReadAsStringAsync();
        }
    }
}
