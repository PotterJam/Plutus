using System.Threading.Tasks;

namespace Plutus.Api.Users;

public interface IUserStore
{
    Task<LoggedInUser> CreateUser(string provider, string username);
    Task<LoggedInUser?> GetUser(string provider, string username);
    Task<LoggedInUser> GetUser(int userId);
}