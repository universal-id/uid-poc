using ClinetOnline.Models;
using System.Threading.Tasks;

namespace ClinetOnline.Services
{
    public interface IAppService
    {
        Task<State> AppStarted(AppStarted appEvent);
    }
}