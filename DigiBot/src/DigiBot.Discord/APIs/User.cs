using System;
using System.Collections.Generic;
using System.Text;

namespace DigiBot.Discord.APIs
{
    public static class User
    {
        // Parameter is user.id
        public static readonly string Get = "api/users/{0}";
        // Parameter is user.id, json body { "usersname" : string, "avatar" : avatar data }
        public static readonly string Patch = "api/users/{0}";

        // GET
        public static readonly string Guilds = "api/users/{0}/guilds";
        // DELETE
        public static readonly string LeaveGuild = "api/users/{0}/guild/{1}";

        // GET
        public static readonly string DirectMessageChannels = "api/users/@me/channels";
        // POST, json body: { "recipient_id": string }
        public static readonly string NewDirectMessageChannel = "api/users/@me/channels";
        // POST, json body: { "access_tokens: [ tokens ], "nicks": dictionary of nick to user id }
        public static readonly string GroupDirectMessageChannel = "api/users/@me/channels";

    }
}
