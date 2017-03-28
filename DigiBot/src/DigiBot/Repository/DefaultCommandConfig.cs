using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace DigiBot.Repository
{
    public class DefaultCommandConfig : ICommandConfig
    {
        private Dictionary<string, string> _user;
        private Dictionary<string, string> _channel;
        private Dictionary<string, string> _server;

        public DefaultCommandConfig(Dictionary<string, string> server, Dictionary<string, string> channel, Dictionary<string, string> user)
        {
            _server = server;
            _channel = channel;
            _user = user;
        }

        public Dictionary<string, string> GetChannelConfig(string Id)
        {
            return _channel;
        }

        public Dictionary<string, string> GetServerConfig(string Id)
        {
            return _server;
        }

        public Dictionary<string, string> GetUserConfig(string Id)
        {
            return _user;
        }
    }
}
