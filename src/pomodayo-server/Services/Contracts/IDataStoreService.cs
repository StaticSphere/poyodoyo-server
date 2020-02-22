using System.Threading.Tasks;
using pomodayo.server.Models;

namespace pomodayo.server.Services.Contracts
{
    public interface IDataStoreService
    {
        Task<UserModel> GetUserAsync(string username);
        Task<UserModel> CreateUserAsync(string username, string password);
    }
}
