namespace DigiBot
{
    public interface IMessageProcessor
    {
        void ProcessMessage(IBotMessage message);
    }
}