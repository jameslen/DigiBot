using StructureMap;

namespace DigiBot
{
    public interface IChannel
    {
        void Send(string text);

        //IContainer Scope { get; }
    }
}