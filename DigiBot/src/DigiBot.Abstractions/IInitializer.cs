using StructureMap;

namespace DigiBot
{
    public interface IInitializer
    {
        void Configure(IBotAppBuilder bot);
        IContainer ConfigureServices(Registry services);
    }
}