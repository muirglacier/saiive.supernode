using System.Threading.Tasks;

namespace Saiive.SuperNode.Abstaction
{
    public interface IPeriodicJob
    {
        Task Run();
    }
}
