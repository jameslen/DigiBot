using System.Threading.Tasks;

namespace DigiBot
{
    public interface IBotSource
    {
        Task Start(IBotHostApplication botHostApplication);
    }
}