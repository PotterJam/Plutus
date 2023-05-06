using System.Threading.Tasks;

namespace Plutus.Api.Users;

public interface IUserService
{
    Task<LoggedInUser> CreateUser(string provider, string username);
    Task<LoggedInUser?> GetUser(string provider, string username);
    Task<LoggedInUser> GetUser(int userId);
}