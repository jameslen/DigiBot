using System;
using System.Collections.Generic;
using System.Text;

namespace DigiBot.Discord.APIs
{
    public static class Guild
    {
        // POST, json body: 
        //{
        //    "name": string,
        //    "region": string,
        //    "icon": string,
        //    "verification_level": int,
        //    "default_message_notifications": int,
        //    "roles" : [ role objects ],
        //    "channels" : [ guild channel objects ]
        //}
        public static readonly string Create = "api/guilds";

        // GET
        public static readonly string Get = "api/guilds/{0}";

        // PATCH, json body: 
        //{
        //    "name": string,
        //    "region": string,
        //    "icon": string,
        //    "verification_level": int,
        //    "default_message_notifications": int,
        //    "afk_channel_id": string,
        //    "afk_timeout": string,
        //    "owner_id": string,
        //    "splash": string,
        //}
        public static readonly string Modify = "api/guilds/{0}";

        // DELETE
        public static readonly string Delete = "api/guilds/{0}";

        public static class Channel
        {
            // GET
            public static readonly string Get = "api/guilds/{0}/channels";

            // POST, json body
            //{
            //    "name": string,
            //    "type": string,
            //    "bitrate": int,
            //    "user_limit": int,
            //    "permission_overwrites": [ overwrites ]
            //}
            public static readonly string Create = "api/guilds/{0}/channels";

            // PATCH, json body:
            //{
            //    "id": string,
            //    "position": int
            //}
            public static readonly string ChangePosition = "api/guilds/{0}/channels";


        }

        public static class Member
        {
            // TODO: Finish
        }

        public static class Role
        {
            // TODO: Finish
        }

        public static class Ban
        {
            // TODO: Finish
        }

        public static class Prune
        {
            // TODO: Finish
        }

        public static class Integrations
        {
            // TODO: Finish
        }

        public static class Embed
        {
            // TODO: Finish
        }

        public static class Invites
        {
            // TODO: Finish
        }

        public static class Voice
        {
            // TODO: Finish
        }
    }
}
