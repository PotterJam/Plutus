using System.Threading.Tasks;
using Plutus.Api.Users;

namespace Plutus.Users;

public class UserService : IUserService
{
    private readonly IUserStore _userStore;
    public UserService(IUserStore userStore)
    {
        _userStore = userStore;
    }
        
    public async Task<LoggedInUser> CreateUser(string provider, string username)
    {
        LoggedInUser user = await _userStore.CreateUser(provider, username);
        
        return user;
    }

    public async Task<LoggedInUser?> GetUser(string provider, string username)
    {
        return await _userStore.GetUser(provider, username);
    }
        
    public async Task<LoggedInUser> GetUser(int userId)
    {
        return await _userStore.GetUser(userId);
    }
}