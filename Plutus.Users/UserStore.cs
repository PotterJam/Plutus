using System;
using System.Data.Common;
using System.Threading.Tasks;
using Extensions;
using Plutus.Api.Database;
using Plutus.Api.Users;

namespace Plutus.Users;

public class UserStore : IUserStore
{
    private readonly IDatabaseCommandExecutor _dbExecutor;

    public UserStore(IDatabaseCommandExecutor dbExecutor)
    {
        _dbExecutor = dbExecutor;
    }

    public async Task<LoggedInUser> CreateUser(string provider, string username)
    {
        return await _dbExecutor.ExecuteCommand(async cmd =>
        {
            cmd.AddParam("@provider", provider);
            cmd.AddParam("@username", username);

            cmd.CommandText = @"
                    INSERT INTO users (provider, username)
                    VALUES (@provider, @username)
                    RETURNING id;";

            var guid = (Guid?) await cmd.ExecuteScalarAsync();

            if (guid == null)
                throw new UserStoreException("Tried to create user, nothing got returned");

            return new LoggedInUser(guid.Value, username);
        });
    }

    public async Task<LoggedInUser?> GetUser(string provider, string username)
    {
        return await _dbExecutor.ExecuteCommand(async cmd =>
        {
            cmd.AddParam("@provider", provider);
            cmd.AddParam("@username", username);

            cmd.CommandText = @"
                    SELECT id, username
                    FROM users
                    WHERE username = @username
                        AND provider = @provider
                    LIMIT 1;";

            var reader = await cmd.ExecuteReaderAsync();

            if (!await reader.ReadAsync())
                return null;
            
            var user = ReadUser(reader);

            await UpdateLastActive(user.InternalId);

            return user;
        });
    }

    private async Task UpdateLastActive(Guid id)
    {
        await _dbExecutor.ExecuteCommand(async cmd =>
        {
            cmd.AddParam("@id", id);

            cmd.CommandText = @"
                    UPDATE users
                    SET last_active = NOW()
                    WHERE id = @id
                    RETURNING id;";

            if (await cmd.ExecuteScalarAsync() == null)
                throw new UserStoreException($"Tried to update last active for user {id}, nothing got returned");
        });
    }

    public Task<LoggedInUser> GetUser(int userId)
    {
        throw new NotImplementedException();
    }
    
    private LoggedInUser ReadUser(DbDataReader reader)
    {
        var id = reader.Get<Guid>("id");
        var username = reader.Get<string>("username"); 

        return new LoggedInUser(id, username);
    }
}