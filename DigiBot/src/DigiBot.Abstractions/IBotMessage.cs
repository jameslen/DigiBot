using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigiBot
{
    public interface IBotMessage
    {
        string Source { get; }
        string Message { get; }
        IUser User { get; }
        IChannel Channel { get; }
        IServer Server { get; }
    }
}
