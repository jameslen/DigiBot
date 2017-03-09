using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigiBot
{
    public class UserCommands : ICommandProcessor
    {
        private IConfigurationRoot _config;

        public UserCommands(IConfigurationRoot config)
        {
            _config = config;

            Prefix = _config["Config:Prefix"];
        }

        public void Echo(string text)
        {
            Reply(text);
        }

        public void IntTest(int i)
        {
            Reply($"int: {i}");
        }

        public void ListUsers()
        {
            var users = SourceMessage.Server.Users;

            StringBuilder sb = new StringBuilder();

            sb.Append("```\\n");

            foreach(var user in users)
            {
                sb.Append($"{user.Name}\\n");
            }

            sb.Append("```\\n");

            Reply(sb.ToString());
        }
    }
}
