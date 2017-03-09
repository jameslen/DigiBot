using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DigiBot
{
    public abstract class ICommandProcessor
    {
        public IBotMessage SourceMessage { get; set; }
        public IUser User => SourceMessage.User;
        public IChannel SourceChannel => SourceMessage.Channel;
        public string MessageText => SourceMessage.Message;

        public string Prefix { get; set; }

        public virtual bool CheckPermissions(IUser user)
        {
            return true;
        }

        public void Reply(string message)
        {
            SourceChannel.Send(message);
        }
    }
}
