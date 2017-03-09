using System;
using StructureMap;

namespace DigiBot
{
    public interface IBotBuilder
    {
        IDigiBot Build();
        IBotBuilder ConfigureServices(Action<Registry> configureServices);
        IBotBuilder UseInitializer<TInit>() where TInit : class;
    }
}