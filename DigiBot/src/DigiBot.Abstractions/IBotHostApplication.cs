using StructureMap;
using System.Threading.Tasks;

namespace DigiBot
{
    public interface IBotHostApplication
    {
        Task ProcessMessage(IBotMessage message);
        IContainer GetNewScope(string id);
    }
}