using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigiBot.Commands
{
    public class AdminCommands : ICommandProcessor
    {
        private IConfigurationRoot _config;
        private List<string> _allowedRoles = new List<string>();

        public AdminCommands(IConfigurationRoot config)
        {
            _config = config;

            Prefix = _config["Config:Prefix"];
        }

        public override bool CheckPermissions(IUser user)
        {
            if(user.IsAdmin)
            {
                return true;
            }

            return _allowedRoles.Intersect(user.Roles).Count() != 0;
        }

        public void AddRole(string role)
        {
            _allowedRoles.Add(role);
        }

        public void RemoveRole(string role)
        {
            _allowedRoles.Remove(role);
        }

        public void GetUserRoles(IUser user)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("```\\n");
            sb.Append($"{user.Name}'s roles:\\n");

            foreach (var role in user.Roles)
            {
                sb.Append($"{role}\\n");
            }

            sb.Append("```\\n");

            Reply(sb.ToString());
        }
    }
}
