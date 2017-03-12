using DigiBot.Discord.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using DigiBot.Discord.Internal;

namespace DigiBot.Discord
{
    public class DiscordChannel : IChannel
    {
        public class Overwrite
        {
            public string Id { get; set; }
            public string Type { get; set; }
            public int Allow { get; set; }
            public int Deny { get; set; }
        }

        public string Id { get; set; }
        public string Guild_Id { get; set; }
        public DiscordServer Guild { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public int Position { get; set; }
        public bool Is_Private { get; set; }
        [JsonConverter(typeof(JsonListToDictionaryById<Overwrite>))]
        public Dictionary<string, Overwrite> Permission_Overwrites { get; set; }
        public string Topic { get; set; }
        public string Last_Message_Id { get; set; }
        public int? Bitrate { get; set; }
        public int? User_Limit { get; set; }

        public DiscordChannel()
        {
        }

        public void Send(string message)
        {
            var messagePayload = $"{{\"content\": \"{message}\", \"tts\": false}}";
            Http.Post<DiscordMessage>(string.Format(APIs.Channel.CreateMessage, Id), messagePayload);
        }

        public DiscordServer Server { get; set; }
        public DiscordHttpClient Http { get; internal set; }
    }
}