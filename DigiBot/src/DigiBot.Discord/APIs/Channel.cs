using System;
using System.Collections.Generic;
using System.Text;

namespace DigiBot.Discord.APIs
{
    public static class Channel
    {
        public static readonly string Get = "api/channels/{0}";

        // PUT, PATCH
        public static readonly string Update = "api/channels/{0}";

        public static readonly string Delete = "api/channels/{0}";

        // GET, query string params: 
        // around, before, after, limit
        public static readonly string GetMessages = "api/channels/{0}/messages";

        // GET
        public static readonly string GetMessage = "api/channels/{0}/messages/{1}";

        // POST, json body:
        // {
        //     content: string,
        //     nonce: string,
        //     tts: bool,
        //     file: file content,
        //     embed: embed object
        // }
        public static readonly string CreateMessage = "api/channels/{0}/messages";
    }
}
