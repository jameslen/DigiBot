using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DigiBot.MessageProcessors
{
    public class SimpleTracker : IMessageProcessor
    {
        private static int __count = 0;

        public SimpleTracker()
        {
            Console.WriteLine($"Simple Tracker instance created ({++__count})");
        }
        
        public void ProcessMessage(IBotMessage message)
        {
            Console.WriteLine($"[Simple Tracker] {message.User.Name}: {message.Message}");
        }
    }
}


