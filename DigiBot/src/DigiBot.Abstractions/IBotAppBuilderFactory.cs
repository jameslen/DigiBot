using StructureMap;

namespace DigiBot
{
    public interface IBotAppBuilderFactory
    {
        IBotAppBuilder Create(IContainer services);
    }
}