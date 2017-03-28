using System.Collections.Generic;

namespace DigiBot
{
    public interface ICommandConfig
    {
        Dictionary<string,string> GetServerConfig(string Id);
        Dictionary<string, string> GetChannelConfig(string Id);
        Dictionary<string, string> GetUserConfig(string Id);

    }
}