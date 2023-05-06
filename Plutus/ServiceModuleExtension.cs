using Plutus.Api.Database;
using Plutus.Api.Users;
using Plutus.Database;
using Plutus.Users;

namespace Plutus;

public static class ServiceModuleExtension
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        // TODO: I should be able to make all of the concrete implementations internal

        return services
            .AddSingleton<IDatabaseCommandExecutor, PostgresCommandExecutor>()
            .AddSingleton<IUserService, UserService>()
            .AddSingleton<IUserStore, UserStore>()
            ;
    }
}