using System;
using System.Data.Common;
using System.Threading.Tasks;
using Plutus.Api.Database;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace Plutus.Database;

public class PostgresCommandExecutor : IDatabaseCommandExecutor
{
    private readonly string  _connectionString;

    public PostgresCommandExecutor(IConfiguration configuration)
    {
        _connectionString = new NpgsqlConnectionStringBuilder
        {
            Database = configuration.GetValue<string>("DB_NAME"),
            Host = configuration.GetValue<string>("DB_HOST"),
            Port = configuration.GetValue<int>("DB_PORT"),
            Username = configuration.GetValue<string>("DB_USER_NAME"),
            Password = configuration.GetValue<string>("DB_PASSWORD"),
            MaxPoolSize = configuration.GetValue<int>("DB_MAX_POOL_SIZE")
        }.ToString();
    }

    public async Task ExecuteCommand(Func<DbCommand, Task> action)
    {
        using var conn = new NpgsqlConnection(_connectionString);
        await conn.OpenAsync();
 
        using var command = conn.CreateCommand();
        await action(command);
    }
        
    public async Task<T> ExecuteCommand<T>(Func<DbCommand, Task<T>> action)
    {
        using var conn = new NpgsqlConnection(_connectionString);
        conn.Open();
 
        using var command = conn.CreateCommand();
        return await action(command);
    }
}